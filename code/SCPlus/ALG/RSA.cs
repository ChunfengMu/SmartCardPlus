using System;
using System.Security.Cryptography;
using xTool;

namespace ALG
{
    public class RSA
    {
        public static string RSA_D;
        public static string RSA_N;
        public static string RSA_E;
        public static string RSA_DP;
        public static string RSA_DQ;
        public static string RSA_INVQ;
        public static string RSA_Q;
        public static string RSA_P;

        public static void GenKey(int dwKeySize, string exponent)
        {
            string info = "key sizes from 16 bits to 16384 bits in increments of 8 bits.";
            exponent = ConvertTool.RemoveSpace(exponent);
            
            int count = 0;
            if (dwKeySize % 8 != 0 || dwKeySize < 16)
                throw new ArgumentException(info);

            BigInteger e = 0, n = 0, d = 0, p = 0, q = 0, dp = 0, dq = 0, invq = 0;

            label1:
            count++;
            if (count > 20)
                throw new ArgumentException("change E or bits.");
            e = new BigInteger(exponent, 16);

            if (dwKeySize < 384)
            {
                Random rand = new Random();
                p = BigInteger.genPseudoPrime(dwKeySize / 2, 3, rand);
                q = BigInteger.genPseudoPrime(dwKeySize / 2, 3, rand);
                n = p * q;
            }
            else
            {
                //只支持长度从 384 位至 16384 位（增量为 8 位）的密钥          
                var rsa = new RSACryptoServiceProvider(dwKeySize);
                RSAParameters temp = rsa.ExportParameters(true);
                n = new BigInteger(temp.Modulus);
                p = new BigInteger(temp.P);
                q = new BigInteger(temp.Q);
            }


            BigInteger oula = (p - 1) * (q - 1);
            try
            {
                d = e.modInverse(oula);
                dp = d % (p - 1);
                dq = d % (q - 1);

                invq = q.modInverse(p);
            }
            catch (Exception ex)
            {
                if(ex.Message != info)
                    goto label1;
            }

            RSA_D = d.ToHexString();
            RSA_N = n.ToHexString();
            RSA_E = e.ToHexString();
            RSA_P = p.ToHexString();
            RSA_Q = q.ToHexString();
            RSA_DP = dp.ToHexString();
            RSA_DQ = dq.ToHexString();
            RSA_INVQ = invq.ToHexString();

        }

        public static void GetE(string sp, string sdp)
        {
            sp = ConvertTool.RemoveSpace(sp);
            sdp = ConvertTool.RemoveSpace(sdp);

            BigInteger e, p, dp;
            p = new BigInteger(sp, 16);
            dp = new BigInteger(sdp, 16);
            e = dp.modInverse(p - 1);
            //e = temp % (p - 1);
            RSA_E = e.ToHexString();
        }

        public static void GenPQKey(string sp, string sq, string exponent)
        {
            sp = ConvertTool.RemoveSpace(sp);
            sq = ConvertTool.RemoveSpace(sq);
            exponent = ConvertTool.RemoveSpace(exponent);
  
            BigInteger e, n, d, p, q, dp, dq, invq;

            p = new BigInteger(sp, 16);
            q = new BigInteger(sq, 16);
            e = new BigInteger(exponent, 16);
            n = p * q;

            BigInteger oula = (p - 1) * (q - 1);
            try
            {
                d = e.modInverse(oula);
                dp = d % (p - 1);
                dq = d % (q - 1);
                invq = q.modInverse(p);

                RSA_D = d.ToHexString();
                RSA_N = n.ToHexString();
                RSA_E = e.ToHexString();
                RSA_P = p.ToHexString();
                RSA_Q = q.ToHexString();
                RSA_DP = dp.ToHexString();
                RSA_DQ = dq.ToHexString();
                RSA_INVQ = invq.ToHexString();
            }
            catch (Exception)
            {
                throw new ArgumentException("e and φ(n) are not coprime, change e or p&q.");
            }
        }

        public static string Encrypt(string source, string n, string d)
        {
            source = ConvertTool.RemoveSpace(source);
            n = ConvertTool.RemoveSpace(n);
            d = ConvertTool.RemoveSpace(d);

            BigInteger D = new BigInteger(ConvertTool.String2Bytes(d));
            BigInteger N = new BigInteger(ConvertTool.String2Bytes(n));
            BigInteger Data = new BigInteger(ConvertTool.String2Bytes(source));

            BigInteger biText = new BigInteger(Data);
            BigInteger biEnText = biText.modPow(D, N);
            return biEnText.ToHexString();

        }

        public static string RSAen(string data, string n, string e)
        {
            return Encrypt(data, n, e);
        }

        public static string RSAde(string data, string n, string d)
        {
            return Encrypt(data, n, d);
        }

        public static string RSACRTde(string data, string p, string q, string dp, string dq, string invq)
        {
            data = ConvertTool.RemoveSpace(data);
            p = ConvertTool.RemoveSpace(p);
            q = ConvertTool.RemoveSpace(q);
            dp = ConvertTool.RemoveSpace(dp);
            dq = ConvertTool.RemoveSpace(dq);
            invq = ConvertTool.RemoveSpace(invq);

            BigInteger cipher = new BigInteger(ConvertTool.String2Bytes(data));
            BigInteger rsaP = new BigInteger(ConvertTool.String2Bytes(p));
            BigInteger rsaQ = new BigInteger(ConvertTool.String2Bytes(q));
            BigInteger rsaDP = new BigInteger(ConvertTool.String2Bytes(dp));
            BigInteger rsaDQ = new BigInteger(ConvertTool.String2Bytes(dq));
            BigInteger rsaINVQ = new BigInteger(ConvertTool.String2Bytes(invq));
            BigInteger m, m1, m2, h;
            m1 = cipher.modPow(rsaDP, rsaP);
            m2 = cipher.modPow(rsaDQ, rsaQ);
            var temp = m1 - m2;
            while (temp < 0)
                temp += rsaP;
            h = (rsaINVQ * temp) % rsaP;
            m = m2 + (h * rsaQ);

            return m.ToHexString();

        }

        public static string PKCS1(string indata, string n, string d)
        {
            indata = ConvertTool.RemoveSpace(indata);
            n = ConvertTool.RemoveSpace(n);
            d = ConvertTool.RemoveSpace(d);
            string data = "0001";
            //FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF ;
            while (data.Length < (n.Length - 72))
                data += "FF";
            data = data + "003021300906052B0E03021A05000414" + Hash.HashSHA1(indata);
            return RSA.RSAde(data, n, d);

        }

        
    }
}
