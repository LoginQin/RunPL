using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace RunPL
{
    public delegate void ShowMessageHandler(string msg);
    public partial class Form1 : Form
    {
        private bool hasRunPl = false;
        private ArrayList Argvs = new ArrayList();
        private bool show_cmd = true;
        private Form2 form2;
        //控件字体
        private Font font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        private int height = -25;
        private int left = 10;
        private int argv_nums = 0;
        private string appPath = null;
        private bool can_run = true;
        private Hashtable ht_assign = new Hashtable();


        public Form1()
        {
            InitializeComponent();
          
        }

        public Form1(string file_path) {
            InitializeComponent();
            this.open_RunPL_file(file_path);
        }
     

        private void btn_select_folder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {

                //找第二个参数
                Button btn = (Button)sender;
                string parm = btn.Name.Split('_')[1];
                Control[] cl = this.tabPage1.Controls.Find("textbox_" + parm, false);
                cl[0].Text = folderBrowserDialog1.SelectedPath;
            }
        }
        
        private string RunCmd(string command)
        {


            //例Process
            Process p = new Process();
            p.StartInfo.FileName = "perl.exe";           //确定程序名
            p.StartInfo.Arguments = " " + command;    //确定程式命令行
            p.StartInfo.UseShellExecute = false;        //Shell的使用
            p.StartInfo.RedirectStandardInput = !this.show_cmd;   //重定向输入

            p.StartInfo.RedirectStandardOutput = !this.show_cmd; //重定向输出
            p.StartInfo.RedirectStandardError = !this.show_cmd;   //重定向输出错误
            p.StartInfo.CreateNoWindow = !this.show_cmd;//不显示命令行窗口
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(this.appPath);
            if (!this.show_cmd)
            {
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            p.OutputDataReceived += new DataReceivedEventHandler(this.show_result);
            p.EnableRaisingEvents = true;
            p.Exited += new EventHandler(this.show_end);

            ThreadPool.QueueUserWorkItem(new WaitCallback(ExeThread), p); 
            //p.StandardOutput.ReadToEnd();
           
           
            return "";        //输出出流取得命令行结果果
        }

        private void show_end(object s, EventArgs e) {
            ShowMessage("运行结束!");
            
        }
        private void ExeThread(object obj)
        {
            Process cmd = obj as Process;
    
            cmd.Start();
           // cmd.Refresh();
            //  
            Application.DoEvents();
            if (!show_cmd)
            {
                cmd.BeginOutputReadLine();
            }

            cmd.WaitForExit();
            if (cmd.ExitCode != 0)
            {
                if(!show_cmd) ShowMessage("\r\n" + cmd.StandardError.ReadToEnd());
            }
            
            if (!show_cmd)
            {
                cmd.Close();
            }
        }



        //线程安全的调用方法,新线程要调用windowForm的控件
        private void ShowMessage(string msg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowMessageHandler(ShowMessage), new object[] { msg });
            }
            else
            {
                if (msg != null)
                {
                    rb_result.AppendText(msg);
                    rb_result.SelectionStart = rb_result.TextLength;
                    rb_result.Focus();
                    if (msg.Equals("运行结束!")) {
                        this.form2.Close();
                    }
                }
            }

        }
 
        private void show_result(object obj, DataReceivedEventArgs data) {

            ShowMessage(data.Data+"\n");

        }

        //运行Perl
        private void btn_run_pl_Click(object sender, EventArgs e)
        {
            string command = "\""+this.appPath + "\"  ";
            ICollection ic = this.ht_assign.Keys;
            int[] com = new int[ic.Count];
            int i= 0;
            foreach (string cm in ic) {
               com[i] = Convert.ToInt32(cm);
               i++;
            }
            Array.Sort<int>(com);
            string _parm = null;
            bool allSet = true;
            foreach (int c in com) {
                _parm = getUserParms(c);
                if (!IsEmpty(_parm))
                {
                    command += " \"" + _parm + "\"";
                }
                else {
                    alertNotSet(c);
                    allSet = false;
                }
            }

            if (allSet)
            {
                RunCmd(command);
                this.form2 = new Form2();
                this.form2.Owner = this;
                this.form2.ShowDialog();
            }

           
        }
        private void alertNotSet(int c) {
            try
            {
                MessageBox.Show(this.tabPage1.Controls.Find("label_" + c, false)[0].Text + " 未设置!");
            }catch(Exception e){
                MessageBox.Show("运行错误, 可能是参数未设置!");
            }
        }
        private bool IsEmpty(string parm) { 
            bool result = false;
            if (parm.Replace(" ", "").Equals("")) {
                result = true;
            }
            return result;
        }

        private string getUserParms(int parm) {
            Control[] cl = this.tabPage1.Controls.Find("textbox_" + parm, false);
           return cl[0].Text;

        }

        private void btn_open_pl_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Argvs.Add( openFileDialog1.FileName );
            }
        }

 
        private void button1_Click(object sender, EventArgs e)
        {
           this.addSelectFolder("输入路径", "", "浏览");

        }

        //选择文件夹路径
        private void addSelectFolder(string title, string default_path, string button_txt) {
            Label mylabel = new Label();
            mylabel.Name = "label_" + argv_nums;
            TextBox myTextBox = new TextBox();
            Button myButton = new Button();
            mylabel.Text = title;
            myTextBox.Text = default_path;
            myButton.Text = button_txt;
            myButton.Name = "button_" + this.argv_nums;

            this.height += myTextBox.Height + 16;
            mylabel.Location = new Point(this.left, this.height+5);
            mylabel.Font = this.font;
            myTextBox.Width = 500;
            myTextBox.Font = this.font;

            myTextBox.Location = new Point( mylabel.Width + 10, this.height);
            myTextBox.Name = "textbox_" + this.argv_nums;
            myTextBox.ReadOnly = true;

            myButton.Location = new Point(mylabel.Width + 10 + myTextBox.Width + 10, this.height);
            myButton.Size = new System.Drawing.Size(87, 27);
            myButton.Click += new EventHandler(this.btn_select_folder_Click);
            this.tabPage1.Controls.Add(mylabel);
            this.tabPage1.Controls.Add(myTextBox);
            this.tabPage1.Controls.Add(myButton);
        }

        //保存文件路径
        private void addSaveFilePath(string title, string default_path, string button_txt, string file_filter)
        {
            Label mylabel = new Label();
            mylabel.Name = "label_" + argv_nums;
            TextBox myTextBox = new TextBox();
            Button myButton = new Button();
            mylabel.Text = title;
            myTextBox.Text = default_path;
            myButton.Text = button_txt;
            myButton.Name = "button_" + argv_nums;

            this.height += myTextBox.Height + 16;
            mylabel.Location = new Point(this.left, this.height + 5);
            mylabel.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            myTextBox.Width = 500;
            myTextBox.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

            myTextBox.Location = new Point(mylabel.Width + 10, this.height);
            myTextBox.Name = "textbox_" + argv_nums;
            myTextBox.ReadOnly = true;

            myButton.Location = new Point(mylabel.Width + 10 + myTextBox.Width + 10, this.height);
            myButton.Size = new System.Drawing.Size(87, 27);
            saveFileDialog1.Filter = file_filter;
            myButton.Click += new EventHandler(this.save_file_event);
            this.tabPage1.Controls.Add(mylabel);
            this.tabPage1.Controls.Add(myTextBox);
            this.tabPage1.Controls.Add(myButton);
        }

        private void save_file_event(object sender, EventArgs e) {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                //找第二个参数
                Button btn = (Button)sender;
                string parm = btn.Name.Split('_')[1];
                Control[] cl = this.tabPage1.Controls.Find("textbox_" + parm, false);
                cl[0].Text = saveFileDialog1.FileName;
            }
        }

        private void initPanel() {
            this.tabPage1.Controls.Clear();
            this.ht_assign.Clear();
            this.height = -25;
            this.left = 10;
            this.argv_nums = 0;
            this.rb_result.Clear();
            this.richbox_manual.Clear();
            this.appPath = null;
        }
        public void open_RunPL_file(string file_path) {

            initPanel();

            StreamReader read = new StreamReader(file_path, Encoding.UTF8);
            //正则表达式的Bug在于 Descript(".......);");  末尾的); 将识别不出
            string state = "(\\$(\\d+)\\s*=\\s*)?(\\w+)\\s*\\((.*?)\\)\\s*;";
            string comment = "#.*";
            string line = null;
            Regex Rstate = new Regex(state, RegexOptions.Singleline);
            Regex Rcom = new Regex(comment);

            line = read.ReadToEnd();
            line = Regex.Replace(line, comment, "");

            if (Rstate.IsMatch(line))
            {
                MatchCollection m = Rstate.Matches(line);
                for (int i = 0; i < m.Count; i++)
                {
                    Expression ex = new Expression(); //构造函数表达式
                    if (m[i].Groups[2].Value.Length > 0)
                    {
                        //说明含有参数
                        ex.assigne = m[i].Groups[2].Value;
                    }
                    ex.func_type = getKeyType(m[i].Groups[3].Value);
                    ex.func_name = m[i].Groups[3].Value; //方法名称
                    ex.func_parm = m[i].Groups[4].Value; //方法参数
                    to_do(ex);
                }
            }

            //一切无错则显示运行按钮
            if (this.hasRunPl && this.can_run)
            {
                this.btn_run_pl.Enabled = true;
            }
        }
        //打开Perl描述文件
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Perl描述文件|*.rpl";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                string file_path = openFileDialog1.FileName;
                this.open_RunPL_file(file_path);

            }
        }


        //从字符串获取方法名称类型
        private KEY getKeyType(string str) {
            int i = 0;
            foreach (string s in Enum.GetNames(typeof(KEY)))
            {
                if (str.Equals(s)) {
                    return (KEY)i;
                }
                i++;
            }
            return KEY.Error;
        }
     

        private void addSelectFilePath(string lable, string file_url, string btn_text) {
            
            Label mylabel = new Label();
            mylabel.Name = "label_" + argv_nums;
            TextBox myTextBox = new TextBox();
            Button myButton = new Button();
            mylabel.Text = lable;
            myTextBox.Text = file_url;
            myButton.Text = btn_text;
            myButton.Name = "button_" + argv_nums;

            this.height += myTextBox.Height + 16;
            mylabel.Location = new Point(this.left, this.height + 5);
            mylabel.Font = font;
            myTextBox.Width = 500;
            myTextBox.Font = font;

            myTextBox.Location = new Point(mylabel.Width + 10, this.height);
            myTextBox.Name = "textbox_" + argv_nums;
            myTextBox.ReadOnly = true;

            myButton.Location = new Point(mylabel.Width + 10 + myTextBox.Width + 10, this.height);
            myButton.Size = new System.Drawing.Size(87, 27);
            myButton.Click += new EventHandler(this.event_select_file);
            this.tabPage1.Controls.Add(mylabel);
            this.tabPage1.Controls.Add(myTextBox);
            this.tabPage1.Controls.Add(myButton);
        }

        private void addTextBox(string label, string parm) {
            Label mylabel = new Label();
            mylabel.Name = "label_" + argv_nums;
            TextBox myTextBox = new TextBox();
            myTextBox.Name = "textbox_" + argv_nums;
            mylabel.Text = label;
            myTextBox.Text = parm;
            this.height += myTextBox.Height + 16;
            mylabel.Location = new Point(this.left, this.height + 5);
            mylabel.Font = this.font;
            myTextBox.Width = 500;
            myTextBox.Font = this.font;

            myTextBox.Location = new Point(mylabel.Width + 10, this.height);
            this.tabPage1.Controls.Add(mylabel);
            this.tabPage1.Controls.Add(myTextBox);
        }
       

        

        private void event_select_file(object sender, EventArgs e) {
            Button btn = (Button)sender;
            string parm = btn.Name.Split('_')[1];
            openFileDialog_d.Filter = (string)ht_assign[parm];
            if (openFileDialog_d.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Control[] cl = this.tabPage1.Controls.Find("textbox_" + parm, false);
                cl[0].Text = openFileDialog_d.FileName;
            }
        }
    

        //判断是否已经声明了同一个参数
        private bool existNum(object parm){
            bool result = false;
            if (ht_assign.ContainsKey(parm))
            {
                result = true;
                can_run = false;
                rb_result.AppendText("参数描述符重复! $" + (string)parm);
            }
            return result;

        }

        private bool checkParmsNum(int parms, int need) {
            return parms == need ? true : false; 
        }
        private void ParmError(string error){
            rb_result.AppendText(error);
            this.can_run = false;
        }

        private void setAppPath(string url) {
            this.appPath = url;
            this.textBox_perl_source_path.Text = this.appPath;
        }
        private void addComboBox(object[] parms) {
            Regex rg = new Regex("\\[.*");
            Label mylabel = new Label();
            mylabel.Name = "label_" + argv_nums;
            ComboBox comboBox1 = new ComboBox();
            comboBox1.Font = this.font;
            comboBox1.Width = 200;
            comboBox1.Name = "textbox_" + this.argv_nums;
            mylabel.Text = (string)parms[0];
            mylabel.Font = font;
            this.height += comboBox1.Height + 16;
            mylabel.Location = new Point(this.left, this.height + 5);
            // 
            // comboBox1
            // 
            comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            string txt = null;
            for (int i = 1; i < parms.Length; i++)
            {
                txt = parms[i].ToString();

                if (rg.IsMatch(txt))
                {
                    txt = txt.Replace("[", "");
                    comboBox1.Items.Add(txt);
                    comboBox1.Text = txt;
                  
                }
                else {
                    comboBox1.Items.Add(txt);
                }
            }
           comboBox1.Location = new Point(mylabel.Width + 10, this.height);
          
         //  comboBox1.Size = new System.Drawing.Size(121, 22);
           comboBox1.TabIndex = 0;
           this.tabPage1.Controls.Add(mylabel);
           this.tabPage1.Controls.Add(comboBox1);
        }
        //根据方法类型执行函数
        private void to_do(Expression ex) {
            Parser p = null;
            ArrayList parms = null;
            try
            {
              p = new Parser(ex.func_parm);
            }
            catch (Exception e) {
                rb_result.AppendText(e.Message);
                this.can_run = false;
                return;
            }

          
             parms = p.getResult();
          
            switch (ex.func_type) { 
                case KEY.Error:
                    ParmError("错误!!Perl描述文件存在无效函数:" + ex.func_name + "\r\n");
                    break;
                case KEY.RunPL:
                    if(checkParmsNum(parms.Count, 1)){
                        this.hasRunPl = true;
                        setAppPath((string)parms[0]);
                    }
                    
                    break;
                case KEY.SaveFile:
                    if (parms.Count != 4)
                    {
                        ParmError("SelectFile 参数错误!!");
                    }
                    else
                    {
                        if (ex.assigne == null) {
                            ParmError("SelectFile 没有赋值对象, 如 $1");
                            return;
                        }
                        this.argv_nums = Convert.ToInt32( ex.assigne );
                        if ( !this.existNum(ex.assigne) )
                        {
                            ht_assign.Add(ex.assigne, parms[3]);
                            addSaveFilePath((string)parms[0], (string)parms[1], (string)parms[2], (string)parms[3]);
                        }
                      
                    }
                   
                    break;
                case KEY.SelectFolder:
                    if (parms.Count != 3)
                    {
                        ParmError("SelectFolder 参数错误!!");
                    }
                    else
                    {
                        if (!this.existNum(ex.assigne))
                        {
                            this.argv_nums = Convert.ToInt32( ex.assigne );
                            ht_assign.Add(ex.assigne, "");
                            addSelectFolder((string)parms[0], (string)parms[1], (string)parms[2]);
                        }
                    }
                    break;
                case KEY.Descript:
                    if(parms.Count != 1) {
                        ParmError("Decript参数错误!!");
                        return;
                    }
                    tb_descript.Text = (string)parms[0];
                    break;
                case KEY.CheckBox:
                    MessageBox.Show("To do show message" + ex.func_parm);
                    break;
                case KEY.ComboBox:
                    if (parms.Count > 2)
                    {
                        if (!existNum(ex.assigne))
                        {
                            this.argv_nums = Convert.ToInt16(ex.assigne);
                            ht_assign.Add(ex.assigne, "");
                            addComboBox(parms.ToArray());
                        }
                    }
                    else {
                        ParmError("ComboBox 参数错误, 至少要两个参数,如: ComboBox('请选择', [1], 2)");
                    }
                    break;
                case KEY.Manual:
                    if (checkParmsNum(parms.Count, 1))
                    {
                        this.richbox_manual.Text = (string)parms[0];
                    }
                    else {
                        ParmError("Manual 参数错误!");
                    }
                    break;
                case KEY.SelectFile:
                    if (parms.Count != 4)
                    {
                        ParmError("SelectFile 参数错误!!");
                    }
                    else
                    {
                        if (ex.assigne != null)
                        {
                            this.argv_nums = Convert.ToInt32(ex.assigne);
                            if (!this.existNum(ex.assigne))
                            {
                                ht_assign.Add(ex.assigne, parms[3]); //用参数名称来保存这个数据
                                addSelectFilePath((string)parms[0], (string)parms[1], (string)parms[2]);
                            }
                        }
                        else 
                        {
                            ParmError("SelectFile 没有赋值对象 例如: $1 !!");
                        }
                        
                    }
                    break;
                case KEY.TextBox:
                    if (this.checkParmsNum(parms.Count, 2))
                    {
                        this.argv_nums = Convert.ToInt32(ex.assigne);
                        if (!this.existNum(ex.assigne))
                        {
                            ht_assign.Add(ex.assigne, "");
                            addTextBox((string)parms[0], (string)parms[1]);                           
                        }
                    }
                    else 
                    {
                        ParmError("TextBox 参数错误, TextBox(\"Label\") , \"default\"");
                    }
                    break;
                case KEY.ShowCMD:
                    if (checkParmsNum(parms.Count, 1))
                    {
                        this.show_cmd = Convert.ToInt16(parms[0]) == 1 ? true : false;
                    }
                    else {
                        ParmError("ShowCMD 参数错误, 应为ShowCMD(1)或ShowCMD(0)");
                    }
                    break;
                default:
                    return;

            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string filename = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            if (Path.GetExtension(filename).ToLower().Equals(".rpl"))
            {
                this.open_RunPL_file(filename);
            }
            else {
                MessageBox.Show("不能打开:\r\n " + Path.GetFileName(filename) + "\r\n该文件不是Perl描述文件(*.rpl)");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (this.appPath == null) return;
            var psi = new ProcessStartInfo("notepad");
            psi.Arguments = " " + this.appPath;
            Process.Start(psi);
         
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            if (this.appPath == null) return;
            Process p = new Process();
            p.StartInfo.FileName = this.appPath;
            p.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.appPath == null) return;
            var psi = new ProcessStartInfo(Path.GetDirectoryName(this.appPath));
            Process.Start(psi);
        }
        
    }

    class Expression {
        public KEY func_type = KEY.Error;
        public string assigne = null;
        public string func_name = null;
        public string func_parm = null;

    }
}
