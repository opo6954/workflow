namespace SMiningClient.WPF
{
    partial class ProcessForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.descText = new System.Windows.Forms.WebBrowser();
            this.procTreeView = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.optionContainer = new System.Windows.Forms.FlowLayoutPanel();
            this.optionPanel = new System.Windows.Forms.TableLayoutPanel();
            this.selectedList = new System.Windows.Forms.CheckedListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.optionContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Controls.Add(this.procTreeView);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox1.Location = new System.Drawing.Point(24, 19);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(475, 742);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Available Process";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowLayoutPanel1.Controls.Add(this.descText);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(6, 354);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(463, 380);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // descText
            // 
            this.descText.Location = new System.Drawing.Point(3, 4);
            this.descText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.descText.MinimumSize = new System.Drawing.Size(23, 25);
            this.descText.Name = "descText";
            this.descText.Size = new System.Drawing.Size(458, 372);
            this.descText.TabIndex = 4;
            this.descText.WebBrowserShortcutsEnabled = false;
            // 
            // procTreeView
            // 
            this.procTreeView.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.procTreeView.Location = new System.Drawing.Point(6, 51);
            this.procTreeView.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.procTreeView.Name = "procTreeView";
            this.procTreeView.Size = new System.Drawing.Size(463, 273);
            this.procTreeView.TabIndex = 6;
            this.procTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.onCandidateSelection);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(4, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Process Hierarchy";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(6, 331);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Process Description";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.optionContainer);
            this.groupBox2.Controls.Add(this.selectedList);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupBox2.Location = new System.Drawing.Point(629, 13);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(543, 748);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selected Process";
            // 
            // optionContainer
            // 
            this.optionContainer.BackColor = System.Drawing.Color.White;
            this.optionContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.optionContainer.Controls.Add(this.optionPanel);
            this.optionContainer.Location = new System.Drawing.Point(6, 381);
            this.optionContainer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.optionContainer.Name = "optionContainer";
            this.optionContainer.Size = new System.Drawing.Size(531, 357);
            this.optionContainer.TabIndex = 5;
            // 
            // optionPanel
            // 
            this.optionPanel.ColumnCount = 3;
            this.optionPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.optionPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.optionPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.optionPanel.Location = new System.Drawing.Point(3, 4);
            this.optionPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.optionPanel.Name = "optionPanel";
            this.optionPanel.RowCount = 10;
            this.optionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionPanel.Size = new System.Drawing.Size(526, 351);
            this.optionPanel.TabIndex = 0;
            // 
            // selectedList
            // 
            this.selectedList.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.selectedList.FormattingEnabled = true;
            this.selectedList.Location = new System.Drawing.Point(6, 55);
            this.selectedList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.selectedList.Name = "selectedList";
            this.selectedList.Size = new System.Drawing.Size(531, 298);
            this.selectedList.TabIndex = 4;
            this.selectedList.SelectedIndexChanged += new System.EventHandler(this.OnProcessSelection);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(5, 359);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Process Options";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(6, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "Process List";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(521, 116);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 29);
            this.button1.TabIndex = 5;
            this.button1.Text = "→";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.onAddBtnClick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(521, 162);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 29);
            this.button2.TabIndex = 6;
            this.button2.Text = "←";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.onDelBtnClick);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(521, 414);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(86, 29);
            this.button3.TabIndex = 7;
            this.button3.Text = "OK";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.onOKClick);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(521, 460);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(86, 29);
            this.button4.TabIndex = 8;
            this.button4.Text = "Cancel";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.onCancelClick);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(521, 226);
            this.button5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(86, 29);
            this.button5.TabIndex = 9;
            this.button5.Text = "↑";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.OnUpBtnClick);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(521, 271);
            this.button6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(86, 29);
            this.button6.TabIndex = 10;
            this.button6.Text = "↓";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.OnDownBtnClick);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(521, 508);
            this.button7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(86, 29);
            this.button7.TabIndex = 11;
            this.button7.Text = "Save";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.onSaveClick);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(521, 554);
            this.button8.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(86, 29);
            this.button8.TabIndex = 12;
            this.button8.Text = "Load";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.onLoadClick);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(521, 598);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(86, 31);
            this.button9.TabIndex = 13;
            this.button9.Text = "Synthesis";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // ProcessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 774);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ProcessForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Data Process Selector";
            this.Load += new System.EventHandler(this.onFormLoad);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.optionContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel optionContainer;
        private System.Windows.Forms.CheckedListBox selectedList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.WebBrowser descText;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TreeView procTreeView;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel optionPanel;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
    }
}