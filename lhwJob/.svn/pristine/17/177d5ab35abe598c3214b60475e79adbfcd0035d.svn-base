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
        List<DataProcess> myDataProcess = new List<DataProcess>();
        

        
        public MainWindow()
        {
            InitializeComponent();

            myDataProcess.Add(new DummyProcess1());
            myDataProcess.Add(new DummyProcess2());
            myDataProcess.Add(new DummyProcess3());


        }

        private void OnBuilderOpen(object sender, RoutedEventArgs e)
        {
            /*
            ProcessForm procForm = new ProcessForm(myDataProcess);
            procForm.Show();
             */
            
            ProcessFlowChart procFlow = new ProcessFlowChart(myDataProcess);
            procFlow.Show();
            
        }
    }

}
