using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace ALG_ECC
{
    public class ECDSA
    {
        public static string[] Sign(string privateKey, string m_hash, string curve = "")
        {
            privateKey = xTool.ConvertTool.RemoveSpace(privateKey);
            m_hash = xTool.ConvertTool.RemoveSpace(m_hash);

            ECDSABase ecdsa = new ECDSABase(new ECCurve(curve));
            BigInteger[] res = ecdsa.SignMessage(BigInteger.Parse("00" + privateKey, System.Globalization.NumberStyles.AllowHexSpecifier),
                BigInteger.Parse("00" + m_hash, System.Globalization.NumberStyles.AllowHexSpecifier));
            string[] tmp = new string[2];
            tmp[0] = Util.ToHexString(res[0], ECCurve.BitLength);
            tmp[1] = Util.ToHexString(res[1], ECCurve.BitLength);

            return tmp;
        }



        public static bool Verify(string publicKey_x, string publicKey_y, string m_hash, string rs_r, string rs_s, string curve = "")
        {
            publicKey_x = xTool.ConvertTool.RemoveSpace(publicKey_x);
            publicKey_y = xTool.ConvertTool.RemoveSpace(publicKey_y);
            m_hash = xTool.ConvertTool.RemoveSpace(m_hash);
            rs_r = xTool.ConvertTool.RemoveSpace(rs_r);
            rs_s = xTool.ConvertTool.RemoveSpace(rs_s);

            ECDSABase ecdsa = new ECDSABase(new ECCurve(curve));
            BigInteger[] res = new BigInteger[2];
            int Length = ECCurve.BitLength / 4;
            res[0] = BigInteger.Parse("00" + rs_r, System.Globalization.NumberStyles.AllowHexSpecifier);
            res[1] = BigInteger.Parse("00" + rs_s, System.Globalization.NumberStyles.AllowHexSpecifier);

            BigInteger x = BigInteger.Parse("00" + publicKey_x, System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger y = BigInteger.Parse("00" + publicKey_y, System.Globalization.NumberStyles.AllowHexSpecifier);

            ECPoint pubKey = new ECPoint(x, y);
            bool b = ecdsa.VerifySignature(pubKey,
            BigInteger.Parse("00" + m_hash, System.Globalization.NumberStyles.AllowHexSpecifier),
            res);
            return b;
        }
    }      
}
