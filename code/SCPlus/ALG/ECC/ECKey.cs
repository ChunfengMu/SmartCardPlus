using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace ALG_ECC
{
    public class ECKey
    {
        public ECCurve Curve;
        public BigInteger PrivateKey;
        public ECPoint PublicKey;

        public ECKey(BigInteger pub, ECPoint point, ECCurve eCCurve = null)
        {
            PrivateKey = pub;
            PublicKey.X = point.X;
            PublicKey.Y = point.Y;
            Curve = eCCurve;
        }

        public ECKey(ECCurve eCCurve = null)
        {
            
            Curve = eCCurve;
            
            /*
            BigInteger P = BigInteger.Parse("00fffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger A = BigInteger.Parse("0000", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger B = BigInteger.Parse("0007", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger Gx = BigInteger.Parse("0079be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger Gy = BigInteger.Parse("00483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger N = BigInteger.Parse("00fffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141", System.Globalization.NumberStyles.AllowHexSpecifier);
            var H = 1;
             */

            //SM2
            /*
            BigInteger P = BigInteger.Parse("00FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFF", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger A = BigInteger.Parse("00FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFC", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger B = BigInteger.Parse("0028E9FA9E9D9F5E344D5A9E4BCF6509A7F39789F515AB8F92DDBCBD414D940E93", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger Gx = BigInteger.Parse("0032C4AE2C1F1981195F9904466A39C9948FE30BBFF2660BE1715A4589334C74C7", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger Gy = BigInteger.Parse("00BC3736A2F4F6779C59BDCEE36B692153D0A9877CC62A474002DF32E52139F0A0", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger N = BigInteger.Parse("00FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFF7203DF6B21C6052B53BBF40939D54123", System.Globalization.NumberStyles.AllowHexSpecifier);
            var H = 1;
             

            
            //512bit
            BigInteger P = BigInteger.Parse("01FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger A = BigInteger.Parse("01FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger B = BigInteger.Parse("0051953EB9618E1C9A1F929A21A0B68540EEA2DA725B99B315F3B8B489918EF109E156193951EC7E937B1652C0BD3BB1BF073573DF883D2C34F1EF451FD46B503F00", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger Gx = BigInteger.Parse("00C6858E06B70404E9CD9E3ECB662395B4429C648139053FB521F828AF606B4D3DBAA14B5E77EFE75928FE1DC127A2FFA8DE3348B3C1856A429BF97E7E31C2E5BD66", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger Gy = BigInteger.Parse("011839296A789A3BC0045C8A5FB42C7D1BD998F54449579B446817AFBD17273E662C97EE72995EF42640C550B9013FAD0761353C7086A272C24088BE94769FD16650", System.Globalization.NumberStyles.AllowHexSpecifier);
            BigInteger N = BigInteger.Parse("01FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFA51868783BF2F966B7FCC0148F709A5D03BB5C9B8899C47AEBB6FB71E91386409", System.Globalization.NumberStyles.AllowHexSpecifier);
            var H = 1;
             
            
            Curve = new ECCurve(P, A, B, new ECPoint(Gx,Gy), N, H);*/
            
            PrivateKey = ECCMath.RandomIntegerBelow(ECCurve.n-2);
            PublicKey = ECCMath.ScalarMult(PrivateKey, ECCurve.G);

        }
    }
}
