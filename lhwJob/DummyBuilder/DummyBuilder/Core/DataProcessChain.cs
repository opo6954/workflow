using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SMining.Core.Data
{
    /**
     * 데이터 프로세스 체인.
     * 여러개의 데이터 프로세스를 묶어서 관리하고 실행하는
     * 기능을 담당하는 클래스
     */
    [Serializable]
    public class DataProcessChain : DataProcess
    {
        DataProcess.DataProcessEventHandler handler;
        public List<DataProcess> Process { get; set; }
        public DataProcess CurrentProcess { get; set; }

        public int Count
        {
            get
            {
                return Process.Count;
            }
        }

        /**
         * 데이터 프로세스 체인.
         * 여러개의 데이터 프로세스를 묶어서 관리하고 실행하는
         * 기능을 담당하는 클래스
         */
        public DataProcessChain()
        {
            Name = "Noname";
            Process = new List<DataProcess>();
            handler = new DataProcess.DataProcessEventHandler(handleDataProcessEvent);
        }

        /**
         * 데이터 프로세스 목록
         */
        public List<DataProcess> Items
        {
            get {
                return Process;
            }
        }

        /**
         * 데이터 프로세스 체인에 새로운 데이터를 추가한다.
         * 이 메소드 호출은 기존 프로세스를 참고하여 추가되는 객체의
         * Next, Previous를 설정한다.
         * 
         * @param[in] p 데이터 프로세스 객체
         * 
         */
        public void addProcess(DataProcess p)
        {
            DataProcess prev = null;
            if (Items.Count > 0)
            {
                prev = Items[Items.Count - 1];
            }
            p.Previous = prev;
            Items.Add(p);
        }

        /**
         * 프로세스 체인을 시작한다
         */
        public override void Run()
        {
            int idx = 0;
            
            if (Process.Count > 0) { CurrentProcess = Process[idx]; }

            List<DataProcessChain> asyncRunList = new List<DataProcessChain>();
            
            Object data;
            data = Input;
            Output = null;

            // 이벤트 객체
            DataProcessEvent e = new DataProcessEvent();
            e.Title = "Running [" + Name + "]";
            e.Source = this;
            e.Type = DataProcessEvent.EVENT_START;
            e.Message = "DataProcessChain [" + Name + "] Started";

            NotifyEvent(e);

            // 체인을 따라 프로세스를 실행
            while (CurrentProcess != null)
            {
                // Streaming 데이터 프로세스
                if (CurrentProcess is StreamDataProcess)
                {
                    List<DataRecord> streamOutput = new List<DataRecord>();
                    idx = RunStream(data, e, idx, streamOutput);
                    data = streamOutput;
                }
                // 데이터 프로세스 체인
                else if (CurrentProcess is DataProcessChain)
                {
                    DataProcessChain pc = (DataProcessChain) CurrentProcess;

                    if (pc.Count > 0)
                    {
                        pc.Input = data;
                        pc.InputFileList = InputFileList;
                        pc.InputFileCount = InputFileCount;

                        asyncRunList.Add(pc);
                        pc.RunAsync();
                    }
                }
                // 일반 데이터 프로세스
                else
                {
                    data = RunNormal(data, e);                    
                }

                // 다음 프로세스가 지정되어 있으면 실행.
                // 즉, 프로세스 자체가 체인 구조를 통해 다음 실행을
                // 선택할 수 있음
                if (CurrentProcess.Next != null)
                {
                    // 메모리 절약을 위해서 기존 데이터를 파괴해도 괜찮으면 레퍼런스를 해제
                    // 나머지는 GC에게 맡긴다.
                    CurrentProcess = CurrentProcess.Next;
                }
                // 다음 프로세스가 지정되어 있지 않으면 순차실행
                // 기본 행동
                else if(idx < (Process.Count - 1))
                {
                    CurrentProcess = Process[++idx];
                }
                // 실행할 프로세스 없음. 루프 종료
                else
                {
                    // 비동기 실행 목록이 남아있으면 모두 종료될 때까지 기다림
                    for (int i = 0; i < asyncRunList.Count; i++)
                    {
                        e.Source = this;
                        e.Type = DataProcessEvent.EVENT_MESSAGE;
                        e.Message = "Waiting [" + asyncRunList[i].Name + " is finished ...";
                        NotifyEvent(e);

                        asyncRunList[i].Join();
                    }

                    // 종료 전 다음 실행을 위해서 GC를 수행
                    System.GC.Collect();
                    break;
                }
            }

            // 출력값 저장
            Output = data;

            e.Source = this;
            e.Type = DataProcessEvent.EVENT_END;
            e.Message = Name;
            NotifyEvent(e);
        }

        // 순차적으로 데이터를 처리하는 일반적인 형태의 루틴
        private object RunNormal(object data, DataProcessEvent e)
        {
            try
            {
                CurrentProcess.DataProcessEventObserver.Add(handler);
                CurrentProcess.Input = data;

                DateTime startTime = DateTime.Now;
                e.Type = DataProcessEvent.EVENT_MESSAGE;
                e.Message = "DataProcess [" + CurrentProcess.Name + "] Started";
                NotifyEvent(e);

                CurrentProcess.Run();

                DateTime endTime = DateTime.Now;
                e.Message = "DataProcess [" + CurrentProcess.Name + "] Finished (" + (endTime - startTime).Seconds + " s)";
                NotifyEvent(e);

                // 종료된 프로세스에 Display 상태가 있으면
                // 이벤트 핸들러에게 통보
                if (CurrentProcess.IsSet("Display"))
                {
                    e.Source = this;
                    e.Type = "Display";
                    e.Title = CurrentProcess.Name;
                    e.Set("Data", CurrentProcess.Output);
                    NotifyEvent(e);
                }

                data = CurrentProcess.Output;

                CurrentProcess.Input = null;
                CurrentProcess.Output = null;
                CurrentProcess.DataProcessEventObserver.Remove(handler);
            }
            catch (Exception ex)
            {
                // 실행 중 예외, 이벤트 등록 해지 후
                CurrentProcess.DataProcessEventObserver.Remove(handler);

                // 예외를 다시 던지기
                throw ex;
            }
            return data;
        }

        // 부분적으로 데이터를 처리하는 스트리밍 처리 형태의 루틴
        private int RunStream(object data, DataProcessEvent e, int startIndex, List<DataRecord> output)
        {
            output.Clear();

            // 현재부터 어디까지가 스트리밍 프로세스인지 조사한다
            int endIndex = startIndex;
            for (int i = startIndex + 1; i < Process.Count; i++)
            {
                if (Process[i] is SMining.Core.Data.StreamDataProcess)
                {
                    endIndex = i;
                }
            }

            // 스트리밍 프로세스 초기화
            List<StreamDataProcess> procs = new List<StreamDataProcess>();
            for (int i = startIndex; i <= endIndex; i++)
            {
                Process[i].DataProcessEventObserver.Add(handler);
                StreamDataProcess sp = (StreamDataProcess)Process[i];
                procs.Add(sp);
            }

            // 스트리밍 프로세스 실행
            List<DataRecord> dataList = StreamDataProcess.Run(procs);
            output.Clear();
            foreach (DataRecord r in dataList)
            {
                output.Add(r);
            }

            // 핸들러 정리 및 마지막으로 수행한 프로세스의 인덱스를 반환
            for (int i = startIndex; i <= endIndex; i++)
            {
                Process[i].DataProcessEventObserver.Remove(handler);
            }
            return endIndex;
        }

        // 데이터 프로세스 이벤트 전파
        public void handleDataProcessEvent(Object sender, DataProcessEvent e)
        {
            // 단, 내부 프로세스의 End는 외부로 전파하지 않음.            
            if (e.Type != DataProcessEvent.EVENT_END)
            {
                e.Source = sender;
                NotifyEvent(e);
            }
            if (e.Type == DataProcessEvent.EVENT_ERROR && sender.GetType() == typeof(DataProcess))
            {
                ((DataProcess) sender).DataProcessEventObserver.Remove(handler);
            }
        }

        /**
         * 바이너리 파일에 현재 객체를 저장한다
         * 
         * @param[in] filename 저장할 파일
         */
        public void save(String filename)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                bf.Serialize(fs, this);
            }
        }

        /**
         * 바이너리 파일에서 객체를 불러온다.
         * 
         * @param[in] filename 불러올 파일
         */
        public static DataProcessChain load(String filename)
        {
            DataProcessChain dp;
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                dp = (DataProcessChain)bf.Deserialize(fs);
            }
            return dp;
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
