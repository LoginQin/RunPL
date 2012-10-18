##RunPL描述文件格式指南

 RunPL描述文件:
       是用来描述某个Perl程序需要的参数, 用RunPL程序运行, 后缀名rpl
 RunPL是为了让不擅长使用命令行的人员使用图形界面调用开发人员的Perl程序的图形工具
 开发人员只需要写一个RunPL来描述Perl程序运行需要的信息(参数).

 RPL全部使用函数式方式描述,  程序会自动按顺序组织$1~$n 个参数, 

--------------------------------------描述文件开始------------------------------------------
###说明要执行的目标Perl程序路径, 必须的
  
  RunPL( "D:\我的资料\Desktop\PERL\app.pl");

###程序概述

  Descript("合并Excel文件, 以文件名为页码,放在Excel表的第一列");

###输入手册
  
  Manual("哈哈啊哈, 这个是手册啊啊, 你应该这么做, 这么做这么做, 知道了吗????");

###是否显示CMD窗体, 如果你需要在CMD输入参数, 选择1

  ShowCMD(1);

--------------------通过界面获取参数-------------------------------------
###显示一个文件夹选择框, $1~$n都是Perl需要的参数

  $1 = SelectFolder("输入路径", &AppLib, "浏览");  

###显示一个保存对话框
  
  $2 = SaveFile("输出文件路径", &AppPath, "选择", "文本文件|*.txt");

###显示一个选择文件对话框, 最后一个参数是C#格式的文件过滤
  
  $3 = SelectFile("选择文件", "", "...", "Excel文件|*.xls");

###你可以设置选择多个文件作为参数,但是$1~$n 是按顺序,而且唯一的,这表示Perl的第1个到第N个参数

  $7 = SelectFile("选择文件", "", "...", "文本文件|*.txt");

###显示一个让用户输入的TextBox, 用于获取输入

  $4 = TextBox("输     入:", "20120304");

###显示一个列表提供给用户选择, []扩起来的是默认值

  $5 = ComboBox("选    择:", "tampalte", [isdefault], "ok,no", 4, 5, 6 );

---------------------------------------------------------------------------------------------
###描述性的函数: 
   
    Descript(), Manual(), RunPL(), ShowCMD();

###描述参数的函数, 格式: $n = 函数名() :
   
      SelectFolder(), SaveFile() , SelectFile(), TextBox(), ComboBox()
      *第一个参数都是Label标记,  并且都需要返回对应的  $1 = 函数名() 

###引用变量

  &AppPath  RunPL程序当前的执行路径
  &AppLib   RunPL程序当前路径下lib目录

##关 于 作  者
覃 炜    
Email: qinwei081@gmail.com
Version: 0.0.1
-= ChineseTiger =-
