using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using ALG;
using gpLib;
using Microsoft.Win32;
using nLuaFramework;
using PCSC;
using xTool;
using Jint;
using Jint.Runtime;

namespace GUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private SCardContext Context;
        private XMLTool apduXML;
        private XMLTool scriptXML;
        private XMLTool cmXML;
        private XMLTool capXML;
        private XMLTool sdXML;
        private XMLTool cfgXML = null;

        private Object tabSelected;

        private int errCount;

        private Object tabCurent;
        //private gplib gp = null;
        public static gplib gp = null;

        public static TreeView installView;
        public static TextBox installBox;
        public static TextBox scriptBox;
        public static gplib installGP;
        public static string secutiyDomain;

        //public static gplib Card;

        List<string> csFile = new List<string>();

        private FileStream sctFS = null;
        private StreamWriter sctSW = null;

        private FileStream capFS = null;
        private StreamWriter capSW = null;

        private ThreadStart loadCAP_ThreadStart = null;
        private Thread loadCAP_Thread = null;

        private ThreadStart runScript_ThreadStart = null;
        private Thread runScript_Thread = null;

        public static Engine mSC; 

        public const string start_tip = "\n------Start------\n";
        public const string success_tip = "\n------Passed------\n";
        public const string finish_tip = "\n------Finished------\n";
        public const string fail_tip = "\n------Failed------\n";
        public const string stop_tip = "\n------Stopped------\n";

        public MainWindow()
        {

            cfgXML = new XMLTool("config.xml");
            InitializeComponent();
            //if (SystemParameters.PrimaryScreenHeight < 810)
            //    this.Height = SystemParameters.PrimaryScreenHeight - 30;

            tabCurent = tabControl.Items.GetItemAt(0);

            installBox = viewOUTPUT;
            scriptBox = scriptresOUTPUT;
            /*
                        gpVer.Text = "2.2";
                        channel.Text = "0";
                        sdXmlBox.Text = "SecurityDomainConfig.xml";
                        kmcCombox.Text = "No Diversify";
                        loadLen.Text = "200";
                        apduTimer.IsChecked = false;
                        jcopport.Text = "8050";
                        radioPlain.IsChecked = false;
                        radioMac.IsChecked = true;
                        radioEncMac.IsChecked = false;*/


            if (cfgXML != null)
            {
                gpVer.Text = cfgXML.Read("config/gpVer", "");
                channel.Text = cfgXML.Read("config/channel", "");
                sdXmlBox.Text = cfgXML.Read("config/SDconfig", "");
                kmcCombox.Text = cfgXML.Read("config/kmc", "");
                loadLen.Text = cfgXML.Read("config/loadLen", "");
                jcopport.Text = cfgXML.Read("config/jcopport", "");

                modeCombox.Visibility = System.Windows.Visibility.Hidden;

                string temp = cfgXML.Read("config/securityLevel", "");
                if (temp == "1")
                    radioMac.IsChecked = true;
                else if (temp == "3")
                    radioEncMac.IsChecked = true;
                else
                    radioPlain.IsChecked = true;

                temp = cfgXML.Read("config/apduTimer", "");
                if (temp.ToLower() == "true")
                    apduTimer.IsChecked = true;
                else
                    apduTimer.IsChecked = false;

                temp = cfgXML.Read("config/scriptLog", "");
                if (temp.ToLower() == "true")
                    scriptLog.IsChecked = true;
                else
                    scriptLog.IsChecked = false;

                temp = cfgXML.Read("config/capLog", "");
                if (temp.ToLower() == "true")
                    capLog.IsChecked = true;
                else
                    capLog.IsChecked = false;
            }
            else
            {
                MessageBox.Show("config.xml not found.");
            }

            apduXML = new XMLTool("apduHist.xml");
            scriptXML = new XMLTool("scriptHist.xml");
            cmXML = new XMLTool(sdXmlBox.Text);
            capXML = new XMLTool("capHist.xml");
            sdXML = new XMLTool("sdHist.xml");


            /*
            readme.Text =
                "version:5.8.8.6" + "\n\n" +
                "support PCSC/CCID reader." + "\n" +
                "support scp01/scp02, security level:" + "\n" +
                "        no secure / C-MAC / C-DECRYPTION and C-MAC." + "\n" +
                "support data authentication pattern (DAP):" + "\n" +
                "        des dap / rsa dap." + "\n" +
                "support delegated management (DM):" + "\n" +
                "        GP2.1 / GP2.2 / China Mobile CMS2AC /" + "\n" +
                "        China Unicom UICC / China Telecom UICC." + "\n" +
                "support APDU send & receive." + "\n" +
                "support .txt script / .lua script." + "\n" +
                "support cap file download." + "\n" +
                "support multi-cap download (.txt)." + "\n" +
                "support view & install & delete card content:" + "\n" +
                "        application、executable load files、security domain." + "\n" +
                "support get card available memory" + "\n" +
                "support KMC diversify: CPG202 / CPG212" + "\n" +
                "support des/3des,sha1,rsa (16-16384bits),PKCS#1." + "\n" +
                "support SM2 SM3 SM4." + "\n" +
                "support JCOP Debug." + "\n" +

                "\n\n" +
                "tips:" + "\n" +
                "hit the Enter key can send apdu in APDU tab." + "\n" +
                "you can drag a script or cap file to LUA tab or CAP tab." + "\n" +
                "button DisCon also can abort you current operation,like download CAP." + "\n" +
                "delete all aid in Applet/Application/SecurityDomain," + "\n" +
                "       if Applet/Application/SecurityDomain selected in VIEW tab." + "\n" +
                "move mouse to one AID you will see Life Cycle or Privileges" + "\n" +
                "       in VIEW tab after click button List." + "\n" +
                "click right mouse button can copy the selected AID in VIEW tab." + "\n" +
                "\n"
                ;
             * **/
        }

        protected override void OnClosed(EventArgs e)
        {
            //GC.Collect();
            System.Environment.Exit(0);
            base.OnClosed(e);
        }

        public static void errPrint(string s)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
            new Action(() =>
            {
                gplib.errCount++;
                gp.print(s);
            }));

        }

        public static void print(string s)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
            new Action(() =>
            {
                gp.print(s);
            }));
        }

        /*
                private void GetCSharpFile(string rootPath)
                {
                    string[] subPaths = System.IO.Directory.GetDirectories(rootPath);//得到所有子目录

                    foreach (string path in subPaths)
                        GetCSharpFile(path);
           
                    string[] files = System.IO.Directory.GetFiles(rootPath,"*.cs");

                    foreach (string file in files)
                        this.csFile.Add(file);
                }

        
                private void RunDLL() {
                    gp = new gplib(GetText(comboBoxSelectReader), "", "", "", "0");
                    gp.setConsole(scriptresOUTPUT);

                    Card = gp;
                    XmlElement root = scriptXML.GetXmlDocumentRoot();
                    string node = "script" + System.DateTime.Now.ToString("yyMMddHHmmssfff");
                    XmlNode temp = scriptXML.CreateElement(node, GetText(comboBoxScript));
                    if (root.FirstChild == null)
                        scriptXML.XmlInsert(temp);
                    else
                    {
                        scriptXML.XmlInsertBefore(root.FirstChild, temp);
                        scriptXML.RemoveLastNode(root, 30);
                    }

                    Assembly assembly = Assembly.LoadFile(GetText(comboBoxScript));

                    Type type = assembly.GetType("SCPlus.SCP");

                    if (type != null)
                        type.InvokeMember("main", BindingFlags.InvokeMethod, null, null, null);
                    else
                        outToBox("Make sure " + GetText(comboBoxScript) + "\n  have namespce:SCPlus, class:SCP, method:main()");
                }
        
         * 
                private void RunCS()
                {
                    gp = new gplib(GetText(comboBoxSelectReader), "", "", "", "0");
                    gp.setConsole(scriptresOUTPUT);
           
                    Card = gp;
                    XmlElement root = scriptXML.GetXmlDocumentRoot();
                    string node = "script" + System.DateTime.Now.ToString("yyMMddHHmmssfff");
                    XmlNode temp = scriptXML.CreateElement(node, GetText(comboBoxScript));
                    if (root.FirstChild == null)
                        scriptXML.XmlInsert(temp);
                    else
                    {
                        scriptXML.XmlInsertBefore(root.FirstChild, temp);
                        scriptXML.RemoveLastNode(root, 30);
                    }

                    csFile.Clear();
          
                    CompilerParameters parameters = new CompilerParameters();
                    parameters.GenerateInMemory = true;
                    parameters.GenerateExecutable = false;
                    parameters.TreatWarningsAsErrors = false;

                    parameters.ReferencedAssemblies.Add("SharpCard.dll");      
                    parameters.ReferencedAssemblies.Add("ALG.dll");
           
                   //parameters.ReferencedAssemblies.Add("PresentationFramework.dll");
                    //parameters.ReferencedAssemblies.Add("xTool.dll");
                   // parameters.ReferencedAssemblies.Add("SmartCard Plus.exe");
                    //parameters.ReferencedAssemblies.Add("System.dll");
                    //parameters.ReferencedAssemblies.Add("System.Xaml.dll");
        //            parameters.ReferencedAssemblies.Add("PresentationCore.dll");
        //            parameters.ReferencedAssemblies.Add("WindowsBase.dll");
                    CodeDomProvider _provider = new Microsoft.CSharp.CSharpCodeProvider();
          
                    csFile.Add(GetText(comboBoxScript));
                    int offset =  GetText(comboBoxScript).LastIndexOf("\\");
                    string dir = GetText(comboBoxScript).Substring(0, offset) + "\\csLib";

                    DirectoryInfo dirRoot = new DirectoryInfo(dir);

                    if (dirRoot.Exists)           
                        GetCSharpFile(dir);

                    CompilerResults results = _provider.CompileAssemblyFromFile(parameters, csFile.ToArray());
            
                    var t1 = System.DateTime.Now;

                    if (results.Errors.Count < 1)
                    {
                        Assembly assembly = results.CompiledAssembly;
                        Type _evaluateType = assembly.GetType("SCPlus.SCP");
                
                        if (_evaluateType != null)
                            _evaluateType.InvokeMember("main", BindingFlags.InvokeMethod, null, null, null);
                        else
                            outToBox("Make sure " + GetText(comboBoxScript) + "\n  have namespce:SCPlus, class:SCP, method:main()");
                    }
                    else
                    {
                        string msg = null;
                        for (int index = 0; index < results.Errors.Count; index++)
                        {
                            CompilerError error = results.Errors[index];
                            msg += "ERROR" + (index + 1) + Environment.NewLine;
                            msg += error.FileName ;
                            msg += "    @line" + error.Line + ",column" + error.Column + Environment.NewLine;
                            msg += error.ErrorText + Environment.NewLine;
                            msg += Environment.NewLine;
                            outToBox(msg);
                        }
                    }

                    var t2 = System.DateTime.Now;

                    outToBox("\n" + GetText(comboBoxScript));
                    if (t2.Millisecond >= t1.Millisecond)
                        outToBox("Time: " + (t2.Hour * 60 * 60 + t2.Minute * 60 + t2.Second - t1.Hour * 60 * 60 - t1.Minute * 60 - t1.Second) + "s " + (t2.Millisecond - t1.Millisecond) + "ms");
                    else
                        outToBox("Time: " + (t2.Hour * 60 * 60 + t2.Minute * 60 + t2.Second - t1.Hour * 60 * 60 - t1.Minute * 60 - t1.Second - 1) + "s " + (t2.Millisecond + 1000 - t1.Millisecond) + "ms");

                }
        */

        private void RunLUA()
        {

            if (gp != null && gp.isDebug())
                gp.stopDebug();
            gp = new gplib(GetText(comboBoxSelectReader), "404142434445464748494A4B4C4D4E4F", "404142434445464748494A4B4C4D4E4F", "404142434445464748494A4B4C4D4E4F", "0");

            gp.setConsole(scriptresOUTPUT);
            gp.setSctLog(sctSW);
            gp.setPort(GetText(jcopport));
            outToBox(start_tip);
            //Card = gp;

            XmlElement root = scriptXML.GetXmlDocumentRoot();
            string node = "script" + System.DateTime.Now.ToString("yyMMddHHmmssfff");
            XmlNode temp = scriptXML.CreateElement(node, GetText(comboBoxScript));
            if (root.FirstChild == null)
                scriptXML.XmlInsert(temp);
            else
            {
                scriptXML.XmlInsertBefore(root.FirstChild, temp);
                scriptXML.RemoveLastNode(root, 30);
            }

            using (LuaFramework lua = new LuaFramework())
            {
                lua.BindLuaApiClass(new NLuaAPI());
                lua.ExecuteFile(GetText(comboBoxScript));
            }
        }


        private void RunJS()
        {

            if (gp != null && gp.isDebug())
                gp.stopDebug();
            gp = new gplib(GetText(comboBoxSelectReader), "404142434445464748494A4B4C4D4E4F", "404142434445464748494A4B4C4D4E4F", "404142434445464748494A4B4C4D4E4F", "0");

            gp.setConsole(scriptresOUTPUT);
            gp.setSctLog(sctSW);
            gp.setPort(GetText(jcopport));
            outToBox(start_tip);

            XmlElement root = scriptXML.GetXmlDocumentRoot();
            string node = "script" + System.DateTime.Now.ToString("yyMMddHHmmssfff");
            XmlNode temp = scriptXML.CreateElement(node, GetText(comboBoxScript));
            if (root.FirstChild == null)
                scriptXML.XmlInsert(temp);
            else
            {
                scriptXML.XmlInsertBefore(root.FirstChild, temp);
                scriptXML.RemoveLastNode(root, 30);
            }

            JsAPI obj = new JsAPI(GetText(comboBoxScript));

            try
            {
                string js = File.ReadAllText(GetText(comboBoxScript));
                mSC = new Engine();
   
                mSC.SetValue("js", obj);

                mSC.Execute(js);
            }
            catch (JavaScriptException ex)
            {
                int line = ex.LineNumber;
                int column = ex.Column;
                string file = "'" + obj.GetScriptPath() + "'" + " or include lib @";
                string info = ex.Message;

                string err = string.Format("File {0} Line {1} Column {2}:", file, line, column) + "\n" + info;
               
                throw new Exception(err + "\n");
            }
            catch (Exception)
            {
                throw;
            }

        }

        //script tab    
        /*        private void RunTXT()
                {         
                    gp = new gplib(GetText(comboBoxSelectReader), "", "", "", "0");
                    gp.setConsole(scriptresOUTPUT);
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        if (apduTimer.IsChecked == true)
                            gp.setTime(true);
                    }));
                    StreamReader sr = new StreamReader(GetText(comboBoxScript), Encoding.Default);
                    string line;

                    XmlElement root = scriptXML.GetXmlDocumentRoot();
                    string node = "script" + System.DateTime.Now.ToString("yyMMddHHmmssfff");
                    XmlNode temp = scriptXML.CreateElement(node, GetText(comboBoxScript));
                    if (root.FirstChild == null)
                        scriptXML.XmlInsert(temp);
                    else
                    {
                        scriptXML.XmlInsertBefore(root.FirstChild, temp);
                        scriptXML.RemoveLastNode(root, 30);
                    }
                    var t1 = System.DateTime.Now;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string temp1 = doScript(line);               

                        if (temp1 == "" || temp1 == null)
                            continue;

                        if (temp1.IndexOf("RESET", 0) >= 0)
                        {
                            gp.Reset();
                            continue;
                        }

                        if (temp1.IndexOf("ASSERT", 0) >= 0)
                        {
                            string resp = temp1.Substring(temp1.IndexOf("ASSERT", 0) + 6);
                            string real = gp.getResponse();
                            string sw = real.Substring(real.Length - 4);
                            if (resp.Length == 4)
                            {
                                if (!resp.Equals(sw, StringComparison.OrdinalIgnoreCase))
                                {
                                    outToBox("ERROR,EXPECT:" + resp + "\n");
                                    this.errCount++;
                                }
                            }
                            else
                            {
                                if (!resp.Equals(real, StringComparison.OrdinalIgnoreCase))
                                {
                                    outToBox("ERROR,EXPECT:" + resp + "\n");
                                    this.errCount++;
                                }
                            }

                        }
                        else
                        {
                            gp.secApdu(temp1);
                        }              
                    }
                    var t2 = System.DateTime.Now;

                    outToBox("\n" + GetText(comboBoxScript));
                    if (t2.Millisecond >= t1.Millisecond)
                        outToBox("Time: " + (t2.Hour * 60 * 60 + t2.Minute * 60 + t2.Second - t1.Hour * 60 * 60 - t1.Minute * 60 - t1.Second) + "s " + (t2.Millisecond - t1.Millisecond) + "ms");
                    else
                        outToBox("Time: " + (t2.Hour * 60 * 60 + t2.Minute * 60 + t2.Second - t1.Hour * 60 * 60 - t1.Minute * 60 - t1.Second - 1) + "s " + (t2.Millisecond + 1000 - t1.Millisecond) + "ms");

                    sr.Close();
                }
        */


        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void scriptClick(object sender, RoutedEventArgs e)
        {
            if (scriptBtn.Content.ToString() == "Stop")
            {
                if (runScript_Thread != null)
                    runScript_Thread.Abort();
                runScript_Thread = null;
                runScript_ThreadStart = null;

                /*try
                {
                    ((IScriptControl)mSC).AddCode("throw '1234'");
                }
                catch (Exception ex) { }*/

               

                    //Thread.CurrentThread.Suspend();
                   //Thread.Sleep(10000);
                    

            }
            else
            {
                if (scriptBtn.Content.ToString() == "Run")
                    scriptBtn.Content = "Stop";

                runScript_ThreadStart = new ThreadStart(this.runScript);
                runScript_Thread = new Thread(runScript_ThreadStart)
                {
                    IsBackground = true
                };
                runScript_Thread.Start();
            }

        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void runScript()
        {
            {
                sctFS = null;
                sctSW = null;
                StreamReader sr = null;
                bool iscount = true;
                DateTime t1, t2;
                t1 = DateTime.MinValue;
                t2 = DateTime.MinValue;
                int lineNum = 0;
                bool includeReset = false;
                int i = 0;
                string logfile = "";

                tabCurent = tabControl.Items.GetItemAt(1);
                try
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        comboBoxSelectReader.IsEnabled = false;
                    }));

                    this.errCount = 0;
                    gplib.errCount = 0;
                    string ver = "00";
                    string sl = "00";
                    t1 = System.DateTime.Now;
                    string fileFormat = System.IO.Path.GetExtension(GetText(comboBoxScript));
                    if (fileFormat.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            if (scriptLog.IsChecked == true && File.Exists(GetText(comboBoxScript)))
                            {
                                string temppp = GetText(comboBoxScript).Remove(GetText(comboBoxScript).LastIndexOf("."));
                                logfile = temppp + "_script_txt.log";
                                sctFS = new FileStream(temppp + "_script_txt.log", FileMode.Create);
                                sctSW = new StreamWriter(sctFS);
                            }
                        }));
                        if (gp != null && gp.isDebug())
                        {
                            gp.stopDebug();
                        }
                        gp = new gplib(GetText(comboBoxSelectReader), "404142434445464748494A4B4C4D4E4F", "404142434445464748494A4B4C4D4E4F", "404142434445464748494A4B4C4D4E4F", "0");
                        gp.setPort(GetText(jcopport));
                        ver = "00";
                        sl = "00";

                        gp.setConsole(scriptresOUTPUT);
                        gp.setSctLog(sctSW);

                        outToBox(start_tip);

                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            if (apduTimer.IsChecked == true)
                                gp.setTime(true);
                        }));
                        sr = new StreamReader(GetText(comboBoxScript), Encoding.Default);
                        string line;

                        XmlElement root = scriptXML.GetXmlDocumentRoot();
                        string node = "script" + System.DateTime.Now.ToString("yyMMddHHmmssfff");
                        XmlNode temp = scriptXML.CreateElement(node, GetText(comboBoxScript));
                        if (root.FirstChild == null)
                            scriptXML.XmlInsert(temp);
                        else
                        {
                            scriptXML.XmlInsertBefore(root.FirstChild, temp);
                            scriptXML.RemoveLastNode(root, 30);
                        }
                        while ((line = sr.ReadLine()) != null)
                        {
                            lineNum++;
                            string temp1 = doScript(line);

                            if (temp1 == "" || temp1 == null)
                                continue;

                            if (temp1.IndexOf("RESET", 0) >= 0)
                            {
                                outToBox("Reader: " + gp.getReaderName());
                                gp.Reset();
                                includeReset = true;
                                continue;
                            }

                            if (!includeReset)
                                throw new Exception("Please add \"reset\" to the beginning of script.");

                            if (temp1.IndexOf("LOADKEY", 0) >= 0)
                            {
                                if (temp1.Length != 107)
                                    throw new ArgumentException("Error, LOADKEY Format Should Be: Version(1byte) + Security Level(1byte) + ENC(16byte) + MAC(16byte) + DEK(16byte)." + "\nLine: " + lineNum);

                                ver = temp1.Substring(7, 2);
                                sl = temp1.Substring(9, 2);

                                string enc = temp1.Substring(11, 32);
                                string mac = temp1.Substring(43, 32);
                                string dek = temp1.Substring(75, 32);

                                gp.setENC(enc);
                                gp.setMAC(mac);
                                gp.setDEK(dek);

                                continue;
                            }

                            if (temp1.IndexOf("AUTH", 0) >= 0)
                            {
                                gp.initUpdate(ver);
                                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                                    outToBox("Error, AUTH Failed!" + "\nLine: " + lineNum);
                                gp.externalAuthenticate(sl);
                                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                                    outToBox("Error, AUTH Failed!" + "\nLine: " + lineNum);
                                continue;
                            }

                            if (temp1.IndexOf("ASSERT", 0) >= 0)
                            {
                                string resp = temp1.Substring(temp1.IndexOf("ASSERT", 0) + 6);
                                string real = gp.getResponse();
                                string sw = real.Substring(real.Length - 4);
                                if (resp.Length == 4)
                                {
                                    if (!resp.Equals(sw, StringComparison.OrdinalIgnoreCase))
                                    {
                                        outToBox("Error, Expected SW: " + resp + "\nLine: " + lineNum);
                                        this.errCount++;
                                    }
                                }
                                else
                                {
                                    if (!resp.Equals(real, StringComparison.OrdinalIgnoreCase))
                                    {
                                        outToBox("Error, Expected Response:" + resp + "\nLine: " + lineNum);
                                        this.errCount++;
                                    }
                                }

                            }
                            else
                            {
                                gp.secApdu(temp1);
                            }
                        }

                    }
                    else if (fileFormat.Equals(".lua", StringComparison.OrdinalIgnoreCase))
                    {
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                      new Action(() =>
                      {
                          if (scriptLog.IsChecked == true && File.Exists(GetText(comboBoxScript)))
                          {
                              string temppp = GetText(comboBoxScript).Remove(GetText(comboBoxScript).LastIndexOf("."));
                              logfile = temppp + "_script_lua.log";
                              sctFS = new FileStream(temppp + "_script_lua.log", FileMode.Create);
                              sctSW = new StreamWriter(sctFS);
                          }
                      }));
                        iscount = false;
                        RunLUA();
                        this.errCount = gplib.errCount;
                    }
                    else if (fileFormat.Equals(".js", StringComparison.OrdinalIgnoreCase))
                    {
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                      new Action(() =>
                      {
                          if (scriptLog.IsChecked == true && File.Exists(GetText(comboBoxScript)))
                          {
                              string temppp = GetText(comboBoxScript).Remove(GetText(comboBoxScript).LastIndexOf("."));
                              logfile = temppp + "_script_js.log";
                              sctFS = new FileStream(temppp + "_script_js.log", FileMode.Create);
                              sctSW = new StreamWriter(sctFS);
                          }
                      }));
                        iscount = false;
                        RunJS();
                        this.errCount = gplib.errCount;
                    }
                    else
                    {
                        throw new ArgumentException("NO SUPPORT FILE: " + fileFormat);
                    }
                    if (this.errCount == 0)
                        i = 1;
                }
                catch (ThreadAbortException)
                {
                    //if (iscount == false)
                     //   this.errCount = (gplib.errCount - 1);
                    i = 3;
                }
                catch (Exception ex)
                {
                    if (iscount == false)
                        this.errCount = gplib.errCount;
                    else
                        this.errCount++;
                    i = 2;
                    outToBox(ex.Message);
                }
                finally
                {
                    t2 = System.DateTime.Now;

                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            comboBoxSelectReader.IsEnabled = true;
                            scriptBtn.Content = "Run";
                        }));


                    if (i == 1)
                    {
                        outToBox("\n" + GetText(comboBoxScript));
                        if (t1 != DateTime.MinValue && t2 != DateTime.MinValue)
                        {
                            TimeSpan ts = t2 - t1;
                            if (ts.Days > 0)
                                outToBox("Total Time: " + ts.Days + " Days " + ts.Hours + " Hours " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds");
                            else if (ts.Hours > 0)
                                outToBox("\nTotal Time: " + ts.Hours + " Hours " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds");
                            else
                                outToBox("Total Time: " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds " + ts.Milliseconds + " Milliseconds");
                        }

                        if (logfile == "")
                            outToBox("\nNo Log Saved");
                        else
                            outToBox("\nLog Saved to " + logfile);

                        outToBox(success_tip);
                    }
                    else if (i == 2)
                    {
                        outToBox("\n" + GetText(comboBoxScript));
                        if (iscount)
                        {
                            this.errCount++;
                        }
                        outToBox("Errors: " + this.errCount);
                        if (t1 != DateTime.MinValue && t2 != DateTime.MinValue)
                        {
                            TimeSpan ts = t2 - t1;
                            if (ts.Days > 0)
                                outToBox("Total Time: " + ts.Days + " Days " + ts.Hours + " Hours " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds");
                            else if (ts.Hours > 0)
                                outToBox("\nTotal Time: " + ts.Hours + " Hours " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds");
                            else
                                outToBox("Total Time: " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds " + ts.Milliseconds + " Milliseconds");
                        }

                        if (logfile == "")
                            outToBox("\nNo Log Saved");
                        else
                            outToBox("\nLog Saved to " + logfile);

                        outToBox(fail_tip);
                    }
                    else if (i == 3)
                    {
                        outToBox("\n" + GetText(comboBoxScript));

                        outToBox("Errors: " + this.errCount);
                        if (t1 != DateTime.MinValue && t2 != DateTime.MinValue)
                        {
                            TimeSpan ts = t2 - t1;
                            if (ts.Days > 0)
                                outToBox("Total Time: " + ts.Days + " Days " + ts.Hours + " Hours " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds");
                            else if (ts.Hours > 0)
                                outToBox("\nTotal Time: " + ts.Hours + " Hours " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds");
                            else
                                outToBox("Total Time: " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds " + ts.Milliseconds + " Milliseconds");
                        }

                        if (logfile == "")
                            outToBox("\nNo Log Saved");
                        else
                            outToBox("\nLog Saved to " + logfile);

                        outToBox(stop_tip);
                    }
                    else
                    {
                        outToBox("\n" + GetText(comboBoxScript));
                        outToBox("Errors: " + this.errCount);
                        if (t1 != DateTime.MinValue && t2 != DateTime.MinValue)
                        {
                            TimeSpan ts = t2 - t1;
                            if (ts.Days > 0)
                                outToBox("Total Time: " + ts.Days + " Days " + ts.Hours + " Hours " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds");
                            else if (ts.Hours > 0)
                                outToBox("\nTotal Time: " + ts.Hours + " Hours " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds");
                            else
                                outToBox("Total Time: " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds " + ts.Milliseconds + " Milliseconds");
                        }

                        if (logfile == "")
                            outToBox("\nNo Log Saved");
                        else
                            outToBox("\nLog Saved to " + logfile);

                        outToBox(fail_tip);
                    }

                    if (sr != null)
                        sr.Close();

                    if (sctSW != null)
                    {
                        sctSW.Flush();
                        sctSW.Close();
                        sctFS.Close();
                    }

                    if (gp != null)
                        gp.setSctLog(null);
                    if (NLuaAPI.gp1 != null)
                        NLuaAPI.gp1.setSctLog(null);
                    if (NLuaAPI.gp2 != null)
                        NLuaAPI.gp2.setSctLog(null);

                    sctSW = null;

                    if (runScript_Thread != null)
                    {
                        runScript_Thread.Abort();
                        runScript_Thread = null;
                    }

                    if (runScript_ThreadStart != null)
                        runScript_ThreadStart = null;
                }
            }
        }

        private void scriptresChangeed(object sender, TextChangedEventArgs e)
        {
            scriptresOUTPUT.ScrollToEnd();
        }

        private void GetScriptLists(object sender, EventArgs e)
        {
            comboBoxScript.Items.Clear();
            XmlElement root = scriptXML.GetXmlDocumentRoot();
            string his, node;

            if (root.HasChildNodes)
            {
                XmlNode temp = root.FirstChild;
                while (temp != null)
                {
                    node = root.Name + "/" + temp.Name;
                    his = scriptXML.Read(node, "");
                    comboBoxScript.Items.Add(his);
                    temp = temp.NextSibling;
                }
            }

        }

        private void openClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                //dialog.Filter = "script files (*.scp)|*.scp";
                Filter = "script files (*.txt, *.lua, *.js)|*.txt; *.lua; *.js"
            };
            if (dialog.ShowDialog() == true)
            {
                comboBoxScript.Text = dialog.FileName;
            }
        }

        //script tab end


        //apdu tab
        private void SendClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                tabCurent = tabControl.Items.GetItemAt(0);
                try
                {
                    if (gp == null)
                        throw new PCSCException(SCardError.NoReaderConnected);
                    gp.setConsole(apduresOUTPUT);
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            if (apduTimer.IsChecked == true)
                                gp.setTime(true);

                            if (apduAuto.IsChecked == true)
                                gp.setAuto(1);
                            else
                                gp.setAuto(0);
                        }));
                    string tempAPDU = GetText(comboBoxAPDU);
                    gp.secApdu(tempAPDU);

                    XmlElement root = apduXML.GetXmlDocumentRoot();
                    string node = "apdu" + System.DateTime.Now.ToString("yyMMddHHmmssfff");
                    XmlNode temp = apduXML.CreateElement(node, tempAPDU.Replace(" ", ""));
                    if (root.FirstChild == null)
                        apduXML.XmlInsert(temp);
                    else
                    {
                        apduXML.XmlInsertBefore(root.FirstChild, temp);
                        apduXML.RemoveLastNode(root, 100);
                    }
                }
                catch (Exception ex)
                {
                    outToBox(ex.Message);
                }
            }).Start();
        }

        private void enterAPDUClick(object sender, KeyEventArgs e)
        {
            new Thread(() =>
            {
                tabCurent = tabControl.Items.GetItemAt(0);
                if (e.Key == Key.Enter)
                {
                    try
                    {
                        if (gp == null)
                            throw new PCSCException(SCardError.NoReaderConnected);
                        gp.setConsole(apduresOUTPUT);
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                            new Action(() =>
                            {
                                if (apduTimer.IsChecked == true)
                                    gp.setTime(true);

                                if (apduAuto.IsChecked == true)
                                    gp.setAuto(1);
                                else
                                    gp.setAuto(0);
                            }));
                        string tempAPDU = GetText(comboBoxAPDU);
                        gp.secApdu(tempAPDU);

                        XmlElement root = apduXML.GetXmlDocumentRoot();
                        string node = "apdu" + System.DateTime.Now.ToString("yyMMddHHmmssfff");
                        XmlNode temp = apduXML.CreateElement(node, tempAPDU.Replace(" ", ""));
                        if (root.FirstChild == null)
                            apduXML.XmlInsert(temp);
                        else
                        {
                            apduXML.XmlInsertBefore(root.FirstChild, temp);
                            apduXML.RemoveLastNode(root, 100);
                        }
                    }
                    catch (Exception ex)
                    {
                        outToBox(ex.Message);
                    }
                }
            }).Start();
        }

        private void apduresChangeed(object sender, TextChangedEventArgs e)
        {
            apduresOUTPUT.ScrollToEnd();
        }

        private void GetAPDULists(object sender, EventArgs e)
        {
            comboBoxAPDU.Items.Clear();
            XmlElement root = apduXML.GetXmlDocumentRoot();
            string his, node;

            if (root.HasChildNodes)
            {
                XmlNode temp = root.FirstChild;
                while (temp != null)
                {
                    node = root.Name + "/" + temp.Name;
                    his = apduXML.Read(node, "");
                    comboBoxAPDU.Items.Add(his);
                    temp = temp.NextSibling;
                }
            }

        }

        //apdu tab end



        private void GetReaderLists(object sender, EventArgs e)
        {
            comboBoxSelectReader.Items.Clear();

            try
            {
                this.Context = gplib.Context;
                this.Context.Establish(SCardScope.System);

                string[] ReadersList = this.Context.GetReaders();
                this.Context.Dispose();
                if (ReadersList != null)
                    foreach (var temp in ReadersList)
                        comboBoxSelectReader.Items.Add(temp);
            }
            catch (Exception ex)
            {
                outToBox(ex.Message);
            }
            finally
            {
                comboBoxSelectReader.Items.Add("JCOP Debug");
            }

        }


        private void DisConnectClick(object sender, RoutedEventArgs e)
        {
            tabCurent = tabControl.SelectedItem;
            try
            {
                installBtn.IsEnabled = false;
                putkeyBtn.IsEnabled = false;
                if (gp != null)
                    outToBox(gp.DisConnect());
                else
                    outToBox("No Connected Reader");
            }
            catch (Exception ex)
            {
                outToBox(ex.Message);
            }

        }

        private void ResetClick(object sender, RoutedEventArgs e)
        {
            tabCurent = tabControl.SelectedItem;
            try
            {
                installBtn.IsEnabled = false;
                putkeyBtn.IsEnabled = false;

                if (gp != null && gp.isDebug())
                    gp.stopDebug();
                gp = new gplib(GetText(comboBoxSelectReader), "", "", "", "0");
                gp.setPort(GetText(jcopport));
                if (apduTimer.IsChecked == true)
                    gp.setTime(true);
                Object apduobj = tabControl.Items.GetItemAt(0);
                Object sctobj = tabControl.Items.GetItemAt(1);
                Object cmobj = tabControl.Items.GetItemAt(2);
                Object vwobj = tabControl.Items.GetItemAt(3);

                if (sctobj.Equals(tabCurent))
                    gp.setConsole(scriptresOUTPUT);
                else if (cmobj.Equals(tabCurent))
                    gp.setConsole(cmresOUTPUT);
                else if (vwobj.Equals(tabCurent))
                    gp.setConsole(viewOUTPUT);
                else
                    gp.setConsole(apduresOUTPUT);
                gp.Reset();
            }
            catch (ArgumentNullException)
            {
                outToBox("No Selectd Redaer\n");
            }
            catch (Exception ex)
            {
                outToBox(ex.Message);
            }

        }


        private void outToBox(string s)
        {
            Object apduobj = tabControl.Items.GetItemAt(0);
            Object sctobj = tabControl.Items.GetItemAt(1);
            Object cmobj = tabControl.Items.GetItemAt(2);
            Object vwobj = tabControl.Items.GetItemAt(3);

            if (sctobj.Equals(tabCurent))
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        scriptresOUTPUT.AppendText(s + "\n");

                        if (scriptLog.IsChecked == true && sctSW != null)
                        {
                            sctSW.WriteLine(s);
                            //sctSW.Flush();
                        }

                    }));
            }
            else if (cmobj.Equals(tabCurent))
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        cmresOUTPUT.AppendText(s + "\n");

                        if (capLog.IsChecked == true && capSW != null)
                        {
                            capSW.WriteLine(s);
                        }
                    }));
            }
            else if (vwobj.Equals(tabCurent))
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        viewOUTPUT.AppendText(s + "\n");
                    }));
            }
            else
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        apduresOUTPUT.AppendText(s + "\n");
                    }));
            }

        }

        private void tabSelectCilck(object sender, SelectionChangedEventArgs e)
        {
            this.tabSelected = tabControl.SelectedItem;
        }

        private string doAPDU(string apdu)
        {
            apdu = apdu.Replace(" ", "");
            apdu = apdu.Replace("\t", "");
            if (apdu.Length == 0)
                throw new ArgumentException("APDU is NULL");
            return apdu;
        }

        private string doScript(string apdu)
        {
            apdu = apdu.Replace(" ", "");
            apdu = apdu.Replace("\t", "");
            if (apdu.StartsWith("#") || apdu.StartsWith("//") || apdu.Length == 0)
                return null;
            char[] noteflag = { '/', '/' };
            apdu = apdu.TrimEnd(noteflag);
            apdu = apdu.TrimEnd('#');

            return apdu.ToUpper();
        }


        private bool RadioisChecked(RadioButton btn)
        {
            bool b = false;
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                new Action(() =>
                {
                    if (btn.IsChecked == true)
                        b = true;
                }));
            return b;
        }




        private void aesCheck(object sender, RoutedEventArgs e)
        {
            icvBox.IsEnabled = true;
            keyBox.IsEnabled = true;
            dataBox.IsEnabled = true;
            desKeyBox.Text = "Key";
            desencBtn.Content = "Encrypt";
            desdecBtn.Content = "Decrypt";
            desdecBtn.IsEnabled = true;

            desKeyBox.Width = 30;
            keyBox.Width = 480;
            keyBox.Margin = new Thickness(40, 59, 0, 0);

            modeCombox.Visibility = System.Windows.Visibility.Visible;
            modeCombox.Items.Clear();
            modeCombox.Items.Add("ECB");
            modeCombox.Items.Add("CBC");

            modeCombox.SelectedIndex = 0;
        }



        private void desCheck(object sender, RoutedEventArgs e)
        {
            icvBox.IsEnabled = true;
            keyBox.IsEnabled = true;
            dataBox.IsEnabled = true;
            desKeyBox.Text = "Key";
            desencBtn.Content = "Encrypt";
            desdecBtn.Content = "Decrypt";
            desdecBtn.IsEnabled = true;

            desKeyBox.Width = 30;
            keyBox.Width = 480;
            keyBox.Margin = new Thickness(40, 59, 0, 0);

            modeCombox.Visibility = System.Windows.Visibility.Visible;
            modeCombox.Items.Clear();
            modeCombox.Items.Add("ECB");
            modeCombox.Items.Add("CBC");

            modeCombox.SelectedIndex = 0;
        }



        private void tdesCheck(object sender, RoutedEventArgs e)
        {
            icvBox.IsEnabled = true;
            keyBox.IsEnabled = true;
            dataBox.IsEnabled = true;
            desKeyBox.Text = "Key";
            desencBtn.Content = "Encrypt";
            desdecBtn.Content = "Decrypt";
            desdecBtn.IsEnabled = true;

            desKeyBox.Width = 30;
            keyBox.Width = 480;
            keyBox.Margin = new Thickness(40, 59, 0, 0);

            modeCombox.Visibility = System.Windows.Visibility.Visible;
            modeCombox.Items.Clear();
            modeCombox.Items.Add("ECB");
            modeCombox.Items.Add("CBC");

            modeCombox.SelectedIndex = 0;
        }

        //sm4
        private void sm4Check(object sender, RoutedEventArgs e)
        {
            icvBox.IsEnabled = true;
            keyBox.IsEnabled = true;
            dataBox.IsEnabled = true;
            desKeyBox.Text = "Key";
            desencBtn.Content = "Encrypt";
            desdecBtn.Content = "Decrypt";
            desencBtn.IsEnabled = true;

            desKeyBox.Width = 30;
            keyBox.Width = 480;
            keyBox.Margin = new Thickness(40, 59, 0, 0);

            modeCombox.Visibility = System.Windows.Visibility.Visible;
            modeCombox.Items.Clear();
            modeCombox.Items.Add("ECB");
            modeCombox.Items.Add("CBC");

            modeCombox.SelectedIndex = 0;
        }


        private void macCheck(object sender, RoutedEventArgs e)
        {
            icvBox.IsEnabled = true;
            keyBox.IsEnabled = true;
            dataBox.IsEnabled = true;
            desKeyBox.Text = "Key";
            desencBtn.Content = "MAC";
            desdecBtn.Content = "Pad";
            desdecBtn.IsEnabled = true;

            desKeyBox.Width = 30;
            keyBox.Width = 480;
            keyBox.Margin = new Thickness(40, 59, 0, 0);

            modeCombox.Visibility = System.Windows.Visibility.Visible;
            modeCombox.Items.Clear();
            modeCombox.Items.Add("DES");
            modeCombox.Items.Add("3DES");
            modeCombox.Items.Add("SM4");

            modeCombox.SelectedIndex = 0;
        }


        private void hashCheck(object sender, RoutedEventArgs e)
        {
            icvBox.IsEnabled = false;
            keyBox.IsEnabled = false;
            dataBox.IsEnabled = true;
            desKeyBox.Text = "Key";
            desencBtn.Content = "HASH";
            desdecBtn.Content = "Open File";
            desdecBtn.IsEnabled = true;

            desKeyBox.Width = 30;
            keyBox.Width = 480;
            keyBox.Margin = new Thickness(40, 59, 0, 0);

            modeCombox.Visibility = System.Windows.Visibility.Visible;

            modeCombox.Items.Clear();
            modeCombox.Items.Add("SHA1");
            modeCombox.Items.Add("SHA224");
            modeCombox.Items.Add("SHA256");
            modeCombox.Items.Add("SHA384");
            modeCombox.Items.Add("SHA512");
            modeCombox.Items.Add("MD5");
            modeCombox.Items.Add("SM3");

            modeCombox.Items.Add("SHA3-224");
            modeCombox.Items.Add("SHA3-256");
            modeCombox.Items.Add("SHA3-384");
            modeCombox.Items.Add("SHA3-512");

            modeCombox.SelectedIndex = 0;

        }


        private void hmacCheck(object sender, RoutedEventArgs e)
        {
            icvBox.IsEnabled = false;
            keyBox.IsEnabled = true;
            dataBox.IsEnabled = true;
            desKeyBox.Text = "Key";
            desencBtn.Content = "HMAC";
            desdecBtn.Content = "Open File";
            desdecBtn.IsEnabled = true;

            desKeyBox.Width = 30;
            keyBox.Width = 480;
            keyBox.Margin = new Thickness(40, 59, 0, 0);

            modeCombox.Visibility = System.Windows.Visibility.Visible;

            modeCombox.Items.Clear();
            modeCombox.Items.Add("SHA1");
            modeCombox.Items.Add("SHA224");
            modeCombox.Items.Add("SHA256");
            modeCombox.Items.Add("SHA384");
            modeCombox.Items.Add("SHA512");
            modeCombox.Items.Add("MD5");
            //modeCombox.Items.Add("SM3");

            //modeCombox.Items.Add("SHA3-224");
            //modeCombox.Items.Add("SHA3-256");
            //modeCombox.Items.Add("SHA3-384");
            //modeCombox.Items.Add("SHA3-512");

            modeCombox.SelectedIndex = 0;

        }



        private void xorCheck(object sender, RoutedEventArgs e)
        {
            icvBox.IsEnabled = false;
            keyBox.IsEnabled = true;
            dataBox.IsEnabled = true;
            desKeyBox.Text = "Data";
            desencBtn.Content = "XOR";
            desdecBtn.Content = "Decrypt";
            desdecBtn.IsEnabled = false;

            desKeyBox.Width = 30;
            keyBox.Width = 480;
            keyBox.Margin = new Thickness(40, 59, 0, 0);

            modeCombox.Visibility = System.Windows.Visibility.Hidden;
        }

        private void randomChecked(object sender, RoutedEventArgs e)
        {
            icvBox.IsEnabled = false;
            keyBox.IsEnabled = false;
            dataBox.IsEnabled = false;
            //desKeyBox.Text = "Data";
            desencBtn.Content = "Gen";
            desdecBtn.Content = "Clear";
            desdecBtn.IsEnabled = true;

            desKeyBox.Width = 30;
            keyBox.Width = 480;
            keyBox.Margin = new Thickness(40, 59, 0, 0);

            modeCombox.Visibility = System.Windows.Visibility.Hidden;
        }

        private void radio0xCheck(object sender, RoutedEventArgs e)
        {
            icvBox.IsEnabled = false;
            keyBox.IsEnabled = false;
            dataBox.IsEnabled = true;
            //desKeyBox.Text = "Data";
            desencBtn.Content = "Add ,0x";
            desdecBtn.Content = "Remove ,0x";
            desdecBtn.IsEnabled = true;

            desKeyBox.Width = 30;
            keyBox.Width = 480;
            keyBox.Margin = new Thickness(40, 59, 0, 0);

            modeCombox.Visibility = System.Windows.Visibility.Hidden;
        }


        private void convertCheck(object sender, RoutedEventArgs e)
        {
            icvBox.IsEnabled = false;
            keyBox.IsEnabled = false;
            dataBox.IsEnabled = true;
            desKeyBox.Text = "Key";
            //desencBtn.Content = "String2Ascii";
            //desdecBtn.Content = "Ascii2String";
            desdecBtn.IsEnabled = true;

            desKeyBox.Width = 30;
            keyBox.Width = 480;
            keyBox.Margin = new Thickness(40, 59, 0, 0);

            modeCombox.Visibility = System.Windows.Visibility.Visible;

            modeCombox.Items.Clear();
            modeCombox.Items.Add("ASCII");
            modeCombox.Items.Add("Base64");
            modeCombox.Items.Add("Base58_Bitcoin");
            modeCombox.Items.Add("Base58_Ripple");
            modeCombox.Items.Add("Base58_Bitcoin_CheckSum");
            modeCombox.Items.Add("Base58_Ripple_CheckSum");


            modeCombox.SelectedIndex = 0;
        }

        private void modeCombox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RadioisChecked(ConvertRadio))
            {
                if (modeCombox.SelectedIndex == 0)//ascii
                {
                    desencBtn.Content = "String2Ascii";
                    desdecBtn.Content = "Ascii2String";
                }
                else if (modeCombox.SelectedIndex == 1)//base64
                {
                    desencBtn.Content = "Encode";
                    desdecBtn.Content = "Decode";
                }
                else if (modeCombox.SelectedIndex == 2)//base58_bitcoin
                {
                    desencBtn.Content = "Encode";
                    desdecBtn.Content = "Decode";
                }
                else if (modeCombox.SelectedIndex == 3)//base58_ripple
                {
                    desencBtn.Content = "Encode";
                    desdecBtn.Content = "Decode";
                }
                else if (modeCombox.SelectedIndex == 4)//base58_bitcoin_CheckSum
                {
                    desencBtn.Content = "Encode";
                    desdecBtn.Content = "Decode";
                }
                else if (modeCombox.SelectedIndex == 5)//base58_ripple_CheckSum
                {
                    desencBtn.Content = "Encode";
                    desdecBtn.Content = "Decode";
                }
            }
            else if (RadioisChecked(des) || RadioisChecked(tdes) || RadioisChecked(aes) ||
                RadioisChecked(sm4))
            {
                if (modeCombox.SelectedIndex == 0)//ecb
                {
                    icvBox.IsEnabled = false;
                }
                else if (modeCombox.SelectedIndex == 1)//cbc
                {
                    icvBox.IsEnabled = true;
                }
            }
            else if (RadioisChecked(sm1))
            {
                if (modeCombox.SelectedIndex < 4)//ecb
                {
                    icvBox.IsEnabled = false;
                }
                else //cbc
                {
                    icvBox.IsEnabled = true;
                }
            }
        }
        private void eccCurve_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (eccCurve.SelectedIndex == 0)//SM2
            {
                idBox.IsEnabled = true;
                EccSignData_Block.Text = "Data";
                sm2encBtn.IsEnabled = true;
                sm2deBtn.IsEnabled = true;
                smdata_cipher.IsEnabled = true;
                smres_cipher.IsEnabled = true;

                EccSignData_Block.Width = 30;
                EccSignData_Block.Margin = new Thickness(8, 397, 0, 0);
            }
            else//ECC
            {
                sm2encBtn.IsEnabled = false;
                sm2deBtn.IsEnabled = false;
                idBox.IsEnabled = false;
                smdata_cipher.IsEnabled = false;
                smres_cipher.IsEnabled = false;
                EccSignData_Block.Text = "e";

                EccSignData_Block.Width = 30;
                EccSignData_Block.Margin = new Thickness(12, 397, 0, 0);
            }
        }
        private void radioCalcCheck(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = @"C:\WINDOWS\system32\calc.exe"
            };
            Process.Start(startInfo);
        }

        private void sm1Check(object sender, RoutedEventArgs e)
        {
            icvBox.IsEnabled = true;
            keyBox.IsEnabled = true;
            dataBox.IsEnabled = true;
            desKeyBox.Text = "EK|AK";
            desencBtn.Content = "Encrypt";
            desdecBtn.Content = "Decrypt";
            desdecBtn.IsEnabled = true;

            desKeyBox.Width = 45;
            keyBox.Width = 465;
            keyBox.Margin = new Thickness(55, 59, 0, 0);

            modeCombox.Visibility = System.Windows.Visibility.Visible;
            modeCombox.Items.Clear();
            modeCombox.Items.Add("ECB 08 Round");
            modeCombox.Items.Add("ECB 0A Round");
            modeCombox.Items.Add("ECB 0C Round");
            modeCombox.Items.Add("ECB 0E Round");
            modeCombox.Items.Add("CBC 08 Round");
            modeCombox.Items.Add("CBC 0A Round");
            modeCombox.Items.Add("CBC 0C Round");
            modeCombox.Items.Add("CBC 0E Round");

            modeCombox.SelectedIndex = 0;
        }

        private void EccGetPubKeyClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string priK = smprikey.Text;
                string[] pubk = ALG_ECC.Util.GetPublicKey(priK, eccCurve.Text.ToString());
                smpubxkey.Text = pubk[0];
                smpubykey.Text = pubk[1];
            }
            catch (Exception ex)
            {
                smpubxkey.Text = ex.Message;
                smpubykey.Text = "";
            }
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void sm2encClick(object sender, RoutedEventArgs e)
        {
            string priK = smprikey.Text;
            string pubx = smpubxkey.Text;
            string puby = smpubykey.Text;
            string data = smdata_cipher.Text;
            try
            {
                //if ((pubx + puby).Length != 128)
                //    throw new Exception("Public Key Wrong");
                //if (data.Length == 0)
                //    throw new Exception("Data = null");
                //if (sm2c1c3c2.IsChecked == true)
                if (eccCurve.Text.ToString() == "SM2")
                {
                    //smres_cipher.Text = ALG.SM.SM2EncryptC1C3C2(data, pubx + puby);
                    smres_cipher.Text = ALG_ECC.SM2.Encrypt(pubx, puby, data);
                }
                else
                    throw new NotImplementedException();
                //else
                //    smres.Text = ALG.SM.SM2Encrypt(data, pubx + puby);
            }
            catch (Exception ex)
            {
                smres_cipher.Text = ex.Message;
            }
        }


        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void sm2deClick(object sender, RoutedEventArgs e)
        {
            string priK = smprikey.Text;
            string pubx = smpubxkey.Text;
            string puby = smpubykey.Text;
            string data = smdata_cipher.Text;
            try
            {
                if (eccCurve.Text.ToString() == "SM2")
                {
                    smres_cipher.Text = ALG_ECC.SM2.Decrypt(priK, data);
                }
                else
                    throw new NotImplementedException();
                //else
                //    smres.Text = ALG.SM.SM2Decrypt(data, priK);
            }
            catch (Exception ex)
            {
                smres_cipher.Text = ex.Message;
            }
        }


        private void sm2sgnClick(object sender, RoutedEventArgs e)
        {
            string priK = smprikey.Text;
            string pubx = smpubxkey.Text;
            string puby = smpubykey.Text;
            string data = smdata.Text;
            string id = idBox.Text;

            try
            {
                string curve = eccCurve.Text.ToString();
                if (curve == "SM2")
                {
                    string[] tmp = ALG_ECC.SM2.Sign(priK, data, id);
                    smRS_R.Text = tmp[0];
                    smRS_S.Text = tmp[1];

                    smres.Text = "Done " + DateTime.Now; ;
                }
                else
                {
                    var tmp = ALG_ECC.ECDSA.Sign(priK, data, curve);
                    smRS_R.Text = tmp[0];
                    smRS_S.Text = tmp[1];
                    smres.Text = "Done " + DateTime.Now; ;
                }
            }
            catch (Exception ex)
            {
                smres.Text = ex.Message;
            }
        }


        private void sm2vrClick(object sender, RoutedEventArgs e)
        {
            string pubx = smpubxkey.Text;
            string puby = smpubykey.Text;
            string data = smdata.Text;
            string rs_r = smRS_R.Text;
            string rs_s = smRS_S.Text;
            string id = idBox.Text;
            try
            {
                string curve = eccCurve.Text.ToString();
                if (curve == "SM2")
                {
                    bool b = ALG_ECC.SM2.Verify(pubx, puby, data, rs_r, rs_s, id);
                    if (b)
                        smres.Text = "True " + DateTime.Now;
                    else
                        smres.Text = "False " + DateTime.Now;
                }
                else
                {
                    bool b = ALG_ECC.ECDSA.Verify(pubx, puby, data, rs_r, rs_s, curve);
                    if (b)
                        smres.Text = "True " + DateTime.Now;
                    else
                        smres.Text = "False " + DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                smres.Text = ex.Message;
            }
        }


        private void sm2GenkeyClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string curve = eccCurve.Text.ToString();
                var key = ALG_ECC.Util.GenerateKeyPair(curve);
                smprikey.Text = key[0];
                smpubxkey.Text = key[1];
                smpubykey.Text = key[2];
            }
            catch (Exception ex)
            {
                smres.Text = ex.Message;
            }
        }


        private void sm2keyCheck(object sender, RoutedEventArgs e)
        {
            //string priK = smprikey.Text;
            string pubx = smpubxkey.Text;
            string puby = smpubykey.Text;
            //string id = idBox.Text;
            try
            {
                setButton(sm2CheckBtn, false);
                string curve = eccCurve.Text.ToString();
                bool ret = false;
                ret = ALG_ECC.Util.VerifyPublickey(pubx, puby, curve);

                if (ret)
                    smres.Text = "Valid Public Key! " + DateTime.Now;
                else
                    smres.Text = "Invalid Public Key! " + DateTime.Now;

            }
            catch (Exception ex)
            {
                smres.Text = "Fail " + " " + ex.Message + " " + DateTime.Now;
            }
            finally
            {
                setButton(sm2CheckBtn, true);
            }
        }

        private void smClear(object sender, RoutedEventArgs e)
        {
            smres.Text = "";
            smprikey.Text = "";
            smpubxkey.Text = "";
            smpubykey.Text = "";
            smRS_R.Text = "";
            smRS_S.Text = "";
            smdata.Text = "";

            smdata_cipher.Text = "";
            smres_cipher.Text = "";
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void desencClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    string icv = GetText(icvBox);
                    string data = GetText(dataBox);
                    string key = GetText(keyBox);
                    string temp = "";

                    setButton(desencBtn, false);
                    if (RadioisChecked(random) == true)
                    {
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                            new Action(() =>
                            {
                                Random ran = new Random();

                                string ran8 = (ran.Next(0, 0xFF)).ToString("X2") + (ran.Next(0, 0xFF)).ToString("X2") +
                                    (ran.Next(0, 0xFF)).ToString("X2") + (ran.Next(0, 0xFF)).ToString("X2") +
                                    (ran.Next(0, 0xFF)).ToString("X2") + (ran.Next(0, 0xFF)).ToString("X2") +
                                    (ran.Next(0, 0xFF)).ToString("X2") + (ran.Next(0, 0xFF)).ToString("X2");
                                res1Box.Text += ran8;
                            }));
                        return;
                    }
                    else if (RadioisChecked(radio0x) == true)
                    {
                        data = ConvertTool.RemoveSpace(data);

                        if (data.Length % 2 != 0)
                            throw new Exception("Invalid Data, Data.Length % 2 != 0");

                        temp = "";
                        for (int i = 0; i < data.Length; i = i + 2)
                        {
                            temp += "0x";
                            if (i + 2 == data.Length)
                                temp += data.Substring(i, 2);
                            else
                                temp += data.Substring(i, 2) + ", ";
                        }
                    }
                    else if (RadioisChecked(mac) == true)
                    {
                        icv = ConvertTool.RemoveSpace(icv);

                        if (GetText(modeCombox) == "DES")
                        {
                            if (icv.Length > 16)
                                icv = icv.Substring(0, 16);
                            temp = ALG.DES.DesMAC(data, key, icv);
                        }
                        else if (GetText(modeCombox) == "3DES")
                        {
                            if (icv.Length > 16)
                                icv = icv.Substring(0, 16);
                            temp = ALG.DES.TriDesMAC(data, key, icv);
                        }
                        else if (GetText(modeCombox) == "SM4")
                            temp = ALG.GuoMi.SM4MAC(data, icv, key);
                    }
                    else if (RadioisChecked(hash) == true)
                    {
                        if (GetText(modeCombox) == "SHA1")
                            temp = ALG.Hash.HashSHA1(data);
                        else if (GetText(modeCombox) == "SHA224")
                            temp = ALG.Hash.HashSHA224(data);
                        else if (GetText(modeCombox) == "SHA256")
                            temp = ALG.Hash.HashSHA256(data);
                        else if (GetText(modeCombox) == "SHA384")
                            temp = ALG.Hash.HashSHA384(data);
                        else if (GetText(modeCombox) == "SHA512")
                            temp = ALG.Hash.HashSHA512(data);
                        else if (GetText(modeCombox) == "MD5")
                            temp = ALG.Hash.MD5(data);
                        else if (GetText(modeCombox) == "SM3")
                            temp = ALG.GuoMi.SM3(data);
                        else if (GetText(modeCombox) == "SHA3-224")
                            temp = ALG.SHA3.SHA3_224(data);
                        else if (GetText(modeCombox) == "SHA3-256")
                            temp = ALG.SHA3.SHA3_256(data);
                        else if (GetText(modeCombox) == "SHA3-384")
                            temp = ALG.SHA3.SHA3_384(data);
                        else if (GetText(modeCombox) == "SHA3-512")
                            temp = ALG.SHA3.SHA3_512(data);
                    }
                    else if (RadioisChecked(hash) == true)
                    {
                        if (GetText(modeCombox) == "SHA1")
                            temp = ALG.Hash.HashSHA1(data);
                        else if (GetText(modeCombox) == "SHA256")
                            temp = ALG.Hash.HashSHA256(data);
                        else if (GetText(modeCombox) == "SHA384")
                            temp = ALG.Hash.HashSHA384(data);
                        else if (GetText(modeCombox) == "SHA512")
                            temp = ALG.Hash.HashSHA512(data);
                        else if (GetText(modeCombox) == "MD5")
                            temp = ALG.Hash.MD5(data);
                        else if (GetText(modeCombox) == "SM3")
                            temp = ALG.GuoMi.SM3(data);
                        else if (GetText(modeCombox) == "SHA3-224")
                            temp = ALG.SHA3.SHA3_224(data);
                        else if (GetText(modeCombox) == "SHA3-256")
                            temp = ALG.SHA3.SHA3_256(data);
                        else if (GetText(modeCombox) == "SHA3-384")
                            temp = ALG.SHA3.SHA3_384(data);
                        else if (GetText(modeCombox) == "SHA3-512")
                            temp = ALG.SHA3.SHA3_512(data);
                    }
                    else if (RadioisChecked(hmac) == true)
                    {

                        if (GetText(modeCombox) == "SHA1")
                            temp = ALG.HMAC.HMAC_SHA1(key, data);
                        else if (GetText(modeCombox) == "SHA224")
                            temp = ALG.HMAC.HMAC_SHA224(key, data);
                        else if (GetText(modeCombox) == "SHA256")
                            temp = ALG.HMAC.HMAC_SHA256(key, data);
                        else if (GetText(modeCombox) == "SHA384")
                            temp = ALG.HMAC.HMAC_SHA384(key, data);
                        else if (GetText(modeCombox) == "SHA512")
                            temp = ALG.HMAC.HMAC_SHA512(key, data);
                        else if (GetText(modeCombox) == "MD5")
                            temp = ALG.HMAC.HMAC_MD5(key, data);

                    }
                    else if (RadioisChecked(des) == true)
                    {
                        icv = ConvertTool.RemoveSpace(icv);

                        if (icv.Length > 16)
                            icv = icv.Substring(0, 16);

                        if (GetText(modeCombox) == "CBC")
                            temp = ALG.DES.DesCBCEn(data, key, icv);
                        else
                            temp = ALG.DES.DesECBEn(data, key);
                    }
                    else if (RadioisChecked(aes) == true)
                    {
                        if (GetText(modeCombox) == "CBC")
                            temp = ALG.AES.AesCBCEn(data, key, icv);
                        else
                            temp = ALG.AES.AesECBEn(data, key);
                    }
                    else if (RadioisChecked(tdes) == true)
                    {
                        if (GetText(modeCombox) == "CBC")
                            temp = ALG.DES.TriDesCBCEn(data, key, icv);
                        else
                            temp = ALG.DES.TriDesECBEn(data, key);
                    }
                    else if (RadioisChecked(sm4) == true)
                    {
                        if (GetText(modeCombox) == "CBC")
                            temp = ALG.GuoMi.SM4EncryptCBC(data, icv, key);
                        else
                            temp = ALG.GuoMi.SM4EncryptECB(data, key);
                    }
                    else if (RadioisChecked(sm1) == true)
                    {
                        icv = ConvertTool.RemoveSpace(icv);
                        data = ConvertTool.RemoveSpace(data);
                        key = ConvertTool.RemoveSpace(key);

                        string res = "";
                        string round = GetText(modeCombox).Substring(4, 2);

                        if (data.Length == 0 || data.Length % 32 != 0)
                            throw new Exception("Invalid Data, Not 16*n bytes");
                        if (key.Length < 32 && key.Length > 64)
                            throw new Exception("Invalid Key, Not 16-32 bytes");
                        if (key.Length % 2 != 0)
                            throw new Exception("Invalid Key, Not 2*n bytes");

                        if (GetText(modeCombox).Substring(0, 3) == "CBC")
                        {
                            if (icv.Length != 32)
                                throw new Exception("Invalid IV, Not 16 bytes");

                            gp = new gplib(GetText(comboBoxSelectReader), "", "", "", "0");
                            gp.Reset();
                            for (int i = 0; i < data.Length; i = i + 32)
                            {
                                string apdu_5 = "80340000";
                                string apdu_data = "90" + round + data.Substring(i, 32) + icv + key;

                                gp.secApdu(apdu_5 + (apdu_data.Length / 2).ToString("X2") + apdu_data);
                                icv = gp.getResponse().Substring(0, gp.getResponse().Length - 4);
                                res += icv;
                            }

                            temp = res;
                        }
                        else
                        {
                            gp = new gplib(GetText(comboBoxSelectReader), "", "", "", "0");
                            gp.Reset();
                            for (int i = 0; i < data.Length; i = i + 32)
                            {
                                string apdu_5 = "80320000";
                                string apdu_data = "90" + round + data.Substring(i, 32) + key;

                                gp.secApdu(apdu_5 + (apdu_data.Length / 2).ToString("X2") + apdu_data);
                                res += gp.getResponse().Substring(0, gp.getResponse().Length - 4);
                            }

                            temp = res;
                        };
                    }

                    else if (RadioisChecked(xor) == true)
                    {
                        key = ConvertTool.RemoveSpace(key);
                        data = ConvertTool.RemoveSpace(data);

                        if (key.Length % 2 != 0)
                            key = "0" + key;

                        if (data.Length % 2 != 0)
                            data = "0" + data;

                        while (key.Length > data.Length)
                        {
                            data = "0" + data;
                        }

                        while (key.Length < data.Length)
                        {
                            key = "0" + key;
                        }

                        byte[] data1 = ConvertTool.String2Bytes(key);
                        byte[] data2 = ConvertTool.String2Bytes(data);
                        byte[] res = new byte[data1.Length];
                        for (int i = 0; i < data1.Length; i++)
                            res[i] = (byte)(data1[i] ^ data2[i]);

                        temp = ConvertTool.Bytes2String(res);
                    }
                    else if (RadioisChecked(ConvertRadio) == true)
                    {
                        int index = 0;
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            index = modeCombox.SelectedIndex;
                        }));

                        if (index == 0)//ascii
                        {
                            temp = xTool.ConvertTool.String2Ascii(data);
                        }
                        else if (index == 1)//base64
                        {
                            temp = xTool.ConvertTool.Base64Encode(data);
                        }
                        else if (index == 2)//base58 bitcoin monero
                        {
                            temp = ALG_ECC.Base58.Base58Encode_BitCoin(data);

                        }
                        else if (index == 3)//base58 ripple
                        {
                            temp = ALG_ECC.Base58.Base58Encode_Ripple(data);

                        }
                        else if (index == 4)//base58 bitcoin monero checksum
                        {
                            temp = ALG_ECC.Base58.Base58Encode_BitCoin(data, true);

                        }
                        else if (index == 5)//base58 ripple checksum
                        {
                            temp = ALG_ECC.Base58.Base58Encode_Ripple(data, true);

                        }
                    }

                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        res1Box.Text = temp;
                    }));
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        res1Box.Text = ex.Message;
                    }));
                }
                finally
                {
                    setButton(desencBtn, true);
                }
            }).Start();

        }

        //decrypt
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        private void desdecClick(object sender, RoutedEventArgs e)
        {
            string icv = icvBox.Text;
            string data = dataBox.Text;
            string key = keyBox.Text;

            try
            {
                if (mac.IsChecked == true)
                {
                    data = ConvertTool.RemoveSpace(data);
                    if (data.Length % 2 != 0)
                        throw new Exception("data.Length % 2 != 0");

                    data += "80";
                    while (data.Length % 16 != 0)
                        data = data + "00";
                    dataBox.Text = data;
                }
                else if (random.IsChecked == true)
                {
                    res1Box.Text = "";
                }
                else if (radio0x.IsChecked == true)
                {
                    string data_tmp = data.Replace(" ", "");
                    data_tmp = data_tmp.Replace("\t", "");
                    data_tmp = data_tmp.ToUpper();
                    data_tmp = data_tmp.Replace("0X", "");
                    res1Box.Text = data_tmp.Replace(",", "");
                }
                else if (hash.IsChecked == true || hmac.IsChecked == true)
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    //dialog.Filter = "all files (*.*)";
                    if (dialog.ShowDialog() == true)
                    {
                        dataBox.Text = dialog.FileName;
                    }
                }
                else if (aes.IsChecked == true)
                {
                    if (GetText(modeCombox) == "CBC")
                        res1Box.Text = ALG.AES.AesCBCDe(data, key, icv);
                    else
                        res1Box.Text = ALG.AES.AesECBDe(data, key);
                }
                else if (des.IsChecked == true)
                {
                    if (icv.Length > 16)
                        icv = icv.Substring(0, 16);
                    if (GetText(modeCombox) == "CBC")
                        res1Box.Text = ALG.DES.DesCBCDe(data, key, icv);
                    else
                        res1Box.Text = ALG.DES.DesECBDe(data, key);
                }
                else if (tdes.IsChecked == true)
                {
                    if (GetText(modeCombox) == "CBC")
                        res1Box.Text = ALG.DES.TriDesCBCDe(data, key, icv);
                    else
                        res1Box.Text = ALG.DES.TriDesECBDe(data, key);
                }
                else if (RadioisChecked(sm4) == true)
                {
                    if (GetText(modeCombox) == "CBC")
                        res1Box.Text = ALG.GuoMi.SM4DecryptCBC(data, icv, key);
                    else
                        res1Box.Text = ALG.GuoMi.SM4DecryptECB(data, key);
                }
                else if (RadioisChecked(sm1) == true)
                {
                    icv = ConvertTool.RemoveSpace(icv);
                    data = ConvertTool.RemoveSpace(data);
                    key = ConvertTool.RemoveSpace(key);

                    string res = "";
                    string round = GetText(modeCombox).Substring(4, 2);

                    if (data.Length == 0 || data.Length % 32 != 0)
                        throw new Exception("Invalid Data, Not 16*n bytes");
                    if (key.Length < 32 && key.Length > 64)
                        throw new Exception("Invalid Key, Not 16-32 bytes");
                    if (key.Length % 2 != 0)
                        throw new Exception("Invalid Key, Not 2*n bytes");

                    if (GetText(modeCombox).Substring(0, 3) == "CBC")
                    {
                        if (icv.Length != 32)
                            throw new Exception("Invalid IV, Not 16 bytes");

                        gp = new gplib(GetText(comboBoxSelectReader), "", "", "", "0");
                        gp.Reset();
                        for (int i = 0; i < data.Length; i = i + 32)
                        {
                            string apdu_5 = "80340000";
                            string apdu_data = "91" + round + data.Substring(i, 32) + icv + key;

                            gp.secApdu(apdu_5 + (apdu_data.Length / 2).ToString("X2") + apdu_data);
                            res += gp.getResponse().Substring(0, gp.getResponse().Length - 4);
                            icv = data.Substring(i, 32);
                        }

                        res1Box.Text = res;
                    }
                    else
                    {
                        gp = new gplib(GetText(comboBoxSelectReader), "", "", "", "0");
                        gp.Reset();
                        for (int i = 0; i < data.Length; i = i + 32)
                        {
                            string apdu_5 = "80320000";
                            string apdu_data = "91" + round + data.Substring(i, 32) + key;

                            gp.secApdu(apdu_5 + (apdu_data.Length / 2).ToString("X2") + apdu_data);
                            res += gp.getResponse().Substring(0, gp.getResponse().Length - 4);
                        }

                        res1Box.Text = res;
                        return;
                    }
                }
                else if (RadioisChecked(ConvertRadio) == true)
                {
                    int index = modeCombox.SelectedIndex;
                    if (index == 0)//ascii
                    {
                        res1Box.Text = xTool.ConvertTool.Ascii2String(data);
                    }
                    else if (index == 1)//base64
                    {
                        res1Box.Text = xTool.ConvertTool.Base64Decode(data);
                    }
                    else if (index == 2)//base58 bitcoin monero
                    {
                        res1Box.Text = ALG_ECC.Base58.Base58Decode_BitCoin(data);

                    }
                    else if (index == 3)//base58 ripple
                    {
                        res1Box.Text = ALG_ECC.Base58.Base58Decode_Ripple(data);

                    }
                    else if (index == 4)//base58 bitcoin monero checksum
                    {
                        res1Box.Text = ALG_ECC.Base58.Base58Decode_BitCoin(data, true);

                    }
                    else if (index == 5)//base58 ripple checksum
                    {
                        res1Box.Text = ALG_ECC.Base58.Base58Decode_Ripple(data, true);

                    }
                }

            }
            catch (Exception ex)
            {
                res1Box.Text = ex.Message;
            }
        }

        private void rsaencClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    setButton(rsaEncBtn, false);
                    string res;
                    var exp = GetText(eBox);
                    if (exp != "" && (GetText(eBox)).Length % 2 != 0)
                        exp = "0" + exp;
                    res = ALG.RSA.RSAen(GetText(rsasrcBox), GetText(nBox), exp);
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        rsaresBox.Text = res;
                    }));
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        rsaresBox.Text = ex.Message;
                    }));
                }
                finally
                {
                    setButton(rsaEncBtn, true);
                }
            }).Start();
        }

        private void rsadecClick(object sender, RoutedEventArgs e)
        {

            new Thread(() =>
            {
                try
                {
                    setButton(rsaDeBtn, false);
                    string res;
                    res = ALG.RSA.RSAde(GetText(rsasrcBox), GetText(nBox), GetText(dBox));
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        rsaresBox.Text = res;
                    }));
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        rsaresBox.Text = ex.Message;
                    }));
                }
                finally
                {
                    setButton(rsaDeBtn, true);
                }
            }).Start();
        }

        private void pkcsBtnClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    setButton(pkcsBtn, false);
                    string res;
                    res = ALG.RSA.PKCS1(GetText(rsasrcBox), GetText(nBox), GetText(dBox));
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        rsaresBox.Text = res;
                    }));
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        rsaresBox.Text = ex.Message;
                    }));
                }
                finally
                {
                    setButton(pkcsBtn, true);
                }
            }).Start();
        }

        private void rsacrtClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    setButton(rsaCRTBtn, false);
                    string res;
                    res = ALG.RSA.RSACRTde(GetText(rsasrcBox), GetText(pBox), GetText(qBox), GetText(dpBox), GetText(dqBox), GetText(invqBox));
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        rsaresBox.Text = res;
                    }));
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        rsaresBox.Text = ex.Message;
                    }));
                }
                finally
                {
                    setButton(rsaCRTBtn, true);
                }
            }).Start();
        }

        private void pqKeyClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    if (GetText(pBox) == "" || GetText(qBox) == "")
                    {
                        throw new ArgumentException("p or q = null");
                    }
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        nBox.Text = "";
                        dBox.Text = "";
                        //pBox.Text = "";
                        //qBox.Text = "";
                        dpBox.Text = "";
                        dqBox.Text = "";
                        invqBox.Text = "";
                    }));
                    setButton(pqKey, false);

                    //int bit = 0;
                    //int exp = 0;
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        rsaresBox.Text = "";
                        //bit = System.Convert.ToInt32(bitBox.Text, 10);
                        //exp = System.Convert.ToInt32(eBox.Text, 16);
                    }));


                    RSA.GenPQKey(GetText(pBox), GetText(qBox), GetText(eBox));

                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        nBox.Text = RSA.RSA_N;
                        dBox.Text = RSA.RSA_D;
                        //pBox.Text = RSA.RSA_P;
                        //qBox.Text = RSA.RSA_Q;
                        dpBox.Text = RSA.RSA_DP;
                        dqBox.Text = RSA.RSA_DQ;
                        invqBox.Text = RSA.RSA_INVQ;
                    }));


                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            rsaresBox.Text = ex.Message;
                        }));

                }
                finally
                {
                    setButton(pqKey, true);
                }
            }).Start();

        }


        private void getEClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    if (GetText(pBox) == "" || GetText(dpBox) == "")
                    {
                        if (GetText(qBox) == "" || GetText(dqBox) == "")
                            throw new ArgumentException("p/dp or q/dp is null");
                        setButton(getEBtn, false);
                        RSA.GetE(GetText(qBox), GetText(dqBox));
                    }
                    else
                    {
                        setButton(getEBtn, false);
                        RSA.GetE(GetText(pBox), GetText(dpBox));
                    }



                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        eBox.Text = RSA.RSA_E;
                    }));


                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            rsaresBox.Text = ex.Message;
                        }));

                }
                finally
                {
                    setButton(getEBtn, true);
                }
            }).Start();

        }

        private void genkeyClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        nBox.Text = "";
                        dBox.Text = "";
                        pBox.Text = "";
                        qBox.Text = "";
                        dpBox.Text = "";
                        dqBox.Text = "";
                        invqBox.Text = "";
                    }));
                    setButton(genRSABtn, false);

                    int bit = 0;
                    string bitstr = "";
                    //int exp = 0;
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        rsaresBox.Text = "";
                        bitstr = bitBox.Text;
                        //exp = System.Convert.ToInt32(eBox.Text, 16);
                    }));

                    bitstr = bitstr.Replace(" ", "");
                    bit = System.Convert.ToInt32(bitstr, 10);
                    RSA.GenKey(bit, GetText(eBox));

                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        nBox.Text = RSA.RSA_N;
                        dBox.Text = RSA.RSA_D;
                        pBox.Text = RSA.RSA_P;
                        qBox.Text = RSA.RSA_Q;
                        dpBox.Text = RSA.RSA_DP;
                        dqBox.Text = RSA.RSA_DQ;
                        invqBox.Text = RSA.RSA_INVQ;
                    }));


                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            rsaresBox.Text = ex.Message;
                        }));

                }
                finally
                {
                    setButton(genRSABtn, true);
                }
            }).Start();

        }

        private void clearrsaClick(object sender, RoutedEventArgs e)
        {
            //bitBox.Text = "1024";
            nBox.Text = "";
            dBox.Text = "";
            //eBox.Text = "10001";
            pBox.Text = "";
            qBox.Text = "";
            dpBox.Text = "";
            dqBox.Text = "";
            invqBox.Text = "";
            rsasrcBox.Text = "";
            rsaresBox.Text = "";
            /*setButton(genRSABtn, true);
            setButton(checkrsaBtn, true);
            setButton(rsaCRTBtn, true);
            setButton(rsaDeBtn, true);
            setButton(rsaEncBtn, true);*/
        }

        private void checkrsaClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        rsaresBox.Text = "";
                    }));
                    setButton(checkrsaBtn, false);
                    var exp = GetText(eBox);
                    if (exp != "" && (GetText(eBox)).Length % 2 != 0)
                        exp = "0" + exp;
                    string sn = GetText(nBox);

                    string data = "012233445566778899AA";
                    string data1 = "0122";
                    if (sn.Length <= 30)
                    {
                        data = "04";
                        data1 = "03";
                    }
                    var rsa_n = GetText(nBox);
                    var rsa_d = GetText(dBox);
                    var rsa_p = GetText(pBox);
                    var rsa_q = GetText(qBox);
                    var rsa_dp = GetText(dpBox);
                    var rsa_dq = GetText(dqBox);
                    var rsa_invq = GetText(invqBox);

                    string res1, res2, res3, res11, res12, res13, res14;
                    int flag = 0;

                    if (rsa_n != "" && rsa_d != "" && exp != "")
                    { //nde 
                        flag += 1;
                    }
                    else if (rsa_n != "" && rsa_d == "" && exp != "")  //ne
                    {
                        flag += 2;
                    }

                    if (rsa_p != "" && rsa_q != "" && rsa_dp != "" && rsa_dq != "" && rsa_invq != "") //crt
                    {
                        flag += 4;
                    }


                    if (flag == 5)
                    {
                        res1 = ALG.RSA.RSAen(data, rsa_n, exp);
                        res2 = ALG.RSA.RSAde(res1, rsa_n, rsa_d);
                        res3 = ALG.RSA.RSACRTde(res1, rsa_p, rsa_q, rsa_dp, rsa_dq, rsa_invq);


                        res11 = ALG.RSA.RSAde(data1, rsa_n, rsa_d);
                        res12 = ALG.RSA.RSACRTde(data1, rsa_p, rsa_q, rsa_dp, rsa_dq, rsa_invq);
                        res13 = ALG.RSA.RSAen(res11, rsa_n, exp);
                        res14 = ALG.RSA.RSAen(res12, rsa_n, exp);

                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            if (res2 == data && res3 == data && res13 == data1 && res14 == data1)
                                rsaresBox.Text = "N D E, CRT Pass " + DateTime.Now;
                            else
                                rsaresBox.Text = "N D E, CRT Fail " + DateTime.Now;
                        }));
                    }
                    else if (flag == 6)
                    {
                        res1 = ALG.RSA.RSAen(data, rsa_n, exp);
                        res3 = ALG.RSA.RSACRTde(res1, rsa_p, rsa_q, rsa_dp, rsa_dq, rsa_invq);

                        res11 = ALG.RSA.RSAen(data1, rsa_n, exp);
                        res13 = ALG.RSA.RSACRTde(res11, rsa_p, rsa_q, rsa_dp, rsa_dq, rsa_invq);


                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            if (res3 == data && res13 == data1)
                                rsaresBox.Text = "N E, CRT Pass " + DateTime.Now;
                            else
                                rsaresBox.Text = "N E, CRT Fail " + DateTime.Now;
                        }));
                    }
                    else if (flag == 1)
                    {
                        res1 = ALG.RSA.RSAen(data, rsa_n, exp);
                        res2 = ALG.RSA.RSAde(res1, rsa_n, rsa_d);

                        res11 = ALG.RSA.RSAen(data1, rsa_n, exp);
                        res13 = ALG.RSA.RSAde(res11, rsa_n, rsa_d);

                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            if (res2 == data && res13 == data1)
                                rsaresBox.Text = "N D E Pass " + DateTime.Now;
                            else
                                rsaresBox.Text = "N D E Fail " + DateTime.Now;
                        }));
                    }
                    else if (flag == 4)
                    {
                        BigInteger p = new BigInteger(ConvertTool.String2Bytes(rsa_p));
                        BigInteger q = new BigInteger(ConvertTool.String2Bytes(rsa_q));
                        BigInteger n = p * q;
                        rsa_n = n.ToHexString();

                        res1 = ALG.RSA.RSAen(data, rsa_n, exp);
                        res3 = ALG.RSA.RSACRTde(res1, rsa_p, rsa_q, rsa_dp, rsa_dq, rsa_invq);

                        res11 = ALG.RSA.RSAen(data1, rsa_n, exp);
                        res13 = ALG.RSA.RSACRTde(res11, rsa_p, rsa_q, rsa_dp, rsa_dq, rsa_invq);


                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            if (res3 == data && res13 == data1)
                                rsaresBox.Text = "CRT Pass " + DateTime.Now;
                            else
                                rsaresBox.Text = "CRT Fail " + DateTime.Now;
                        }));
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            rsaresBox.Text = "no key Checked " + DateTime.Now;
                        }));
                    }

                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        rsaresBox.Text = "FAIL: " + ex.Message + " " + DateTime.Now;
                    }));
                }
                finally
                {
                    setButton(checkrsaBtn, true);
                }
            }).Start();
        }

        private void setButton(Button btn, bool b)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                new Action(() =>
                {
                    btn.IsEnabled = b;
                }));
        }


        //Card.M
        private void GetCAPLists(object sender, EventArgs e)
        {
            comboBoxCM.Items.Clear();
            XmlElement root = capXML.GetXmlDocumentRoot();
            string his, node;

            if (root.HasChildNodes)
            {
                XmlNode temp = root.FirstChild;
                while (temp != null)
                {
                    node = root.Name + "/" + temp.Name;
                    his = capXML.Read(node, "");
                    comboBoxCM.Items.Add(his);
                    temp = temp.NextSibling;
                }
            }

        }

        private void opencapClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "CAP Files (*.cap, *.txt)|*.cap;*.txt"
            };
            if (dialog.ShowDialog() == true)
            {
                comboBoxCM.Text = dialog.FileName;
            }
        }

        private void cmresChangeed(object sender, TextChangedEventArgs e)
        {
            cmresOUTPUT.ScrollToEnd();
        }


        private void loadCAP()
        {
            capFS = null;
            capSW = null;
            tabCurent = tabControl.Items.GetItemAt(2);
            string capfile = "";
            string logfile = "";
            int outflag = 0;
            StreamReader sr = null;
            int flag = 0;

            DateTime t1, t2;
            t1 = DateTime.MinValue;
            t2 = DateTime.MinValue;

            try
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                new Action(() =>
                {
                    comboBoxSelectReader.IsEnabled = false;
                    if (capLog.IsChecked == true && File.Exists(comboBoxCM.Text))
                    {
                        string temppp = GetText(comboBoxCM).Remove(GetText(comboBoxCM).LastIndexOf("."));
                        logfile = temppp + "_cap_log.log";
                        capFS = new FileStream(temppp + "_cap_log.log", FileMode.Create);
                        capSW = new StreamWriter(capFS);
                    }
                }));
                outToBox(start_tip);

                string ft = System.IO.Path.GetExtension(GetText(comboBoxCM));
                t1 = System.DateTime.Now;
                if (ft.Equals(".cap", StringComparison.InvariantCultureIgnoreCase))
                {
                    capfile = GetText(comboBoxCM);
                    downcap(capfile);

                    t2 = System.DateTime.Now;
                    if (t1 != DateTime.MinValue && t2 != DateTime.MinValue)
                    {
                        TimeSpan ts = t2 - t1;
                        if (ts.Days > 0)
                            outToBox("Total Time: " + ts.Days + " Days " + ts.Hours + " Hours " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds");
                        else if (ts.Hours > 0)
                            outToBox("Total Time: " + ts.Hours + " Hours " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds");
                        else
                            outToBox("Total Time: " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds " + ts.Milliseconds + " Milliseconds");
                    }
                    outflag = 1;
                    //outToBox(finish_tip);
                }
                else if (ft.Equals(".txt", StringComparison.InvariantCultureIgnoreCase))
                {
                    string filetmp = GetText(comboBoxCM);
                    capfile = "";
                    sr = new StreamReader(filetmp, Encoding.Default);
                    while ((capfile = sr.ReadLine()) != null)
                    {
                        string temp1 = doScript(capfile);
                        if (temp1 == "" || temp1 == null)
                            continue;
                        capfile = Path.GetDirectoryName(filetmp) + "\\" + capfile;
                        downcap(capfile);
                        outToBox("");
                    }

                    t2 = System.DateTime.Now;
                    if (t1 != DateTime.MinValue && t2 != DateTime.MinValue)
                    {
                        outToBox(filetmp);
                        TimeSpan ts = t2 - t1;
                        if (ts.Days > 0)
                            outToBox("Total Time: " + ts.Days + " Days " + ts.Hours + " Hours " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds");
                        else if (ts.Hours > 0)
                            outToBox("Total Time: " + ts.Hours + " Hours " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds");
                        else
                            outToBox("Total Time: " + ts.Minutes + " Minutes " + ts.Seconds + " Seconds " + ts.Milliseconds + " Milliseconds");
                    }

                    outflag = 1;
                    //outToBox(finish_tip);                   
                    sr.Close();
                }
                else
                {
                    flag = -1;
                    outToBox("NO SUPPORT FILE!");
                    //outToBox(fail_tip);
                }
            }
            catch (ThreadAbortException)
            {
                //outToBox("cap: " + capfile);
                outflag = 2;
                //outToBox(stop_tip);
            }
            catch (ArgumentException ex)
            {
                flag = -1;
                outToBox(ex.Message);
                //outToBox("cap: " + capfile);
                //outToBox(fail_tip);
            }
            catch (NotSupportedException ex)
            {
                flag = -1;
                outToBox(ex.Message);
                //outToBox("cap: " + capfile);
                //outToBox(fail_tip);
            }
            catch (FileNotFoundException ex)
            {
                flag = -1;
                outToBox(ex.Message);
                //outToBox("cap: " + capfile);
                //outToBox(fail_tip);
            }
            catch (DirectoryNotFoundException ex)
            {
                flag = -1;
                outToBox(ex.Message);
                //outToBox("cap: " + capfile);
                //outToBox(fail_tip);
            }
            catch (PathTooLongException ex)
            {
                flag = -1;
                outToBox(ex.Message);
                //outToBox("cap: " + capfile);
                //outToBox(fail_tip);
            }
            catch (Exception ex)
            {
                outToBox(ex.Message);
                //outToBox("cap: " + capfile);
                //outToBox(fail_tip);
            }
            finally
            {

                if (outflag == 0)
                {
                    outToBox("\ncap: " + capfile);

                    if (logfile == "")
                        outToBox("\nNo Log Saved");
                    else
                        outToBox("\nLog Saved to " + logfile);

                    outToBox(fail_tip);
                }
                else if (outflag == 1)
                {
                    if (logfile == "")
                        outToBox("\nNo Log Saved");
                    else
                        outToBox("\nLog Saved to " + logfile);

                    outToBox(finish_tip);
                }
                else if (outflag == 2)
                {
                    outToBox("\ncap: " + capfile);

                    if (logfile == "")
                        outToBox("\nNo Log Saved");
                    else
                        outToBox("\nLog Saved to " + logfile);

                    outToBox(stop_tip);
                }

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                new Action(() =>
                {
                    comboBoxSelectReader.IsEnabled = true;
                    loadBtn.Content = "Run";
                    Cap2ApduBtn.IsEnabled = true;
                }));

                if (sr != null)
                    sr.Close();

                if (capSW != null)
                {
                    capSW.Flush();
                    capSW.Close();
                    capFS.Close();
                }
                capSW = null;
                capFS = null;
                if (flag == 0)
                {
                    XmlElement roots = capXML.GetXmlDocumentRoot();
                    string nodes = "cap" + System.DateTime.Now.ToString("yyMMddHHmmssfff");
                    XmlNode temp = capXML.CreateElement(nodes, GetText(comboBoxCM));
                    if (roots.FirstChild == null)
                        capXML.XmlInsert(temp);
                    else
                    {
                        capXML.XmlInsertBefore(roots.FirstChild, temp);
                        capXML.RemoveLastNode(roots, 30);
                    }

                    if (loadCAP_Thread != null)
                    {
                        loadCAP_Thread.Abort();
                        loadCAP_Thread = null;
                    }
                    if (loadCAP_ThreadStart != null)
                        loadCAP_ThreadStart = null;
                }
            }
        }

        private delegate string GetTextHandle(Control control);
        private string GetText(Control control)
        {
            if (this.Dispatcher.Thread != System.Threading.Thread.CurrentThread)
            {
                return (string)this.Dispatcher.Invoke(new GetTextHandle(this.GetText), control);
            }
            else
            {
                if (control.GetType() == typeof(TextBox))
                {
                    return ((TextBox)control).Text;
                }
                else if (control.GetType() == typeof(ComboBox))
                {
                    return ((ComboBox)control).Text;
                }
                else if (control.GetType() == typeof(Button))
                {
                    return (string)((Button)control).Content;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private void loadClick(object sender, RoutedEventArgs e)
        {
            Cap2ApduBtn.IsEnabled = false;
            if (loadBtn.Content.ToString() == "Stop")
            {
                if (loadCAP_Thread != null)
                    loadCAP_Thread.Abort();
                loadCAP_Thread = null;
                loadCAP_ThreadStart = null;
            }
            else
            {
                if (loadBtn.Content.ToString() == "Run")
                    loadBtn.Content = "Stop";


                loadCAP_ThreadStart = new ThreadStart(this.loadCAP);
                loadCAP_Thread = new Thread(loadCAP_ThreadStart)
                {
                    IsBackground = true
                };
                loadCAP_Thread.Start();
            }

        }

        private void Cap2Apdu()
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        loadBtn.IsEnabled = false;
                    }));
            tabCurent = tabControl.Items.GetItemAt(2);
            int iflag = 0;
            int len = System.Convert.ToInt32(GetText(loadLen));
            capSW = null;
            capFS = null;

            outToBox(start_tip);

            string ft = System.IO.Path.GetExtension(GetText(comboBoxCM));
            if (ft.Equals(".cap", StringComparison.InvariantCultureIgnoreCase))
            {
                FileStream fs = null;
                StreamWriter sw = null;
                try
                {
                    //CapTool.SHA1;
                    string capData = CapTool.GetComponent(GetText(comboBoxCM));

                    outToBox(GetText(comboBoxCM));
                    outToBox("Executable Load File AID : \n " + CapTool.PackageAID + "  |  " + ConvertTool.Ascii2String(CapTool.PackageAID));
                    outToBox("Executable Module AID : ");
                    if (CapTool.ClassAID == null || CapTool.ClassAID.Length == 0)
                        outToBox(" NULL");
                    else
                    {
                        for (int i = 0; i < CapTool.ClassAID.Length; i++)
                            outToBox(" " + CapTool.ClassAID[i] + "  |  " + ConvertTool.Ascii2String(CapTool.ClassAID[i]));
                    }
                    outToBox("SHA1: " + CapTool.SHA1 + "\n");
                    string temppp = GetText(comboBoxCM).Remove(GetText(comboBoxCM).LastIndexOf("."));
                    fs = new FileStream(temppp + "_cap_apdu.log", FileMode.Create);
                    sw = new StreamWriter(fs);

                    outToBox("Saved to " + temppp + "_cap_apdu.log");

                    sw.WriteLine("Path: \n " + GetText(comboBoxCM) + "  <" + DateTime.Now + ">");
                    sw.WriteLine("");
                    sw.WriteLine("Executable Load File AID : \n " + CapTool.PackageAID + "  |  " + ConvertTool.Ascii2String(CapTool.PackageAID));
                    sw.WriteLine("Executable Module AID : ");
                    if (CapTool.ClassAID == null || CapTool.ClassAID.Length == 0)
                        sw.WriteLine(" NULL");
                    else
                    {
                        for (int i = 0; i < CapTool.ClassAID.Length; i++)
                            sw.WriteLine(" " + CapTool.ClassAID[i] + "  |  " + ConvertTool.Ascii2String(CapTool.ClassAID[i]));
                    }
                    sw.WriteLine("\n" + "SHA1: " + CapTool.SHA1 + "\n");

                    sw.WriteLine("80E6 0200 " + (CapTool.PackageAID.Length / 2 + 5).ToString("X2") + " " + (CapTool.PackageAID.Length / 2).ToString("X2") + CapTool.PackageAID + " 00000000" + "\n");
                    int count = capData.Length / (len * 2);
                    int offset = 0;
                    int rem = capData.Length % (len * 2);
                    int p2 = 0;

                    while (count > 1)
                    {
                        sw.WriteLine("80E8 00" + p2.ToString("X2") + " " + len.ToString("X2") + " " + capData.Substring(offset, len * 2));
                        p2++;
                        p2 &= 0xFF;
                        count--;
                        offset += len * 2;
                    }

                    if (rem != 0)
                    {
                        if (capData.Length / (len * 2) > 0)
                        {
                            sw.WriteLine("80E8 00" + p2.ToString("X2") + " " + len.ToString("X2") + " " + capData.Substring(offset, len * 2));
                            p2++;
                            p2 &= 0xFF;
                            offset += len * 2;
                        }
                        sw.WriteLine("80E8 80" + p2.ToString("X2") + " " + (rem / 2).ToString("X2") + " " + capData.Substring(offset));
                    }
                    else
                        sw.WriteLine("80E8 80" + p2.ToString("X2") + " " + len.ToString("X2") + " " + capData.Substring(offset, len * 2));

                    sw.Flush();

                    //sw.Close();
                    //fs.Close();

                }
                catch (Exception ex)
                {
                    iflag = 1;
                    outToBox(GetText(comboBoxCM));

                    outToBox(ex.Message);
                    outToBox(fail_tip);
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        loadBtn.IsEnabled = true;
                    }));

                    if (sw != null)
                        sw.Close();
                    if (fs != null)
                       fs.Close();
                }
            }
            else if (ft.Equals(".txt", StringComparison.InvariantCultureIgnoreCase))
            {
                StreamReader sr = null;
                string line = "";
                FileStream fs = null;
                StreamWriter sw = null;
                try
                {
                    sr = new StreamReader(GetText(comboBoxCM), Encoding.Default);
                    string temppp = GetText(comboBoxCM);
                    temppp = temppp.Remove(temppp.LastIndexOf("."));
                    fs = new FileStream(temppp + "_cap_txt_apdu.log", FileMode.Create);
                    sw = new StreamWriter(fs);
                    while ((line = sr.ReadLine()) != null)
                    {
                        string temp1 = doScript(line);
                        if (temp1 == "" || temp1 == null)
                            continue;
                        line = Path.GetDirectoryName(GetText(comboBoxCM)) + "\\" + line;
                        string capData = CapTool.GetComponent(line);

                        outToBox(line);
                        outToBox("Executable Load File AID : \n " + CapTool.PackageAID + "  |  " + ConvertTool.Ascii2String(CapTool.PackageAID));
                        outToBox("Executable Module AID : ");
                        if (CapTool.ClassAID == null || CapTool.ClassAID.Length == 0)
                            outToBox(" NULL");
                        else
                        {
                            for (int i = 0; i < CapTool.ClassAID.Length; i++)
                                outToBox(" " + CapTool.ClassAID[i] + "  |  " + ConvertTool.Ascii2String(CapTool.ClassAID[i]));
                        }
                        outToBox("SHA1 : " + CapTool.SHA1 + "\n");

                        sw.WriteLine("Path: \n " + line + "  <" + DateTime.Now + ">");
                        sw.WriteLine("");
                        sw.WriteLine("Executable Load File AID : \n " + CapTool.PackageAID + "  |  " + ConvertTool.Ascii2String(CapTool.PackageAID));
                        sw.WriteLine("Executable Module AID : ");
                        if (CapTool.ClassAID == null || CapTool.ClassAID.Length == 0)
                            sw.WriteLine(" NULL");
                        else
                        {
                            for (int i = 0; i < CapTool.ClassAID.Length; i++)
                                sw.WriteLine(" " + CapTool.ClassAID[i] + "  |  " + ConvertTool.Ascii2String(CapTool.ClassAID[i]));
                        }
                        sw.WriteLine("\n" + "SHA1 : " + CapTool.SHA1 + "\n"); ;

                        sw.WriteLine("80E6 0200 " + (CapTool.PackageAID.Length / 2 + 5).ToString("X2") + " " + (CapTool.PackageAID.Length / 2).ToString("X2") + CapTool.PackageAID + " 00000000" + "\n");
                        int count = capData.Length / (len * 2);
                        int offset = 0;
                        int rem = capData.Length % (len * 2);
                        int p2 = 0;

                        while (count > 1)
                        {
                            sw.WriteLine("80E8 00" + p2.ToString("X2") + " " + len.ToString("X2") + " " + capData.Substring(offset, len * 2));
                            p2++;
                            p2 &= 0xFF;
                            count--;
                            offset += len * 2;
                        }

                        if (rem != 0)
                        {
                            if (capData.Length / (len * 2) > 0)
                            {
                                sw.WriteLine("80E8 00" + p2.ToString("X2") + " " + len.ToString("X2") + " " + capData.Substring(offset, len * 2));
                                p2++;
                                p2 &= 0xFF;
                                offset += len * 2;
                            }
                            sw.WriteLine("80E8 80" + p2.ToString("X2") + " " + (rem / 2).ToString("X2") + " " + capData.Substring(offset));
                        }
                        else
                            sw.WriteLine("80E8 80" + p2.ToString("X2") + " " + len.ToString("X2") + " " + capData.Substring(offset, len * 2));

                        sw.WriteLine();
                        sw.Flush();
                    }
                    outToBox("Saved to " + temppp + "_cap_txt_apdu.log");
                    //sw.Close();
                    //fs.Close();
                    //sr.Close();
                }
                catch (Exception ex)
                {
                    iflag = 1;
                    if (sr != null)
                        sr.Close();
                    outToBox(ex.Message);
                    outToBox(line);
                    outToBox(fail_tip);
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        loadBtn.IsEnabled = true;
                    }));
                    if (sw != null)
                        sw.Close();
                    if (fs != null)
                        fs.Close();
                    if (sr != null)
                        sr.Close();
                }
            }
            else
            {
                iflag = 1;
                outToBox("NO SUPPORT FILE");
            }

            if (iflag == 0)
            {
                XmlElement roots = capXML.GetXmlDocumentRoot();
                string nodes = "cap" + System.DateTime.Now.ToString("yyMMddHHmmssfff");
                XmlNode temp = capXML.CreateElement(nodes, GetText(comboBoxCM));
                if (roots.FirstChild == null)
                    capXML.XmlInsert(temp);
                else
                {
                    capXML.XmlInsertBefore(roots.FirstChild, temp);
                    capXML.RemoveLastNode(roots, 30);
                }
            }
            outToBox(finish_tip);
        }

        private void Cap2ApduClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                Cap2Apdu();
            }).Start();
        }

        private void downcap(string capfile)
        {
            XmlElement root = cmXML.GetXmlDocumentRoot();

            string aid, ver, enc, mac, dek, kmc, rsa_n, rsa_d, seid, iccid, node, dap_n, dap_d;
            string sl = "", chl;
            int len;
            node = root.Name + "/" + "aid";
            aid = cmXML.Read(node, "", true);
            node = root.Name + "/" + "kmc";
            kmc = cmXML.Read(node, "", true);
            node = root.Name + "/" + "ver";
            ver = cmXML.Read(node, "", true);
            node = root.Name + "/" + "enc";
            enc = cmXML.Read(node, "", true);
            node = root.Name + "/" + "mac";
            mac = cmXML.Read(node, "", true);
            node = root.Name + "/" + "dek";
            dek = cmXML.Read(node, "", true);
            node = root.Name + "/" + "rsa_n";
            rsa_n = cmXML.Read(node, "", true);
            node = root.Name + "/" + "rsa_d";
            rsa_d = cmXML.Read(node, "", true);
            node = root.Name + "/" + "iccid";
            iccid = cmXML.Read(node, "", true);
            node = root.Name + "/" + "seid";
            seid = cmXML.Read(node, "", true);
            node = root.Name + "/" + "dap_key1";
            dap_n = cmXML.Read(node, "", true);
            node = root.Name + "/" + "dap_key2";
            dap_d = cmXML.Read(node, "", true);

            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       if (radioMac.IsChecked == true)
                           sl = "01";
                       else if (radioEncMac.IsChecked == true)
                           sl = "03";
                       else
                           sl = "00";
                   }));

            chl = GetText(channel);

            int isDM = 0;
            int isDAP = 0;
            string sha1 = "";

            string capData = CapTool.GetComponent(capfile);
            string aidLen = ((CapTool.PackageAID.Length) / 2).ToString("X2");
            string installLen = ((CapTool.PackageAID.Length) / 2 + 5).ToString("X2");
            string delLen = ((CapTool.PackageAID.Length + 4) / 2).ToString("X2");

            len = System.Convert.ToInt32(GetText(loadLen));
            string s = GetText(kmcCombox);

            if (gp != null && gp.isDebug())
                gp.stopDebug();

            if (GetText(kmcCombox) == "CPG202")
            {
                gp = new gplib(GetText(comboBoxSelectReader), "", "", "", chl, kmc);
                gp.setKeyType(1);
            }
            else if (GetText(kmcCombox) == "CPG212")
            {
                gp = new gplib(GetText(comboBoxSelectReader), "", "", "", chl, kmc);
                gp.setKeyType(2);
            }
            else
                gp = new gplib(GetText(comboBoxSelectReader), enc, mac, dek, chl);
            gp.setPort(GetText(jcopport));
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
            new Action(() =>
            {
                if (apduTimer.IsChecked == true)
                    gp.setTime(true);
            }));
            gp.setConsole(cmresOUTPUT);
            gp.setCapLog(capSW);

            gp.Reset();

            gp.secApdu("00A40400" + (aid.Length / 2).ToString("X2") + aid);
            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                throw new ArgumentException("Select Failed!");

            gp.initUpdate(ver);
            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                throw new ArgumentException("Auth Failed!");
            gp.externalAuthenticate(sl);
            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                throw new ArgumentException("Auth Failed!");

            if (aid.Length > 9)
            {
                gp.secApdu("80F24000" + ((aid.Length / 2 + 2).ToString("X2")) + "4F" + ((aid.Length / 2).ToString("X2")) + aid);
                if ((gp.getResponse()).Length > 4)
                {
                    aid = (gp.getResponse()).Substring(2, (System.Convert.ToInt32((gp.getResponse()).Substring(0, 2), 16)) * 2);
                    string privilege = (gp.getResponse()).Substring(((gp.getResponse()).Length) - 6, 2);
                    int pri = System.Convert.ToInt32(privilege, 16);

                    if ((pri & 0xA0) == 0xA0)
                        isDM = 1;

                    if ((pri & 0xC0) == 0xC0)
                    {
                        isDAP = 1;
                        sha1 = CapTool.SHA1;
                    }
                }
            }


            if (isDM > 0)
            {
                sha1 = CapTool.SHA1;
                installLen = ((CapTool.PackageAID.Length) / 2 + 25).ToString("X2");
                gp.setRSA(rsa_n, rsa_d, "");

                gp.setSE_ICC_ID(iccid);

                if (GetText(gpVer) == "China Mobile")
                {
                    isDM = 3;
                    gp.setSE_ICC_ID(seid);
                }
                else if (GetText(gpVer) == "China Unicom")
                    isDM = 4;
                else if (GetText(gpVer) == "China Telecom")
                    isDM = 4;
                else if (GetText(gpVer) == "2.2")
                    isDM = 2;
                else if (GetText(gpVer) == "2.1")
                    isDM = 1;

                gp.setDMType(isDM);

            }

            gp.secApdu("80E40080" + delLen + "4F" + aidLen + CapTool.PackageAID);

            gp.secApdu("80E60200" + installLen + aidLen + CapTool.PackageAID + "00" + (sha1.Length / 2).ToString("X2") + sha1 + "0000");
            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                throw new ArgumentException("Load Failed!");

            int count = capData.Length / (len * 2);
            int offset = 0;
            int rem = capData.Length % (len * 2);
            int p2 = 0;
            string sdap = "";
            if (isDAP > 0)
            {
                if (dap_n.Length == 256)
                {
                    if (dap_d.Length != 256)
                        throw new ArgumentException("DAP KEY Error!");
                    sdap = "E281" + (aid.Length / 2 + 0x85).ToString("X2") + "4F" + (aid.Length / 2).ToString("X2") + aid + "C38180" + ALG.RSA.PKCS1(sha1, dap_n, dap_d);
                }
                else if (dap_n.Length == 32)
                {
                    sha1 = sha1 + "80000000";
                    sdap = "E2" + (aid.Length / 2 + 12).ToString("X2") + "4F" + (aid.Length / 2).ToString("X2") + aid + "C308" + ALG.DES.TriDesMAC(sha1, dap_n);
                }
                else
                    throw new ArgumentException("DAP KEY Error!");
                gp.secApdu("80E800" + p2.ToString("X2") + (sdap.Length / 2).ToString("X2") + sdap);
                p2 = 1;
            }

            while (count > 1)
            {
                gp.secApdu("80E800" + p2.ToString("X2") + len.ToString("X2") + capData.Substring(offset, len * 2));
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Load Failed!");

                p2++;
                p2 &= 0xFF;
                count--;
                offset += len * 2;
            }

            if (rem != 0)
            {
                if (capData.Length / (len * 2) > 0)
                {
                    gp.secApdu("80E800" + p2.ToString("X2") + len.ToString("X2") + capData.Substring(offset, len * 2));
                    if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                        throw new ArgumentException("Load Failed!");

                    p2++;
                    p2 &= 0xFF;
                    offset += len * 2;
                }
                gp.secApdu("80E880" + p2.ToString("X2") + (rem / 2).ToString("X2") + capData.Substring(offset));

            }
            else
                gp.secApdu("80E880" + p2.ToString("X2") + len.ToString("X2") + capData.Substring(offset, len * 2));

            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                throw new ArgumentException("Load Failed!");


            outToBox("\n" + capfile);

            outToBox("Executable Load File AID : \n " + CapTool.PackageAID + "  |  " + ConvertTool.Ascii2String(CapTool.PackageAID));
            string tmps = "Executable Module AID : ";
            if (CapTool.ClassAID == null || CapTool.ClassAID.Length == 0)
                tmps += "\n NULL";
            else
            {
                for (int i = 0; i < CapTool.ClassAID.Length; i++)
                    tmps += "\n " + CapTool.ClassAID[i] + "  |  " + ConvertTool.Ascii2String(CapTool.ClassAID[i]);
            }
            outToBox(tmps);
        }


        //VIEW
        private void viewChanged(object sender, TextChangedEventArgs e)
        {
            viewOUTPUT.ScrollToEnd();
        }

        private void buildList()
        {

            int lenAID = 0;
            string sAID = "";
            int iPRI = 0;
            string tempSW = "";
            string temp = "";
            string life = "";
            string cpad = "                                                      ";
            string pad = "";
            string padbegin = "  |  ";
            string endpad = "";

            TreeViewItem applet = null;
            TreeViewItem application = null;
            TreeViewItem isd = null;
            TreeViewItem ssd = null;

            try
            {
                tabCurent = tabControl.Items.GetItemAt(3);

                XmlElement root = cmXML.GetXmlDocumentRoot();

                string aid, ver, kmc, enc, mac, dek, rsa_n, rsa_d, seid, iccid, node;
                string sl = "00", chl = "0";

                node = root.Name + "/" + "aid";
                aid = cmXML.Read(node, "", true);
                node = root.Name + "/" + "ver";
                ver = cmXML.Read(node, "", true);
                node = root.Name + "/" + "kmc";
                kmc = cmXML.Read(node, "", true);
                node = root.Name + "/" + "enc";
                enc = cmXML.Read(node, "", true);
                node = root.Name + "/" + "mac";
                mac = cmXML.Read(node, "", true);
                node = root.Name + "/" + "dek";
                dek = cmXML.Read(node, "", true);
                node = root.Name + "/" + "rsa_n";
                rsa_n = cmXML.Read(node, "", true);
                node = root.Name + "/" + "rsa_d";
                rsa_d = cmXML.Read(node, "", true);
                node = root.Name + "/" + "iccid";
                iccid = cmXML.Read(node, "", true);
                node = root.Name + "/" + "seid";
                seid = cmXML.Read(node, "", true);

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                       new Action(() =>
                       {


                           if (radioMac.IsChecked == true)
                               sl = "01";
                           else if (radioEncMac.IsChecked == true)
                               sl = "03";
                           else
                               sl = "00";

                           installBtn.IsEnabled = false;
                           putkeyBtn.IsEnabled = false;

                           chl = channel.Text;

                           treeView.Items.Clear();
                           applet = new TreeViewItem
                           {
                               Header = "Applet"
                           };

                           application = new TreeViewItem
                           {
                               Header = "Application"
                           };

                           isd = new TreeViewItem
                           {
                               Header = "Issuer Security Domain"
                           };

                           ssd = new TreeViewItem
                           {
                               Header = "Supplementary Security Domain"
                           };

                           treeView.Items.Add(applet);
                           treeView.Items.Add(application);
                           treeView.Items.Add(isd);
                           treeView.Items.Add(ssd);
                       }));

                outToBox(start_tip);

                if (gp != null && gp.isDebug())
                    gp.stopDebug();

                if (GetText(kmcCombox) == "CPG202")
                {
                    gp = new gplib(GetText(comboBoxSelectReader), "", "", "", chl, kmc, aid, ver, sl);
                    gp.setKeyType(1);
                }
                else if (GetText(kmcCombox) == "CPG212")
                {
                    gp = new gplib(GetText(comboBoxSelectReader), "", "", "", chl, kmc, aid, ver, sl);
                    gp.setKeyType(2);
                }
                else
                    gp = new gplib(GetText(comboBoxSelectReader), enc, mac, dek, chl, "", aid, ver, sl);

                gp.setPort(GetText(jcopport));
                gp.setConsole(viewOUTPUT);
                gp.setRSA(rsa_n, rsa_d, "");

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       if (apduTimer.IsChecked == true)
                           gp.setTime(true);
                   }));

                installGP = gp;
                gp.Reset();
                gp.secApdu("00A40400" + (aid.Length / 2).ToString("X2") + aid);
                //if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000" || (gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "6283")
                //    throw new ArgumentException("Select Failed!");

                gp.initUpdate(ver);
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Auth Failed!");
                gp.externalAuthenticate(sl);
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Auth Failed!");

                int isDM = 0;
                if (aid.Length > 9)
                {
                    gp.secApdu("80F24000" + ((aid.Length / 2 + 2).ToString("X2")) + "4F" + ((aid.Length / 2).ToString("X2")) + aid);
                    if ((gp.getResponse()).Length > 4)
                    {
                        string privilege = (gp.getResponse()).Substring(((gp.getResponse()).Length) - 6, 2);
                        int pri = System.Convert.ToInt32(privilege, 16);

                        if ((pri & 0xA0) == 0xA0)
                            isDM = 1;
                    }
                }

                if (isDM > 0)
                {
                    gp.setSE_ICC_ID(iccid);

                    if (GetText(gpVer) == "China Mobile")
                    {
                        isDM = 3;
                        gp.setSE_ICC_ID(seid);
                    }
                    else if (GetText(gpVer) == "China Unicom")
                        isDM = 4;
                    else if (GetText(gpVer) == "China Telecom")
                        isDM = 4;
                    else if (GetText(gpVer) == "2.2")
                        isDM = 2;
                    else if (GetText(gpVer) == "2.1")
                        isDM = 1;
                    gp.setDMType(isDM);
                }

                if (GetText(gpVer) == "2.1")
                    gp.setPutRSAKeyType(0);
                else
                    gp.setPutRSAKeyType(1);

                life = "";
                temp = "";
                gp.secApdu("80F28000024F00");
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) == "9000")
                {
                    lenAID = System.Convert.ToInt32((gp.getResponse()).Substring(0, 2), 16);
                    sAID = (gp.getResponse()).Substring(2, lenAID * 2);
                    life = (gp.getResponse()).Substring(2 + lenAID * 2, 2);
                    iPRI = System.Convert.ToInt32((gp.getResponse()).Substring(4 + lenAID * 2, 2), 16);
                    if (life == "01")
                        life = "Status: OP_READY";
                    else if (life == "07")
                        life = "Status: INITIALIZED";
                    else if (life == "0F" || life == "0f")
                        life = "Status: SECURED";
                    else if (life == "7F" || life == "7f")
                        life = "Status: CARD_LOCKED";
                    else if (life == "FF" || life == "ff")
                        life = "Status: TERMINATED";

                    if ((iPRI & 4) == 4)
                        life += " | Privilege: Default Selected or Card Reset";
                    else
                        life += " | Privilege: May not Default Selected";

                    pad = cpad.Substring(0, 32 - sAID.Length) + padbegin;

                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            TreeViewItem tempItem = new TreeViewItem
                            {
                                Header = sAID + pad + ConvertTool.Ascii2String(sAID) + endpad
                            };

                            isd.Items.Add(tempItem);
                            tempItem.ToolTip = life;
                        }));

                }

                temp = "";
                gp.secApdu("80F24000024F00");
                tempSW = (gp.getResponse()).Substring((gp.getResponse()).Length - 4);

                if (tempSW != "9000" && tempSW != "6A88" && tempSW != "6310")
                    throw new ArgumentException("Get Status Failed!");
                if (tempSW != "6A88")
                    temp = (gp.getResponse()).Substring(0, (gp.getResponse()).Length - 4);

                while (tempSW == "6310")
                {
                    gp.secApdu("80F24001024F00");
                    tempSW = (gp.getResponse()).Substring((gp.getResponse()).Length - 4);
                    temp += (gp.getResponse()).Substring(0, (gp.getResponse()).Length - 4);
                }

                int offset = 0;
                while (offset < temp.Length)
                {
                    lenAID = System.Convert.ToInt32((temp).Substring(offset, 2), 16);
                    offset += 2;
                    sAID = (temp).Substring(offset, lenAID * 2);
                    offset += 2 * lenAID;

                    int ilife = System.Convert.ToInt32((temp).Substring(offset, 2), 16);

                    offset += 2;
                    iPRI = System.Convert.ToInt32((temp).Substring(offset, 2), 16);
                    offset += 2;

                    pad = cpad.Substring(0, 32 - sAID.Length) + padbegin;

                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            TreeViewItem tempItem1 = new TreeViewItem
                            {
                                Header = sAID + pad + ConvertTool.Ascii2String(sAID) + endpad
                            };

                            if ((iPRI & 128) == 128)
                            {
                                life = "";
                                if (ilife == 3)
                                    life = "Status: INSTALLED";
                                else if (ilife == 7)
                                    life = "Status: INSTALLED & SELECTABLE";
                                else if (ilife == 0xf)
                                    life = "Status: PERSONALIZED";
                                else if ((ilife & 0x83) == 0x83)
                                    life = "Status: LOCKED";

                                life += " | Privilege: ";

                                if ((iPRI & 0xA0) == 0xA0)
                                    life += "Delegated Management";
                                else
                                    life += "Not Delegated Management";

                                if ((iPRI & 0xC1) == 0xC1)
                                    life += ", Mandated DAP Verification";
                                else if ((iPRI & 0xC0) == 0xC0)
                                    life += ", DAP Verification";
                                else
                                    life += ", Not DAP Verification";

                                tempItem1.ToolTip = life;
                                ssd.Items.Add(tempItem1);
                            }
                            else
                            {
                                life = "Status: NULL";
                                if (ilife == 3)
                                    life = "Status: INSTALLED";
                                else if (ilife == 7)
                                    life = "Status: INSTALLED & SELECTABLE";
                                else if ((ilife & 0x83) == 0x83)
                                    life = "Status: LOCKED";

                                life += " | Privilege: ";

                                if ((iPRI & 4) == 4)
                                    life += "Default Selected or Card Reset";
                                else
                                    life += "May not Default Selected";

                                if ((iPRI & 2) == 2)
                                    life += ", CVM Management";

                                tempItem1.ToolTip = life;
                                application.Items.Add(tempItem1);
                            }
                        }));
                }

                temp = "";
                gp.secApdu("80F21000024F00");
                tempSW = (gp.getResponse()).Substring((gp.getResponse()).Length - 4);

                if (tempSW != "9000" && tempSW != "6A88" && tempSW != "6310")
                    throw new ArgumentException("Get Status Failed!");
                if (tempSW != "6A88")
                    temp = (gp.getResponse()).Substring(0, (gp.getResponse()).Length - 4);

                while (tempSW == "6310")
                {
                    gp.secApdu("80F21001024F00");
                    tempSW = (gp.getResponse()).Substring((gp.getResponse()).Length - 4);
                    temp += (gp.getResponse()).Substring(0, (gp.getResponse()).Length - 4);
                }

                offset = 0;
                while (offset < temp.Length)
                {
                    lenAID = System.Convert.ToInt32((temp).Substring(offset, 2), 16);
                    offset += 2;
                    sAID = (temp).Substring(offset, lenAID * 2);
                    offset += 2 * lenAID;
                    offset += 4;

                    pad = cpad.Substring(0, 32 - sAID.Length) + padbegin;
                    TreeViewItem tempItem2 = null;
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            tempItem2 = new TreeViewItem
                            {
                                Header = sAID + pad + ConvertTool.Ascii2String(sAID) + endpad,

                                ToolTip = "Status: LOADED"
                            };
                            applet.Items.Add(tempItem2);
                        }));

                    int count = 0;
                    count = System.Convert.ToInt32((temp).Substring(offset, 2), 16);
                    offset += 2;
                    while (count > 0)
                    {
                        lenAID = System.Convert.ToInt32((temp).Substring(offset, 2), 16);
                        offset += 2;
                        sAID = (temp).Substring(offset, lenAID * 2);
                        offset += 2 * lenAID;

                        pad = cpad.Substring(0, 32 - sAID.Length) + padbegin;

                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                            new Action(() =>
                            {
                                TreeViewItem tempItem22 = new TreeViewItem
                                {
                                    Header = sAID + pad + ConvertTool.Ascii2String(sAID) + endpad
                                };
                                tempItem2.Items.Add(tempItem22);
                            }));
                        count--;
                    }


                }

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       installView = treeView;
                       applet.IsExpanded = true;
                       application.IsExpanded = true;
                       isd.IsExpanded = true;
                       ssd.IsExpanded = true;

                       //delBtn.IsEnabled = true;
                       installBtn.IsEnabled = true;
                   }));

                outToBox(finish_tip);

            }
            catch (Exception EX)
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       if (applet != null && ssd != null)
                       {
                           applet.IsExpanded = true;
                           application.IsExpanded = true;
                           isd.IsExpanded = true;
                           ssd.IsExpanded = true;
                       }

                       installBtn.IsEnabled = false;
                       putkeyBtn.IsEnabled = false;
                   }));

                outToBox(EX.Message);
                outToBox(fail_tip);
            }

        }

        private void listClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                buildList();
            }).Start();
        }

        private void deleteApp()
        {
            tabCurent = tabControl.Items.GetItemAt(3);
            TreeViewItem obj = null;
            try
            {
                XmlElement root = cmXML.GetXmlDocumentRoot();

                string aid, ver, kmc, enc, mac, dek, rsa_n, rsa_d, iccid, seid, node;
                string sl = "00", chl = "0";
                string tempAID = "";
                int isDM = 0;

                string objHeader = "";

                node = root.Name + "/" + "aid";
                aid = cmXML.Read(node, "", true);
                node = root.Name + "/" + "ver";
                ver = cmXML.Read(node, "", true);
                node = root.Name + "/" + "kmc";
                kmc = cmXML.Read(node, "", true);
                node = root.Name + "/" + "enc";
                enc = cmXML.Read(node, "", true);
                node = root.Name + "/" + "mac";
                mac = cmXML.Read(node, "", true);
                node = root.Name + "/" + "dek";
                dek = cmXML.Read(node, "", true);
                node = root.Name + "/" + "rsa_n";
                rsa_n = cmXML.Read(node, "", true);
                node = root.Name + "/" + "rsa_d";
                rsa_d = cmXML.Read(node, "", true);
                node = root.Name + "/" + "iccid";
                iccid = cmXML.Read(node, "", true);
                node = root.Name + "/" + "seid";
                seid = cmXML.Read(node, "", true);

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                       new Action(() =>
                       {


                           if (radioMac.IsChecked == true)
                               sl = "01";
                           else if (radioEncMac.IsChecked == true)
                               sl = "03";
                           else
                               sl = "00";

                           chl = channel.Text;

                           obj = treeView.SelectedItem as TreeViewItem;
                       }));


                outToBox(start_tip);
                if (obj == null)
                    outToBox("No Item Selected\n");
                else
                {

                    if (gp != null && gp.isDebug())
                        gp.stopDebug();

                    if (GetText(kmcCombox) == "CPG202")
                    {
                        gp = new gplib(GetText(comboBoxSelectReader), "", "", "", chl, kmc, aid, ver, sl);
                        gp.setKeyType(1);
                    }
                    else if (GetText(kmcCombox) == "CPG212")
                    {
                        gp = new gplib(GetText(comboBoxSelectReader), "", "", "", chl, kmc, aid, ver, sl);
                        gp.setKeyType(2);
                    }

                    else
                        gp = new gplib(GetText(comboBoxSelectReader), enc, mac, dek, chl, "", aid, ver, sl);
                    gp.setPort(GetText(jcopport));
                    gp.setConsole(viewOUTPUT);
                    gp.setRSA(rsa_n, rsa_d, "");

                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            installBtn.IsEnabled = false;
                            putkeyBtn.IsEnabled = false;

                            if (apduTimer.IsChecked == true)
                                gp.setTime(true);
                            objHeader = obj.Header.ToString();
                        }));
                    installGP = gp;
                    gp.Reset();

                    gp.secApdu("00A40400" + (aid.Length / 2).ToString("X2") + aid);
                    if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                        throw new ArgumentException("Select Failed!");

                    gp.initUpdate(ver);
                    if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                        throw new ArgumentException("Auth Failed!");
                    gp.externalAuthenticate(sl);
                    if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                        throw new ArgumentException("Auth Failed!");

                    if (aid.Length > 9)
                    {
                        gp.secApdu("80F24000" + ((aid.Length / 2 + 2).ToString("X2")) + "4F" + ((aid.Length / 2).ToString("X2")) + aid);
                        if ((gp.getResponse()).Length > 4)
                        {
                            string privilege = (gp.getResponse()).Substring(((gp.getResponse()).Length) - 6, 2);
                            int pri = System.Convert.ToInt32(privilege, 16);

                            if ((pri & 0xA0) == 0xA0)
                                isDM = 1;
                        }
                    }

                    if (isDM > 0)
                    {
                        gp.setSE_ICC_ID(iccid);
                        if (GetText(gpVer) == "China Mobile")
                        {
                            isDM = 3;
                            gp.setSE_ICC_ID(seid);
                        }
                        else if (GetText(gpVer) == "China Unicom")
                            isDM = 4;
                        else if (GetText(gpVer) == "China Telecom")
                            isDM = 4;
                        else if (GetText(gpVer) == "2.2")
                            isDM = 2;
                        else if (GetText(gpVer) == "2.1")
                            isDM = 1;

                        gp.setDMType(isDM);
                    }


                    if (objHeader == "Applet")
                    {
                        outToBox("Delete All Applet");
                        int i = obj.Items.Count;
                        while (i > 0)
                        {
                            TreeViewItem temp = obj.Items.GetItemAt(i - 1) as TreeViewItem;
                            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                                new Action(() =>
                                {
                                    tempAID = temp.Header.ToString();
                                    int len = tempAID.IndexOf("|", 0);
                                    if (len > 0)
                                        tempAID = tempAID.Substring(0, len);
                                    tempAID = tempAID.Replace(" ", "");

                                }));
                            gp.secApdu("80E40080" + ((tempAID.Length + 4) / 2).ToString("X2") + "4F" + (tempAID.Length / 2).ToString("X2") + tempAID);
                            i--;
                        }
                    }
                    else if (objHeader == "Application")
                    {
                        outToBox("Delete All Application");
                        int i = obj.Items.Count;
                        while (i > 0)
                        {
                            TreeViewItem temp = obj.Items.GetItemAt(i - 1) as TreeViewItem;
                            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                                new Action(() =>
                                {
                                    tempAID = temp.Header.ToString();
                                    int len = tempAID.IndexOf("|", 0);
                                    if (len > 0)
                                        tempAID = tempAID.Substring(0, len);
                                    tempAID = tempAID.Replace(" ", "");
                                }));
                            gp.secApdu("80E40000" + ((tempAID.Length + 4) / 2).ToString("X2") + "4F" + (tempAID.Length / 2).ToString("X2") + tempAID);
                            i--;
                        }
                    }
                    else if (objHeader == "Issuer Security Domain")
                    {
                        outToBox("Delete Issuer Security Domain");
                        int i = obj.Items.Count;
                        while (i > 0)
                        {
                            TreeViewItem temp = obj.Items.GetItemAt(i - 1) as TreeViewItem;
                            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                                 new Action(() =>
                                 {
                                     tempAID = temp.Header.ToString();
                                     int len = tempAID.IndexOf("|", 0);
                                     if (len > 0)
                                         tempAID = tempAID.Substring(0, len);
                                     tempAID = tempAID.Replace(" ", "");
                                 }));
                            gp.secApdu("80E40000" + ((tempAID.Length + 4) / 2).ToString("X2") + "4F" + (tempAID.Length / 2).ToString("X2") + tempAID);
                            i--;
                        }
                    }
                    else if (objHeader == "Supplementary Security Domain")
                    {
                        outToBox("Delete All Supplementary Security Domain");
                        int i = obj.Items.Count;
                        while (i > 0)
                        {
                            TreeViewItem temp = obj.Items.GetItemAt(i - 1) as TreeViewItem;
                            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                                new Action(() =>
                                {
                                    tempAID = temp.Header.ToString();
                                    int len = tempAID.IndexOf("|", 0);
                                    if (len > 0)
                                        tempAID = tempAID.Substring(0, len);
                                    tempAID = tempAID.Replace(" ", "");
                                }));
                            gp.secApdu("80E40000" + ((tempAID.Length + 4) / 2).ToString("X2") + "4F" + (tempAID.Length / 2).ToString("X2") + tempAID);
                            i--;
                        }
                    }
                    else
                    {
                        TreeViewItem temp2 = obj.Parent as TreeViewItem;
                        string temp2Header = "";
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                                new Action(() =>
                                {
                                    tempAID = obj.Header.ToString();
                                    temp2Header = temp2.Header.ToString();
                                }));
                        if (temp2Header == "Applet")
                        {
                            int len = tempAID.IndexOf("|", 0);
                            if (len > 0)
                                tempAID = tempAID.Substring(0, len);
                            tempAID = tempAID.Replace(" ", "");

                            outToBox("Delete a Applet");
                            gp.secApdu("80E40080" + ((tempAID.Length + 4) / 2).ToString("X2") + "4F" + (tempAID.Length / 2).ToString("X2") + tempAID);
                        }
                        else if (temp2Header == "Application")
                        {
                            int len = tempAID.IndexOf("|", 0);
                            if (len > 0)
                                tempAID = tempAID.Substring(0, len);
                            tempAID = tempAID.Replace(" ", "");

                            outToBox("Delete a Application");
                            gp.secApdu("80E40000" + ((tempAID.Length + 4) / 2).ToString("X2") + "4F" + (tempAID.Length / 2).ToString("X2") + tempAID);
                        }
                        else if (temp2Header == "Issuer Security Domain" ||
                            temp2Header == "Supplementary Security Domain")
                        {
                            int len = tempAID.IndexOf("|", 0);
                            if (len > 0)
                                tempAID = tempAID.Substring(0, len);
                            tempAID = tempAID.Replace(" ", "");

                            outToBox("Delete a Security Domain");
                            gp.secApdu("80E40000" + ((tempAID.Length + 4) / 2).ToString("X2") + "4F" + (tempAID.Length / 2).ToString("X2") + tempAID);
                        }
                        else
                        {
                            outToBox("Delete a Executable Module");
                            int len = tempAID.IndexOf("|", 0);
                            if (len > 0)
                                tempAID = tempAID.Substring(0, len);
                            tempAID = tempAID.Replace(" ", "");

                            gp.secApdu("80E40000" + ((tempAID.Length + 4) / 2).ToString("X2") + "4F" + (tempAID.Length / 2).ToString("X2") + tempAID);

                        }
                    }
                }

                outToBox(finish_tip);

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       installBtn.IsEnabled = true;
                       putkeyBtn.IsEnabled = true;
                   }));

            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       installBtn.IsEnabled = false;
                       putkeyBtn.IsEnabled = false;
                   }));

                outToBox(ex.Message);
                outToBox(fail_tip);
            }

        }

        private void delClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                deleteApp();
            }).Start();
        }

        private void treeViewRightClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem obj = treeView.SelectedItem as TreeViewItem;

            if (obj == null)
                outToBox("No Item Copied");
            else
            {
                string temp = obj.Header.ToString();
                int len = temp.IndexOf("|", 0);
                if (len > 0)
                    temp = temp.Substring(0, len);
                temp = temp.Trim();
                Clipboard.SetDataObject(temp);
                outToBox("Copied " + temp);
            }
        }

        private void installClick(object sender, RoutedEventArgs e)
        {
            (new InstallWindow()).Show(this);
        }

        private void getCFGHist(object sender, EventArgs e)
        {
            sdXmlBox.Items.Clear();

            XmlElement root = sdXML.GetXmlDocumentRoot();
            string his, node;
            bool isAdd = true;
            if (root.HasChildNodes)
            {
                XmlNode temp = root.FirstChild;
                while (temp != null)
                {
                    node = root.Name + "/" + temp.Name;
                    his = sdXML.Read(node, "");
                    if (his == "SecurityDomainConfig.xml")
                        isAdd = false;
                    sdXmlBox.Items.Add(his);
                    temp = temp.NextSibling;
                }
            }

            if (isAdd)
                sdXmlBox.Items.Add("SecurityDomainConfig.xml");
        }

        private void configOpenClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml"
            };
            if (dialog.ShowDialog() == true)
            {
                string file = dialog.FileName;
                int offset = file.LastIndexOf("\\");

                string emp = file.Substring(offset + 1);
                if (offset > -1)
                    sdXmlBox.Text = emp;

                XmlElement root = sdXML.GetXmlDocumentRoot();
                string node = "cfg" + System.DateTime.Now.ToString("yyMMddHHmmssfff");
                XmlNode temp = sdXML.CreateElement(node, GetText(sdXmlBox));
                if (root.FirstChild == null)
                    sdXML.XmlInsert(temp);
                else
                {
                    sdXML.XmlInsertBefore(root.FirstChild, temp);
                    sdXML.RemoveLastNode(root, 50);
                }
                cmXML = new XMLTool(sdXmlBox.Text);
                cfgXML.Update("config/SDconfig", "", sdXmlBox.Text);
            }

        }

        private void sdXmlChange(object sender, EventArgs e)
        {
            if (sdXmlBox.Text == "")
                sdXmlBox.Text = "SecurityDomainConfig.xml";
            cmXML = new XMLTool(sdXmlBox.Text);

            cfgXML.Update("config/SDconfig", "", sdXmlBox.Text);
        }

        private void getSpace()
        {

            tabCurent = tabControl.Items.GetItemAt(3);

            try
            {
                XmlElement root = cmXML.GetXmlDocumentRoot();

                string aid, ver, kmc, enc, mac, dek, rsa_n, rsa_d, iccid, seid, node;
                string sl = "00", chl = "0";
                int isDM = 0;

                node = root.Name + "/" + "aid";
                aid = cmXML.Read(node, "", true);
                node = root.Name + "/" + "ver";
                ver = cmXML.Read(node, "", true);
                node = root.Name + "/" + "kmc";
                kmc = cmXML.Read(node, "", true);
                node = root.Name + "/" + "enc";
                enc = cmXML.Read(node, "", true);
                node = root.Name + "/" + "mac";
                mac = cmXML.Read(node, "", true);
                node = root.Name + "/" + "dek";
                dek = cmXML.Read(node, "", true);
                node = root.Name + "/" + "rsa_n";
                rsa_n = cmXML.Read(node, "", true);
                node = root.Name + "/" + "rsa_d";
                rsa_d = cmXML.Read(node, "", true);
                node = root.Name + "/" + "iccid";
                iccid = cmXML.Read(node, "", true);
                node = root.Name + "/" + "seid";
                seid = cmXML.Read(node, "", true);

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                       new Action(() =>
                       {

                           if (radioMac.IsChecked == true)
                               sl = "01";
                           else if (radioEncMac.IsChecked == true)
                               sl = "03";
                           else
                               sl = "00";

                           chl = channel.Text;
                           installBtn.IsEnabled = false;
                           putkeyBtn.IsEnabled = false;
                       }));

                outToBox(start_tip);

                if (gp != null && gp.isDebug())
                    gp.stopDebug();

                if (GetText(kmcCombox) == "CPG202")
                {
                    gp = new gplib(GetText(comboBoxSelectReader), "", "", "", chl, kmc, aid, ver, sl);
                    gp.setKeyType(1);
                }
                else if (GetText(kmcCombox) == "CPG212")
                {
                    gp = new gplib(GetText(comboBoxSelectReader), "", "", "", chl, kmc, aid, ver, sl);
                    gp.setKeyType(2);
                }
                else
                    gp = new gplib(GetText(comboBoxSelectReader), enc, mac, dek, chl, "", aid, ver, sl);
                gp.setPort(GetText(jcopport));
                gp.setConsole(viewOUTPUT);
                gp.setRSA(rsa_n, rsa_d, "");

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       if (apduTimer.IsChecked == true)
                           gp.setTime(true);
                   }));

                installGP = gp;
                gp.Reset();
                gp.secApdu("00A40400" + (aid.Length / 2).ToString("X2") + aid);
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");

                gp.initUpdate(ver);
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");
                gp.externalAuthenticate(sl);
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");

                if (aid.Length > 9)
                {
                    gp.secApdu("80F24000" + ((aid.Length / 2 + 2).ToString("X2")) + "4F" + ((aid.Length / 2).ToString("X2")) + aid);
                    if ((gp.getResponse()).Length > 4)
                    {
                        string privilege = (gp.getResponse()).Substring(((gp.getResponse()).Length) - 6, 2);
                        int pri = System.Convert.ToInt32(privilege, 16);

                        if ((pri & 0xA0) == 0xA0)
                            isDM = 1;
                    }
                }

                if (isDM > 0)
                {
                    gp.setSE_ICC_ID(iccid);
                    if (GetText(gpVer) == "China Mobile")
                    {
                        isDM = 3;
                        gp.setSE_ICC_ID(seid);
                    }
                    else if (GetText(gpVer) == "China Unicom")
                        isDM = 4;
                    else if (GetText(gpVer) == "China Telecom")
                        isDM = 4;
                    else if (GetText(gpVer) == "2.2")
                        isDM = 2;
                    else if (GetText(gpVer) == "2.1")
                        isDM = 1;

                    gp.setDMType(isDM);
                }

                gp.secApdu("80E4 0080 12 4F10FA0100FF0109020D37040E506070AF0E");
                if (isDM > 0)
                    gp.secApdu("80E6 0200 29 10FA0100FF0109020D37040E506070AF0E 00 14 0A4DA9DCCE73EC079A87C7196838D2399C2F290D 00 00");
                else
                    gp.secApdu("80E6 0200 15 10FA0100FF0109020D37040E506070AF0E 00 00 00 00");
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");

                gp.secApdu("80E8 0000 C8 C4820189010023DECAFFED020204000110FA0100FF0109020D37040E506070AF0E086765745370616365020021002300210014000B003A000E009C000A001D00000075023700000000000001010004000B01030107A00000006201010300140110F0000000000000000000000000000E00000C06000E000000800302000107010000002007009C000210188C000A180389017A05308F00063D8C0007181D0441181D258B00047A0321188B000B60037A198B00032D1A0425730064FFCAFFCA000918038901701218");
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");
                gp.secApdu("80E8 8001 C5 117800900B8700183D850104418901038D000C117FFF6DE91A03038D000C8D00053B1A05058D000C8D00053B1A07048D000C8D00053B1A1006AF018D00053B190310088B000D180187008D0008600D8D00097008116D008D00027A08000A0000000000000000000005003A000E02000200020002010680070103800A0103800302068010060100020006000001068008110680081206800300038003030680081003800A0809001D00070A330A04042C0F0012050A040A0707270B03070307030908070508");
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");

                if (isDM == 3 || isDM == 4)
                    gp.secApdu("80E6 0C00 45 10FA0100FF0109020D37040E506070AF0E10F0000000000000000000000000000E0010FA0100FF0109020D37040E506070AEEE01000EC900EF0AA008810101A5038201C000");
                else
                    gp.secApdu("80E6 0C00 39 10FA0100FF0109020D37040E506070AF0E10F0000000000000000000000000000E0010FA0100FF0109020D37040E506070AEEE010002C90000");
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");

                gp.secApdu("00A4040010 FA0100FF0109020D37040E506070AEEE");
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");
                gp.secApdu("80CA000000");
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");

                int e2p = System.Convert.ToInt32((gp.getResponse()).Substring(0, 4), 16) + 397;
                int dtr = System.Convert.ToInt32((gp.getResponse()).Substring(4, 4), 16);
                int rtr = System.Convert.ToInt32((gp.getResponse()).Substring(8, 4), 16);
                int cunt = System.Convert.ToInt32((gp.getResponse()).Substring(12, 4), 16);

                gp.Reset();
                gp.secApdu("00A40400" + (aid.Length / 2).ToString("X2") + aid);
                gp.initUpdate(ver);
                gp.externalAuthenticate(sl);
                gp.secApdu("80E4 0080 12 4F10FA0100FF0109020D37040E506070AF0E");

                outToBox("\nAmount of memory available:");
                outToBox(" MEMORY_TYPE_PERSISTENT (Flash/EEPROM): " + (e2p / 1024.0 + cunt * 30) + "k");
                outToBox(" MEMORY_TYPE_TRANSIENT_RESET (RAM): " + rtr / 1024.0 + "k");
                outToBox(" MEMORY_TYPE_TRANSIENT_DESELECT (RAM): " + dtr / 1024.0 + "k" + "\n");

                outToBox(finish_tip);

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       installBtn.IsEnabled = true;
                       putkeyBtn.IsEnabled = true;
                   }));
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       installBtn.IsEnabled = false;
                       putkeyBtn.IsEnabled = false;
                   }));

                outToBox(ex.Message);
                outToBox(fail_tip);
            }

        }

        private void getSpaceClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                getSpace();
            }).Start();
        }


        private void HypeLink1_OnClick(object sender, RoutedEventArgs e)
        {
            //home
            System.Diagnostics.Process.Start("http://map.im/");
        }

        private void HypeLink2_OnClick(object sender, RoutedEventArgs e)
        {
            //github
            System.Diagnostics.Process.Start("https://github.com/APDU/SmartCardPlus");
        }

        private void HypeLink3_OnClick(object sender, RoutedEventArgs e)
        {
            //api
            System.Diagnostics.Process.Start("http://map.im/notes/2018/09/01/luaapi.html");
        }

        private void HypeLink4_OnClick(object sender, RoutedEventArgs e)
        {
            //api
            System.Diagnostics.Process.Start("http://map.im/notes/2019/09/01/jsapi.html");
        }


        private void putKeyClick(object sender, RoutedEventArgs e)
        {
            (new PutKeyWindow()).Show(this);
        }

        private void selectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            putkeyBtn.IsEnabled = false;
            SelectBtn.IsEnabled = false;
            var obj = treeView.SelectedItem as TreeViewItem;
            if (obj != null)
            {
                delBtn.IsEnabled = true;
                var temp2 = obj.Parent as TreeViewItem;
                if (temp2 != null)
                {
                    if (temp2.Header.ToString() == "Issuer Security Domain" ||
                                temp2.Header.ToString() == "Supplementary Security Domain")
                    {
                        secutiyDomain = obj.Header.ToString();
                        int len = secutiyDomain.IndexOf("|", 0);
                        if (len > 0)
                            secutiyDomain = secutiyDomain.Substring(0, len);
                        secutiyDomain = secutiyDomain.Replace(" ", "");
                        putkeyBtn.IsEnabled = true;

                    }

                    SelectBtn.IsEnabled = true;
                }
            }
            else
                delBtn.IsEnabled = false;
        }

        private void scriptDrag(object sender, DragEventArgs e)
        {
            comboBoxScript.Text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
        }

        private void capDrag(object sender, DragEventArgs e)
        {
            comboBoxCM.Text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
        }

        private void getStatusClick(object sender, RoutedEventArgs e)
        {
            tabCurent = tabControl.Items.GetItemAt(3);

            try
            {
                XmlElement root = cmXML.GetXmlDocumentRoot();

                string aid, ver, kmc, enc, mac, dek, rsa_n, rsa_d, iccid, seid, node;
                string sl = "00", chl = "0";

                node = root.Name + "/" + "aid";
                aid = cmXML.Read(node, "", true);
                node = root.Name + "/" + "ver";
                ver = cmXML.Read(node, "", true);
                node = root.Name + "/" + "kmc";
                kmc = cmXML.Read(node, "", true);
                node = root.Name + "/" + "enc";
                enc = cmXML.Read(node, "", true);
                node = root.Name + "/" + "mac";
                mac = cmXML.Read(node, "", true);
                node = root.Name + "/" + "dek";
                dek = cmXML.Read(node, "", true);
                node = root.Name + "/" + "rsa_n";
                rsa_n = cmXML.Read(node, "", true);
                node = root.Name + "/" + "rsa_d";
                rsa_d = cmXML.Read(node, "", true);
                node = root.Name + "/" + "iccid";
                iccid = cmXML.Read(node, "", true);
                node = root.Name + "/" + "seid";
                seid = cmXML.Read(node, "", true);

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                       new Action(() =>
                       {

                           if (radioMac.IsChecked == true)
                               sl = "01";
                           else if (radioEncMac.IsChecked == true)
                               sl = "03";
                           else
                               sl = "00";

                           chl = channel.Text;
                           installBtn.IsEnabled = false;
                           putkeyBtn.IsEnabled = false;
                       }));

                outToBox(start_tip);

                if (gp != null && gp.isDebug())
                    gp.stopDebug();

                if (GetText(kmcCombox) == "CPG202")
                {
                    gp = new gplib(GetText(comboBoxSelectReader), "", "", "", chl, kmc, aid, ver, sl);
                    gp.setKeyType(1);
                }
                else if (GetText(kmcCombox) == "CPG212")
                {
                    gp = new gplib(GetText(comboBoxSelectReader), "", "", "", chl, kmc, aid, ver, sl);
                    gp.setKeyType(2);
                }
                else
                    gp = new gplib(GetText(comboBoxSelectReader), enc, mac, dek, chl, "", aid, ver, sl);
                gp.setPort(GetText(jcopport));
                gp.setConsole(viewOUTPUT);
                //gp.setRSA(rsa_n, rsa_d, "");

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       if (apduTimer.IsChecked == true)
                           gp.setTime(true);
                   }));

                //installGP = gp;
                gp.Reset();
                gp.secApdu("00A40400" + (aid.Length / 2).ToString("X2") + aid);
                //if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                //    throw new ArgumentException("Failed!");

                gp.initUpdate(ver);
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");
                gp.externalAuthenticate(sl);
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");


                gp.secApdu("80F28000 02 4F00");
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Get Status Failed!");

                int offset = 0;
                string temp = (gp.getResponse()).Substring(0, (gp.getResponse()).Length - 4);
                int lenAID = System.Convert.ToInt32((temp).Substring(offset, 2), 16);
                offset += 2;
                offset += 2 * lenAID;
                int ilife = System.Convert.ToInt32((temp).Substring(offset, 2), 16);

                string life = "NULL";
                if (ilife == 1)
                    life = "OP_READY";
                else if (ilife == 7)
                    life = "INITIALIZED";
                else if (ilife == 0xF)
                    life = "SECURED";
                else if (ilife == 0x7F)
                    life = "CARD_LOCKED";
                else if (ilife == 0xFF)
                    life = "TERMINATED";

                outToBox("\nCard Life Cycle State: " + life);
                outToBox(finish_tip);
            }
            catch (Exception ex)
            {
                outToBox(ex.Message);
                outToBox(fail_tip);
            }

        }

        private void SetStatusClick(object sender, RoutedEventArgs e)
        {
            tabCurent = tabControl.Items.GetItemAt(3);

            try
            {
                XmlElement root = cmXML.GetXmlDocumentRoot();

                string aid, ver, kmc, enc, mac, dek, rsa_n, rsa_d, iccid, seid, node;
                string sl = "00", chl = "0";

                node = root.Name + "/" + "aid";
                aid = cmXML.Read(node, "", true);
                node = root.Name + "/" + "ver";
                ver = cmXML.Read(node, "", true);
                node = root.Name + "/" + "kmc";
                kmc = cmXML.Read(node, "", true);
                node = root.Name + "/" + "enc";
                enc = cmXML.Read(node, "", true);
                node = root.Name + "/" + "mac";
                mac = cmXML.Read(node, "", true);
                node = root.Name + "/" + "dek";
                dek = cmXML.Read(node, "", true);
                node = root.Name + "/" + "rsa_n";
                rsa_n = cmXML.Read(node, "", true);
                node = root.Name + "/" + "rsa_d";
                rsa_d = cmXML.Read(node, "", true);
                node = root.Name + "/" + "iccid";
                iccid = cmXML.Read(node, "", true);
                node = root.Name + "/" + "seid";
                seid = cmXML.Read(node, "", true);

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                       new Action(() =>
                       {

                           if (radioMac.IsChecked == true)
                               sl = "01";
                           else if (radioEncMac.IsChecked == true)
                               sl = "03";
                           else
                               sl = "00";

                           chl = channel.Text;
                           installBtn.IsEnabled = false;
                           putkeyBtn.IsEnabled = false;
                       }));

                outToBox(start_tip);

                if (gp != null && gp.isDebug())
                    gp.stopDebug();

                if (GetText(kmcCombox) == "CPG202")
                {
                    gp = new gplib(GetText(comboBoxSelectReader), "", "", "", chl, kmc, aid, ver, sl);
                    gp.setKeyType(1);
                }
                else if (GetText(kmcCombox) == "CPG212")
                {
                    gp = new gplib(GetText(comboBoxSelectReader), "", "", "", chl, kmc, aid, ver, sl);
                    gp.setKeyType(2);
                }
                else
                    gp = new gplib(GetText(comboBoxSelectReader), enc, mac, dek, chl, "", aid, ver, sl);

                gp.setConsole(viewOUTPUT);
                gp.setPort(GetText(jcopport));
                //gp.setRSA(rsa_n, rsa_d, "");

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       if (apduTimer.IsChecked == true)
                           gp.setTime(true);
                   }));

                //installGP = gp;
                gp.Reset();
                gp.secApdu("00A40400" + (aid.Length / 2).ToString("X2") + aid);
                //if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                //    throw new ArgumentException("Failed!");

                gp.initUpdate(ver);
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");
                gp.externalAuthenticate(sl);
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Failed!");


                gp.secApdu("80F28000 02 4F00");
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Get Status Failed!");

                int offset = 0;
                string temp = (gp.getResponse()).Substring(0, (gp.getResponse()).Length - 4);
                int lenAID = System.Convert.ToInt32((temp).Substring(offset, 2), 16);
                offset += 2;
                offset += 2 * lenAID;
                int ilife = System.Convert.ToInt32((temp).Substring(offset, 2), 16);

                string life = "NULL";

                if (ilife == 1)
                    life = "INITIALIZED";
                else if (ilife == 7)
                    life = "SECURED";
                else if (ilife == 0xF)
                    life = "CARD_LOCKED";
                else if (ilife == 0x7F)
                    life = "SECURED";

                string setdemo = "Set Card Life Cycle State to: " + life;

                if (ilife == 1)
                    gp.secApdu("80F0 8007 00");
                else if (ilife == 7)
                    gp.secApdu("80F0 800F 00");
                else if (ilife == 0xF)
                    gp.secApdu("80F0 807F 00");
                else if (ilife == 0x7F)
                    gp.secApdu("80F0 800F 00");
                //else if (ilife == 0xFF)
                //    life = "TERMINATED";

                /*
                gp.secApdu("80F28000 02 4F00");
                if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                    throw new ArgumentException("Get Status Failed!");
                offset = 0;
                temp = (gp.getResponse()).Substring(0, (gp.getResponse()).Length - 4);
                lenAID = System.Convert.ToInt32((temp).Substring(offset, 2), 16);
                offset += 2;
                offset += 2 * lenAID;
                ilife = System.Convert.ToInt32((temp).Substring(offset, 2), 16);

                life = "NULL";
                if (ilife == 1)
                    life = "OP_READY";
                else if (ilife == 7)
                    life = "INITIALIZED";
                else if (ilife == 0xF)
                    life = "SECURED";
                else if (ilife == 0x7F)
                    life = "CARD_LOCKED";
                else if (ilife == 0xFF)
                    life = "TERMINATED";	*/


                outToBox("\n" + setdemo);

                outToBox(finish_tip);
                //outToBox("\nCurrent Card Life Cycle State: " + life + "\n");

            }
            catch (Exception ex)
            {
                outToBox(ex.Message);
                outToBox(fail_tip);
            }

        }

        private void SelectClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var obj = treeView.SelectedItem as TreeViewItem;
                string aid = "";
                if (obj != null)
                {
                    var temp2 = obj.Parent as TreeViewItem;
                    if (temp2 != null)
                    {
                        if (temp2.Header.ToString() == "Issuer Security Domain" ||
                                    temp2.Header.ToString() == "Supplementary Security Domain" ||
                                    temp2.Header.ToString() == "Applet" ||
                                    temp2.Header.ToString() == "Application")
                        {
                            aid = obj.Header.ToString();
                            int len = aid.IndexOf("|", 0);
                            if (len > 0)
                                aid = aid.Substring(0, len);
                            aid = aid.Replace(" ", "");

                            //gp = new gplib(GetText(comboBoxSelectReader), "", "", "", "", "", "", "", "");
                            gp.secApdu("00A40400" + (aid.Length / 2).ToString("X2") + aid);
                            return;
                        }
                        else
                        {

                            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                                    new Action(() =>
                                    {
                                        aid = obj.Header.ToString();

                                    }));

                            int len = aid.IndexOf("|", 0);
                            if (len > 0)
                                aid = aid.Substring(0, len);
                            aid = aid.Replace(" ", "");

                            gp.secApdu("00A40400" + (aid.Length / 2).ToString("X2") + aid);
                            return;

                        }
                    }
                }

                outToBox("No AID Selected");
            }
            catch (Exception ex)
            {
                outToBox(ex.Message);
                //outToBox("\n------Failed------");
            }
        }

        private void updateJCOPPort(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (gp != null)
                    gp.setPort(jcopport.Text);
                cfgXML.Update("config/jcopport", "", jcopport.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("config.xml " + ex.Message);
            }
        }

        private void defaultCfgClick(object sender, RoutedEventArgs e)
        {
            try
            {
                gpVer.Text = "2.2";
                channel.Text = "0";
                sdXmlBox.Text = "SecurityDomainConfig.xml";
                kmcCombox.Text = "No Diversify";
                loadLen.Text = "200";
                apduTimer.IsChecked = false;
                jcopport.Text = "8050";
                radioPlain.IsChecked = false;
                radioMac.IsChecked = true;
                radioEncMac.IsChecked = false;
                scriptLog.IsChecked = true;
                capLog.IsChecked = false;

                cfgXML.Update("config/gpVer", "", "2.2");
                cfgXML.Update("config/channel", "", "0");
                cfgXML.Update("config/SDconfig", "", "SecurityDomainConfig.xml");
                cfgXML.Update("config/kmc", "", "No Diversify");
                cfgXML.Update("config/loadLen", "", "200");
                cfgXML.Update("config/jcopport", "", "8050");
                cfgXML.Update("config/securityLevel", "", "1");
                cfgXML.Update("config/apduTimer", "", "false");
                cfgXML.Update("config/scriptLog", "", "true");
                cfgXML.Update("config/capLog", "", "false");
            }
            catch (Exception ex)
            {
                MessageBox.Show("config.xml " + ex.Message);
            }

        }

        private void radioPlainChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                cfgXML.Update("config/securityLevel", "", "0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("config.xml " + ex.Message);
            }
        }

        private void radioMacChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                cfgXML.Update("config/securityLevel", "", "1");
            }
            catch (Exception ex)
            {
                MessageBox.Show("config.xml " + ex.Message);
            }
        }

        private void radioEncMacChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                cfgXML.Update("config/securityLevel", "", "3");
            }
            catch (Exception ex)
            {
                MessageBox.Show("config.xml " + ex.Message);
            }
        }

        private void loadLenChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int len = System.Convert.ToInt32(GetText(loadLen)); ;
                if (len <= 0 || len > 0x1F4 || len % 2 != 0)
                {
                    MessageBox.Show("Load length Error!");
                }
                else
                    cfgXML.Update("config/loadLen", "", loadLen.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("config.xml " + ex.Message);
            }
        }


        private void timerClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (apduTimer.IsChecked == true)
                    cfgXML.Update("config/apduTimer", "", "true");
                else
                    cfgXML.Update("config/apduTimer", "", "false");
            }
            catch (Exception ex)
            {
                MessageBox.Show("config.xml " + ex.Message);
            }
        }

        private void scriptlogClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (scriptLog.IsChecked == true)
                    cfgXML.Update("config/scriptLog", "", "true");
                else
                    cfgXML.Update("config/scriptLog", "", "false");
            }
            catch (Exception ex)
            {
                MessageBox.Show("config.xml " + ex.Message);
            }
        }

        private void caplogClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (capLog.IsChecked == true)
                    cfgXML.Update("config/capLog", "", "true");
                else
                    cfgXML.Update("config/capLog", "", "false");
            }
            catch (Exception ex)
            {
                MessageBox.Show("config.xml " + ex.Message);
            }
        }

        private void kmcChanged(object sender, EventArgs e)
        {
            try
            {
                cfgXML.Update("config/kmc", "", kmcCombox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("config.xml " + ex.Message);
            }
        }

        private void gpVerChanged(object sender, EventArgs e)
        {
            try
            {
                cfgXML.Update("config/gpVer", "", gpVer.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("config.xml " + ex.Message);
            }
        }

        private void channelChanged(object sender, EventArgs e)
        {
            try
            {
                cfgXML.Update("config/channel", "", channel.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("config.xml " + ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void apdu_Clear_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            apduresOUTPUT.Clear();
        }
        private void apdu_Copy_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(apduresOUTPUT.SelectedText);
        }
        private void apdu_CopyAll_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(apduresOUTPUT.Text);
        }


        private void script_OpenFolder_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Directory.GetParent(comboBoxScript.Text).FullName);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        scriptresOUTPUT.AppendText(ex.Message + "\n");
                    }));
            }
        }
        private void script_Edit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(comboBoxScript.Text);
            }
            catch (Exception ex) {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        scriptresOUTPUT.AppendText(ex.Message + "\n");
                    }));
            }
        }
        private void script_Clear_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            scriptresOUTPUT.Clear();
        }
        private void script_Copy_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(scriptresOUTPUT.SelectedText);
        }
        private void script_CopyAll_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(scriptresOUTPUT.Text);
        }


        private void cap_OpenFolder_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Directory.GetParent(comboBoxCM.Text).FullName);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        cmresOUTPUT.AppendText(ex.Message + "\n");
                    }));
            }
        }
        private void cap_Edit_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(comboBoxCM.Text);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                    new Action(() =>
                    {
                        cmresOUTPUT.AppendText(ex.Message + "\n");
                    }));
            }
        }
        private void cap_Clear_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            cmresOUTPUT.Clear();
        }
        private void cap_Copy_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(cmresOUTPUT.SelectedText);
        }
        private void cap_CopyAll_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(cmresOUTPUT.Text);
        }



        private void view_Clear_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            viewOUTPUT.Clear();
        }
        private void view_Copy_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(viewOUTPUT.SelectedText);
        }
        private void view_CopyAll_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetDataObject(viewOUTPUT.Text);
        }


    }
}
