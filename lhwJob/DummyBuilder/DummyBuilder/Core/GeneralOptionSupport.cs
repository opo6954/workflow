using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMining.Core.Data
{
    /**
     * 일반 옵션 설정 기능을 지원하는 클래스.
     * 이 클래스를 상속받는 클래스들은 모두 Set, Get, IsSet 옵션을 사용할 수 있다.
     */
    [Serializable]
    public class GeneralOptionSupport
    {
        protected Dictionary<String, Object> optionMap;

        /**
        * 특정한 키 값을 설정한다
        * 특정한 상태 설정용으로 쓰인다.
        * 
        * @param[in] key 설정할 키 이름
        */
        public virtual void Set(String key)
        {
            Set(key, new Object());
        }

        /**
         * 특정한 키 값의 설정을 해제한다.
         * 
         * @param[in] key 해제할 키 이름
         */
        public virtual void UnSet(String key)
        {
            if (IsSet(key))
            {
                optionMap.Remove(key);
            }
        }

        /**
         * 특정한 키 값이 설정되어 있는지를 조사한다.
         * 
         * @param[in] key 조사할 키 이름
         * @return 키가 존재하는지 여부
         */
        public virtual bool IsSet(String key)
        {
            if (optionMap == null) return false;
            return optionMap.ContainsKey(key);
        }


        /**
         * 프로퍼티에 특정한 값을 추가한다
         * 
         * @param[in] key 설정할 값 이름
         * @param[in] value 설정할 값
         */
        public virtual Object Set(String key, Object value)
        {
            Object before_value = null;
            if (optionMap == null)
            {
                optionMap = new Dictionary<String, Object>(5);
            }
            if (optionMap.ContainsKey(key))
            {
                before_value = optionMap[key];
                optionMap.Remove(key);
            }
            // 특수 케이스 : optionProperties가 있는 경우
            else if (IsSet("Option.List"))
            {
                DataProperty[] props = (DataProperty[]) Get("Option.List");
                foreach (DataProperty dp in props)
                {
                    if (dp.Name == key)
                    {
                        before_value = dp.Value;
                        dp.Value = value;
                        break;
                    }
                }
            }
            optionMap.Add(key, value);
            return before_value;
        }

        /**
         * 프로퍼티에서 특정한 값을 가져온다
         * 
         * @param[in] key 가져올 값의 이름
         */
        public virtual Object Get(String key)
        {
            if (IsSet(key))
            {
                return optionMap[key];
            }
            
            // 특수 케이스 : optionProperties가 있는 경우
            if (IsSet("Option.List"))
            {
                DataProperty[] props = (DataProperty[])Get("Option.List");
                foreach (DataProperty dp in props)
                {
                    if (dp.Name == key)
                    {
                        return dp.Value;
                    }
                }
            }
            return null;
        }

    }
}
