using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SMining.Core.Data;
using SMiningClient.WPF;
using DummyBuilder.Process;

namespace DummyBuilder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        //function process is defined

        DummyProcessParent dummyProcessParent1;
        DummyProcessParent dummyProcessParent2;

        List<DummyProcessParent> dummyProcessList;
        

        
        public MainWindow()
        {
            InitializeComponent();
            
            dummyProcessParent1 = new DummyProcessParent(new List<DataProcess>{new DummyProcess1(), new DummyProcess2()},"DummyParent1");
            dummyProcessParent2 = new DummyProcessParent(new List<DataProcess> { new DummyProcess1(), new DummyProcess2(), new DummyProcess3() }, "DummyParent2");

            dummyProcessList = new List<DummyProcessParent> { dummyProcessParent1,dummyProcessParent2};

            
        }

        private void OnBuilderOpen(object sender, RoutedEventArgs e)
        {
            /*
            ProcessForm procForm = new ProcessForm(procList);
            procForm.Show();
             */

            ProcessFlowChart procFlow = new ProcessFlowChart(dummyProcessList);
            procFlow.Show();
        }
    }

    //dummy process의 parent를 위한 임시 class임, 걍 list of dataprocess랑 category name으로 구성됨
    [Serializable]
    public class DummyProcessParent
    {
        public List<DataProcess> procList = new List<DataProcess>();

        public String nameOfCategory;

        public DummyProcessParent(List<DataProcess> _procList, String name)
        {
            procList = _procList;
            nameOfCategory = name;
        }
    }
}
