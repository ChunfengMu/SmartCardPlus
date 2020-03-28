using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using gpLib;
namespace GUI
{
    /// <summary>
    /// PutKeyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PutKeyWindow : Window
    {
        private gplib gp = MainWindow.installGP;
        private string sdAID = MainWindow.secutiyDomain;
        public PutKeyWindow()
        {
            InitializeComponent();
      
        }
        public void Show(Window owner)
        {
            this.Owner = owner;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            this.Show();
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

        private void putKeyClick(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    outToBox(MainWindow.start_tip);

                    string p1 = "";
                    string p2 = "";
                    string keyType = "";
                    string kvn = "";
                    string key1 = "";
                    string key2 = "";
                    string key3 = "";

                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            if (p1Combox.Text == "Add")
                                p1 = "00";
                            else if (p1Combox.Text == "Replace")
                                p1 = kvnOld.Text.Replace(" ","");

                            if (p2Combox.Text == "Single Key")
                                p2 = "01";
                            else if (p2Combox.Text == "Multiple keys")
                                p2 = "81";

                            keyType = typeComboBox.Text.Replace(" ","");

                            kvn = kvnNew.Text.Replace(" ", "");

                            key1 = Key1.Text.Replace(" ", "");
                            key2 = Key2.Text.Replace(" ", "");
                            key3 = Key3.Text.Replace(" ", "");
                        }));

                    if (p1 == "" || keyType == "" || kvn == "" || key1 == "")
                    {
                        outToBox("Argument(s) = NULL");
                        throw new Exception("666666ExCurr!");
                    }

                    if (p1.Length == 1)
                    {
                        p1 = "0" + p1;
                    }
                    else if (p1.Length != 2)
                    {
                        outToBox("P1 length wrong");
                        throw new Exception("666666ExCurr!");
                    }

                    if (kvn.Length == 1)
                    {
                        kvn = "0" + kvn;                      
                    }
                    else if (kvn.Length != 2) {
                        outToBox("KVN length wrong");
                        throw new Exception("666666ExCurr!");
                    }

                    if (p2 == "01")
                    {
                        if (keyType == "A0") {
                            if ( key2 == "")
                            {
                                outToBox("Argument(s) = NULL");
                                throw new Exception("666666ExCurr!");
                            }
                            gp.Reset();
                            gp.secApdu("00A40400" + (sdAID.Length / 2).ToString("X2") + sdAID);
                            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                                throw new ArgumentException("Select Failed!");
                            gp.initUpdate(gp.getKeyVer());
                            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                                throw new ArgumentException("Auth Failed!");
                            gp.externalAuthenticate(gp.getSecurityLevel());
                            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                                throw new ArgumentException("Auth Failed!");

                            if (gp.getPutRSAKeyType() == 0)
                                gp.secApdu("80D8" + p1 + p2 + ((key1 + key2).Length / 2 + 5).ToString("X2") + kvn + "A1" + (key1.Length / 2).ToString("X2") + key1 + "A0" + (key2.Length / 2).ToString("X2") + key2);
                            else
                                gp.secApdu("80D8" + p1 + p2 + ((key1 + key2).Length / 2 + 5 + 1).ToString("X2") + kvn + "A1" + (key1.Length / 2).ToString("X2") + key1 + "A0" + (key2.Length / 2).ToString("X2") + key2 +"00");
                            
                            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                                throw new ArgumentException("Put Key Failed!");
                        }
                        else {
                            if (key1.Length != 32)
                            {
                                outToBox("Key length wrong");
                                throw new Exception("666666ExCurr!");
                            }

                            if (key2.Length % 2 != 0)
                                key2 = "0" + key2;

                            gp.Reset();
                            gp.secApdu("00A40400" + (sdAID.Length / 2).ToString("X2") + sdAID);
                            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                                throw new ArgumentException("Select Failed!");
                            gp.initUpdate(gp.getKeyVer());
                            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                                throw new ArgumentException("Auth Failed!");
                            gp.externalAuthenticate(gp.getSecurityLevel());
                            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                                throw new ArgumentException("Auth Failed!");

                            gp.secApdu("80D8" + p1 + p2 + "17" + kvn + "8010" + ALG.DES.TriDesECBEn(key1, gp.getSKdek()) + "03" + (ALG.DES.TriDesECBEn("0000000000000000", key1)).Substring(0, 6));
                            if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                                throw new ArgumentException("Put Key Failed!");
                        }
                    }
                    else if (p2 == "81")
                    {
                        if (key2 == "" || key3 == "")
                        {
                            outToBox("Argument(s) = NULL");
                            throw new Exception("666666ExCurr!");
                        }
                        if (key1.Length != 32 || key2.Length != 32 || key3.Length != 32)
                        {
                            outToBox("Key length wrong");
                            throw new Exception("666666ExCurr!");
                        }
                        gp.Reset();
                        gp.secApdu("00A40400" + (sdAID.Length / 2).ToString("X2") + sdAID);
                        if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                            throw new ArgumentException("Select Failed!");
                        gp.initUpdate(gp.getKeyVer());
                        if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                            throw new ArgumentException("Auth Failed!");
                        gp.externalAuthenticate(gp.getSecurityLevel());
                        if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                            throw new ArgumentException("Auth Failed!");

                        gp.secApdu("80D8" + p1 + p2 + "43" + kvn + "8010" + ALG.DES.TriDesECBEn(key1, gp.getSKdek()) + "03" + (ALG.DES.TriDesECBEn("0000000000000000", key1)).Substring(0, 6) + "8010" + ALG.DES.TriDesECBEn(key2, gp.getSKdek()) + "03" + (ALG.DES.TriDesECBEn("0000000000000000", key2)).Substring(0, 6) + "8010" + ALG.DES.TriDesECBEn(key3, gp.getSKdek()) + "03" + (ALG.DES.TriDesECBEn("0000000000000000", key3)).Substring(0, 6));
                        if ((gp.getResponse()).Substring((gp.getResponse()).Length - 4) != "9000")
                            throw new ArgumentException("Put Key Failed!");
                    }
                    else
                    {
                        outToBox("Argument(s) = NULL");
                        throw new Exception("666666ExCurr!");
                    }
                       
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

        private void keyTypeChange(object sender, EventArgs e)
        {
            if (p2Combox.SelectedIndex == 0 && typeComboBox.SelectedIndex != 0) //single des
            {
                textBlockKey1.Text = "KEY";
                textBlockKey2.Text = "";
                textBlockKey3.Text = "";
                Key1.Text = "000102030405060708090A0B0C0D0E0F";
                Key3.Text = "";
                Key2.Text = "";
                Key1.IsEnabled = true;
                Key2.IsEnabled = false;
                Key3.IsEnabled = false;
            }
            else if (p2Combox.SelectedIndex == 1 && typeComboBox.SelectedIndex != 0) //mutil des
                {
                    textBlockKey1.Text = "ENC";
                    textBlockKey2.Text = "MAC";
                    textBlockKey3.Text = "DEK";
                
                    Key1.Text = "AEA57E0042780CEBBBD7CA2FE63CFF05";
                    Key2.Text = "888D0508C5AF62E085F7D3FB713DBA33";
                    Key3.Text = "42C9F4D5271376425FC49D5C7E697CA0";
                    Key1.IsEnabled = true;
                    Key2.IsEnabled = true;
                    Key3.IsEnabled = true;
                }
            else if (p2Combox.SelectedIndex == 0 && typeComboBox.SelectedIndex == 0) //rsa
            {
                textBlockKey1.Text = "modulus";
                textBlockKey2.Text = "exponent";
                textBlockKey3.Text = "";
               
                Key1.Text = "AD5E61FA6F27ABE06E31B20B990171CE7F6C8F34B59CE5887F0FA4E03A45DB518638E35F94AD65986DBCA4303C9483F836B0DA24A7DC5D5D43B975E86D5A1AE49D293CE46BFF5AF8DFE69F2523231E04FAA58C6DDE68BA2071CEC1F5DE452489E243FEFEB2D69AE427CE268D88932E0ADAAF55B2DD36461DA62251342FD6D7B1";
                Key2.Text = "010001";
                Key3.Text = "";
                Key1.IsEnabled = true;
                Key2.IsEnabled = true;
                Key3.IsEnabled = false;
            }
            else if (p2Combox.SelectedIndex == 1 && typeComboBox.SelectedIndex == 0) //multi rsa
            {
                textBlockKey1.Text = "";
                textBlockKey2.Text = "";
                textBlockKey3.Text = "";

                Key1.Text = "";
                Key2.Text = "";
                Key3.Text = "";
                Key1.IsEnabled = false;
                Key2.IsEnabled = false;
                Key3.IsEnabled = false;
            }
        }

        private void p1Change(object sender, SelectionChangedEventArgs e)
        {
            if (p1Combox.SelectedIndex == 0)
                kvnOld.IsEnabled = false;
            else
                kvnOld.IsEnabled = true;
        }

        private void p2Change(object sender, EventArgs e)
        {           
            if (p2Combox.SelectedIndex == 0 && typeComboBox.SelectedIndex != 0) //single des
            {
                textBlockKey1.Text = "KEY";
                textBlockKey2.Text = "";
                textBlockKey3.Text = "";
                Key1.Text = "000102030405060708090A0B0C0D0E0F";
                Key3.Text = "";
                Key2.Text = "";
                Key1.IsEnabled = true;
                Key2.IsEnabled = false;
                Key3.IsEnabled = false;
            }
            else if (p2Combox.SelectedIndex == 1 && typeComboBox.SelectedIndex != 0) //mutil des
            {
                textBlockKey1.Text = "ENC";
                textBlockKey2.Text = "MAC";
                textBlockKey3.Text = "DEK";

                Key1.Text = "AEA57E0042780CEBBBD7CA2FE63CFF05";
                Key2.Text = "888D0508C5AF62E085F7D3FB713DBA33";
                Key3.Text = "42C9F4D5271376425FC49D5C7E697CA0";
                Key1.IsEnabled = true;
                Key2.IsEnabled = true;
                Key3.IsEnabled = true;
            }
            else if (p2Combox.SelectedIndex == 0 && typeComboBox.SelectedIndex == 0) //rsa
            {
                textBlockKey1.Text = "modulus";
                textBlockKey2.Text = "exponent";
                textBlockKey3.Text = "";

                Key1.Text = "AD5E61FA6F27ABE06E31B20B990171CE7F6C8F34B59CE5887F0FA4E03A45DB518638E35F94AD65986DBCA4303C9483F836B0DA24A7DC5D5D43B975E86D5A1AE49D293CE46BFF5AF8DFE69F2523231E04FAA58C6DDE68BA2071CEC1F5DE452489E243FEFEB2D69AE427CE268D88932E0ADAAF55B2DD36461DA62251342FD6D7B1";
                Key2.Text = "010001";
                Key3.Text = "";
                Key1.IsEnabled = true;
                Key2.IsEnabled = true;
                Key3.IsEnabled = false;
            }
            else if (p2Combox.SelectedIndex == 1 && typeComboBox.SelectedIndex == 0) //multi rsa
            {
                textBlockKey1.Text = "";
                textBlockKey2.Text = "";
                textBlockKey3.Text = "";

                Key1.Text = "";
                Key2.Text = "";
                Key3.Text = "";
                Key1.IsEnabled = false;
                Key2.IsEnabled = false;
                Key3.IsEnabled = false;
            }
        }
     
    }
}
