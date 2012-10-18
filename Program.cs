using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RunPL
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] Argvs)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 form1 = null;
            if(Argvs.Length > 0){
                form1 = new Form1(Argvs[0]);
            }else {
                form1 = new Form1();
            }
            Application.Run(form1);
        }
    }
}
