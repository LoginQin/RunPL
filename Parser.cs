using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
namespace RunPL
{
    enum KEY {
        Descript, Manual, Command, ShowCMD, SelectFolder, SaveFile, RunPL, ComboBox, TextBox, SelectFile, CheckBox, Error
    }
    class Parser
    {
        private string parm;
        private ArrayList result;
        private char[] c_parms;
        private char curr;
        private int index;
        private bool endParse;
        private Hashtable VarPath;
        public Parser(string parm) {
            this.parm = parm;
            this.c_parms = this.parm.ToCharArray();
            this.index = 0;
            this.result = new ArrayList();
            this.endParse = false;
            this.VarPath = new Hashtable();
            VarPath.Add("&AppPath", System.Environment.CurrentDirectory);
            VarPath.Add("&AppLib", System.Environment.CurrentDirectory + "\\lib");
            this.do_parser();
            
        }

        private char getCh() {
            if (index < this.parm.Length)
            {
                this.curr = c_parms[index];
                this.index++;
            }
            else {
                this.endParse = true;
            }     
            return this.curr;
        }

        private void do_parser(){
            string blank = "\\s";
            string number = "\\d";
            Regex Rnumber = new Regex(number);
            Regex Rblank = new Regex(blank);
            while (index < this.parm.Length && !this.endParse)
            {
                this.curr = getCh();
                if (Rblank.IsMatch(this.curr.ToString()))
                {
                    continue;
                }
                if ( char.IsDigit(this.curr) ){
                    this.result.Add(getNum());
                    continue;
                }
                switch (curr)
                {
                    case '"':
                        this.result.Add(getStrings('"'));
                        break;
                    case '\'':
                        this.result.Add(getStrings('\''));
                        break;
                    case '&':
                        this.result.Add(getVar());
                        break;
                    case '[':
                        index--;
                        this.result.Add(getStrings(']'));
                        break;
                    case ',':
                        continue;

                    default:
                        throw new Exception("\r\nRunPL错误, 在 " + this.parm +" 中存在无法解析字符:" + curr);
                }
            }
        }
        private string getStrings(char endWith) {
            int start = index;
            getCh();
            int len = 0;
            string subs = null;
            while (index < this.parm.Length && this.curr != endWith) {
                len++;
                this.curr = getCh();
            }
            if (index != this.parm.Length) {
                getCh();
            }

            subs = this.parm.Substring(start, len);
            return subs;
        }
        private string getNum() {
            string num = "";
            while (char.IsDigit(this.curr) && !endParse) {
                num += this.curr;
                getCh();
            }
            return num;
        }
        private string getVar() {
            int start = index-1;
            bool skip = false;
            getCh();
            int len = 0;
            string subs = null;
            while (index < this.parm.Length && this.curr != ',') {
                len++;
                getCh();
                if (this.curr == '.') { //&AppPath."dfsfdfdf"   &AppLib.'dfdf'
                    subs = this.parm.Substring(start, len+1);
                    subs = (string)VarPath[subs];
                    subs += getStrings(getCh());
                    skip = true;
                }
            }
            if (!skip)
            {
                subs = this.parm.Substring(start, len+1);
                subs = (string)VarPath[subs];
            }

            return subs;
        }

        public ArrayList getResult() {
            return this.result;
        }


    }
}
