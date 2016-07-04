using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SMining.Core.Data;

namespace SMining.Core.Data
{
    /**
     * 데이터 프로세스에서 발생하는 이벤트 클래스
     */
    public class DataProcessEvent : System.EventArgs
    {
        // 이벤트 상수
        public const String EVENT_START = "Start.DE";
        public const String EVENT_ERROR = "Error.DE";
        public const String EVENT_WARN = "Warn.DE";
        public const String EVENT_MESSAGE = "Message.DE";
        public const String EVENT_END = "End.DE";

        private GeneralOptionSupport options = new GeneralOptionSupport();

        /**
         * 기본 이벤트 생성자
         */
        public DataProcessEvent()
            : this(null, "Undefiend", "Untitled", "")
        {
        }

        /**
         * 이벤트 생성자
         * 
         * @param[in] src 이벤트 발생 객체
         * @param[in] type 이벤트의 타입
         * @param[in] title 이벤트 제목
         * @param[in] message 이벤트의 구체적인 내용
         */
        public DataProcessEvent(Object src, String type, String title, String message)
        {
            Source = src;
            Type = type;
            Title = title;
            Message = message;
        }

        /**
         * 이벤트가 발생한 객체
         */
        public Object Source { get; set; }

        /**
         * 이벤트 종류
         */
        public String Type { get; set; }

        /**
         * 이벤트 제목
         */
        public String Title { get; set; }

        /**
         * 이벤트 내용
         */
        public String Message { get; set; }

        /**
         * GeneralOptionSupport 클래스 참조
         */
        public virtual void Set(String key)
        {
            options.Set(key);
        }

        /**
         * GeneralOptionSupport 클래스 참조
         */
        public virtual void UnSet(String key)
        {
            options.UnSet(key);
        }


        /**
         * GeneralOptionSupport 클래스 참조
         */
        public virtual bool IsSet(String key)
        {
            return options.IsSet(key);
        }

        /**
         * GeneralOptionSupport 클래스 참조
         */
        public virtual Object Set(String key, Object value)
        {
            return options.Set(key, value);
        }

        /**
         * GeneralOptionSupport 클래스 참조
         */
        public virtual Object Get(String key)
        {
            return options.Get(key);
        }
    }
}
