using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using SMining.Core.Data;

namespace SMiningClient.WPF
{
    public partial class ProcessForm : Form
    {
        // 처리할 프로세스 체인
        private DataProperty procMap = new DataProperty();

        // 생성자
        public ProcessForm(List<DataProcess> procList)
        {
            InitializeComponent();
            // 리스트 목록에 프로세스 추가
            foreach(DataProcess proc in procList) {

                procMap.Set(proc.Name, proc);

                // 이름을 구분자로 나눔
                string[] cat_names = proc.Name.Split('.');

                // 최상위 목록 탐색
                TreeNodeCollection c = procTreeView.Nodes;
                TreeNode n = getChildNodeByName(cat_names[0], procTreeView.Nodes);
                if (n == null)
                {
                    n = new TreeNode(cat_names[0]);
                    procTreeView.Nodes.Add(n);
                }

                // 이하 반복으로 추가
                TreeNode child = null;
                for (int i = 1; i < cat_names.Length; i++)
                {
                    string cname = cat_names[i];
                    child = getChildNodeByName(cat_names[0], n.Nodes);
                    if (child == null)
                    {
                        child = new TreeNode(cname);
                        n.Nodes.Add(child);
                    }
                    n = child;
                }
                
            }
        }

        private TreeNode getChildNodeByName(string name, TreeNodeCollection n)
        {
            TreeNode c = null;
            if (n != null)
            {
                for (int i = 0; i < n.Count; i++)
                {
                    TreeNode n1 = n[i];
                    if (n1.Text == name)
                    {
                        c = n[i];
                        break;
                    }
                }
            }            
            return c;
        }

        // 폼 초기화
        private void onFormLoad(object sender, EventArgs e)
        {
            
        }

        // OK 버튼 클릭
        private void onOKClick(object sender, EventArgs e)
        {
            
        }

        // Cancel 버튼 클릭
        private void onCancelClick(object sender, EventArgs e)
        {
            Dispose();
        }

        // 추가 버튼 클릭
        private void onAddBtnClick(object sender, EventArgs e)
        {
            // 선택한 아이템을 선택 목록에 추가
            TreeNode n = procTreeView.SelectedNode;
            if (n != null)
            {
                string name = "";
                while (n != null)
                {
                    name = n.Text + name;
                    if (n.Parent != null)
                    {
                        name = "." + name;
                    }
                    n = n.Parent;
                }

                object obj = (DataProcess)procMap.Get(name);
                if (obj != null)
                {
                    DataProcess dp = (DataProcess)((DataProcess)obj).Clone();
                    selectedList.Items.Add(dp);

                    // 옵션에 대한 기본값 설정
                    DataProperty[] options = (DataProperty[]) dp.Get("Option.List");
                    foreach (DataProperty p in options)
                    {
                        dp.Set(p.Name, p.Value);
                    }
                }
            }
        }

        // 제거 버튼 클릭
        private void onDelBtnClick(object sender, EventArgs e)
        {
            // 선택된 아이템을 선택 목록에서 제외
            int idx = selectedList.SelectedIndex;
            if (idx >= 0)
            {
                selectedList.Items.RemoveAt(idx);
                if (idx > 0)    selectedList.SelectedIndex = idx - 1;
            }
        }

        // 프로세스 후보 목록 선택
        private void onCandidateSelection(object sender, TreeViewEventArgs e)
        {
            // 선택한 아이템의 설명을 디스플레이
            // 선택한 아이템을 선택 목록에 추가
            TreeNode n = procTreeView.SelectedNode;
            if (n != null)
            {
                string name = "";
                while (n != null)
                {
                    name = n.Text + name;
                    if (n.Parent != null)
                    {
                        name = "." + name;
                    }
                    n = n.Parent;
                }

                object obj = (DataProcess)procMap.Get(name);
                if (obj != null)
                {
                    descText.DocumentText = ((DataProcess)obj).Description;
                }
            }
        }
        
        // 선택 목록에서 프로세스 선택
        private void OnProcessSelection(object sender, EventArgs e)
        {
            Object obj = selectedList.SelectedItem;

            // 프로세스 설명 추가 표시
            if (obj != null)
            {
                descText.DocumentText = ((DataProcess)obj).Description;
            }

            if (obj != null)
            {
                DataProcess dp = (DataProcess) obj;

                // 옵션 목록을 얻어온다
                if (dp.IsSet("Option.List"))
                {
                    DataProperty[] options = (DataProperty[]) dp.Get("Option.List");

                    // 컴포넌트 추가 및 이벤트 핸들러 생성
                    optionPanel.Controls.Clear();
                    optionPanel.RowCount = options.Length;
                    if (options.Length > 3)
                    {
                        optionPanel.AutoScroll = true;
                    }
                    else
                    {
                        optionPanel.AutoScroll = false;
                    }

                    foreach (DataProperty op in options)
                    {
                        // 레이블
                        Label l = new Label();
                        l.Padding = new Padding(0, 5, 0, 0);
                        l.Font = selectedList.Font;
                        l.Text = op.Description;
                        l.Dock = DockStyle.Fill;
                        optionPanel.Controls.Add(l);

                        String compType = "";
                        if(op.IsSet("Component.Type")) {
                         compType = (String) op.Get("Component.Type");
                        }
                        
                        // 컴포넌트 타입 : File & Directory
                        if(compType.StartsWith("File") || compType.StartsWith("Directory"))
                        {
                            // 필드
                            TextBox box = new TextBox();
                            box.Font = selectedList.Font;
                            box.Text = op.Value.ToString();
                            box.Dock = DockStyle.Fill;
                            optionPanel.Controls.Add(box);

                            // 버튼
                            Button btn = new Button();
                            btn.Text = "Select";
                            btn.Font = l.Font;
                            optionPanel.Controls.Add(btn);

                            // 이벤트 핸들러
                            FileEventHandler fe = new FileEventHandler();
                            fe.DirectoryMode = compType.StartsWith("Directory");
                            fe.MultiSelectMode = compType.EndsWith("Multi");
                            fe.DataProcess = dp;
                            fe.OptionProperty = op;
                            fe.TextBox = box;
                            btn.Click += fe.handleEvent;
                            
                        }
                        // 컴포넌트 타입 : TextArea
                        else if (compType == "TextArea")
                        {
                            // 필드
                            TextBox box = new TextBox();
                            box.Font = selectedList.Font;
                            box.Text = op.Value.ToString();
                            box.Dock = DockStyle.Fill;
                            Size sz = box.Size;
                            sz.Height = 100;
                            sz.Width = 500;
                            //sz.Height = 50;
                            //sz.Width = 250;
                            box.Size = sz;
                            box.Multiline = true;                            
                            optionPanel.Controls.Add(box);

                            // 공백
                            Label dummy = new Label();
                            optionPanel.Controls.Add(dummy);

                            // 이벤트 핸들러
                            ComponentEventHandler c = new ComponentEventHandler();
                            c.DataProcess = dp;
                            c.OptionProperty = op;
                            box.TextChanged += c.handleEvent;                        
                        }
                        // 컴포넌트 타입 : ComboBox
                        else if (compType == "ComboBox")
                        {
                            // 필드
                            ComboBox cbox = new ComboBox();
                            cbox.DropDownStyle = ComboBoxStyle.DropDownList;
                            cbox.Font = selectedList.Font;
                            cbox.Text = op.Value.ToString();
                            cbox.Dock = DockStyle.Fill;
                            Size sz = cbox.Size;
                            sz.Height = 50;
                            sz.Width = 500;
                            cbox.Size = sz;

                            String[] values;
                            if (op.IsSet("DefaultValues"))
                            {
                                values = (String[]) op.Get("DefaultValues");
                            }
                            else {
                                values = op.Value.ToString().Split(',');
                                op.Set("DefaultValues", values);
                                op.Value = values[0];
                            }                             
                            foreach (String s in values)
                            {
                                cbox.Items.Add(s.Trim());
                            }                            
                            dp.Set(op.Name, op.Value);

                            if (values != null)
                            {
                                cbox.SelectedItem = op.Value.ToString();
                            }
                            else
                            {
                                cbox.SelectedIndex = 0;
                            }
                            optionPanel.Controls.Add(cbox);

                            // 공백
                            Label dummy = new Label();
                            optionPanel.Controls.Add(dummy);

                            // 이벤트 핸들러
                            ComboBoxEventHandler c = new ComboBoxEventHandler();
                            c.DataProcess = dp;
                            c.ComboBox = cbox;
                            c.OptionProperty = op;
                            cbox.SelectedIndexChanged += c.handleEvent;
                        }
                        // 일반적인 경우
                        else
                        {
                            // 필드
                            TextBox box = new TextBox();
                            box.Font = selectedList.Font;
                            box.Text = op.Value.ToString();
                            box.Dock = DockStyle.Fill;
                            optionPanel.Controls.Add(box);

                            // 공백
                            Label dummy = new Label();
                            optionPanel.Controls.Add(dummy);

                            // 이벤트 핸들러
                            ComponentEventHandler c = new ComponentEventHandler();
                            c.DataProcess = dp;
                            c.OptionProperty = op;
                            box.TextChanged += c.handleEvent;
                        }
                    }                    
                }                
            }
        }

        // 선택 목록에서 프로세스 순서 위로 올리기
        private void OnUpBtnClick(object sender, EventArgs e)
        {
            int idx = selectedList.SelectedIndex;
            if (idx > 0)
            {
                DataProcess p1 = (DataProcess) selectedList.Items[idx - 1];
                DataProcess p2 = (DataProcess)selectedList.Items[idx];
                selectedList.Items.Remove(p2); 
                selectedList.Items.Insert(idx - 1, p2);
                selectedList.SelectedIndex = idx - 1;
            }
        }

        // 선택 목록에서 프로세스 순서 아래로 내리기
        private void OnDownBtnClick(object sender, EventArgs e)
        {
            int idx = selectedList.SelectedIndex;
            if (idx < (selectedList.Items.Count - 1))
            {
                DataProcess p1 = (DataProcess)selectedList.Items[idx];
                DataProcess p2 = (DataProcess)selectedList.Items[idx + 1];
                selectedList.Items.Remove(p1);
                selectedList.Items.Insert(idx + 1, p1);
                selectedList.SelectedIndex = idx + 1;
            }
        }

        // 현재 선택 목록 저장하기
        private void onSaveClick(object sender, EventArgs e)
        {
            
        }

        // 선택 목록 불러오기
        private void onLoadClick(object sender, EventArgs e)
        {
           
        }

        private void button9_Click(object sender, EventArgs e)
        {
            
        }

        //private void button10_Click(object sender, EventArgs e)
        //{
        //    int idx = -1;
        //    idx = selectedList.SelectedIndex;
        //    if (idx > -1)
        //    {
        //        object obj = (DataProcess)selectedList.Items[idx - 1];

        //        if (obj != null)
        //        {
        //            DataProcess dp = (DataProcess)((DataProcess)obj).Clone();

        //            // 옵션에 대한 기본값 설정
        //            DataProperty[] options = (DataProperty[])dp.Get("Option.List");
        //            foreach (DataProperty p in options)
        //            {
        //                dp.Set(p.Name, p.Value);
        //            }
        //        }
        //    }
        //}
    }

    // 데이터 프로퍼티에 대한 이벤트 핸들러
    class ComponentEventHandler
    {
        public DataProcess DataProcess { get; set; }
        public DataProperty OptionProperty { get; set; }

        public void handleEvent(Object sender, System.EventArgs e)
        {
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox) sender;
            String val = tb.Text;

            Object obj = DataProperty.Parse(val);

            // Int값은 Double에 할당할 수 있으므로 편의상, 특별히 예외적으로 처리
            if (obj.GetType() == typeof(int) && OptionProperty.Type == typeof(double))
            {
                double dv = (int)obj;
                OptionProperty.Value = dv;
                DataProcess.Set(OptionProperty.Name, OptionProperty.Value);
            }
           else if (obj.GetType() == typeof(double) && OptionProperty.Type == typeof(int))
            {
                int dv = Convert.ToInt32(obj);
                OptionProperty.Value = dv;
                DataProcess.Set(OptionProperty.Name, OptionProperty.Value);
            }
            else if (obj.GetType() == typeof(double) && OptionProperty.Type == typeof(string))
            {
                string dv = Convert.ToString(obj);
                OptionProperty.Value = dv;
                DataProcess.Set(OptionProperty.Name, OptionProperty.Value);
            }
            else
            {
                if (obj.GetType() != OptionProperty.Type)
                {
                    MessageBox.Show("타입이 맞지 않음");
                }
                else
                {
                    OptionProperty.Value = obj;
                    DataProcess.Set(OptionProperty.Name, OptionProperty.Value);
                }
            }
        }
    }

    // 파일 선택 버튼에 대한 이벤트 핸들러
    class FileEventHandler
    {
        public DataProcess DataProcess { get; set; }
        public DataProperty OptionProperty { get; set; }
        public TextBox TextBox { get; set;  }
        public System.Windows.Controls.TextBox TextBox2 { get; set; }

        public bool DirectoryMode { get; set; }
        public bool MultiSelectMode { get; set; }

        public static string inifolderpath { get; set; }

        public FileEventHandler()
        {
            DirectoryMode = false;
            MultiSelectMode = false;
        }

        public void handleEvent(Object sender, System.EventArgs e)
        {
            string[] filenames = null;            

            // 다이얼로그 생성
            if (!DirectoryMode)
            {
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "All Files|*.*|csv Files|*.csv|filter Files|*.filter|dp Files|*.dp";
                dlg.FilterIndex = 1;     // FilterIndex는 1부터 시작 (여기서는 *.txt)
                dlg.RestoreDirectory = true;

                dlg.Multiselect = MultiSelectMode;
                dlg.DefaultExt = "*.*";
                dlg.InitialDirectory = inifolderpath;

                Nullable<bool> result = dlg.ShowDialog();
                string directoryname = string.Empty;

                if (dlg.FileName != "" && dlg != null)
                    directoryname = System.IO.Path.GetDirectoryName(dlg.FileName);

                inifolderpath = directoryname;

                if (result != false)
                {
                    filenames = dlg.FileNames;
                }
            }
            else
            {
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.SelectedPath = inifolderpath; 

                dlg.ShowDialog();
                string path = dlg.SelectedPath;

                inifolderpath = dlg.SelectedPath; 

                if (Directory.Exists(path))
                {
                    filenames = new string[] { path };
                }                
            }

            if (filenames != null)
            {
                string files = "";

                if (filenames.Length == 1)
                {
                    files = filenames[0];
                }
                else
                {
                    for (int i = 0; i < filenames.Length; i++)
                    {
                        files += filenames[i];
                        if (i < (filenames.Length - 1))
                        {
                            files += ", ";
                        }
                    }
                }

                if (TextBox != null)
                    TextBox.Text = files;
                if(TextBox2 != null)
                    TextBox2.Text = files;
                
                OptionProperty.Value = files;
                DataProcess.Set(OptionProperty.Name, OptionProperty.Value);
            }
            else
            {
                if(TextBox != null)
                    TextBox.Text = "";
                if (TextBox2 != null)
                    TextBox2.Text = "";
                
            }
        }
    }

    // 파일 선택 버튼에 대한 이벤트 핸들러
    class ComboBoxEventHandler
    {
        public DataProcess DataProcess { get; set; }
        public DataProperty OptionProperty { get; set; }
        public ComboBox ComboBox { get; set; }
        public System.Windows.Controls.ComboBox ComboBox2 { get; set; }
        
        public ComboBoxEventHandler()
        {
        }

        public void handleEvent(Object sender, System.EventArgs e)
        {
            if (ComboBox.SelectedIndex >= 0)
            {
                if(ComboBox != null)
                    OptionProperty.Value = ComboBox.SelectedItem.ToString();
                if (ComboBox2 != null)
                    OptionProperty.Value = ComboBox2.SelectedItem.ToString();

                DataProcess.Set(OptionProperty.Name, OptionProperty.Value);
            }
        }
    }
}
