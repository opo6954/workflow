using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMining.Core.Data;

namespace DummyBuilder.Process
{
    class DummyProcess3 : DataProcess
    {
        public DummyProcess3()
        {
            Name = "Dummy.Process3";
            InputType = typeof(List<DataRecord>);
            OutputType = typeof(List<DataRecord>);
            Description = "<h3> 프로세스3 </h3>" +
                            "더미 프로세스 3";

            // 옵션으로 설정 가능한 키들
            DataProperty[] optionProperties = {
                new DataProperty("Option.Condition", "", typeof(string), "결합 속성")
            };
            Set("Option.List", optionProperties);
            Set("Input.Number", "string");
        }

        public override void Run()
        {
            Output = Input;   
        }

        public override object Clone()
        {
            DummyProcess3 clone = new DummyProcess3();

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
