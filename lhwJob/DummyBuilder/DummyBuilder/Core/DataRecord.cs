using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SMining.Core.Data
{
    /**
     * 일반 데이터 레코드 클래스, 이 클래스는 DataProperty들을 저장하는 것으로
     * 하나의 속성의 집합, 즉, 레코드를 이루며 DataProperty와 함께 비정형 데이터를
     * 다루는 클래스로서 사용된다
     */
    [Serializable]
    public class DataRecord
    {
        /**
         * 데이터 레코드를 내용이 아니라 개별적으로 부여한 ID로 구분해야할 때 사용
         */
        public string ID { get; set; }


        /**
         * 간단하게 데이터레코드를 생성하기 위한 메소드.
         * 매개변수로 { 이름, 값, 이름, 값 ... } 형식의 배열을 받는다
         */
        public static DataRecord create(object[] nameAndValues)
        {
            DataRecord r = new DataRecord();

            for (int i = 0; i < nameAndValues.Length; i += 2)
            {
                r.Properties.Add(new DataProperty(nameAndValues[i].ToString(), nameAndValues[i+1], nameAndValues[i+1].GetType()));
            }
            return r;
        }

        /**
         * 간단하게 데이터레코드를 생성하기 위한 메소드.
         * 매개변수로 { 이름, 값, 이름, 값 ... } 형식의 2중 배열을 받는다
         */
        public static List<DataRecord> create(object[][] nameAndValues)
        {
            List<DataRecord> dataList = new List<DataRecord>();
            for (int i = 0; i < nameAndValues.Length; i++)
            {
                dataList.Add(create(nameAndValues[i]));
            }
            return dataList;
        }

        // 데이터 프로퍼티 목록
        private List<DataProperty> properties = new List<DataProperty>();
        // private ObservableCollection<DataProperty> properties = new ObservableCollection<DataProperty>();

        public DataRecord(params DataProperty[] properties)
        {
            foreach (var property in properties)
                Properties.Add(property);
        }

        /**
         * 레코드가 가지고 있는 프로퍼티 목록을 반환한다
         * 
         * @return 데이터 프로퍼티 목록
         */
        public List<DataProperty> Properties
        {
            get { return properties; }
        }

        /**
         * 레코드가 가지고 있는 i번째 프로퍼티의 이름을 반환한다
         * 
         * @param[in] i 프로퍼티 인덱스
         * @return i번째 프로퍼티 이름
         */
        public String getNameAt(int i)
        {
            return properties[i].Name;
        }

        /**
         * 레코드가 가지고 있는 i번째 프로퍼티 값을 반환한다
         * 
         * @param[in] i 프로퍼티 인덱스
         * @return i번째 프로퍼티 값
         */
        public Object getValueAt(int i)
        {
            return properties[i].Value;
        }

        /**
         * 레코드가 가지고 있는 특정 이름의 데이터 프로퍼티를 반환한다.
         * 없으면 null을 반환한다.
         */
        public DataProperty getPropertyOf(String name)
        {
            int idx = indexOf(name);
            if (idx >= 0)
            {
                return Properties[idx];
            }
            return null;
        }

        /**
         * 레코드가 가지고 있는 특정 이름의 데이터 프로퍼티의 값을 반환한다.
         * 없으면 null을 반환한다.
         */
        public object getValueOf(String name)
        {
            int idx = indexOf(name);
            if (idx >= 0)
            {
                return Properties[idx].Value;
            }
            return null;
        }

        /**
         * 레코드가 가지고 있는 프로퍼티의 수를 반환한다.
         */
        public int Count { get { return properties.Count; } }

        /**
         * key를 가지고 있는 프로퍼티의 수를 반환한다.
         * 
         * @param[in] key 키 값
         *
         * @return key 값을 가지고 있는 프로퍼티의 수
         */
        public int CountWithKey(String key)
        {       
            int count = 0;
            foreach (DataProperty p in Properties)
            {
                if (p.IsSet(key)) count++;
            }
            return count;
        }

        /**
         * 레코드가 가지고 있는 프로퍼티 중 해당 키를 소유하고 있는
         * 첫번째 프로퍼티의 인덱스를 반환한다. 없으면 -1을 반환한다.
         * 
         * @param[in] key 키 값
         * @param[in] start 탐색을 시작할 위치
         * @return 해당 키를 소유하고 있는 프로퍼티의 첫번째 인덱스
         */
        public int IndexOfKey(String key, int start)
        {
            for (int i = start; i < Properties.Count; i++)
            {
                if (Properties[i].IsSet(key))
                    return i;
            }
            return -1;
        }

        /**
         * 레코드가 가지고 있는 프로퍼티 중 해당 키들을 소유하고 있는 프로퍼티에 대한
         * 인덱스 배열을 반환한다.
         * 
         * 예)
         * "Display" 키를 가지면서 "Label" 키를 가지지 않는 프로퍼티 인덱스
         * IndiciesOfKey("Display", "!Label");
         * 
         * @param[in] keys 키 값의 배열
         * @return 해당 키에 대한 검사 결과
         */
        public int[] IndiciesOfKey(params string[] keys)
        {
            // 조건에 맞는 인덱스를 얻어옴
            ArrayList indexList = new ArrayList();
            for (int i = 0; i < Properties.Count; i++)
            {
                DataProperty p = Properties[i];
                int count = 0;
                foreach (String condition in keys)
                {
                    // Not 조건
                    if (condition.StartsWith("!"))
                    {
                        if (!p.IsSet(condition.Substring(1))) count++;
                        else break;
                    }
                    else
                    {
                        if (p.IsSet(condition)) count++;
                        else break;
                    }
                }
                if (count == keys.Length) { indexList.Add(i); }
            }

            int[] indices = new int[indexList.Count];
            for (int i = 0; i < indexList.Count; i++)
            {
                indices[i] = (int) indexList[i];
            }
            return indices;
        }

        /**
         * 레코드가 가지고 있는 프로퍼티 중 해당 키를 소유하고 있는지에 대한
         * 검사 결과 배열을 반환한다.
         * 
         * @param[in] key 키 값
         * @return 해당 키에 대한 검사 결과
         */
        public bool[] checkOfKey(String key)
        {
            bool[] result = new bool[Count];
            for (int i = 0; i < Properties.Count; i++)
            {
                result[i] = Properties[i].IsSet(key);                    
            }
            return result;
        }

        /**
         * 레코드의 특정 프로퍼티의 인덱스를 반환한다
         * 
         * @param[in] name 인덱스를 찾고자 하는 프로퍼티의 이름
         * @return name 프로퍼티의 인덱스
         */
        public int indexOf(String name)
        {
            for(int i = 0; i < properties.Count; i++)
            {
                DataProperty p = properties[i];
                if (p.Name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        /**
         * 레코드 내부의 데이터를 double 배열로 반환한다.
         * 불가능할 경우, 예외가 발생할 수 있다.
         */
        public double[] toDoubleArray()
        {
            double[] da = new double[Properties.Count];
            for (int i = 0; i < da.Length; i++)
            {
                object val = Properties[i].Value;
                if (val.GetType() == typeof(int))
                {
                    da[i] = (int) val;
                }
                else if (val.GetType() == typeof(double))
                {
                    da[i] = (double)val;
                }
                else
                {
                    throw new Exception(Properties[i].Name + " is not interger or double");
                }
            }
            return da;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DataRecord))       return false;
            
            DataRecord other = (DataRecord)obj;

            // ID가 부여되어 있지 않을 경우 내용을 비교
            if (ID == null)
            {                
                if (Properties.Count != other.Properties.Count)
                {
                    return false;
                }

                for (int i = 0; i < Properties.Count; i++)
                {
                    if (!Properties[i].Value.Equals(other.Properties[i].Value))
                        return false;
                }
                return true;
            }
            // ID가 부여되어 있을 경우 ID를 비교
            else
            {
                return ID.Equals(other.ID);
            }
            
        }

        public override int GetHashCode()
        {
            if (ID == null)
            {
                string s = "";
                for (int i = 0; i < Properties.Count; i++)
                {
                    s += Properties[i].Value.ToString();
                }
                return s.GetHashCode();
            }
            else
            {
                return ID.GetHashCode();
            }            
        }
    }
}
