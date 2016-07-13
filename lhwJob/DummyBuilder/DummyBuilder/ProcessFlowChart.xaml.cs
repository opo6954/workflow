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
using Northwoods.GoXam.Layout;
 

using SMining.Core.Data;
using SMiningClient.WPF;

namespace SMiningClient.WPF
{
    /// <summary>
    /// ProcessFlowChart.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    

    public partial class ProcessFlowChart : Window
    {
        public List<DataProcess> allProcList;
        public List<DataProcess> finalProcList;
        public List<DataProcess> result;
        int[,] adjMatrix;
        public MyNodeData start;
        public MyNodeData end;
        

        //treeview에서의 parent를 저장해놓기 위해서
        public List<TreeViewItem> parentTree = new List<TreeViewItem>();

        //실제 탐색하는 path --> 디버깅용
        public String pathName;

        DiagramLayout initLayout;

        
        public ProcessFlowChart(List<DataProcess> procList)
        {
            InitializeComponent();

            


            allProcList = procList;
            finalProcList = new List<DataProcess>();
            result = new List<DataProcess>();

            var model = new GraphLinksModel<MyNodeData, String, String, MyLinkData>();

            start = new MyNodeData() { Key="Start", isInit = false, Name="Start", Color="LightBlue", inputNum=0, outputNum=1, Location= new Point(10,10), _Deletable=false};
            end = new MyNodeData() { Key="End", isInit=false, Name="End", Color="LightBlue", inputNum=1, outputNum=0, Location = new Point(400, 550), _Deletable=false};

            model.NodesSource = new ObservableCollection<MyNodeData>() {
                start,end
            };

            model.Modifiable = true;
            model.HasUndoManager = true;

            myDiagram.Model = model;


            //treeview 형식으로 process 집어넣기
            //treeview 형식은 일단 parent가 존재하고 그 parent에 data process list가 있고 이름이 있음
            for (int idx = 0; idx < allProcList.Count; idx++)
            {
                var dataProcess = allProcList[idx];
                String currName = dataProcess.Name;

                String[] divided = currName.Split('.');

                //만일 parent 없는 경우 바로 말단으로 취급

                //3가지로 나누자, 제일 윗단 parent, 중간 parent, 말단 leaf로 나눕시다d

                var currParent = new TreeViewItem();

                for (int i = 0; i < divided.Length; i++)
                {

                    if (i == 0)//제일 윗단 parent
                    {
                        bool isExistParent = false;

                        foreach (TreeViewItem treeitem in parentTree)
                        {
                            if ((string)(treeitem.Header) == divided[i])
                            {
                                currParent = treeitem;
                                isExistParent = true;
                                break;
                            }
                        }

                        if (isExistParent == false)
                        {
                            TreeViewItem item = new TreeViewItem();
                            item.Header = divided[i];
                            item.AllowDrop = true;

                            parentTree.Add(item);

                            currParent = parentTree[parentTree.Count - 1];
                        }
                    }
                    else if (i > 0 && i < divided.Length - 1)//중간의 parent일 경우
                    {
                        TreeViewItem item = new TreeViewItem();
                        item.Header = divided[i];
                        item.AllowDrop = true;

                        currParent.Items.Add(item);

                        currParent = item;
                    }
                    else//완전 말단 leaf일 경우
                    {
                        var nodeData = new MyNodeData()
                        {
                            Key = idx.ToString(),
                            FuncBinding = (DataProcess)dataProcess.Clone(),
                            Name = dataProcess.Name,
                            ParentName = currParent.Header as String,
                            Color = "Lavender",
                            inputNum = 1,
                            outputNum = 1,
                            Deletable = true
                        };

                        // 개별 색상
                        int colorDiff = (int)(((float)idx / allProcList.Count) * 60);
                        nodeData.BodyColor2 = System.Windows.Media.Color.FromRgb((byte) (180 + colorDiff), (byte) 200, (byte)(220 - colorDiff));
                        

                        MyNodeTVVersion nodeItem = new MyNodeTVVersion(nodeData);
                        nodeItem.AllowDrop = true;
                        nodeItem.Header = divided[i];
                        
                        if (currParent != null)
                            currParent.Items.Add(nodeItem);
                    }
                }
            }
                

            foreach(TreeViewItem item in parentTree)
            {
                treeView.Items.Add(item);
            }


            initLayout = myDiagram.Layout as DiagramLayout;


        }


        
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
                    if (allProcList[i].Name == node.Name)
                    {
                        node.FuncBinding = (DataProcess)allProcList[i].Clone();
                        node.isInit = false;
                        break;
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
        
        //cycle존재를 확인하는 함수, 최초 발견한 cyclepath를 넘겨준다.
        private bool isExistCycle(int[,] adjMatrix, List<int> cyclePath)
        {
            int totalNode = adjMatrix.GetLength(0);
            bool[] isVisit = new bool[totalNode];
            bool[] isRecursiveStack = new bool[totalNode];
            

            //방문하지 않은 모든 node에 대해서 DFS 수행, recursive stack에 만날 시 cycle로 인정
            for (int i = 0; i < totalNode; i++)
            {
                //물론 cycle에서 어느 부분이 발생했는지 알려줘야 합니다.
                if (isVisit[i] == false)
                    if (cycleSearch(cyclePath,adjMatrix, i, ref isVisit, ref isRecursiveStack) == true)//1번이라도 cycle 발생시 바로 종료
                    {
                        return true;
                    }
            }
            return false;
        }

        //cycle을 확인하는 함수임
        public bool cycleSearch(List<int> previous, int[,] adjMatrix, int idx,ref bool[] isVisit,ref bool[] isRecursiveStack)
        {
            previous.Add(idx);
            if (isVisit[idx] == false)
            {
                isVisit[idx] = true;
                isRecursiveStack[idx] = true;
                
                for (int i = 0; i < adjMatrix.GetLength(0); i++)
                {
                    if (adjMatrix[idx, i] == 1)
                    {
                        if (isVisit[i] == false && cycleSearch(previous, adjMatrix, i, ref isVisit, ref isRecursiveStack) == true)
                        {
                            return true;
                        }
                        else if (isRecursiveStack[i] == true)//if it meets the recursiveStack, it is cycle, so return true
                        {
                            previous.Add(i);

                            

                            return true;
                        }
                    }
                }
                previous.RemoveAt(previous.Count - 1);
            }
            //if all jobs from the child are finished, it is deleted from the recursiveStack and return false
            isRecursiveStack[idx] = false;
            return false;
        }


        private void findJunctionNode(List<DataProcess> processList, int[,] adjMatrix, ref int[] totInNode,ref int[] totOutNode)
        {
            int sumInto = 0;
            int sumOutfrom = 0;
            for(int i=0; i<adjMatrix.GetLength(0); i++)
            {
                sumInto = 0;
                sumOutfrom = 0;

                for (int j = 0; j < adjMatrix.GetLength(0); j++)
                {
                    sumOutfrom = sumOutfrom + adjMatrix[i, j];//i에서 나가는 노드 총합
                    sumInto = sumInto + adjMatrix[j, i];//i로 들어오는 노드 총합
                }
                
                totInNode[i] = sumInto;//i번 노드의 들어오는 노드 총합
                totOutNode[i] = sumOutfrom;//i번 노드의 나가는 노드 총합
            }
        }
               
        //재귀호출로 chain 형태로 그래프를 변형하자
        //임시로 chainMultiComponent와 chainComponent라고 하는 클래스를 정의했는데 나중에 dataProcesschain 만드는 함수랑 합칠 수 있을듯 아직 chain만드는게 correct하지 않아서 일단 냅둡시다
        private int makeChainRecursive(List<DataProcess> processList, int[,] adjMatrix, int currIdx,
            int[] totInNode, int[] totOutNode, chainMultiComponent currChainComponent, chainMultiComponent prevChainComponent, ref List<int> nonSearchedIdx, ref List<chainMultiComponent> nonSearchCurrChain, bool isStart)
        {
            //만일 들어오는 node일 경우 그 idx를 리턴, 그 idx부터 다시 탐색을 시작한다

                if (totInNode[currIdx] > 1 && isStart == false)
                {
                    if (nonSearchedIdx.Contains(currIdx) == false)
                    {
                        nonSearchedIdx.Add(currIdx);
                        nonSearchCurrChain.Add(prevChainComponent);
                    }

                    return currIdx;
                }
                //그냥 단일 node일 경우 || 처음 시작점이 들어오는 노드일시 무시하고 검색 시작하자(이미 이전단계에서 하던거니까)
                if ((totInNode[currIdx] <= 1 && totOutNode[currIdx] <= 1) || (totOutNode[currIdx] <= 1 && isStart == true))
                {
                    chainComponent myChainComponent = new chainComponent();
                    myChainComponent.idx = currIdx;
                    currChainComponent.multiChain.Add(myChainComponent);

                    for (int i = 0; i < adjMatrix.GetLength(0); i++)
                    {
                        if (adjMatrix[currIdx, i] != 0)
                        {
                            makeChainRecursive(processList, adjMatrix, i, totInNode, totOutNode, currChainComponent, prevChainComponent, ref nonSearchedIdx, ref nonSearchCurrChain, false);
                        }
                    }
                }
                //만일 또 다른 나가는 node일 경우, 새롭게 chain을 만들자d
                if (totOutNode[currIdx] > 1)
                {
                    chainComponent myChainComponent = new chainComponent();
                    myChainComponent.idx = currIdx;
                    currChainComponent.multiChain.Add(myChainComponent);

                    for (int i = 0; i < adjMatrix.GetLength(0); i++)
                    {
                        if (adjMatrix[currIdx, i] != 0)
                        {
                            chainMultiComponent myChain = new chainMultiComponent();
                            currChainComponent.multiChain.Add(myChain);

                            makeChainRecursive(processList, adjMatrix, i, totInNode, totOutNode, myChain, currChainComponent, ref nonSearchedIdx, ref nonSearchCurrChain, false);
                        }
                    }
                }
            return 0;
        }

        //만들어진 chainComponent를 이용해서 dataProcessChain으로 변형하기, makeChainRecursive함수랑 추후에 합칠 수 있을듯
        private DataProcess makeDataProcessChain(chainComponent searchList, List<DataProcess> processList, int cnt)
        {
            if (searchList is chainMultiComponent)//list일시...
            {
                chainMultiComponent myMulti = searchList as chainMultiComponent;

                DataProcessChain myChain = new DataProcessChain();
                myChain.Name = "Data Chain" + cnt.ToString();

                for (int i = 0; i < myMulti.multiChain.Count; i++)
                {
                    myChain.addProcess(makeDataProcessChain(myMulti.multiChain[i],processList,cnt++));
                }
                return myChain;
            }
            else//single node일시...
            {
                if (searchList.idx > 1)
                {
                    
                    return processList[searchList.idx - 2].Clone() as DataProcess;
                }
                else
                {
                    
                    return processList[0];
                }
            }
        }

        //만들어진 chain을 확인하는 함수, 디버깅용
        private void searchProcessChain(List<DataProcess> processList, chainComponent searchList)
        {
            if (searchList is chainMultiComponent)
            {
                chainMultiComponent myMulti = searchList as chainMultiComponent;
                
                pathName = pathName + " [ ";

                for (int i = 0; i < myMulti.multiChain.Count; i++)
                {
                    searchProcessChain(processList, myMulti.multiChain[i]);
                }

                pathName = pathName + " ] ";
            }
            else
            {
                //pathName = pathName + " " + searchList.idx.ToString();
                
                if (searchList.idx > 1)
                    pathName = pathName + " " + processList[searchList.idx - 2].Name;
                else
                {
                    if (searchList.idx == 0)
                        pathName = pathName + " start ";
                    else if (searchList.idx == 1)
                        pathName = pathName + " end ";

                }
                  
            }   
        }

        //chain만드는 root함수
        private void buildProcessTreeWithChain(List<DataProcess> processList, int[,] adjMatrix)
        {
            int[] totInNode = new int[adjMatrix.GetLength(0)];
            int[] totOutNode = new int[adjMatrix.GetLength(0)];

            //모든 node의 들어오는 노드 총합 및 나가는 노드 총합을 totInNode랑 totOutNode에 저장한다
            findJunctionNode(processList, adjMatrix, ref totInNode, ref totOutNode);

            List<int> searchList = new List<int>();
            List<chainMultiComponent> searchListCurrChain = new List<chainMultiComponent>();



            searchList.Add(0);
            searchListCurrChain.Add(new chainMultiComponent());

            chainMultiComponent mainChain = new chainMultiComponent();

            int currIdx;

            while (searchList.Count != 0)
            {
                currIdx = searchList[0];

                makeChainRecursive(processList, adjMatrix, currIdx, totInNode, totOutNode, mainChain, mainChain, ref searchList, ref searchListCurrChain, true);
                searchList.RemoveAt(0);
                searchListCurrChain.RemoveAt(0);
            }


            searchProcessChain(finalProcList, mainChain);

            //디버깅용 path 출력
            MessageBox.Show(pathName);

            //clear
            pathName = "";

            
            DataProcessChain finalResult =  makeDataProcessChain(mainChain, processList,0) as DataProcessChain;

            //start랑 end node는 뺍시다
            //최종 list of dataprocess는 result에 저장됨
            for (int i = 1; i < finalResult.Process.Count-1; i++)
            {
                result.Add(finalResult.Process[i]);
            }
        }



        //최종 output은 finalProcList, 즉 모든 process가 들어간 list와 process의 연결 정보를 알려주는 adj matrix로 구성
        private void travelProcessTree(List<DataProcess> processList, int[,] adjMatrix)
        {
            int totalNode = adjMatrix.GetLength(0);//without start, end node
            int numProcess = totalNode-2;//including start, end node

            String res = "";

            bool[] isVisit = new bool[totalNode];
            List<int> searchList = new List<int>();

            searchList.Add(0);

            int currIdx;

            while (searchList.Count != 0)
            {
                currIdx = searchList[0];
                isVisit[currIdx] = true;

                if (currIdx > 1)
                    res = res + processList[currIdx-2] + " ";

                for (int i = 0; i < totalNode; i++)
                {
                    if (adjMatrix[currIdx, i] == 1 && isVisit[i] == false)
                        searchList.Insert(1, i);
                }

                searchList.RemoveAt(0);
            }
            
            

        }

        private bool buildProcessTree(List<Node> nodeList)
        {
            bool loopContinuous = false;

            Node currNode;

            String res = "";

            adjMatrix = new int[nodeList.Count, nodeList.Count];
            bool[] isVisit = new bool[nodeList.Count];

            
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

                isVisit[currNodeTemplate._idx] = true;

                if (currNodeTemplate.Name == "End")
                    loopContinuous = true;
                
                List<Link> linkOut = (currNode.LinksOutOf).ToList<Link>();

                //현재 나가는 link에 대해서 확인
                foreach (Link k in linkOut)
                {
                    Node nextNode = k.ToNode;
                    MyNodeData nextNodeTemplate = nextNode.Data as MyNodeData;
                    adjMatrix[currNodeTemplate._idx, nextNodeTemplate._idx] = 1;
                    
                    if (isVisit[nextNodeTemplate._idx] == false)
                    {
                        searchList.Insert(1, nextNodeTemplate._idx);
                    }
                }

                searchList.RemoveAt(0);//처음 index 제거
            }

            if (loopContinuous == false)
            {
                MessageBox.Show("시작에서 끝점까지 연결이 되어있지 않습니다.");
                return false;
            }
            else
            {
                List<int> cyclePath = new List<int>();

                bool isCycle = isExistCycle(adjMatrix,cyclePath);

                if (isCycle == true)
                {
                    int idx=-1;
                    String cyclePathTxt = "";

                    for (int i = 0; i < cyclePath.Count-1; i++)
                    {
                        if (cyclePath[i] == cyclePath[cyclePath.Count - 1])
                        {
                            idx = i;
                            break;
                        }
                    }
                    if (idx != -1)
                    {
                        for (int i = idx; i < cyclePath.Count; i++)
                        {
                            cyclePathTxt = cyclePathTxt + finalProcList[cyclePath[i] - 2].Name;
                            if (i < cyclePath.Count - 1)
                                cyclePathTxt = cyclePathTxt + "->";
                        }
                    }
                    
                    MessageBox.Show("cycle이 존재합니다. 다시 Process를 설정하세요\n" + "Cycle 위치: " + cyclePathTxt);
                    return false;
                }
                else
                {
                    travelProcessTree(finalProcList, adjMatrix);
                    buildProcessTreeWithChain(finalProcList, adjMatrix);
                }
                return true;
            }
        }

        private void treeView_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            DependencyObject uie = treeView.InputHitTest(e.GetPosition(treeView)) as DependencyObject;

            

            if (uie is TextBlock)
            {
                TreeViewItem tvi = e.Source as TreeViewItem;

                if (tvi.Items.Count == 0)//count가 0일 경우 말단 item임 따라서 드래그 앤 드랍 가능함
                {
                    
                    MyNodeTVVersion nodeTV = e.Source as MyNodeTVVersion;

                    nodeTV.Focus();
                    

                    if (nodeTV.myData._dataProcess != null)
                    {
                        string head = "<html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'></head>" +
                                    "<style type='text/css'> * { font-family: 맑은 고딕; font-size: 10pt; }; H2 { font-size: 14pt; }; </style><body>";
                        processDescription.NavigateToString(head + nodeTV.myData._dataProcess.Description + "</body></html>");
                    }

                    DragDrop.DoDragDrop(treeView, nodeTV, DragDropEffects.Move);
                    e.Handled = true;
                }
            }
        }

        private void myDiagram_Drop_1(object sender, DragEventArgs e)
        {
            MyNodeTVVersion nodeTV = e.Data.GetData(typeof(MyNodeTVVersion)) as MyNodeTVVersion;            
            MyNodeData generatedNode = nodeTV.myData.Clone() as MyNodeData;
            generatedNode.Location = e.GetPosition(myDiagram);

            // Deep Copy
            generatedNode._dataProcess = (DataProcess) nodeTV.myData._dataProcess.Clone();            
            
            // 드랍 지점을 센터로 조정
            Point loc = generatedNode.Location;
            loc.X -= 50;
            loc.Y -= 50;
            generatedNode.Location = loc;

            myDiagram.StartTransaction("add node");
            myDiagram.Model.AddNode(generatedNode);
            myDiagram.CommitTransaction("add node");
        }

        // 취소 버튼 클릭시
        private void OnCancelButonClick(object sender, RoutedEventArgs e)
        {
            result.Clear();
            finalProcList.Clear();
            Close();
        }
        //Layout 버튼 클릭시
        private void OnLayoutButtonClick(object sender, RoutedEventArgs e)
        {
            var lay = new Northwoods.GoXam.Layout.LayeredDigraphLayout();
            myDiagram.Layout = lay;
            myDiagram.Layout.DoLayout(myDiagram.Nodes, myDiagram.Links);

            myDiagram.Layout = initLayout;
        } 

        // 확인 버튼 클릭시
        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            saveCurrFlow();
        }
        //Debug Save 버튼 누를시 현재의 flow를 저장한다.

        private void saveCurrFlow()
        {
            finalProcList.Clear();
            List<Node> nodeList = myDiagram.Nodes.ToList<Node>();
            Node currNode = nodeList[0];

            //node 불러와서 표시하는 것만 하면 됩니다



            if ((nodeList[0].LinksOutOf).ToList<Link>().Count == 0)
            {
                MessageBox.Show("시작점이 연결되지 않았습니다.");
                return;
            }
            else if ((nodeList[1].LinksInto).ToList<Link>().Count == 0)
            {
                connect2End(nodeList);
            }

            if (buildProcessTree(nodeList))
            {
                //DEBUG
                //Close();
            }
        }
        private void OnDebugSaveFlow(object sender, RoutedEventArgs e)
        {
            saveCurrFlow();
            if (result.Count > 0)
                MessageBox.Show("최근 flow가 저장되었습니다.");
        }
        //Debug Load 버튼 누를 시 가장 최근에 저장된 flow를 불러온다.
        private void OnDebugLoadFlow(object sender, RoutedEventArgs e)
        {
            clearDiagram();

            if (result.Count == 0)
            {
                MessageBox.Show("저장된 flow가 없습니다");

            }
            else//저장된 게 있을 경우 result를 node랑 link로 재구성하자
            {
                reconProcessChain2Node(result);
            }
        }
        //Debug clear 버튼 누를 시 모든 flow가 지워진다d
        private void OnDebugClearFlow(object sender, RoutedEventArgs e)
        {
            result.Clear();
            finalProcList.Clear();

            clearDiagram();
        }


        private void connect2End(List<Node> nodeList)
        {
            myDiagram.StartTransaction("addLink2End");
            for (int i = 2; i < nodeList.Count; i++)
            {
                if ((nodeList[i].LinksOutOf).ToList<Link>().Count == 0)
                {
                    myDiagram.Model.AddLink(nodeList[i].Data, null, nodeList[1].Data, null);
                }
            }

            myDiagram.CommitTransaction("addLink2End");
        }

        private void clearDiagram()
        {
            

            int nodeN = myDiagram.Nodes.ToList<Node>().Count;
            int linkN = myDiagram.Links.ToList<Link>().Count;

            myDiagram.StartTransaction("deleteNode");

            for (int i = 2; i < nodeN; i++)
            {
                myDiagram.Model.RemoveNode(myDiagram.Nodes.ToList<Node>()[2].Data);
            }

            myDiagram.CommitTransaction("deleteNode");
        }

        private MyNodeData reconProcessChainInside(DataProcess currProcess)
        {
         
            



            //만일 child들이 있을 경우
            if (currProcess is DataProcessChain)
            {
                DataProcessChain dpc = currProcess as DataProcessChain;
                
                MyNodeData newParent = prevParent;


                bool isParallel = false;
                bool prevSingleNode = false;

                for (int i = 0; i < dpc.Items.Count; i++)
                {
                    //만일 chain일 경우

                    MyNodeData parentNode = currParent;
                    List<MyNodeData> chainLeafNode = new List<MyNodeData>();
                    if (dpc.Items[i] is DataProcessChain)
                    {
                        
                        if (prevSingleNode == true)//singleNode가 나온 이후 dpc가 나온 후
                        {
                            prevSingleNode = false;
                            isParallel = true;
                            MyNodeData myParent = currParent;

                            myParent = reconProcessChainInside(prevProcess, dpc.Items[i], prevParent, myParent);

                            chainLeafNode.Add(myParent);

                            



                        }
                    }
                    else//만일 걍 node일 경우
                    {
                        MyNodeData myNode = new MyNodeData()
                        {
                            FuncBinding = (DataProcess)currProcess.Clone(),
                            Name = currProcess.Name,
                            Color = "Lavender",
                            inputNum = 1,
                            outputNum = 1,
                            Deletable = true
                        };

                        myDiagram.StartTransaction("Add node");
                        myDiagram.Model.AddNode(myNode);
                        myDiagram.CommitTransaction("Add node");

                        if (isParallel == true)//dpc 탐색 다 끝난 후일시 이어붙이자
                        {
                            for (int j = 0; j < chainLeafNode.Count; j++)
                            {
                                myDiagram.StartTransaction("Add link");
                                myDiagram.Model.AddLink(chainLeafNode[j], null, myNode, null);
                                myDiagram.CommitTransaction("Add link");
                            }
                            isParallel = false;
                        }
                        else if (isParallel == false)
                        {
                            myDiagram.StartTransaction("Add link");
                            myDiagram.Model.AddLink(parentNode, null, myNode, null);
                            myDiagram.CommitTransaction("Add link");
                            
                            parentNode = myNode;
                            prevSingleNode = true;
                        }
                    }
                    
                }
            }
            else
            {
                MyNodeData myNode = new MyNodeData()
                {
                    FuncBinding = (DataProcess)currProcess.Clone(),
                    Name = currProcess.Name,
                    Color = "Lavender",
                    inputNum = 1,
                    outputNum = 1,
                    Deletable = true
                };

                myDiagram.StartTransaction("Add node");
                myDiagram.Model.AddNode(myNode);
                myDiagram.Model.AddLink(currParent, null, myNode, null);
                myDiagram.CommitTransaction("Add node");


                return myNode;

            }


            return null;

            

        }

        private void reconProcessChain2Node(List<DataProcess> result)
        {
            MyNodeData parent = start;

            DataProcessChain mainChain = new DataProcessChain();

            reconProcessChainInside(mainChain.Items[0] ,parent);


            myDiagram.StartTransaction("Add start link");
            myDiagram.Model.AddLink(start, null, myDiagram.Nodes.ToList<Node>()[3], null);
            myDiagram.CommitTransaction("Add start link");

            if(myDiagram.Nodes.ToList<Node>()[1].LinksInto.ToList<Link>().Count == 0)
                connect2End(myDiagram.Nodes.ToList<Node>());


        }

        /*
         *  int totalNode = adjMatrix.GetLength(0);//without start, end node
               int numProcess = totalNode-2;//including start, end node

               String res = "";

               bool[] isVisit = new bool[totalNode];
               List<int> searchList = new List<int>();

               searchList.Add(0);

               int currIdx;

               while (searchList.Count != 0)
               {
                   currIdx = searchList[0];
                   isVisit[currIdx] = true;

                   if (currIdx > 1)
                       res = res + processList[currIdx-2] + " ";

                   for (int i = 0; i < totalNode; i++)
                   {
                       if (adjMatrix[currIdx, i] == 1 && isVisit[i] == false)
                           searchList.Insert(1, i);
                   }

                   searchList.RemoveAt(0);
               }
         * */




    }

    [Serializable]
    public class chainComponent
    {
        public int idx;//index of single component

        public chainComponent()
        {
            
        }
    };
    [Serializable]
    public class chainMultiComponent : chainComponent
    {
        public List<chainComponent> multiChain;

        public chainMultiComponent()
        {
            multiChain = new List<chainComponent>();
        }

    }
        


    [Serializable]
    public class MyNodeTVVersion : TreeViewItem
    {
        public MyNodeData myData;

        public MyNodeTVVersion(MyNodeData _myData)
        {
            myData = _myData;
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

        public Color BodyColor1 { get; set; }
        public Color BodyColor2 { get; set; }

        public MyNodeData()
        {
            BodyColor1 = System.Windows.Media.Color.FromRgb(255, 255, 255);
            BodyColor2 = System.Windows.Media.Color.FromRgb(220, 220, 230);
        }
        

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
