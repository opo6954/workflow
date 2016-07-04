using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMining.Core.Data
{
    /**
     * 데이터 처리를 담당하는 최상위 인터페이스
     */
    [Serializable]
    public abstract class StreamDataProcess : DataProcess, StreamReader<DataRecord>
    {
        /**
         * StreamDataProcess의 라이프 싸이클 이벤트 메소드
         * 이 함수는 최초 실행시 호출된다.
         */
        public abstract void        OnBeforeStart();

        /**
         * StreamDataProcess의 라이프 싸이클 이벤트 메소드
         * 이 함수는 종료 직전에 호출된다.
         */
        public abstract void        OnBeforeClose();

        // DataReader, StreamReader 인터페이스 구현
        public abstract void            open();
        public abstract void            close();
        public abstract bool            moveNext();
        public abstract DataRecord      next();
        public abstract object          getData();
        public abstract int             getIndex();
        public abstract int             getCount();

        public static List<DataRecord> Run(List<StreamDataProcess> procs)
        {
            // 출력값
            List<DataRecord> dataList = new List<DataRecord>();

            // 첫 프로세스에서 스트림이 끊길 때까지 반복
            StreamDataProcess sProc = procs[0];

            foreach(StreamDataProcess sp in procs) {
                sp.open();
                sp.OnBeforeStart();
            }

            DataRecord r;
            int loopCount = 0;
            while (sProc.moveNext())
            {
                r = sProc.next();
                // 두번쨰 프로세스부터 스트리밍 체인
                if (procs.Count > 1)
                {
                    for (int i = 1; i < procs.Count; i++)
                    {
                        StreamDataProcess cProc = procs[i];
                        cProc.Input = r;

                        // 최초 실행 통보
                        if (loopCount == 0)
                        {
                            cProc.OnBeforeStart();
                        }
                        // 처리가 끝나고 다음 프로세스로 진행해도 괜찮은가? 그렇지 않다면 다음 입력을 기다리며 루프 종료
                        if (!cProc.moveNext())
                        {
                            break;
                        }
                        // 마지막 스트리밍 체인까지 모두 처리가 되었다면 결과를 누적
                        else if (i == (procs.Count - 1))
                        {
                            dataList.Add(r);
                        }
                    }
                    loopCount++;
                }
                // 단일 스트리밍 프로세스
                else
                {
                    dataList.Add(r);
                }
            }

            // 스트림 프로세스는 종료 전에 데이터가 들어오기를 기다릴 수 있으므로 이를 통보하고,
            // 마지막 사이클을 수행한 후에 종료            
            r = null;
            for (int i = 1; i < procs.Count; i++)
            {
                StreamDataProcess cProc = procs[i];
                cProc.Input = r;
                cProc.OnBeforeClose();
                if (cProc.moveNext())
                {
                    r = cProc.next();
                }
                cProc.close();
            }
            if (r != null)
            {
                dataList.Add(r);
            }
            sProc.OnBeforeClose();
            sProc.close();
            return dataList;
        }        
    }
}
