using System;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using gpLib;
namespace GUI
{
    /// <summary>
    /// install.xaml 的交互逻辑
    /// </summary>
    public partial class InstallWindow : Window
    {
        private gplib gp = MainWindow.installGP;
        public InstallWindow()
        {
            InitializeComponent();
            privBox.Text = "00";     
        }

        public void Show(Window owner)
        {
            this.Owner = owner;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;  
            this.Show();
        }

        private void getPackageList(object sender, EventArgs e)
        {
            packageBox.Items.Clear();
            if (MainWindow.installView == null)
                return;
            TreeViewItem item = MainWindow.installView.Items.GetItemAt(0) as TreeViewItem;
            if (item == null)
                return;
            int i = item.Items.Count;
            while (i > 0)
            {
                TreeViewItem temp = item.Items.GetItemAt(i - 1) as TreeViewItem;
                if (temp == null)
                    return;
                if (temp.HasItems == true)
                    packageBox.Items.Add(temp.Header);
                i--;
            }
        }

        private void getAppletList(object sender, EventArgs e)
        {
            appletBox.Items.Clear();
            if (MainWindow.installView == null)
                return;
            TreeViewItem item = MainWindow.installView.Items.GetItemAt(0) as TreeViewItem;
            if (item == null)
                return;
            string appletHeader = packageBox.Text;
            int i = item.Items.Count;
            while (i > 0)
            {
                TreeViewItem temp = item.Items.GetItemAt(i - 1) as TreeViewItem;
                if (temp == null)
                    return;
                if ((temp.HasItems == true) && ((temp.Header.ToString()) == appletHeader))
                {
                    int k = 0;
                    k = temp.Items.Count;
                    while (k > 0)
                    {
                        TreeViewItem temp2 = temp.Items.GetItemAt(k - 1) as TreeViewItem;
                        if (temp2 == null)
                            return;
                        appletBox.Items.Add(temp2.Header);
                        k--;
                    }
                }

                i--;
            }
        }

            
        private void outToBox(string s)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       MainWindow.installBox.AppendText(s);
                   }));            
        }

        private void closeClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void installClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                string instanceAID = "";
                string appletAID = "";
                string packageAID = "";
                string c9 = "";
                string privilege = "";
                int len = 0;

                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                   new Action(() =>
                   {
                       instanceAID = instanceBox.Text.Replace(" ", "");
                       len = instanceAID.IndexOf("|");
                       if (len > 0)
                           instanceAID = instanceAID.Remove(len);

                       appletAID = appletBox.Text.Replace(" ", "");
                       len = appletAID.IndexOf("|");
                       if (len > 0)
                           appletAID = appletAID.Remove(len);

                       packageAID = packageBox.Text.Replace(" ", "");
                       len = packageAID.IndexOf("|");
                       if (len > 0)
                           packageAID = packageAID.Remove(len);

                       c9 = C9Box.Text.Replace(" ", "");
                       privilege = privBox.Text.Replace(" ", "");
                   }));
                
                try
                {
                    outToBox(MainWindow.start_tip);

                    if (this.gp == null)
                    {
                        outToBox("Please click List button first\n");
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                            new Action(() =>
                            {
                                this.Close();
                            }));
                        
                        throw new Exception("666666ExCurr!");                      
                    }

                    if (instanceAID == "" || appletAID == "" || packageAID == "" || c9 == "" || privilege == "")
                    {
                        outToBox("Argument(s) = NULL");
                        throw new Exception("666666ExCurr!");     
                    }

                    gp.Reset();

                    gp.secApdu("00A40400" + (gp.getAID().Length / 2).ToString("X2") + gp.getAID());
                    if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                        throw new ArgumentException("Select Failed!");

                    gp.initUpdate(gp.getKeyVer());
                    if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                        throw new ArgumentException("Auth Failed!");
                    gp.externalAuthenticate(gp.getSecurityLevel());
                    if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                        throw new ArgumentException("Auth Failed!");

                    gp.secApdu("80E40000" + (instanceAID.Length / 2 + 2).ToString("X2") + "4F" + (instanceAID.Length / 2).ToString("X2") + instanceAID);
                    gp.secApdu("80E60C00" + ((instanceAID + packageAID + appletAID + privilege + c9).Length / 2 + 6).ToString("X2") + (packageAID.Length / 2).ToString("X2") + packageAID + (appletAID.Length / 2).ToString("X2") + appletAID + (instanceAID.Length / 2).ToString("X2") + instanceAID + (privilege.Length / 2).ToString("X2") + privilege + (c9.Length / 2).ToString("X2") + c9 + "00");
                    if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                        throw new ArgumentException("Install Failed!");

                    outToBox(MainWindow.finish_tip);
                }
                catch (Exception ex)
                {
                    if (ex.Message != "666666ExCurr!")
                        outToBox(ex.Message);
                    outToBox(MainWindow.fail_tip);
                }             
            }).Start();        
        }
    }
}
