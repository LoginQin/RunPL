##RunPL描述文件格式指南

 RunPL描述文件:  
      是用来描述某个Perl程序需要的参数, 用RunPL程序运行, 后缀名rpl
 RunPL是为了让不擅长使用命令行的人员使用图形界面调用开发人员的Perl程序的图形工具
 开发人员只需要写一个RunPL来描述Perl程序运行需要的信息(参数).

 RPL全部使用函数式方式描述,  程序会自动按顺序组织$1~$n 个参数, 
---
###示例
  例如,某个脚本app.pl需要两个参数: 扫描路径, 保存文件名. 一般的专业人员使用命令行


```
perl app.pl "D:/我的文档/word文件" "D:/桌面/合并文档.doc"
```

  如果我们用RunPL, 可以生成一个图形界面, 让用户可以用鼠标进行选择.

<img src="raw/master/assert/combineword.png" />

  像上面的界面, 开发人员可以通过下面的描述来动态生成界面.


```
RunPL("D:\RunPL\Lib\Perl-work\Word\app\app.pl");
ShowCMD(1);
$1 = SelectFolder("选择路径", "", "浏览");
$2 = SaveFile("保存位置", "合并文档.doc", "选择", "Word文档|*.doc");
Descript("合并Word文档, 请选择合并位置, 系统会自动搜索该文件夹下所有的Word文件进行合并~");
```

--------------------------------------描述文件开始------------------------------------------

###说明要执行的目标Perl程序路径, 必须的
    RunPL( "D:\我的资料\Desktop\PERL\app.pl");

###程序概述
    Descript("合并Excel文件, 以文件名为页码,放在Excel表的第一列");

###输入手册
    Manual("哈哈啊哈, 这个是手册啊啊, 你应该这么做, 这么做这么做, 知道了吗????");

###是否显示CMD窗体, 如果你需要在CMD输入参数, 选择1
    ShowCMD(1);

------------------------------------通过界面获取参数-------------------------------------

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
###UPDATE
    本版本增加了一个函数Command("程序名", "默认参数");
    原本RunPL是用来执行Perl脚本的, 但是后来我让其可以作为更多脚本或者任何命令行运行程序的"界面"
比如java, node, ruby等.
    可以强制使用Command来定义使用的程序.
    比如java的

```
Command("java");
```
或者有默认依赖库的方式

```
Command("java", "-cp xxx.jar -jar");
RunPL("app.jar");
$1 = ComboBox("选    择:", "tampalte", [isdefault], "ok,no", 4, 5, 6 );
```
上文将生成一个选单:
<img src="raw/master/assert/combox.png" />

上文等效于命令行下运行:

```
java -cp xxx.jar -jar app.jar isdefault 
```

###描述性的函数: 
    Descript(), Manual(), RunPL(), ShowCMD();

###描述参数的函数, 格式: $n = 函数名() :
    SelectFolder(), SaveFile() , SelectFile(), TextBox(), ComboBox()

   *第一个参数都是Label标记,  并且都需要返回对应的  $1 = 函数名()*

###引用变量
    &AppPath  RunPL程序当前的执行路径  
    &AppLib   RunPL程序当前路径下lib目录  
###下载

[RunPL - 0.1.0](https://raw.github.com/LoginQin/RunPL/master/assert/RunPL.zip)

##关 于 作  者
覃 炜  
Email: qinwei081@gmail.com  
Version: 0.0.1  
-= ChineseTiger =-  

