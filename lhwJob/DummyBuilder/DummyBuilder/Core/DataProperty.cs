using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using SMining.Core.Data;

namespace SMining.Core.Data
{
    /**
     * 일반 데이터 속성 클래스. 이 클래스는 이름 / 값을 저장하는 래퍼 클래스로
     * 동작하며 DataRecord와 함께 비정형 데이터를 다루는 클래스로서 사용된다
     */
    [Serializable]
    public class DataProperty : GeneralOptionSupport, ICloneable
    {
        /// <summary>
        ///  모든 데이터 프로퍼티가 공유하는 전역 리소스
        /// </summary>
        protected static DataProperty global = new DataProperty();

        /**
         * 일반 데이터 속성 객체의 기본 생성자
         */
        public DataProperty() {
            Set("Display");
        }

        /**
         * 일반 데이터 속성의 생성자
         * 
         * @param[in] name 데이터 이름
         * @param[in] value 데이터 값
         * @param[in] type 데이터 타입
         */
        public DataProperty(string name, object value, Type type) : this()
        {
            Name = name;
            Value = value;
            Type = type;
        }

        /**
         * 일반 데이터 속성의 생성자
         * 
         * @param[in] name 데이터 이름
         * @param[in] value 데이터 값
         * @param[in] type 데이터 타입
         * @param[in] desc 설명
         */
        public DataProperty(string name, object value, Type type, String desc) : this(name, value, type)
        {
            Description = desc;
        }

        /**
         * 프로퍼티의 이름
         */
        public virtual String Name { get; set; }

        /**
         * 프로퍼티의 값
         */
        public virtual Object Value { get; set; }

        /**
         * 프로퍼티 값의 타입
         */
        public virtual Type Type { get; set; }

        /**
         * 프로퍼티에 대한 설명
         */
        public virtual String Description { get; set; }

        /**
         * 문자열로부터 타입을 추측하여 반환한다.
         * 
         * @param[in] value 추측할 타입의 값
         * @return          추측되는 타입, 실패할 경우 String
         */
        public static Type inferType(String value)
        {
            value = value.Trim();
            
            // 정수
            try
            {
                Int32.Parse(value);
                return typeof(int);
            }
            catch (Exception e) { }

            // 실수
            try
            {
                Double.Parse(value);
                return typeof(double);
            }
            catch (Exception e) { }

            // 날짜
            try
            {
                DateTime.Parse(value);
                return typeof(DateTime);
            }
            catch (Exception e) { }

            // 알 수 없음. 문자열로 처리
            return typeof(String);
        }

        /**
         * 문자열로부터 실제 타입의 값을 만들어 반환한다.
         */
         public static Object Parse(String value)
         {
            Object val = value;
            Type realType = inferType(value);
            if (realType != typeof(String))
            {
                // 정수
                if (realType == typeof(Int32))
                {
                    val = Int32.Parse(value);
                }
                // 실수
                else if (realType == typeof(Double))
                {
                    val = Double.Parse(value);
                }
                // 날짜
                else if (realType == typeof(Double))
                {
                    val = DateTime.Parse(value);
                }
            }
            return val;
        }

         public Object Clone()
         {
             DataProperty clone = new DataProperty(Name, Value, Type);
             clone.optionMap = new Dictionary<string, object>();
             foreach (String key in clone.optionMap.Keys)
             {
                 clone.Set(key, Get(key));
             }
             return clone;
         }

         public override bool Equals(object obj)
         {
             if (GetType() != obj.GetType())
             {
                 return false;
             }
             DataProperty other = (DataProperty)obj;
             if (other.Name == Name && other.Type == Type)
             {
                 return true;
             }
             return false;
         }
    }
}
