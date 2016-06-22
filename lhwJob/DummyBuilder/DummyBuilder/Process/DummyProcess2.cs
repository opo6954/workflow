using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMining.Core.Data;

namespace DummyBuilder.Process
{
    class DummyProcess2 : DataProcess
    {
        public DummyProcess2()
        {
            Name = "Dummy.Process2";
            InputType = typeof(List<DataRecord>);
            OutputType = typeof(List<DataRecord>);
            Description = "<h3> 프로세스2 </h3>" +
                            "더미 프로세스 2";

            // 옵션으로 설정 가능한 키들
            DataProperty[] optionProperties = {
                new DataProperty("Option.HeaderLine", 0, typeof(int), "줄 번호"),
                new DataProperty("Option.Properties", "", typeof(string), "속성")
            };
            
            Set("Option.List", optionProperties);
        }

        public override void Run()
        {
            Output = Input;   
        }

        public override object Clone()
        {
            DummyProcess2 clone = new DummyProcess2();

            clone.Input = Input;
            clone.Output = Output;

            if (optionMap != null)
            {
                foreach (String key in optionMap.Keys)
                {
                    // 개별 설정이 필요한 옵션을 빼고 복사
                    if (key != "Option.List")
                    {
                        clone.Set(key, optionMap[key]);
                    }
                }
            }
            return clone;
        }
    }
}
