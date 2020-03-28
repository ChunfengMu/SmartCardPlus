using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace ALG_ECC
{
    public class Util
    {
        public static string ToHexString(BigInteger bignum, int BitLength)
        {
            byte[] data = bignum.ToByteArray();
            int dataLength = data.Length;
            string result = "";

            for (int i = dataLength - 1; i >= 0; i--)
            {
                result += data[i].ToString("X2");
            }

            while (result.Length < BitLength / 4)
                result = "0" + result;

            while (result.Length > BitLength / 4)
                result = result.Substring(2);

            return result;
        }

        public static string[] GetPublicKey(string privatekey, string curve = "")
        {
            privatekey = xTool.ConvertTool.RemoveSpace(privatekey);

            ECDSABase ecdsa = new ECDSABase(new ECCurve(curve));
            BigInteger PrivateKey = BigInteger.Parse("00" + privatekey, System.Globalization.NumberStyles.AllowHexSpecifier);

            if (!ECCMath.RangeBetween(PrivateKey, 1, ECCurve.n - 1))
                throw new Exception("Invalid Private Key");
            var PublicKey = ECCMath.ScalarMult(PrivateKey, ECCurve.G);
            string[] key = new string[2];

            key[0] = Util.ToHexString(PublicKey.X, ECCurve.BitLength);
            key[1] = Util.ToHexString(PublicKey.Y, ECCurve.BitLength);

            return key;
        }

        public static string[] GenerateKeyPair(string curve = "")
        {
            ECDSABase ecdsa = new ECDSABase(new ECCurve(curve));
            ECKey key = ecdsa.GenerateKeyPair();
            string[] keyPair = new string[3];

            keyPair[0] = Util.ToHexString(key.PrivateKey, ECCurve.BitLength);
            keyPair[1] = Util.ToHexString(key.PublicKey.X, ECCurve.BitLength);
            keyPair[2] = Util.ToHexString(key.PublicKey.Y, ECCurve.BitLength);
            return keyPair;
        }


        public static bool VerifyPublickey(string pub_x, string pub_y, string curve = "")
        {
            pub_x = xTool.ConvertTool.RemoveSpace(pub_x);
            pub_y = xTool.ConvertTool.RemoveSpace(pub_y);

            ECDSABase ecdsa = new ECDSABase(new ECCurve(curve));
            BigInteger x = BigInteger.Parse("00" + pub_x, System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger y = BigInteger.Parse("00" + pub_y, System.Globalization.NumberStyles.AllowHexSpecifier);

            ECPoint p = new ECPoint(x, y);

            if (ECCMath.IsInfinityPoint(p))
                return false;

            if (!ECCMath.RangeBetween(x, BigInteger.One, ECCurve.p-1))
                return false;
            if (!ECCMath.RangeBetween(y, BigInteger.One, ECCurve.p - 1))
                return false;

            if (!ECCMath.IsOnCurve(p))
                return false;

            ECPoint O = ECCMath.ScalarMult(ECCurve.n, p);

            if (!ECCMath.IsInfinityPoint(O))
                return false;
                //throw new Exception("O is NOT Infinity Point");

            return true;
        }
    }
}
