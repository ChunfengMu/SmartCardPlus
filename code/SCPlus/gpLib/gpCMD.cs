using ALG;
using PCSC;
using System;
using System.Windows.Controls;
using System.Windows;
using xTool;
using System.Reflection;
using System.ComponentModel;
using System.IO;
namespace gpLib
{
    public class gplib : IDisposable
    {
        private Control control;

        private bool isTime;
        private bool isPrint;

        private string aid;
        private string ver;
        private string sl;

        private int DMType = 1;
        private int PutRSAKeyType = 0;
        private string se_icc_id = "";

        private int kmcType;
        private string kmc;
        private string enc;
        private string mac;
        private string dek;
        private string scp;
        private string macIni;
        private string channel;
        private string securityLevel;
        private string hostRandom;

        private string skenc;
        private string skmac;
        private string skdek;

        private SCardReader Reader;
        private string ReaderName = "";
        //private SCardContext Context;
        public static SCardContext Context = new SCardContext();
        private string Command;
        private string Response;

        private string n;
        private string d;
        private string e;

        public string port;

        private int autoRes = 1;

        private JCOP debug = null;

        public static int errCount = 0;

        private StreamWriter SctLog = null;

        private StreamWriter CapLog = null;


        public string getReaderName()
        {
            return this.ReaderName;
        }

        public void setPort(string sport)
        {
            sport = xTool.ConvertTool.RemoveSpace(sport);
            this.port = sport;
        }

        public void setKeyType(int type)
        {
            this.kmcType = type;
        }

        public void setPrint(bool b)
        {
            this.isPrint = b;
        }

        public void setTime(bool b)
        {
            this.isTime = b;
        }

        public void setConsole(Control con)
        {
            this.control = con;
        }

        public void setSctLog(StreamWriter sctSW)
        {
            this.SctLog = sctSW;
        }

        public StreamWriter getSctLog()
        {
            return this.SctLog;
        }


        public void setCapLog(StreamWriter sctSW)
        {
            this.CapLog = sctSW;
        }

        public StreamWriter getCapLog()
        {
            return this.CapLog;
        }

        public void print(string s)
        {
            if (this.control == null || this.isPrint == false)
                return;
            if (control.GetType() == typeof(TextBox))
            {
                if (((TextBox)control).Text.Length > 15000)
                {
                    ((TextBox)control).Clear();
                    //GC.Collect();
                }

                ((TextBox)control).Text += (s + "\n");

                if (null != SctLog)
                    SctLog.WriteLine(s);

                if (null != CapLog)
                    CapLog.WriteLine(s);
            }
            else if (control.GetType() == typeof(ComboBox))
            {
                ((ComboBox)control).Text += s;
            }
            else if (control.GetType() == typeof(RichTextBox))
            {
                ((RichTextBox)control).AppendText((s + "\n"));
            }
        }

        public void setSE_ICC_ID(string id)
        {
            id = ConvertTool.RemoveSpace(id);
            this.se_icc_id = id;
        }

        public void setPutRSAKeyType(int type)
        {
            this.PutRSAKeyType = type;
        }

        public int getPutRSAKeyType()
        {
            return this.PutRSAKeyType;
        }

        public void setDMType(int type)
        {
            this.DMType = type;
        }

        public int getDMType()
        {
            return this.DMType;
        }

        public string getSecurityLevel()
        {
            return this.sl;
        }

        public string getKeyVer()
        {
            return this.ver;
        }

        public string getAID()
        {
            return this.aid;
        }

        public void setAuto(int flag)
        {
            this.autoRes = flag;
        }

        public void setChannel(string channel)
        {
            channel = ConvertTool.RemoveSpace(channel);
            this.channel = channel;
        }

        public void setENC(string enc)
        {
            enc = ConvertTool.RemoveSpace(enc);
            this.enc = enc;
        }

        public void setMAC(string mac)
        {
            mac = ConvertTool.RemoveSpace(mac);
            this.mac = mac;
        }

        public void setDEK(string dek)
        {
            dek = ConvertTool.RemoveSpace(dek);
            this.dek = dek;
        }

        public void setRSA_n(string rsa_n)
        {
            rsa_n = ConvertTool.RemoveSpace(rsa_n);
            this.n = rsa_n;
        }

        public void setRSA_d(string rsa_d)
        {
            rsa_d = ConvertTool.RemoveSpace(rsa_d);
            this.d = rsa_d;
        }

        public void setRSA(string rsa_n, string rsa_d, string rsa_e)
        {
            rsa_n = ConvertTool.RemoveSpace(rsa_n);
            rsa_d = ConvertTool.RemoveSpace(rsa_d);
            rsa_e = ConvertTool.RemoveSpace(rsa_e);
            this.n = rsa_n;
            this.d = rsa_d;
            this.e = rsa_e;
        }

        public gplib(string reader, string enc, string mac, string dek, string channel, string kmc = "", string aid = "", string ver = "", string sl = "")
        {
            //if (reader == "")
            //    throw new ArgumentException("No selected Reader or Reader name is NULL");
            enc = ConvertTool.RemoveSpace(enc);
            mac = ConvertTool.RemoveSpace(mac);
            dek = ConvertTool.RemoveSpace(dek);
            channel = ConvertTool.RemoveSpace(channel);
            kmc = ConvertTool.RemoveSpace(kmc);
            aid = ConvertTool.RemoveSpace(aid);
            ver = ConvertTool.RemoveSpace(ver);
            sl = ConvertTool.RemoveSpace(sl);

            this.isPrint = true;
            this.isTime = false;
            this.control = null;
            this.DMType = 0;
            this.autoRes = 1;
            this.enc = enc;
            this.mac = mac;
            this.dek = dek;
            this.macIni = "0000000000000000";
            this.channel = channel;
            this.ReaderName = reader;
            this.aid = aid;
            this.ver = ver;
            this.sl = sl;
            this.kmc = kmc;
            this.kmcType = 0;

            if (reader == "JCOP Debug")
            {
                this.debug = new JCOP();
                return;
            }
            this.Reader = new SCardReader(Context);
            Context.Establish(SCardScope.System);
        }


        public string getSKdek()
        {
            return this.skdek;
        }

        public string getAPDU()
        {
            return this.Command;
        }

        public string getResponse()
        {
            return this.Response;
        }

        private string getEnumDescription(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {

                object[] attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)

                    return ((DescriptionAttribute)attrs[0]).Description;

            }

            return en.ToString();
        }


        public string DisConnect()
        {
            if (isDebug() == false)
            {
                SCardError rc = this.Reader.Disconnect(SCardReaderDisposition.Unpower);
                if (rc == SCardError.Success)
                    return ("DisConnect Success");
                else
                    return ("PCSC: " + "0x" + ((int)rc).ToString("X8") + ", " + getEnumDescription(rc));
            }
            else
            {
                this.debug.stop();
                return ("JCOP Debug Stop");
            }
        }

        public void stopDebug()
        {
            this.debug.stop();
        }

        public bool isDebug()
        {
            if (this.ReaderName == "JCOP Debug")
                return true;
            else
                return false;
        }

        public void Reset()
        {
            this.securityLevel = "00";
            try
            {
                string temp;
                byte[] atr;
                if (isDebug() == false)
                {
                    if (this.ReaderName == null || this.ReaderName == "")
                    {
                        errCount++;
                        throw new PCSCException(SCardError.NoReaderSelected, "PCSC: 0x" + ((int)SCardError.NoReaderSelected).ToString("X8") + ", " + getEnumDescription(SCardError.NoReaderSelected));
                    }
                    if (this.Reader.IsConnected)
                        this.Reader.Disconnect(SCardReaderDisposition.Reset);
                    SCardError rc = this.Reader.Connect(this.ReaderName, SCardShareMode.Shared, SCardProtocol.Any);

                    if (rc != SCardError.Success)
                    {
                        errCount++;
                        throw new PCSCException(rc, "PCSC: 0x" + ((int)rc).ToString("X8") + ", " + getEnumDescription(rc));
                        //throw new ArgumentException("Description: " + getEnumDescription(rc) + "\n" + "PCSC  Error: " + "0x" + ((int)rc).ToString("X8") + "\n");
                    }
                    else
                    {

                        this.Reader.GetAttrib(SCardAttribute.AtrString, out atr);

                        temp = ConvertTool.Bytes2String(atr);
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                            new Action(() =>
                            {
                                print("ATR: " + temp);
                                /*if (this.isTime)
                                    {
                                        var tm = (t2.Hour * 60 * 60 + t2.Minute * 60 + t2.Second) * 1000 + t2.Millisecond - (t1.Hour * 60 * 60 + t1.Minute * 60 + t1.Second) * 1000 - t1.Millisecond;
                                        print("Time: " + tm + "ms\n");
                                    }*/
                            }));
                    }
                }
                else
                {
                    int iport = System.Convert.ToInt32(this.port, 10);
                    temp = ConvertTool.Bytes2String(debug.reset(iport));
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            print("ATR: " + temp);
                        }));
                }

                this.Response = temp;

            }
            //catch (ArgumentNullException)
            //{
            //    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
            //                  new Action(() =>
            //                  {
            //                      print("\n" + "No Selectd Redaer" + "\n\n");
            //                  }));
            //}
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                               new Action(() =>
                               {
                                   print(ex.Message);
                               }));
            }

        }

        public string APDU(string sapdu, string response)
        {
            sapdu = xTool.ConvertTool.RemoveSpace(sapdu);

            string apdu = this.claChannel(sapdu.Substring(0, 2)) + sapdu.Substring(2);
            byte[] temp = this.send(ConvertTool.String2Bytes(apdu));
            string resp = ConvertTool.Bytes2String(temp);

            if (response != "")
            {
                if (response.Length == 4)
                {
                    string sw = resp.Substring(resp.Length - 4);
                    if (!sw.Equals(response, StringComparison.OrdinalIgnoreCase))
                    {

                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                            new Action(() =>
                            {
                                print("Error, Expected SW: " + response);
                                errCount++;
                            }));
                    }
                }
                else
                {
                    if (!resp.Equals(response, StringComparison.OrdinalIgnoreCase))
                    {
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                           new Action(() =>
                           {
                               print("Error, Expected Response: " + response);
                               errCount++;
                           }));
                    }
                }
            }

            return resp;
        }

        private void send(string sapdu)
        {
            sapdu = xTool.ConvertTool.RemoveSpace(sapdu);

            string apdu = this.claChannel(sapdu.Substring(0, 2)) + sapdu.Substring(2);
            this.Command = apdu;
            byte[] temp = this.send(ConvertTool.String2Bytes(apdu));
            string resp = ConvertTool.Bytes2String(temp);

            if ((resp.Length == 4) && (this.autoRes != 0))
            {
                if (resp.Substring(0, 2) == "6C" || resp.Substring(0, 2) == "6c")
                {
                    temp = this.send(ConvertTool.String2Bytes(apdu.Substring(0, apdu.Length - 2) + resp.Substring(2, 2)));
                    resp = ConvertTool.Bytes2String(temp);
                }
                else if (resp.Substring(0, 2) == "61")
                {
                    temp = this.send(ConvertTool.String2Bytes(this.claChannel("00") + "C00000" + resp.Substring(2, 2)));
                    resp = ConvertTool.Bytes2String(temp);
                    while (resp.Substring(0, 2) == "61") //61xx
                    {
                        temp = this.send(ConvertTool.String2Bytes(this.claChannel("00") + "C00000" + resp.Substring(2, 2)));
                        resp = ConvertTool.Bytes2String(temp);
                    }
                }
            }
            this.Response = resp;
        }

        private byte[] send(byte[] apdu)
        {
            byte[] resp = new byte[290];

            SCardError rt;
            string temp = "";
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            temp = xTool.ConvertTool.Bytes2String(apdu);
                            if (temp.Length > 9)
                                print("-> " + temp.Substring(0, 4) + " " + temp.Substring(4, 4) + " " + temp.Substring(8, 2) + " " + temp.Substring(10));
                            else if (temp.Length > 7)
                                print("-> " + temp.Substring(0, 4) + " " + temp.Substring(4, 4));
                            else
                                print("-> " + temp);

                        }));
            if (temp.Length < 8)
                throw new PCSCException(SCardError.ApduFormatWrong, "PCSC: 0x" + ((int)SCardError.ApduFormatWrong).ToString("X8") + ", " + getEnumDescription(SCardError.ApduFormatWrong));

            var t1 = System.DateTime.Now;
            var t2 = t1;
            if (isDebug())
            {
                t1 = System.DateTime.Now;
                resp = this.debug.send(apdu);
                t2 = System.DateTime.Now;
            }
            else
            {
                t1 = System.DateTime.Now;
                rt = this.Reader.Transmit(SCardPCI.GetPci(this.Reader.ActiveProtocol), apdu, ref resp);
                t2 = System.DateTime.Now;
                if (rt != SCardError.Success)
                    throw new PCSCException(rt, "PCSC: 0x" + ((int)rt).ToString("X8") + ", " + getEnumDescription(rt));
                //throw new ArgumentException("Description: " + getEnumDescription(rt) + "\n" + "PCSC  Error: " + "0x" + ((int)rt).ToString("X8") + "\n");
            }

            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Input,
                        new Action(() =>
                        {
                            print("<- " + xTool.ConvertTool.Bytes2String(resp));
                            if (this.isTime)
                            {
                                var tm = (t2.Hour * 60 * 60 + t2.Minute * 60 + t2.Second) * 1000 + t2.Millisecond - (t1.Hour * 60 * 60 + t1.Minute * 60 + t1.Second) * 1000 - t1.Millisecond;
                                print("Time: " + tm + "ms");
                            }
                        }));

            return resp;
        }

        private string claChannel(string cla)
        {
            int temp1 = System.Convert.ToInt32(cla, 16);
            int temp2 = System.Convert.ToInt32(this.channel);

            return (temp1 + temp2).ToString("X2");
        }

        private string Pading80(string str, bool force = false)
        {
            if (force)
            {
                str += "80";
                while ((str.Length % 16) != 0) str += "00";
            }
            else
            {
                if ((str.Length % 16) != 0)
                    str += "80";
                while ((str.Length % 16) != 0)
                    str += "00";
            }

            return str;
        }

        public void initUpdate(string ver)
        {
            ver = ConvertTool.RemoveSpace(ver);

            if (ver.Length == 1)
                ver = "0" + ver;

            if (ver.Length != 2)
                throw new ArgumentException("Please check \"ver\" in Security Domain xml file");

            if (this.kmcType == 0)
            {
                if (this.enc.Length != 32 || this.mac.Length != 32 || this.dek.Length != 32)
                    throw new ArgumentException("Please check \"enc or mac or dek\" in Security Domain xml file");
            }
            else
            {
                if (this.kmc.Length != 32)
                    throw new ArgumentException("Please check \"kmc\" in Security Domain xml file");
            }

            Random ran = new Random();

            string ran8 = (ran.Next(0, 0xFF)).ToString("X2") + (ran.Next(0, 0xFF)).ToString("X2") +
                (ran.Next(0, 0xFF)).ToString("X2") + (ran.Next(0, 0xFF)).ToString("X2") +
                (ran.Next(0, 0xFF)).ToString("X2") + (ran.Next(0, 0xFF)).ToString("X2") +
                (ran.Next(0, 0xFF)).ToString("X2") + (ran.Next(0, 0xFF)).ToString("X2");
            this.hostRandom = ran8;
            string init = "8050" + ver + "0008" + ran8;

            this.send(init);

            if ((this.Response).Substring((this.Response).Length - 4) == "9000" && this.kmcType == 1)//cpg202
            {
                /*
                string data1 = this.Response.Substring(0, 12) + "F001" + this.Response.Substring(16, 4) + "000000000F01";
                string data2 = this.Response.Substring(0, 12) + "F002" + this.Response.Substring(16, 4) + "000000000F02";
                string data3 = this.Response.Substring(0, 12) + "F003" + this.Response.Substring(16, 4) + "000000000F03";
                */

                string data1 = this.Response.Substring(0, 4) + this.Response.Substring(8, 8) + "F001" + this.Response.Substring(0, 4) + this.Response.Substring(8, 8) + "0F01";
                string data2 = this.Response.Substring(0, 4) + this.Response.Substring(8, 8) + "F002" + this.Response.Substring(0, 4) + this.Response.Substring(8, 8) + "0F02";
                string data3 = this.Response.Substring(0, 4) + this.Response.Substring(8, 8) + "F003" + this.Response.Substring(0, 4) + this.Response.Substring(8, 8) + "0F03";

                this.enc = ALG.DES.TriDesECBEn(data1, this.kmc);
                this.mac = ALG.DES.TriDesECBEn(data2, this.kmc);
                this.dek = ALG.DES.TriDesECBEn(data3, this.kmc);
            }
            else if ((this.Response).Substring((this.Response).Length - 4) == "9000" && this.kmcType == 2) //cpg212
            {
                /*
                string data1 = this.Response.Substring(0, 12) + "F001" + this.Response.Substring(16, 4) + "000000000F01";
                string data2 = this.Response.Substring(0, 12) + "F002" + this.Response.Substring(16, 4) + "000000000F02";
                string data3 = this.Response.Substring(0, 12) + "F003" + this.Response.Substring(16, 4) + "000000000F03";
                */

                string data1 = this.Response.Substring(8, 12) + "F001" + this.Response.Substring(8, 12) + "0F01";
                string data2 = this.Response.Substring(8, 12) + "F002" + this.Response.Substring(8, 12) + "0F02";
                string data3 = this.Response.Substring(8, 12) + "F003" + this.Response.Substring(8, 12) + "0F03";

                this.enc = ALG.DES.TriDesECBEn(data1, this.kmc);
                this.mac = ALG.DES.TriDesECBEn(data2, this.kmc);
                this.dek = ALG.DES.TriDesECBEn(data3, this.kmc);
            }
        }

        public void externalAuthenticate(string sl)
        {
            sl = ConvertTool.RemoveSpace(sl);

            if (sl.Length == 1)
                sl = "0" + sl;

            this.securityLevel = sl;
            this.scp = this.Response.Substring(22, 2);

            if (this.scp == "02")
            {
                string zero12 = "000000000000000000000000";

                this.skenc = DES.TriDesCBCEn("0182" + this.Response.Substring(24, 4) + zero12, this.enc);
                this.skmac = DES.TriDesCBCEn("0101" + this.Response.Substring(24, 4) + zero12, this.mac);
                this.skdek = DES.TriDesCBCEn("0181" + this.Response.Substring(24, 4) + zero12, this.dek);

                string hostCipher = DES.TriDesCBCEn(this.Response.Substring(24, 16) + hostRandom + "8000000000000000", this.skenc);
                hostCipher = hostCipher.Substring(32, 16);

                string authCmd = "8482" + sl + "0010";

                this.macIni = DES.TriDesMAC(Pading80(authCmd + hostCipher), this.skmac);

                this.send(authCmd + hostCipher + this.macIni);
            }
            else if (this.scp == "01")
            {
                var tmp_str = this.Response.Substring(32, 8) + hostRandom.Substring(0, 8) + this.Response.Substring(24, 8) + hostRandom.Substring(8, 8);

                this.skenc = DES.TriDesECBEn(tmp_str, this.enc);
                this.skmac = DES.TriDesECBEn(tmp_str, this.mac);
                this.skdek = this.dek;

                string hostCipher = DES.TriDesCBCEn(this.Response.Substring(24, 16) + hostRandom + "8000000000000000", this.skenc);
                hostCipher = hostCipher.Substring(32, 16);

                string authCmd = "8482" + sl + "0010";
                this.macIni = DES.TriDesCBCEn(authCmd + hostCipher + "800000", this.skmac).Substring(16, 16);

                this.send(authCmd + hostCipher + this.macIni);
            }
        }

        public void secApdu(string sapdu)
        {
            sapdu = ConvertTool.RemoveSpace(sapdu);

            if (sapdu.Length < 9)
            {
                this.send(sapdu);
                return;
            }
            int cla_t = System.Convert.ToInt32(sapdu.Substring(0, 2), 16);
            int cla = System.Convert.ToInt32(sapdu.Substring(0, 2), 16);
            int ins = System.Convert.ToInt32(sapdu.Substring(2, 2), 16);
            string p1p2 = sapdu.Substring(4, 4);
            int lc = System.Convert.ToInt32(sapdu.Substring(8, 2), 16);
            string data;
            if (sapdu.Length == 10)
                data = "";
            else
                data = sapdu.Substring(10, lc * 2);

            string le;
            string mac = "";

            if ((10 + lc * 2 + 2) == sapdu.Length)
                le = sapdu.Substring(10 + lc * 2, 2);
            else
                le = "";

            int P1 = System.Convert.ToInt32(sapdu.Substring(4, 2), 16);
            if (DMType == 1) //gp2.1
            {
                if (ins == 0xe6 && P1 != 0x20)
                {
                    if (this.n.Length != 256 || this.d.Length != 256)
                        throw new ArgumentException("Please check \"rsa_n or rsa_d\" in Security Domain xml file");

                    lc = lc - 1;
                    string s = calcToken(p1p2 + (lc.ToString("X2")) + data.Substring(0, lc * 2), this.n, this.d);

                    data = data.Substring(0, lc * 2) + "80" + s;
                    lc = data.Length / 2;
                }
            }
            else if (DMType == 2)//gp2.2
            {
                if (ins == 0xe6 && P1 != 0x20)
                {
                    if (this.n.Length != 256 || this.d.Length != 256)
                        throw new ArgumentException("Please check \"rsa_n or rsa_d\" in Security Domain xml file");

                    lc = lc - 1;
                    string s = calcToken(p1p2 + (lc.ToString("X2")) + data.Substring(0, lc * 2), this.n, this.d);

                    data = data.Substring(0, lc * 2) + "80" + s;
                    lc = data.Length / 2;
                }
                else if (ins == 0xe4 && data.Length >= 14)
                {
                    if (this.n.Length != 256 || this.d.Length != 256)
                        throw new ArgumentException("Please check \"rsa_n or rsa_d\" in Security Domain xml file");

                    string s = calcToken(p1p2 + (lc.ToString("X2")) + data.Substring(0, lc * 2), this.n, this.d);

                    data = data.Substring(0, lc * 2) + "9E8180" + s;
                    lc = data.Length / 2;
                }
            }
            else if (DMType == 3)//cmcc
            {
                if (ins == 0xe6 && P1 != 0x20)
                {
                    if (this.n.Length != 256 || this.d.Length != 256)
                        throw new ArgumentException("Please check \"rsa_n or rsa_d\" in Security Domain xml file");
                    if (this.se_icc_id.Length != 20)
                        throw new ArgumentException("Please check \"seid\" in Security Domain xml file");

                    lc = lc - 1;
                    string s = calcToken(p1p2 + ((lc + se_icc_id.Length / 2 + 1).ToString("X2")) + (se_icc_id.Length / 2).ToString("X2") + se_icc_id + data.Substring(0, lc * 2), this.n, this.d);

                    data = data.Substring(0, lc * 2) + "80" + s;
                    lc = data.Length / 2;
                }
                else if (ins == 0xe4 && data.Length >= 14)
                {
                    if (this.n.Length != 256 || this.d.Length != 256)
                        throw new ArgumentException("Please check \"rsa_n or rsa_d\" in Security Domain xml file");
                    if (this.se_icc_id.Length != 20)
                        throw new ArgumentException("Please check \"seid\" in Security Domain xml file");

                    string s = calcToken(p1p2 + ((lc + se_icc_id.Length / 2 + 1).ToString("X2")) + (se_icc_id.Length / 2).ToString("X2") + se_icc_id + data.Substring(0, lc * 2), this.n, this.d);

                    data = data.Substring(0, lc * 2) + "9E8180" + s;
                    lc = data.Length / 2;
                }
            }
            else if (DMType == 4)//cuc
            {
                if (ins == 0xe6 && P1 != 0x20)
                {
                    if (this.n.Length != 256 || this.d.Length != 256)
                        throw new ArgumentException("Please check \"rsa_n or rsa_d\" in Security Domain xml file");
                    if (this.se_icc_id.Length != 20)
                        throw new ArgumentException("Please check \"iccid\" in Security Domain xml file");

                    lc = lc - 1;
                    string s = calcToken(this.se_icc_id + p1p2 + (lc.ToString("X2")) + data.Substring(0, lc * 2), this.n, this.d);

                    data = data.Substring(0, lc * 2) + "80" + s;
                    lc = data.Length / 2;
                }
                else if (ins == 0xe4 && data.Length >= 14)
                {
                    if (this.n.Length != 256 || this.d.Length != 256)
                        throw new ArgumentException("Please check \"rsa_n or rsa_d\" in Security Domain xml file");
                    if (this.se_icc_id.Length != 20)
                        throw new ArgumentException("Please check \"iccid\" in Security Domain xml file");

                    string s = calcToken(this.se_icc_id + p1p2 + (lc.ToString("X2")) + data.Substring(0, lc * 2), this.n, this.d);

                    data = data.Substring(0, lc * 2) + "9E8180" + s;
                    lc = data.Length / 2;
                }
            }

            int sl;
            if (ins == 0xA4)
                this.securityLevel = "00";
            string apdu_head5 = cla.ToString("X2") + ins.ToString("X2") + p1p2 + lc.ToString("X2");
            sl = System.Convert.ToInt32(this.securityLevel, 16);
            if (sl >= 1 && ins != 0x70)
            {
                cla |= 4;
                lc += 8;

                apdu_head5 = cla.ToString("X2") + ins.ToString("X2") + p1p2 + lc.ToString("X2");

                if (this.scp == "01")
                {
                    string icv = DES.TriDesCBCEn(this.macIni, this.skmac);
                    mac = DES.TriDesCBCEn(Pading80(apdu_head5 + data, true), this.skmac, icv);
                    mac = mac.Substring(mac.Length - 16, 16);
                    this.macIni = mac;
                }
                else
                {
                    mac = DES.TriDesMAC(Pading80(this.macIni + apdu_head5 + data, true), this.skmac);
                    this.macIni = mac;
                }
            }

            if (sl >= 3 && ins != 0x70)
            {

                if (this.scp == "01")
                {
                    lc -= 8;
                    data = lc.ToString("X2") + data;
                    data = DES.TriDesCBCEn(Pading80(data), this.skenc);
                    lc = data.Length / 2 + 8;
                    apdu_head5 = cla.ToString("X2") + ins.ToString("X2") + p1p2 + lc.ToString("X2");
                }
                else
                {
                    data = DES.TriDesCBCEn(Pading80(data, true), this.skenc);
                    lc = data.Length / 2 + 8;
                    apdu_head5 = cla.ToString("X2") + ins.ToString("X2") + p1p2 + lc.ToString("X2");
                }
            }

            if (lc == 0)
                this.send(apdu_head5);
            else
                this.send(apdu_head5 + data + mac + le);

        }

        public string calcToken(string indata, string n, string d)
        {
            indata = ConvertTool.RemoveSpace(indata);
            n = ConvertTool.RemoveSpace(n);
            d = ConvertTool.RemoveSpace(d);

            string data = "0001";
            //FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF ;
            while (data.Length < (n.Length - 72))
                data += "FF";
            data = data + "003021300906052B0E03021A05000414" + Hash.HashSHA1(indata);
            string token;
            token = RSA.RSAde(data, n, d);

            return token;
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~gplib()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Reader != null)
                Reader.Dispose();
        }
    }
}