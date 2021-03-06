﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace SMining.Core.Data
{
    /**
     * 데이터 처리를 담당하는 최상위 인터페이스
     */
    [Serializable]
    public abstract class DataProcess : GeneralOptionSupport, ICloneable
    {
        /// <summary>
        ///  모든 데이터 프로세스가 공유하는 전역 리소스
        /// </summary>
        protected static DataProperty global = new DataProperty();

        // 데이터 프로세스 이벤트 델리게이트
        public delegate void DataProcessEventHandler(Object sender, DataProcessEvent e);
        
        // 데이터 프로세스 이벤트 정의, 원래는 event 매커니즘을 활용했으나 내부 리스너들을
        // cascade하는게 어려워서 직접 컨트롤하는 것으로 변경
        public List<DataProcessEventHandler> DataProcessEventObserver { get; set; }

        // 비동기 실행자
        [NonSerialized]
        private Thread taskRunner;


        public DataProcess()
        {
            CleanAfterRun = true;
            DataProcessEventObserver = new List<DataProcessEventHandler>(5);
        }

        /**
         * 비동기 실행자. runAsync() 호출 이후에 생성
         */
        public Thread TaskRunner { get { return taskRunner; } }

        /**
         * 데이터 프로세스의 이름
         */
        public String Name { get; set; }

        /**
         * 입력 데이터 프로퍼티
         */
        public Object Input { get; set; }

        /**
         * 입력 데이터의 타입
         */
        public Type InputType { get; set; }

        /**
         * 출력 데이터 프로퍼티
         */
        public Object Output { get; set; }

        /**
         * 출력 데이터의 타입
         */
        public Type OutputType { get; set; }

        /**
         * 이전 데이터 처리 객체
         */
        public DataProcess Previous { get; set; }

        /**
         * 다음 데이터 처리 객체
         */
        public DataProcess Next { get; set; }

        /**
         * 데이터 프로세스에 대한 설명
         */
        public String Description { get; set; }

        /**
         * Input 파일 정보
         */
        public List<string> InputFileList { get; set; }

        /**
         * InputFile 개수 정보
         */
        public List<int> InputFileCount { get; set; }

        
        /**
         * 실행 이후에 Input 데이터를 파괴할 것인지 여부
         * 기본값은 False (보존)
         */
        public bool CleanAfterRun { get; set; }

        /**
         * 데이터 처리를 수행
         */
        public abstract void Run();

        /**
         * 데이터 처리를 수행하되, 비동기로 수행
         */
        public void RunAsync()
        {
            if (TaskRunner != null && TaskRunner.IsAlive)
            {
                TaskRunner.Abort();
            }
            taskRunner = new Thread(new ThreadStart(Run));
            taskRunner.SetApartmentState(ApartmentState.STA);
            taskRunner.IsBackground = true;
            taskRunner.Start();
        }

        /**
         * 데이터를 비동기로 수행할 경우, 종료시까지 기다림
         */
        public void Join()
        {
            if (taskRunner != null && taskRunner.IsAlive)
            {
                taskRunner.Join();
            }
        }

        /**
         * 특정 이름의 데이터 처리를 수행
         * 
         * @param[in] name 수행할 작업의 이름
         */
        public virtual void Run(String name) {
            throw new NotImplementedException("This method is not implented");
        }

        /**
         * 이벤트 핸들러들에게 이벤트를 통보한다
         */
        protected void NotifyEvent(DataProcessEvent e)
        {
            if (e != null)
            {
                List<DataProcessEventHandler> observers = new List<DataProcessEventHandler>(DataProcessEventObserver);
                foreach (DataProcessEventHandler eh in observers)
                {
                    eh(this, e);
                }
            }
        }

        /**
         * 이벤트 핸들러들에게 이벤트를 통보한다
         */
        protected void NotifyEvent(String type, String message, Object data)
        {
            DataProcessEvent e = new DataProcessEvent();
            e.Source = this;
            e.Type = type;
            e.Message = message;
            if (data != null)
            {
                e.Set("Data", data);
            }
            NotifyEvent(e);
        }

        /**
         * 객체를 복사하여 반환한다.
         * 구체적인 데이터 프로세스 객체들은 반드시 이 메소드를 구현하여야 한다.
         */
        public abstract Object Clone();        

        /**
         * 객체를 표현하는 문자열을 반환한다
         * 
         * @return 객체의 문자열 표현. 기본 구현은 프로세스명을 반환
         */
        public override string ToString()
        {
            return Name;
        }

    }
}
