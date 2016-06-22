using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SMining.Core.Data;

namespace DummyBuilder.Process
{
    class DummyProcess1 : DataProcess
    {
        public DummyProcess1()
        {
            Name = "Dummy.Process1";
            InputType = typeof(List<DataRecord>);
            OutputType = typeof(List<DataRecord>);
            Description = "<h3> 프로세스1 </h3>" +
                            "더미 프로세스 1";

            // 옵션으로 설정 가능한 키들
            DataProperty[] optionProperties = {
                new DataProperty("Option.File", "", typeof(string), "파일"),
                new DataProperty("Option.Directory", "", typeof(string), "디렉토리"),

            };

            optionProperties[0].Set("Component.Type", "File");
            optionProperties[1].Set("Component.Type", "Directory");

            Set("Option.List", optionProperties);
        }

        public override void Run()
        {
            Output = Input;   
        }

        public override object Clone()
        {
            DummyProcess1 clone = new DummyProcess1();

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
