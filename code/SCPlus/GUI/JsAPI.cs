using GUI;
using ALG;
using gpLib;
using xTool;
using System;
using System.IO;
using System.Text;
using PCSC;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Collections.Generic;

namespace GUI
{
    public class JsAPI
    {
        private string sReader1 = "";
        private string sReader2 = "";
        public static gplib gp1 = null;
        public static gplib gp2 = null;

        private bool isReset = false;
        private bool isReset_gp1 = false;
        private bool isReset_gp2 = false;

        private const string error_prefix = "ERROR, ";

        private string currentScriptPath;

        private List<string> includeList;

        public string GetScriptPath()
        {
            return this.currentScriptPath;
        }

        public JsAPI(string path)
        {
            this.currentScriptPath = path;
            includeList = new List<string>();
        }

        public void printlib()
        {
            try
            {
                foreach(var lib in includeList)
                    print(lib);
            }
            catch (Exception)
            {
            }
        }

        public void include(string path)
        {
            string prePath = string.Empty;
            string includeScriptPath = string.Empty;

            try
            {
                includeScriptPath = TextHelper.GetAbsolutePath(this.currentScriptPath, path);
                prePath = this.currentScriptPath;

                if (includeList.Contains(includeScriptPath))
                    return;
                else
                    includeList.Add(includeScriptPath);


                this.currentScriptPath = includeScriptPath;
                string str_script = TextHelper.getScript(includeScriptPath);

                MainWindow.mSC.Execute(str_script);
            }
            catch (Exception ex)
            {
                print(error_prefix + "include " + path + " failed.");
            }
            finally
            {
                this.currentScriptPath = prePath;
            }
        }

        public string hmac_sha1(string key, string data)
        {
            try
            {
                return ALG.HMAC.HMAC_SHA1(key, data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string hmac_sha224(string key, string data)
        {
            try
            {
                return ALG.HMAC.HMAC_SHA224(key, data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string hmac_sha256(string key, string data)
        {
            try
            {
                return ALG.HMAC.HMAC_SHA256(key, data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string hmac_sha384(string key, string data)
        {
            try
            {
                return ALG.HMAC.HMAC_SHA384(key, data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string hmac_sha512(string key, string data)
        {
            try
            {
                return ALG.HMAC.HMAC_SHA512(key, data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string hmac_md5(string key, string data)
        {
            try
            {
                return ALG.HMAC.HMAC_MD5(key, data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }



        public string sha3_224(string data)
        {
            try
            {
                return ALG.SHA3.SHA3_224(data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sha3_256(string data)
        {
            try
            {
                return ALG.SHA3.SHA3_256(data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sha3_384(string data)
        {
            try
            {
                return ALG.SHA3.SHA3_384(data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sha3_512(string data)
        {
            try
            {
                return ALG.SHA3.SHA3_512(data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sha1(string data)
        {
            try
            {
                return ALG.Hash.HashSHA1(data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sha224(string data)
        {
            try
            {
                return ALG.Hash.HashSHA224(data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sha256(string data)
        {
            try
            {
                return ALG.Hash.HashSHA256(data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sha384(string data)
        {
            try
            {
                return ALG.Hash.HashSHA384(data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sha512(string data)
        {
            try
            {
                return ALG.Hash.HashSHA512(data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string md5(string data)
        {
            try
            {
                return ALG.Hash.MD5(data);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }



        public string aes_ecb_encrypt(string plaintext, string key)
        {
            try
            {
                return ALG.AES.AesECBEn(plaintext, key);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string aes_ecb_decrypt(string cipher, string key)
        {
            try
            {
                return ALG.AES.AesECBDe(cipher, key);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string aes_cbc_encrypt(string plaintext, string key, string icv = "00000000000000000000000000000000")
        {
            try
            {
                return ALG.AES.AesCBCEn(plaintext, key, icv);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string aes_cbc_decrypt(string cipher, string key, string icv = "00000000000000000000000000000000")
        {
            try
            {
                return ALG.AES.AesCBCDe(cipher, key, icv);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string des_ecb_encrypt(string plaintext, string key)
        {
            try
            {
                return ALG.DES.DesECBEn(plaintext, key);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string des_ecb_decrypt(string cipher, string key)
        {
            try
            {
                return ALG.DES.DesECBDe(cipher, key);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }
      

        public string des_cbc_encrypt(string plaintext, string key, string icv = "0000000000000000")
        {
            try
            {
                return ALG.DES.DesCBCEn(plaintext, key, icv);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string des_cbc_decrypt(string cipher, string key, string icv = "0000000000000000")
        {
            try
            {
                return ALG.DES.DesCBCDe(cipher, key, icv);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string des_mac(string plaintext, string key, string icv = "0000000000000000")
        {
            try
            {
                return ALG.DES.DesMAC(plaintext, key, icv);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string triple_des_mac(string plaintext, string key, string icv = "0000000000000000")
        {
            try
            {
                return ALG.DES.TriDesMAC(plaintext, key, icv);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string triple_des_ecb_encrypt(string plaintext, string key)
        {
            try
            {
                return ALG.DES.TriDesECBEn(plaintext, key);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string triple_des_ecb_decrypt(string cipher, string key)
        {
            try
            {
                return ALG.DES.TriDesECBDe(cipher, key);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string triple_des_cbc_encrypt(string plaintext, string key, string icv = "0000000000000000")
        {
            try
            {
                return ALG.DES.TriDesCBCEn(plaintext, key, icv);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string triple_des_cbc_decrypt(string cipher, string key, string icv = "0000000000000000")
        {
            try
            {
                return ALG.DES.TriDesCBCDe(cipher, key, icv);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string[] rsa_generate_key(int KeySize, string exponent)
        {
            try
            {
                ALG.RSA.GenKey(KeySize, exponent);

                string[] s = new string[7];
                s[0] = RSA.RSA_N;
                s[1] = RSA.RSA_D;
                s[2] = RSA.RSA_P;
                s[3] = RSA.RSA_Q;
                s[4] = RSA.RSA_DP;
                s[5] = RSA.RSA_DQ;
                s[6] = RSA.RSA_INVQ;
                
                return s;
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string rsa_encrypt(string data, string n, string e)
        {
            try
            {
                return ALG.RSA.RSAen(data, n, e);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string rsa_decrypt(string data, string n, string d)
        {
            try
            {
                return ALG.RSA.RSAde(data, n, d);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string rsa_crt_decrypt(string data, string p, string q, string dp, string dq, string invq)
        {
            try
            {
                return ALG.RSA.RSACRTde(data, p, q, dp, dq, invq);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string rsa_calc_e(string p, string dp)
        {
            try
            {
                RSA.RSA_E = null;
                ALG.RSA.GetE(p, dp);
                return RSA.RSA_E;
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string pkcs_1(string data, string n, string d)
        {
            try
            {
                return ALG.RSA.PKCS1(data, n, d);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sm3(string sPlain)
        {
            try
            {
                return ALG.GuoMi.SM3(sPlain);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string[] ecc_generate_key(string curve)
        {
            try
            {
                string[] s = ALG_ECC.Util.GenerateKeyPair(curve);
                return s;
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string[] ecc_calc_public_key(string privatekey, string curve)
        {
            try
            {
                string[] s = ALG_ECC.Util.GetPublicKey(privatekey, curve);
                return s;
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string[] ecdsa_sign(string e, string sPriKey, string curve)
        {
            try
            {
                return ALG_ECC.ECDSA.Sign(sPriKey, e, curve);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public bool ecdsa_verify(string e, string sRS_R, string sRS_S, string sPubKey_x, string sPubKey_y, string curve)
        {
            try
            {
                return ALG_ECC.ECDSA.Verify(sPubKey_x, sPubKey_y, e, sRS_R, sRS_S, curve);
            }
            catch (Exception ex)
            {
                error(ex.Message);
                return false;
            }
        }




        public string[] sm2_generate_key()
        {
            try
            {
                string[] s = ALG_ECC.Util.GenerateKeyPair("SM2");
                return s;
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string[] sm2_calc_public_key(string privatekey)
        {
            try
            {
                string[] s = ALG_ECC.Util.GetPublicKey(privatekey, "SM2");
                return s;// string.Join(",", s);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sm2_encrypt(string sPlaintext, string sPubkey_x, string sPubkey_y)
        {
            try
            {
                return ALG_ECC.SM2.Encrypt(sPubkey_x, sPubkey_y, sPlaintext);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sm2_decrypt(string sCiphertext, string sPrikey)
        {
            try
            {
                return ALG_ECC.SM2.Decrypt(sPrikey, sCiphertext);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }



        public string[] sm2_sign(string sPlain, string sPriKey, string id = "")
        {
            try
            {
                return ALG_ECC.SM2.Sign(sPriKey, sPlain, id);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public bool sm2_verify(string sPlain, string sRS_R, string sRS_S, string sPubKey_x, string sPubKey_y, string id = "")
        {
            try
            {
                return ALG_ECC.SM2.Verify(sPubKey_x, sPubKey_y, sPlain, sRS_R, sRS_S, id);
            }
            catch (Exception ex)
            {
                error(ex.Message);
                return false;
            }
        }


        public string sm4_cbc_encrypt(string input, string key, string iv = "00000000000000000000000000000000")
        {
            try
            {
                return ALG.GuoMi.SM4EncryptCBC(input, iv, key);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sm4_ecb_encrypt(string input, string key)
        {
            try
            {
                return ALG.GuoMi.SM4EncryptECB(input, key);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sm4_cbc_decrypt(string input, string key, string iv = "00000000000000000000000000000000")
        {
            try
            {
                return ALG.GuoMi.SM4DecryptCBC(input, iv, key);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sm4_ecb_decrypt(string input, string key)
        {
            try
            {
                return ALG.GuoMi.SM4DecryptECB(input, key);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string sm4_mac(string input, string key, string iv = "00000000000000000000000000000000")
        {
            try
            {
                return ALG.GuoMi.SM4MAC(input, iv, key);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }



        public string xor(string key, string data)
        {
            try
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

                return ConvertTool.Bytes2String(res);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string big_num_add(string a, string b, int radix)
        {
            try
            {
                a = ConvertTool.RemoveSpace(a);
                b = ConvertTool.RemoveSpace(b);

                BigInteger bignum_a = new BigInteger(a, radix);
                BigInteger bignum_b = new BigInteger(b, radix);
                return (bignum_a + bignum_b).ToString(radix);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string big_num_subtract(string a, string b, int radix)
        {
            try
            {
                a = ConvertTool.RemoveSpace(a);
                b = ConvertTool.RemoveSpace(b);

                BigInteger bignum_a = new BigInteger(a, radix);
                BigInteger bignum_b = new BigInteger(b, radix);
                return (bignum_a - bignum_b).ToString(radix);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }



        public int big_num_compare(string a, string b, int radix)
        {
            try
            {
                a = ConvertTool.RemoveSpace(a);
                b = ConvertTool.RemoveSpace(b);

                BigInteger bignum_a = new BigInteger(a, radix);
                BigInteger bignum_b = new BigInteger(b, radix);
                if (bignum_a == bignum_b)
                    return 0;
                else if (bignum_a > bignum_b)
                    return 1;
                else return -1;
            }
            catch (Exception ex)
            {
                error(ex.Message);
                return -1;
            }
        }


        //[NLuaFunction("rshift")]
        //public int RShift(int i, int j)
        //{
        //    try
        //    {
        //        return i >> j;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrPrint(ex.Message);
        //        return -1;
        //    }
        //}
        //[NLuaFunction("lshift")]
        //public int LShift(int i, int j)
        //{
        //    try
        //    {
        //        return i << j;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrPrint(ex.Message);
        //        return -1;
        //    }
        //}

        //[NLuaFunction("bit_xor")]
        //public int LuaXor(int i, int j)
        //{
        //    try
        //    {
        //        return i ^ j;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrPrint(ex.Message);
        //        return -1;
        //    }
        //}
        //[NLuaFunction("bit_and")]
        //public int LuaAnd(int i, int j)
        //{
        //    try
        //    {
        //        return i & j;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrPrint(ex.Message);
        //        return -1;
        //    }
        //}

        //[NLuaFunction("bit_or")]
        //public int LuaOr(int i, int j)
        //{
        //    try
        //    {
        //        return i | j;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrPrint(ex.Message);
        //        return -1;
        //    }
        //}



        public DateTime get_time()
        {
            return DateTime.Now;
        }


        public int time_subtract(DateTime t1, DateTime t2)
        {
            try
            {
                TimeSpan ts;
                if (t1 > t2)
                    ts = t1 - t2;
                else
                    ts = t2 - t1;
                return ts.Milliseconds;
            }
            catch (Exception ex)
            {
                error(ex.Message);
                return -1;
            }
        }



        /*public string get_dir(string dir, int level = 0)
        {
            try
            {
                while (level < 0)
                {
                    level++;
                    dir = Directory.GetParent(dir).FullName;
                }
                return dir;
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }*/




        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public void print(string objs)
        {
            try
            {
                MainWindow.print(objs);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }


        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public void error(string objs)
        {
            try
            {
                MainWindow.errPrint(error_prefix + objs);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }



        public string get_session_key_dek()
        {
            try
            {
                return MainWindow.gp.getSKdek();
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public void set_security_domain_key(string enc, string mac, string dek)
        {
            try
            {
                MainWindow.gp.setENC(enc);
                MainWindow.gp.setMAC(mac);
                MainWindow.gp.setDEK(dek);
            }
            catch (Exception)
            {
                error("Function set_security_domain_key Failed");
            }
        }


        public string initialize_update(string ver)
        {
            try
            {
                if (!isReset)
                {
                    throw new Exception("Please add function \"reset\" before initialize_update");
                }
                MainWindow.gp.initUpdate(ver);
                return MainWindow.gp.getResponse();
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string external_authenticate(string sl)
        {
            try
            {
                if (!isReset)
                {
                    throw new Exception("Please add function \"reset\" before external_authenticate");
                }
                MainWindow.gp.externalAuthenticate(sl);
                return MainWindow.gp.getResponse();
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string send_apdu_gp(string apdu)
        {
            try
            {
                if (!isReset)
                {
                    throw new Exception("Please add function \"reset\" before send_apdu_gp \"" + apdu + "\"");
                }
                MainWindow.gp.secApdu(apdu);
                return MainWindow.gp.getResponse();
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string send_apdu(string apdu, string expect)
        {
            return send_apdu(apdu, expect, "");
        }

        public string send_apdu(string apdu, string expect, string readerName = "")
        {
            try
            {
                string res;
                if (sReader1 == readerName && readerName != "")
                {
                    if (!isReset_gp1)
                    {
                        throw new Exception("Please add function \"reset\" before send_apdu \"" + apdu + "\"");
                    }
                    res = gp1.APDU(apdu, expect);
                }
                else if (sReader2 == readerName && readerName != "")
                {
                    if (!isReset_gp2)
                    {
                        throw new Exception("Please add function \"reset\" before send_apdu \"" + apdu + "\"");
                    }
                    res = gp2.APDU(apdu, expect);
                }
                else
                {
                    if (!isReset)
                    {
                        throw new Exception("Please add function \"reset\" before send_apdu \"" + apdu + "\"");
                    }
                    res = MainWindow.gp.APDU(apdu, expect);
                }
                return res;
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }

        public string send(string apdu, string expect)
        {
            return send(apdu, expect, "");
        }

        public string send(string apdu, string expect, string readerName = "")
        {
            try
            {
                string res;
                if (sReader1 == readerName && readerName != "")
                {
                    if (!isReset_gp1)
                    {
                        throw new Exception("Please add function \"reset\" before send \"" + apdu + "\"");
                    }
                    res = gp1.APDU(apdu, expect);
                }
                else if (sReader2 == readerName && readerName != "")
                {
                    if (!isReset_gp2)
                    {
                        throw new Exception("Please add function \"reset\" before send \"" + apdu + "\"");
                    }
                    res = gp2.APDU(apdu, expect);
                }
                else
                {
                    if (!isReset)
                    {
                        throw new Exception("Please add function \"reset\" before send \"" + apdu + "\"");
                    }
                    res = MainWindow.gp.APDU(apdu, expect);
                }
                return res;
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public gplib reader1(string readerName)
        {
            try
            {
                if (readerName == "")
                    return null;
                gp1 = new gplib(readerName, "", "", "", "0");
                gp1.setConsole(MainWindow.scriptBox);
                if (MainWindow.gp.getSctLog() != null)
                    gp1.setSctLog(MainWindow.gp.getSctLog());
                sReader1 = readerName;
                return gp1;
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public gplib reader2(string readerName)
        {
            try
            {
                if (readerName == "")
                    return null;
                gp2 = new gplib(readerName, "", "", "", "0");
                gp2.setConsole(MainWindow.scriptBox);
                if (MainWindow.gp.getSctLog() != null)
                    gp2.setSctLog(MainWindow.gp.getSctLog());
                sReader2 = readerName;
                return gp2;
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string reset()
        {
            return reset("");
        }

        public string reset(string readerName = "")
        {
            try
            {
                if (sReader1 == readerName && readerName != "")
                {
                    isReset_gp1 = true;
                    print("Reader: " + gp1.getReaderName());
                    gp1.Reset();
                    return gp1.getResponse();
                }
                else if (sReader2 == readerName && readerName != "")
                {
                    isReset_gp2 = true;
                    print("Reader: " + gp2.getReaderName());
                    gp2.Reset();
                    return gp2.getResponse();
                }
                else
                {
                    isReset = true;
                    print("Reader: " + MainWindow.gp.getReaderName());
                    MainWindow.gp.Reset();
                    return MainWindow.gp.getResponse();
                }
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }



        public string string2ascii(string str)
        {
            try
            {
                return xTool.ConvertTool.String2Ascii(str);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string ascii2string(string str)
        {
            try
            {
                return xTool.ConvertTool.Ascii2String(str);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string base64_encode(string str)
        {
            try
            {
                return xTool.ConvertTool.Base64Encode(str);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string base64_decode(string str)
        {
            try
            {
                return xTool.ConvertTool.Base64Decode(str);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string base58_encode(string str, bool b = false) //bitcoin monero
        {
            try
            {
                return ALG_ECC.Base58.Base58Encode_BitCoin(str, b);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string base58_decode(string str, bool b = false)
        {
            try
            {
                return ALG_ECC.Base58.Base58Decode_BitCoin(str, b);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string base58_encode_ripple(string str, bool b = false)
        {
            try
            {
                return ALG_ECC.Base58.Base58Encode_Ripple(str, b);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


        public string base58_decode_ripple(string str, bool b = false)
        {
            try
            {
                return ALG_ECC.Base58.Base58Decode_Ripple(str, b);
            }
            catch (Exception ex)
            {
                print(error_prefix + ex.Message);
                return null;
            }
        }


    }
}
