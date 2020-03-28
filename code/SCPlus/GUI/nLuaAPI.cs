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
using System.Windows;


namespace nLuaFramework
{
    class NLuaAPI
    {
        private string sReader1 = "";
        private string sReader2 = "";
        public static gplib gp1 = null;
        public static gplib gp2 = null;

        private bool isReset = false;
        private bool isReset_gp1 = false;
        private bool isReset_gp2 = false;

        private const string error_prefix = "ERROR, ";

        [NLuaFunction("hmac_sha1")]
        public string Hmac_sha1(string key, string data)
        {
            try
            {
                return ALG.HMAC.HMAC_SHA1(key, data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("hmac_sha224")]
        public string Hmac_sha224(string key, string data)
        {
            try
            {
                return ALG.HMAC.HMAC_SHA224(key, data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("hmac_sha256")]
        public string Hmac_sha256(string key, string data)
        {
            try
            {
                return ALG.HMAC.HMAC_SHA256(key, data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("hmac_sha384")]
        public string Hmac_sha384(string key, string data)
        {
            try
            {
                return ALG.HMAC.HMAC_SHA384(key, data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("hmac_sha512")]
        public string Hmac_sha512(string key, string data)
        {
            try
            {
                return ALG.HMAC.HMAC_SHA512(key, data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("hmac_md5")]
        public string Hmac_md5(string key, string data)
        {
            try
            {
                return ALG.HMAC.HMAC_MD5(key, data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }


        [NLuaFunction("sha3_224")]
        public string Sha3_224(string data)
        {
            try
            {
                return ALG.SHA3.SHA3_224(data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sha3_256")]
        public string Sha3_256(string data)
        {
            try
            {
                return ALG.SHA3.SHA3_256(data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sha3_384")]
        public string Sha3_384(string data)
        {
            try
            {
                return ALG.SHA3.SHA3_384(data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sha3_512")]
        public string Sha3_512(string data)
        {
            try
            {
                return ALG.SHA3.SHA3_512(data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sha1")]
        public string Sha1(string data)
        {
            try
            {
                return ALG.Hash.HashSHA1(data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sha224")]
        public string Sha224(string data)
        {
            try
            {
                return ALG.Hash.HashSHA224(data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sha256")]
        public string Sha256(string data)
        {
            try
            {
                return ALG.Hash.HashSHA256(data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sha384")]
        public string Sha384(string data)
        {
            try
            {
                return ALG.Hash.HashSHA384(data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sha512")]
        public string Sha512(string data)
        {
            try
            {
                return ALG.Hash.HashSHA512(data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("md5")]
        public string Md5(string data)
        {
            try
            {
                return ALG.Hash.MD5(data);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }


        [NLuaFunction("aes_ecb_encrypt")]
        public string AesECBEn(string plaintext, string key)
        {
            try
            {
                return ALG.AES.AesECBEn(plaintext, key);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("aes_ecb_decrypt")]
        public string AesECBde(string cipher, string key)
        {
            try
            {
                return ALG.AES.AesECBDe(cipher, key);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("aes_cbc_encrypt")]
        public string AesCBCEn(string plaintext, string key, string icv = "00000000000000000000000000000000")
        {
            try
            {
                return ALG.AES.AesCBCEn(plaintext, key, icv);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("aes_cbc_decrypt")]
        public string AesCBCde(string cipher, string key, string icv = "00000000000000000000000000000000")
        {
            try
            {
                return ALG.AES.AesCBCDe(cipher, key, icv);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("des_ecb_encrypt")]
        public string DesECBEn(string plaintext, string key)
        {
            try
            {
                return ALG.DES.DesECBEn(plaintext, key);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("des_ecb_decrypt")]
        public string DesECBde(string cipher, string key)
        {
            try
            {
                return ALG.DES.DesECBDe(cipher, key);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("des_cbc_encrypt")]
        public string DesCBCEn(string plaintext, string key, string icv = "0000000000000000")
        {
            try
            {
                return ALG.DES.DesCBCEn(plaintext, key, icv);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("des_cbc_decrypt")]
        public string DesCBCde(string cipher, string key, string icv = "0000000000000000")
        {
            try
            {
                return ALG.DES.DesCBCDe(cipher, key, icv);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("des_mac")]
        public string DesMAC(string plaintext, string key, string icv = "0000000000000000")
        {
            try
            {
                return ALG.DES.DesMAC(plaintext, key, icv);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("triple_des_mac")]
        public string TriDesMAC(string plaintext, string key, string icv = "0000000000000000")
        {
            try
            {
                return ALG.DES.TriDesMAC(plaintext, key, icv);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("triple_des_ecb_encrypt")]
        public string TriDesECBen(string plaintext, string key)
        {
            try
            {
                return ALG.DES.TriDesECBEn(plaintext, key);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("triple_des_ecb_decrypt")]
        public string TriDesECBde(string cipher, string key)
        {
            try
            {
                return ALG.DES.TriDesECBDe(cipher, key);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("triple_des_cbc_encrypt")]
        public string TriDesCBCen(string plaintext, string key, string icv = "0000000000000000")
        {
            try
            {
                return ALG.DES.TriDesCBCEn(plaintext, key, icv);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("triple_des_cbc_decrypt")]
        public string TriDesCBCde(string cipher, string key, string icv = "0000000000000000")
        {
            try
            {
                return ALG.DES.TriDesCBCDe(cipher, key, icv);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("rsa_generate_key")]
        public string[] RSA_Gen(int KeySize, string exponent)
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
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("rsa_encrypt")]
        public string RSAen(string data, string n, string e)
        {
            try
            {
                return ALG.RSA.RSAen(data, n, e);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("rsa_decrypt")]
        public string RSAde(string data, string n, string d)
        {
            try
            {
                return ALG.RSA.RSAde(data, n, d);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("rsa_crt_decrypt")]
        public string RSACRTde(string data, string p, string q, string dp, string dq, string invq)
        {
            try
            {
                return ALG.RSA.RSACRTde(data, p, q, dp, dq, invq);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("rsa_calc_e")]
        public string RSA_calc_e(string p, string dp)
        {
            try
            {
                RSA.RSA_E = null;
                ALG.RSA.GetE(p, dp);
                return RSA.RSA_E;
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("pkcs_1")]
        public string PKCS_1(string data, string n, string d)
        {
            try
            {
                return ALG.RSA.PKCS1(data, n, d);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sm3")]
        public string Sm3(string sPlain)
        {
            try
            {
                return ALG.GuoMi.SM3(sPlain);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("ecc_generate_key")]
        public string[] Ecc_Gen(string curve)
        {
            try
            {
                string[] s = ALG_ECC.Util.GenerateKeyPair(curve);
                return s;
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("ecc_calc_public_key")]
        public string[] Ecc_Get_PubliccKey(string privatekey, string curve)
        {
            try
            {
                string[] s = ALG_ECC.Util.GetPublicKey(privatekey, curve);
                return s;
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("ecdsa_sign")]
        public string[] EcdsaSign(string e, string sPriKey, string curve)
        {
            try
            {
                return ALG_ECC.ECDSA.Sign(sPriKey, e, curve);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("ecdsa_verify")]
        public bool EcdsaVerify(string e, string sRS_R, string sRS_S, string sPubKey_x, string sPubKey_y, string curve)
        {
            try
            {
                return ALG_ECC.ECDSA.Verify(sPubKey_x, sPubKey_y, e, sRS_R, sRS_S, curve);
            }
            catch (Exception ex)
            {
                ErrPrint(ex.Message);
                return false;
            }
        }



        [NLuaFunction("sm2_generate_key")]
        public string[] Sm2_Gen()
        {
            try
            {
                string[] s = ALG_ECC.Util.GenerateKeyPair("SM2");              
                return s;
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sm2_calc_public_key")]
        public string[] SM2_Get_PubliccKey(string privatekey)
        {
            try
            {
                string[] s = ALG_ECC.Util.GetPublicKey(privatekey, "SM2");
                return s;
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sm2_encrypt")]
        public string Sm2EncC1C3C2(string sPlaintext, string sPubkey_x, string sPubkey_y)
        {
            try
            {
                return ALG_ECC.SM2.Encrypt(sPubkey_x, sPubkey_y, sPlaintext);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sm2_decrypt")]
        public string Sm2DeC1C3C2(string sCiphertext, string sPrikey)
        {
            try
            {
                return ALG_ECC.SM2.Decrypt(sPrikey, sCiphertext);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }


        [NLuaFunction("sm2_sign")]
        public string[] Sm2Sign(string sPlain, string sPriKey, string id="")
        {
            try
            {
                return ALG_ECC.SM2.Sign(sPriKey, sPlain, id);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sm2_verify")]
        public bool Sm2Verify(string sPlain, string sRS_R, string sRS_S, string sPubKey_x, string sPubKey_y, string id ="")
        {
            try
            {
                return ALG_ECC.SM2.Verify(sPubKey_x, sPubKey_y, sPlain, sRS_R, sRS_S, id);
            }
            catch (Exception ex)
            {
                ErrPrint(ex.Message);
                return false;
            }
        }

        [NLuaFunction("sm4_cbc_encrypt")]
        public string Sm4EncryptCBC(string input, string key, string iv = "00000000000000000000000000000000")
        {
            try
            {
                return ALG.GuoMi.SM4EncryptCBC(input, iv, key);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sm4_ecb_encrypt")]
        public string Sm4EncryptECB(string input, string key)
        {
            try
            {
                return ALG.GuoMi.SM4EncryptECB(input, key);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sm4_cbc_decrypt")]
        public string Sm4DecryptCBC(string input, string key, string iv = "00000000000000000000000000000000")
        {
            try
            {
                return ALG.GuoMi.SM4DecryptCBC(input, iv, key);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sm4_ecb_decrypt")]
        public string Sm4DecryptECB(string input, string key)
        {
            try
            {
                return ALG.GuoMi.SM4DecryptECB(input, key);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("sm4_mac")]
        public string Sm4MAC(string input, string key, string iv = "00000000000000000000000000000000")
        {
            try
            {
                return ALG.GuoMi.SM4MAC(input, iv, key);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }


        [NLuaFunction("xor")]
        public string LuaXors(string key, string data)
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
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("big_num_add")]
        public string Bignum_add(string a, string b, int radix)
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
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("big_num_subtract")]
        public string Bignum_subtract(string a, string b, int radix)
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
                Print(error_prefix + ex.Message);
                return null;
            }
        }


        [NLuaFunction("big_num_compare")]
        public int Bignum_compare(string a, string b, int radix)
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
                ErrPrint(ex.Message);
                return -1;
            }
        }

        [NLuaFunction("rshift")]
        public int RShift(int i, int j)
        {
            try
            {
                return i >> j;
            }
            catch (Exception ex)
            {
                ErrPrint(ex.Message);
                return -1;
            }
        }
        [NLuaFunction("lshift")]
        public int LShift(int i, int j)
        {
            try
            {
                return i << j;
            }
            catch (Exception ex)
            {
                ErrPrint(ex.Message);
                return -1;
            }
        }

        [NLuaFunction("bit_xor")]
        public int LuaXor(int i, int j)
        {
            try
            {
                return i ^ j;
            }
            catch (Exception ex)
            {
                ErrPrint(ex.Message);
                return -1;
            }
        }
        [NLuaFunction("bit_and")]
        public int LuaAnd(int i, int j)
        {
            try
            {
                return i & j;
            }
            catch (Exception ex)
            {
                ErrPrint(ex.Message);
                return -1;
            }
        }

        [NLuaFunction("bit_or")]
        public int LuaOr(int i, int j)
        {
            try
            {
                return i | j;
            }
            catch (Exception ex)
            {
                ErrPrint(ex.Message);
                return -1;
            }
        }


        [NLuaFunction("get_time")]
        public DateTime GetTime()
        {
            return DateTime.Now;
        }

        [NLuaFunction("time_subtract")]
        public int TimeSubtract(DateTime t1, DateTime t2)
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
                ErrPrint(ex.Message);
                return -1;
            }
        }
        

        [NLuaFunction("get_dir")]
        public string GetParentDir(string dir, int level=0)
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
                Print(error_prefix + ex.Message);
                return null;
            }
        }



        [NLuaFunction("print")]
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public void Print(params object[] objs)
        {
            string s = "";
            try
            {
                if (objs == null)
                    s += "nil";
                else
                {
                    foreach (object obj in objs)
                    {
                        if (null == obj)
                            s += "nil";
                        else
                            s += obj.ToString();
                    }
                }
                
                MainWindow.print(s);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        [NLuaFunction("error")]
        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        public void ErrPrint(params object[] objs)
        {
            string s = "";
            try
            {
                if (objs == null)
                    s += "nil";
                else
                {
                    foreach (object obj in objs)
                    {
                        if (null == obj)
                            s += "nil";
                        else
                            s += obj.ToString();
                    }
                }
                MainWindow.errPrint(error_prefix + s);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }


        [NLuaFunction("get_session_key_dek")]
        public string GetSKdek()
        {
            try
            {
                return MainWindow.gp.getSKdek();
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("set_security_domain_key")]
        public void SetKey(string enc, string mac, string dek)
        {
            try
            {
                MainWindow.gp.setENC(enc);
                MainWindow.gp.setMAC(mac);
                MainWindow.gp.setDEK(dek);
            }
            catch (Exception)
            {
                ErrPrint("Function set_security_domain_key Failed");
            }
        }

        [NLuaFunction("initialize_update")]
        public string Init(string ver)
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
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("external_authenticate")]
        public string Ext(string sl)
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
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("send_apdu_gp")]
        public string SecApdu(string apdu)
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
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("send_apdu")]
        public string Send_apdu(string apdu, string expect, string readerName = "")
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
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("send")]
        public string Send(string apdu, string expect, string readerName = "")
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
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("reader1")]
        public gplib Reader1(string readerName)
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
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("reader2")]
        public gplib Reader2(string readerName)
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
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("reset")]
        public string Reset(string readerName = "")
        {
            try
            {            
                if (sReader1 == readerName && readerName != "")
                {
                    isReset_gp1 = true;
                    Print("Reader: " + gp1.getReaderName());
                    gp1.Reset();
                    return gp1.getResponse();
                }
                else if (sReader2 == readerName && readerName != "")
                {
                    isReset_gp2 = true;
                    Print("Reader: " + gp2.getReaderName());
                    gp2.Reset();
                    return gp2.getResponse();
                }
                else
                {
                    isReset = true;
                    Print("Reader: " + MainWindow.gp.getReaderName());
                    MainWindow.gp.Reset();
                    return MainWindow.gp.getResponse();
                }
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }


        [NLuaFunction("string2ascii")]
        public string String2Ascii(string str)
        {
            try
            {               
                return xTool.ConvertTool.String2Ascii(str);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("ascii2string")]
        public string Ascii2String(string str)
        {
            try
            {
                return xTool.ConvertTool.Ascii2String(str);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("base64_encode")]
        public string Base64_Encode(string str)
        {
            try
            {
                return xTool.ConvertTool.Base64Encode(str);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("base64_decode")]
        public string Base64_Decode(string str)
        {
            try
            {
                return xTool.ConvertTool.Base64Decode(str);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("base58_encode")]
        public string Base58_Encode(string str, bool b = false) //bitcoin monero
        {
            try
            {
                return ALG_ECC.Base58.Base58Encode_BitCoin(str, b);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("base58_decode")]
        public string Base58_Decode(string str, bool b = false)
        {
            try
            {
                return ALG_ECC.Base58.Base58Decode_BitCoin(str, b);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("base58_encode_ripple")]
        public string Base58_Encode_Ripple(string str, bool b = false)
        {
            try
            {
                return ALG_ECC.Base58.Base58Encode_Ripple(str, b);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        [NLuaFunction("base58_decode_ripple")]
        public string Base58_Decode_Ripple(string str, bool b = false)
        {
            try
            {
                return ALG_ECC.Base58.Base58Decode_Ripple(str, b);
            }
            catch (Exception ex)
            {
                Print(error_prefix + ex.Message);
                return null;
            }
        }

        /*

                //以下为旧API

                [nLuaFunction("getSKdek")]
                public string getSKdek_()
                {
                    return MainWindow.gp.getSKdek();          
                }

                [nLuaFunction("setKey")]
                public void setKey_(string enc, string mac, string dek)
                {
                    MainWindow.gp.setENC(enc);
                    MainWindow.gp.setMAC(mac);
                    MainWindow.gp.setDEK(dek);           
                }

                [nLuaFunction("init")]
                public string init_(string ver)
                {
                    MainWindow.gp.initUpdate(ver);
                    return MainWindow.gp.getResponse();
                }

                [nLuaFunction("ext")]
                public string ext_(string sl)
                {
                    MainWindow.gp.externalAuthenticate(sl);
                    return MainWindow.gp.getResponse();
                }

                [nLuaFunction("secApdu")]
                public string secApdu_(string apdu)
                {
                    MainWindow.gp.secApdu(apdu);
                    return MainWindow.gp.getResponse();
                }

                [nLuaFunction("sm2Enc")]
                public string sm2Enc_(string sPlaintext, string sPubkey)
                {
                    return ALG.SM.SM2Encrypt(sPlaintext, sPubkey);
                }

                [nLuaFunction("sm2De")]
                public string sm2De_(string sCiphertext, string sPrikey)
                {
                    return ALG.SM.SM2Decrypt(sCiphertext, sPrikey);
                }

                [nLuaFunction("sm2EncC1C3C2")]
                public string sm2EncC1C3C2_(string sPlaintext, string sPubkey)
                {
                    return ALG.SM.SM2EncryptC1C3C2(sPlaintext, sPubkey);
                }

                [nLuaFunction("sm2DeC1C3C2")]
                public string sm2DeC1C3C2_(string sCiphertext, string sPrikey)
                {
                    return ALG.SM.SM2DecryptC1C3C2(sCiphertext, sPrikey);
                }

                [nLuaFunction("sm2Sign")]
                public string sm2Sign_(string sPlain, string id, string sPubKey, string sPriKey)
                {
                    return ALG.SM.SM2Signature(sPlain, id, sPubKey, sPriKey);
                }

                [nLuaFunction("sm2Verify")]
                public bool sm2Verify_(string sPlain, string id, string sRS, string sPubKey)
                {
                    return ALG.SM.SM2Verify(sPlain, id, sRS, sPubKey);
                }


                [nLuaFunction("sm4EncryptCBC")]
                public string sm4EncryptCBC_(string input, string iv, string key)
                {
                    return ALG.SM.SM4EncryptCBC(input, iv, key);
                }

                [nLuaFunction("sm4EncryptECB")]
                public string sm4EncryptECB_(string input, string key)
                {
                    return ALG.SM.SM4EncryptECB(input, key);
                }

                [nLuaFunction("sm4DecryptCBC")]
                public string sm4DecryptCBC_(string input, string iv, string key)
                {
                    return ALG.SM.SM4DecryptCBC(input, iv, key);
                }

                [nLuaFunction("sm4DecryptECB")]
                public string sm4DecryptECB_(string input, string key)
                {
                    return ALG.SM.SM4DecryptECB(input, key);
                }

                [nLuaFunction("sm4MAC")]
                public string sm4MAC_(string input, string iv, string key)
                {
                    return ALG.SM.SM4MAC(input, iv, key);
                }


                [nLuaFunction("getDir")]
                public string getDir_()
                {
                    return LuaFramework.dir;
                }

                [nLuaFunction("aesECBen")]
                public string aesECBEn_(string plaintext, string key)
                {
                    return ALG.AES.AesECBEn(plaintext, key);
                }

                [nLuaFunction("aesECBde")]
                public string aesECBde_(string cipher, string key)
                {
                    return ALG.AES.AesECBDe(cipher, key);
                }

                [nLuaFunction("aesCBCen")]
                public string aesCBCEn_(string plaintext, string key, string icv = "00000000000000000000000000000000")
                {
                    return ALG.AES.AesCBCEn(plaintext, key, icv);
                }

                [nLuaFunction("aesCBCde")]
                public string aesCBCde_(string cipher, string key, string icv = "00000000000000000000000000000000")
                {
                    return ALG.AES.AesCBCDe(cipher, key, icv);
                }

                [nLuaFunction("desECBen")]
                public string desECBEn_(string plaintext, string key)
                {
                    return ALG.DES.DesECBEn(plaintext, key);
                }

                [nLuaFunction("desECBde")]
                public string desECBde_(string cipher, string key)
                {
                    return ALG.DES.DesECBDe(cipher, key);
                }

                [nLuaFunction("desCBCen")]
                public string desCBCEn_(string plaintext, string key, string icv = "0000000000000000")
                {
                    return ALG.DES.DesCBCEn(plaintext, key, icv);
                }

                [nLuaFunction("desCBCde")]
                public string desCBCde_(string cipher, string key, string icv = "0000000000000000")
                {
                    return ALG.DES.DesCBCDe(cipher, key, icv);
                }

                [nLuaFunction("triDesMAC")]
                public string triDesMAC_(string plaintext, string key, string icv)
                {
                    return ALG.DES.TriDesMAC(plaintext, key, icv);
                }

                [nLuaFunction("triDesECBen")]
                public string triDesECBen_(string plaintext, string key)
                {
                    return ALG.DES.TriDesECBEn(plaintext, key);
                }

                [nLuaFunction("triDesECBde")]
                public string triDesECBde_(string cipher, string key)
                {
                    return ALG.DES.TriDesECBDe(cipher, key);
                }

                [nLuaFunction("triDesCBCen")]
                public string triDesCBCen_(string plaintext, string key, string icv = "0000000000000000")
                {
                    return ALG.DES.TriDesCBCEn(plaintext, key, icv);
                }

                [nLuaFunction("triDesCBCde")]
                public string triDesCBCde_(string cipher, string key, string icv = "0000000000000000")
                {
                    return ALG.DES.TriDesCBCDe(cipher, key, icv);
                }

                [nLuaFunction("RSAen")]
                public string RSAen_(string data, string n, string e)
                {
                    return ALG.RSA.RSAen(data, n, e);
                }

                [nLuaFunction("RSAde")]
                public string RSAde_(string data, string n, string d)
                {
                    return ALG.RSA.RSAde(data, n, d);
                }

                [nLuaFunction("RSACRTde")]
                public string RSACRTde_(string data, string p, string q, string dp, string dq, string invq)
                {
                    return ALG.RSA.RSACRTde(data, p, q, dp, dq, invq);
                }

                [nLuaFunction("bitxor")]
                public int luaXor_(int i, int j)
                {
                    return i ^ j;
                }
                [nLuaFunction("bitand")]
                public int luaAnd_(int i, int j)
                {
                    return i & j;
                }

                [nLuaFunction("bitor")]
                public int luaOr_(int i, int j)
                {
                    return i | j;
                }
        */
    }
}
