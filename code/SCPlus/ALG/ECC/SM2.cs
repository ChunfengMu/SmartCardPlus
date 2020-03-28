using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace ALG_ECC
{
    public class SM2
    {
        public static string[] Sign(string privateKey, string data, string id="")
        {
            privateKey = xTool.ConvertTool.RemoveSpace(privateKey);
            data = xTool.ConvertTool.RemoveSpace(data);
            id = xTool.ConvertTool.RemoveSpace(id);
           
            if (data.Length % 2 != 0)
                throw new Exception("Invalid Data");

            if (id.Length % 2 != 0)
                throw new Exception("Invalid ID");

            ECDSABase ecdsa = new ECDSABase(new ECCurve("SM2"));

            if (id == "" || id == null)
                id = "31323334353637383132333435363738";
            int idLen = id.Length * 4;
            string [] pubkey = Util.GetPublicKey(privateKey,"SM2");
            string idlen_string = idLen.ToString("X");
            while (idlen_string.Length<4)
                idlen_string = "0" + idlen_string;
            string tmp = idlen_string + id + Util.ToHexString(ECCurve.a, ECCurve.BitLength) + Util.ToHexString(ECCurve.b, ECCurve.BitLength) +
                Util.ToHexString(ECCurve.G.X, ECCurve.BitLength) + Util.ToHexString(ECCurve.G.Y, ECCurve.BitLength) + pubkey[0] + pubkey[1];           
            
            string Za = ALG.GuoMi.SM3(tmp);
            string e = ALG.GuoMi.SM3(Za + data);        
            
            BigInteger[] res = ecdsa.SignMessage_SM2(BigInteger.Parse("00" + privateKey, System.Globalization.NumberStyles.AllowHexSpecifier),
                BigInteger.Parse("00" + e, System.Globalization.NumberStyles.AllowHexSpecifier));
            string[] sign_rs = new string[2];
            sign_rs[0] = Util.ToHexString(res[0], ECCurve.BitLength);
            sign_rs[1] = Util.ToHexString(res[1], ECCurve.BitLength);

            return sign_rs;
        }

        public static bool Verify(string publicKey_x, string publicKey_y, string data, string rs_r, string rs_s ,string id="")
        {
            publicKey_x = xTool.ConvertTool.RemoveSpace(publicKey_x);
            publicKey_y = xTool.ConvertTool.RemoveSpace(publicKey_y);
            data = xTool.ConvertTool.RemoveSpace(data);
            rs_r = xTool.ConvertTool.RemoveSpace(rs_r);
            rs_s = xTool.ConvertTool.RemoveSpace(rs_s);
            id = xTool.ConvertTool.RemoveSpace(id);

            if (data.Length % 2 != 0)
                throw new Exception("Invalid Data");

            if (id.Length % 2 != 0)
                throw new Exception("Invalid ID");

            ECDSABase ecdsa = new ECDSABase(new ECCurve("SM2"));

            if (id == "" || id == null)
                id = "31323334353637383132333435363738";
            int idLen = id.Length * 4;           
            string idlen_string = idLen.ToString("X");
            while (idlen_string.Length < 4)
                idlen_string = "0" + idlen_string;
            string tmp = idlen_string + id + Util.ToHexString(ECCurve.a, ECCurve.BitLength) + Util.ToHexString(ECCurve.b, ECCurve.BitLength) +
                Util.ToHexString(ECCurve.G.X, ECCurve.BitLength) + Util.ToHexString(ECCurve.G.Y, ECCurve.BitLength) + publicKey_x + publicKey_y;

            string Za = ALG.GuoMi.SM3(tmp);
            string e = ALG.GuoMi.SM3(Za + data);
                  
            BigInteger[] res = new BigInteger[2];
            int Length = ECCurve.BitLength / 4;
            res[0] = BigInteger.Parse("00" + rs_r, System.Globalization.NumberStyles.AllowHexSpecifier);
            res[1] = BigInteger.Parse("00" + rs_s, System.Globalization.NumberStyles.AllowHexSpecifier);

            BigInteger x = BigInteger.Parse("00" + publicKey_x, System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger y = BigInteger.Parse("00" + publicKey_y, System.Globalization.NumberStyles.AllowHexSpecifier);

            ECPoint pubKey = new ECPoint(x, y);
            bool b = ecdsa.VerifySignature_SM2(pubKey,
            BigInteger.Parse("00" + e, System.Globalization.NumberStyles.AllowHexSpecifier),
            res);
            return b;
        }

        public static string Encrypt(string publicKey_x, string publicKey_y, string data)
        {
            publicKey_x = xTool.ConvertTool.RemoveSpace(publicKey_x);
            publicKey_y = xTool.ConvertTool.RemoveSpace(publicKey_y);
            data = xTool.ConvertTool.RemoveSpace(data);

            if (data.Length % 2 != 0)
                throw new Exception("Invalid Data");

            ECDSABase ecdsa = new ECDSABase(new ECCurve("SM2"));

            string c1, c2, c3, t;
            c1 = c2 = c3 = t = "";
            string x2, y2;
            x2 = y2 = "";

            //publicKey_x = "435B39CCA8F3B508C1488AFC67BE491A0F7BA07E581A0E4849A5CF70628A7E0A";
            //publicKey_y = "75DDBA78F15FEECB4C7895E2C1CDF5FE01DEBB2CDBADF45399CCF77BBA076A42";

            BigInteger x = BigInteger.Parse("00" + publicKey_x, System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger y = BigInteger.Parse("00" + publicKey_y, System.Globalization.NumberStyles.AllowHexSpecifier);
            ECPoint pubkey = new ECPoint(x, y);
            bool isContinue = true;
            while (isContinue)
            {
                //BigInteger k = BigInteger.Parse("004C62EEFD6ECFC2B95B92FD6C3D9575148AFA17425546D49018E5388D49DD7B4F", System.Globalization.NumberStyles.AllowHexSpecifier);
                //data = "656E6372797074696F6E207374616E64617264";
                BigInteger k = ECCMath.RandomIntegerBelow(ECCurve.n);                
                ECPoint point1 = ECCMath.ScalarMult(k, ECCurve.G);
                c1 = Util.ToHexString(point1.X, ECCurve.BitLength) + Util.ToHexString(point1.Y, ECCurve.BitLength);
               
                ECPoint S = ECCMath.ScalarMult(ECCurve.h, pubkey);
                if (ECCMath.IsInfinityPoint(S))
                    throw new Exception("S is Infinity Point");

                ECPoint point2 = ECCMath.ScalarMult(k, pubkey);
                x2 = Util.ToHexString(point2.X, ECCurve.BitLength);
                y2 = Util.ToHexString(point2.Y, ECCurve.BitLength);
                
                t = KDF( x2 + y2 ,data.Length*4);
                for (int i = 0; i < t.Length; i++)
                {
                    if (t.Substring(i, 1) != "0")
                    {
                        isContinue = false;
                        break;
                    }
                }
            }
            c2 = XOR(data,t);
            c3 = ALG.GuoMi.SM3(x2 + data + y2);
            return c1 + c3 + c2;
        }

        public static string Decrypt(string privateKey, string data)
        {
            privateKey = xTool.ConvertTool.RemoveSpace(privateKey);
            data = xTool.ConvertTool.RemoveSpace(data);

            if (data.Length % 2 != 0)
                throw new Exception("Invalid Cipher");

            if (data.Length < 96 * 2)
                throw new Exception("Invalid Cipher");

            ECDSABase ecdsa = new ECDSABase(new ECCurve("SM2"));

            string c1, c2, c3, t;
            c1 = data.Substring(0, 128);
            c3 = data.Substring(128, 64);
            c2 = data.Substring(128+64);
            t = "";
            string x2, y2;
            x2 = y2 = "";
            bool isZero = true;

            BigInteger x = BigInteger.Parse("00" + c1.Substring(0,64), System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger y = BigInteger.Parse("00" + c1.Substring(64), System.Globalization.NumberStyles.AllowHexSpecifier);
            ECPoint C1 = new ECPoint(x, y);
            if (!ECCMath.IsOnCurve(C1))
                throw new Exception("SM2 Decrypt Failed, C1 Not On Curve");

            ECPoint S = ECCMath.ScalarMult(ECCurve.h, C1);
            if (ECCMath.IsInfinityPoint(S))
                throw new Exception("S is Infinity Point");

            BigInteger prikey = BigInteger.Parse("00" + privateKey, System.Globalization.NumberStyles.AllowHexSpecifier);

            ECPoint point2 = ECCMath.ScalarMult(prikey, C1);

            x2 = Util.ToHexString(point2.X, ECCurve.BitLength);
            y2 = Util.ToHexString(point2.Y, ECCurve.BitLength);

            t = KDF(x2 + y2, c2.Length * 4);

            for (int i = 0; i < t.Length; i++)
            {
                if (t.Substring(i, 1) == "0")
                {
                    continue;
                }
                else
                {
                    isZero = false;
                    break;
                }
            }

            if (isZero)
                throw new Exception("t = 0");

            string plaindata = XOR(c2, t);

            string u = ALG.GuoMi.SM3(x2 + plaindata + y2);

            if(!u.Equals(c3,StringComparison.OrdinalIgnoreCase))
                throw new Exception("SM2 Decrypt Failed, u != C3");

            return plaindata;
        }

        private static string XOR(string data1, string data2)
        {
            if (data1.Length % 2 != 0)
                data1 = "0" + data1;
            if (data2.Length % 2 != 0)
                data2 = "0" + data2;

            while (data1.Length < data2.Length)
                data1 = "00" + data1;
            while (data1.Length > data2.Length)
                data2 = "00" + data2;

            byte[] bdata1 = xTool.ConvertTool.String2Bytes(data1);
            byte[] bdata2 = xTool.ConvertTool.String2Bytes(data2);
            byte[] res = new byte[bdata1.Length];
            for (int i = 0; i < bdata1.Length; i++)
                res[i] = (byte)(bdata1[i] ^ bdata2[i]);

            return xTool.ConvertTool.Bytes2String(res);
        }

        private static string KDF(string Z, int bitlen)
        {
            uint ct = 1;
            uint keyLen = (uint)(bitlen / 8);
            uint hashlen = 256/8;
            string res = "";
            for (uint i = 1; i < keyLen / hashlen; i++)
            {              
                res += ALG.GuoMi.SM3(Z + xTool.ConvertTool.Bytes2String(UInt32_To_Bytes(ct)));
                ct++;
            }

            if (keyLen % hashlen == 0)
            {
                res += ALG.GuoMi.SM3(Z + xTool.ConvertTool.Bytes2String(UInt32_To_Bytes(ct)));
            }
            else
            {
                string temp = ALG.GuoMi.SM3(Z + xTool.ConvertTool.Bytes2String(UInt32_To_Bytes(ct)));
                res += temp.Substring(0, (int)(2*(keyLen % hashlen)));
            }
            return res;
        }
        internal static byte[] UInt32_To_Bytes(uint n)
        {
            byte[] bs = new byte[4];
            bs[0] = (byte)(n >> 24);
            bs[1] = (byte)(n >> 16);
            bs[2] = (byte)(n >> 8);
            bs[3] = (byte)(n);
            return bs;
        }
    }
}
