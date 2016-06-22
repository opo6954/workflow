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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;


using SMining.Core.Data;
using SMiningClient.WPF;

namespace DummyBuilder
{
    /// <summary>
    /// ProcessFlowChart.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    

    public partial class ProcessFlowChart : Window
    {
        public List<DummyProcessParent> allProcList;
        public List<DataProcess> finalProcList;
        int[,] adjMatrix;
        public MyNodeData start;
        public MyNodeData end;

        
        

        public ProcessFlowChart(List<DummyProcessParent> procList)
        {
            InitializeComponent();



            allProcList = procList;
            finalProcList = new List<DataProcess>();

            var model = new GraphLinksModel<MyNodeData, String, String, MyLinkData>();

            start = new MyNodeData() { Key="Start", isInit = false, Name="Start", Color="LightBlue", inputNum=0, outputNum=1, Location= new Point(10,10), _Deletable=false};
            end = new MyNodeData() { Key="End", isInit=false, Name="End", Color="LightBlue", inputNum=1, outputNum=0, Location = new Point(200,10), _Deletable=false};

            model.NodesSource = new ObservableCollection<MyNodeData>() {
                start,end
            };

            model.Modifiable = true;
            model.HasUndoManager = true;

            myDiagram.Model = model;
            myDiagram.AllowDrop = true;



            /*
             * <go:Diagram.Layout>
                <go:GridLayout CellSize="5 5" WrappingColumn="1" Sorting="Forward" />
            </go:Diagram.Layout>
             * */

            int idx = 0;


            //treeview 형식으로 process 집어넣기
            //treeview 형식은 일단 parent가 존재하고 그 parent에 data process list가 있고 이름이 있음
            foreach(DummyProcessParent processParent in allProcList)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = processParent.nameOfCategory;

                Palette p = new Palette();
                p.Model = new GraphLinksModel<MyNodeData, String, String, MyLinkData>();
                p.NodeTemplate = (DataTemplate)FindResource("MyNodeTemplate");
                p.BorderBrush = Brushes.Azure;
                p.BorderThickness = new Thickness(1);
                p.Width = 267;
                p.Height = 303;
                p.HorizontalAlignment = HorizontalAlignment.Left;
                p.VerticalAlignment = VerticalAlignment.Top;
                p.Background = Brushes.White;

                p.GridSnapCellSize = new Size(5,5);

                Northwoods.GoXam.Layout.GridLayout g = new Northwoods.GoXam.Layout.GridLayout();

                g.CellSize = new Size(5, 5);
                g.WrappingColumn = 1;
                g.Sorting = Northwoods.GoXam.Layout.GridSorting.Forward;

                p.Layout = g;

                
                //WrappingColumn="1" Sorting="Forward"
                List<MyNodeData> initNodeData = new List<MyNodeData>();
                
                foreach (DataProcess dataProcess in processParent.procList)
                {
                    initNodeData.Add(new MyNodeData() { Key = idx.ToString(), FuncBinding = (DataProcess)dataProcess.Clone(), Name = dataProcess.Name, ParentName=processParent.nameOfCategory, Color = "Lavender", inputNum = 1, outputNum = 1, Deletable = true });
                }

                

                p.Model.NodesSource = initNodeData;

                item.Items.Add(p);

                treeView.Items.Add(item);
                
            }



            //myPalette2.Model.NodesSource = initNodeData;
        }

        //adding item to children in stackpanel...

        
        //processor 아이콘 누를 경우의 이벤트로서 누르면 description하고 parameter 입력하는 창 갱신해야함
        private void Icon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyNodeData node = Part.FindAncestor<Node>(sender as UIElement).Data as MyNodeData;
            
            Node n = Part.FindAncestor<Node>(sender as UIElement);

            if (node._dataProcess != null)
            {
                string head = "<html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'></head>" +
                            "<style type='text/css'> * { font-family: 맑은 고딕; font-size: 10pt; }; H2 { font-size: 14pt; }; </style><body>";
                processDescription.NavigateToString(head + node._dataProcess.Description + "</body></html>");
            }
            if (node._dataProcess != null)
            {
                OnParameterInput(node);
            }
        }

        private void OnParameterInput(MyNodeData node)
        {
            if (node.isInit == true)
            {
                for (int i = 0; i < allProcList.Count; i++)
                {
                    if (allProcList[i].nameOfCategory == node.ParentName)
                    {
                        for (int j = 0; j < allProcList[i].procList.Count; j++)
                        {
                            if (allProcList[i].procList[j].Name == node.FuncBinding.Name)
                            {
                                node.FuncBinding = (DataProcess)allProcList[i].procList[j].Clone();
                                node.isInit = false;
                                break;
                            }
                        }
                    }
                }
            }

            DataProcess dp = node.FuncBinding;

            panelLayout.Children.Clear();

            if (dp.IsSet("Option.List"))
            {
                DataProperty[] options = (DataProperty[])dp.Get("Option.List");

                int idx = 0;
                Thickness bottomMargin = new Thickness(0, 0, 0, 15);
                foreach (DataProperty op in options)
                {
                    Label l = new Label();                    
                    l.Content = op.Description;
                    panelLayout.Children.Add(l);

                    String compType = "";
                    if (op.IsSet("Component.Type"))
                    {
                        compType = (String)op.Get("Component.Type");
                    }

                    if (compType.StartsWith("File") || compType.StartsWith("Directory"))
                    {
                        // 필드
                        TextBox box = new TextBox();
                        box.Text = op.Value.ToString();
                        box.IsReadOnly = true;
                        box.VerticalAlignment = VerticalAlignment.Top;
                       
                        panelLayout.Children.Add(box);                        

                        // 버튼
                        Button btn = new Button();
                        btn.Height = 20;
                        btn.Content = "선택";
                        btn.VerticalAlignment = VerticalAlignment.Top;
                        btn.Margin = bottomMargin;

                        panelLayout.Children.Add(btn);
                        
                        // 이벤트 핸들러
                        FileEventHandler fe = new FileEventHandler();
                        fe.DirectoryMode = compType.StartsWith("Directory");
                        fe.MultiSelectMode = compType.EndsWith("Multi");
                        fe.DataProcess = dp;
                        fe.OptionProperty = op;
                        fe.TextBox2 = box;
                        btn.Click += fe.handleEvent;
                    }

                    else if (compType == "TextArea")
                    {
                        TextBox box = new TextBox();
                        box.Text = op.Value.ToString();
                        box.Margin = bottomMargin;

                        panelLayout.Children.Add(box);                        

                        ComponentEventHandler c = new ComponentEventHandler();
                        c.DataProcess = dp;
                        c.OptionProperty = op;
                        box.TextChanged += c.handleEvent;
                    }

                    else if (compType == "ComboBox")
                    {
                        ComboBox cBox = new ComboBox();
                        cBox.Text = op.Value.ToString();

                        String[] values;
                        if (op.IsSet("DefaultValues"))
                        {
                            values = (String[])op.Get("DefaultValues");
                        }
                        else
                        {
                            values = op.Value.ToString().Split(',');
                            op.Set("DefaultValues", values);
                            op.Value = values[0];
                        }
                        foreach (String s in values)
                        {
                            cBox.Items.Add(s.Trim());
                        }
                        dp.Set(op.Name, op.Value);

                        if (values != null)
                        {
                            cBox.SelectedItem = op.Value.ToString();
                        }
                        else
                        {
                            cBox.SelectedIndex = 0;
                        }

                        cBox.Margin = bottomMargin;
                        panelLayout.Children.Add(cBox);

                        ComboBoxEventHandler c = new ComboBoxEventHandler();
                        c.DataProcess = dp;
                        c.ComboBox2 = cBox;
                        c.OptionProperty = op;
                        cBox.SelectionChanged += c.handleEvent;
                    }

                    else
                    {
                        TextBox box = new TextBox();
                        box.Text = op.Value.ToString();
                        box.Margin = bottomMargin;
                        box.VerticalAlignment = VerticalAlignment.Top;

                        panelLayout.Children.Add(box);
                        
                        ComponentEventHandler c = new ComponentEventHandler();
                        c.DataProcess = dp;
                        c.OptionProperty = op;
                        box.TextChanged += c.handleEvent;                        
                    }
                    idx = idx + 1;
                }                                
            }
            



        }
   
        private Node findNodeWithStr(String name, List<Node> list)
        {
            foreach (Node n in list)
            {
                MyNodeData myData = n.Data as MyNodeData;

                if (myData.Key == name)
                    return n;
            }
            return null;
        }


        private void button_OK_Click_1(object sender, RoutedEventArgs e)
        {
            finalProcList.Clear();
            

            

            List<Node> nodeList = myDiagram.Nodes.ToList<Node>();
            Node currNode = nodeList[0];

            if ((nodeList[0].LinksOutOf).ToList<Link>().Capacity == 0)
                MessageBox.Show("시작점이 연결되지 않았습니다.");
            else if ((nodeList[1].LinksInto).ToList<Link>().Capacity == 0)
                MessageBox.Show("종료점이 연결되지 않았습니다.");
            else
                buildProcessTree(nodeList);


        }
        //cycle 여부 확인하는 함수
        //요거만 짜면 될듯
        private bool isExistCycle()
        {


            return false;
        }
        

        //최종 output은 finalProcList, 즉 모든 process가 들어간 list와 process의 연결 정보를 알려주는 adj matrix로 구성, 탐색하는 거는 따로 구현을 다시 해야함
        private void travelProcessTree(List<DataProcess> processList, int[,] adjMatrix)
        {
            int totalNode = adjMatrix.GetLength(0);//without start, end node
            int numProcess = totalNode-2;//including start, end node

            String res = "";

            bool[] visit = new bool[totalNode];
            List<int> searchList = new List<int>();

            

            searchList.Add(0);

            int currIdx;

            while (searchList.Count != 0)
            {
                currIdx = searchList[0];

                if (currIdx > 1)
                    res = res + processList[currIdx-2] + " ";


                for (int i = 0; i < totalNode; i++)
                {
                    if (adjMatrix[currIdx, i] == 1)
                        searchList.Insert(1, i);
                }

                searchList.RemoveAt(0);
            }

            MessageBox.Show(res);


        }

        private void buildProcessTree(List<Node> nodeList)
        {
            bool endLoop = false;
            bool success = false;
            bool noInit = false;
            bool loopContinuous = false;

            Node currNode;

            String res = "";

            adjMatrix = new int[nodeList.Count, nodeList.Count];
            bool[] visit = new bool[nodeList.Count];

            
            List<int> searchList = new List<int>();
            
            

            searchList.Add(0);

            

            //연결된 모든 process를 한 list에 몽땅 집어넣자
            for (int i = 0; i < nodeList.Count; i++)
            {
                if(i>1)//except start and end node
                    finalProcList.Add((nodeList[i].Data as MyNodeData).FuncBinding);
                (nodeList[i].Data as MyNodeData)._idx = i;//finalProcList에 들어간 idx를 말함

            }

            while (searchList.Count != 0)
            {
                //현재 node의 나가는 link를 list로 저장
                currNode = nodeList[searchList[0]];
                MyNodeData currNodeTemplate = currNode.Data as MyNodeData;

                res = res + currNodeTemplate.Name + " ";

                visit[currNodeTemplate._idx] = true;

                if (currNodeTemplate.Name == "End")
                    loopContinuous = true;
                
                List<Link> linkOut = (currNode.LinksOutOf).ToList<Link>();

                //현재 나가는 link에 대해서 확인
                foreach (Link k in linkOut)
                {
                    Node nextNode = k.ToNode;
                    MyNodeData nextNodeTemplate = nextNode.Data as MyNodeData;

                    if (visit[nextNodeTemplate._idx] == false)
                    {
                        searchList.Insert(1, nextNodeTemplate._idx);
                        adjMatrix[currNodeTemplate._idx, nextNodeTemplate._idx] = 1;
                    }
                }

                searchList.RemoveAt(0);//처음 index 제거
            }

            if (loopContinuous == false)
            {
                MessageBox.Show("시작에서 끝점까지 연결이 되어있지 않습니다.");
            }
            else
            {
                MessageBox.Show(res);
            }

            travelProcessTree(finalProcList, adjMatrix);
            


            /*
      
                while (endLoop == false)
                {
                    List<Link> linkOut = (currNode.LinksOutOf).ToList<Link>();
                    int cnt = 0;
                    foreach (Link k in linkOut)
                    {
                        Node nextNode = k.ToNode;

                        if (nextNode == null)
                        {
                            cnt++;
                        }

                        if ((nextNode.Data as MyNodeData).Name == "End")
                        {
                            success = true;
                            endLoop = true;
                        }
                        else if ((nextNode.Data as MyNodeData).isInit == true)
                        {
                            noInit = true;
                            endLoop = true;
                        }
                        else
                        {
                            DataProcess dp = (nextNode.Data as MyNodeData)._dataProcess;

                            if (dp != null)
                            {
                                finalProcList.Add(dp);

                                Node next = findNodeWithStr((nextNode.Data as MyNodeData).Key, nodeList);
                                if (next != null)
                                    currNode = next;
                            }
                        }
                    }

                    if (cnt == linkOut.Count)
                    {                        
                        endLoop = true;
                    }
                }
             * */
        }

        //주어진 process list를 순서대로 수행하는 함수(BFS 방식으로 수행될듯)
        private void searchDataProcessList()
        {
            
        }


        private void button_Cancel_Click_1(object sender, RoutedEventArgs e)
        {
            finalProcList.Clear();
        }
    }


    [Serializable]  // serializable in WPF to support the clipboard
    public class MyNodeData : GraphLinksModelNodeData<String>
    {
        public string _name;
        public string _parentName;
        public int _inputNum;
        public int _outputNum;
        public bool _Deletable;
        public bool[] _availableInput = new bool[3]{false,false,false};
        public bool[] _availableOutput = new bool[3]{false,false,false};
        public bool isInit = true;
        public int _idx;
        

        public DataProcess _dataProcess;


        public DataProcess FuncBinding
        {
            get { return _dataProcess; }
            set
            {
                _dataProcess = (DataProcess)value.Clone();
            }
        }

        public String ParentName
        {
            get { return _parentName; }
            set
            {
                _parentName = value;
            }

        }

        public String Name
        {
            get { return _name; }
            set
            {
                int idx = value.LastIndexOf('.');
                if (idx >= 0)
                {
                    _name = value.Substring(idx + 1);
                }
                else
                {
                    _name = value;
                }                
            }
        }
    
        public String Color
        {
            get { return _Color; }
            set
            {
                if (_Color != value)
                {
                    String old = _Color;
                    _Color = value;
                }
            }
        }

        public bool Deletable
        {
            get { return _Deletable; }
            set
            {
                _Deletable = value;
            }
        }


        public int inputNum
        {
            get { return _inputNum; }
            set
            {
                _inputNum = value;
                for (int i = 0; i < _inputNum; i++)
                {
                    _availableInput[i] = true;
                }
            }
        }

        public int outputNum
        {
            get { return _outputNum; }
            set
            {
                _outputNum = value;
                for (int i = 0; i < _outputNum; i++)
                    _availableOutput[i] = true;
            }
        }

        public bool[] AvailableInput
        {
            get
            {
                return _availableInput;
            }
        }

        public bool[] AvailableOutput
        {
            get
            {
                return _availableOutput;
            }
        }

        private String _Color = "White";
    }

    [Serializable]
    public class MyLinkData : GraphLinksModelLinkData<String, String>
    {

    }
}
