using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace ALG_ECC
{
    public class ECDSABase
    {
        public ECCurve Curve;

        public ECDSABase(ECCurve curve)
        {
            Curve = curve;
        }

        /// <summary>
        /// Generates a random private-public key pair.
        /// </summary>
        /// <returns>The tuple consists of a private and public key</returns>
        public ECKey GenerateKeyPair()
        {
            return new ECKey(Curve);
        }

        
        /// <summary>
        /// Generates signature.
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public BigInteger[] SignMessage(BigInteger privateKey, BigInteger e)
        {
            BigInteger r = 0;
            BigInteger s = 0;

            while (r == 0 || s == 0)
            {
                BigInteger k = ECCMath.RandomIntegerBelow(ECCurve.n);
                ECPoint point = ECCMath.ScalarMult(k, ECCurve.G);

                r = ECCMath.MathMod(point.X, ECCurve.n);
                s = ECCMath.MathMod((e + r * privateKey) * ECCMath.InverseMod(k, ECCurve.n), ECCurve.n);
            }

            BigInteger[] bigInteger = new BigInteger[3];
            bigInteger[0] = r;
            bigInteger[1] = s;
            return bigInteger;
        }

        public bool VerifySignature(ECPoint publicKey, BigInteger e, BigInteger[] signature)
        {
            if (!ECCMath.RangeBetween(signature[0], BigInteger.One, ECCurve.n - 1))
                return false;
            if (!ECCMath.RangeBetween(signature[1], BigInteger.One, ECCurve.n - 1))
                return false;

            BigInteger w = ECCMath.InverseMod(signature[1], ECCurve.n);
            BigInteger u1 = ECCMath.MathMod((e * w), ECCurve.n);
            BigInteger u2 = ECCMath.MathMod((signature[0] * w), ECCurve.n);

            ECPoint point = ECCMath.PointAdd(ECCMath.ScalarMult(u1, ECCurve.G), ECCMath.ScalarMult(u2, publicKey));
            //if (ECCMath.IsInfinityPoint(point))
            //    return false;
            return ECCMath.MathMod(signature[0], ECCurve.n) == ECCMath.MathMod(point.X, ECCurve.n);
        }


        public BigInteger[] SignMessage_SM2(BigInteger privateKey, BigInteger e)
        {
            BigInteger r = 0;
            BigInteger s = 0;
            BigInteger k = 0;

            while (r == 0 || s == 0 || r + k == 0)
            {
                k = ECCMath.RandomIntegerBelow(ECCurve.n);
                ECPoint point = ECCMath.ScalarMult(k, ECCurve.G);

                r = ECCMath.MathMod(e + point.X, ECCurve.n);
                s = ECCMath.MathMod((k - r * privateKey) * ECCMath.InverseMod((1 + privateKey), ECCurve.n), ECCurve.n);
            }

            BigInteger[] bigInteger = new BigInteger[3];
            bigInteger[0] = r;
            bigInteger[1] = s;
            return bigInteger;
        }

        public bool VerifySignature_SM2(ECPoint publicKey, BigInteger e, BigInteger[] signature)
        {
            if (!ECCMath.RangeBetween(signature[0], BigInteger.One, ECCurve.n - 1))
                return false;
            if (!ECCMath.RangeBetween(signature[1], BigInteger.One, ECCurve.n - 1))
                return false;

            BigInteger t = ECCMath.MathMod((signature[0] + signature[1]), ECCurve.n);
            if (t.IsZero)
                return false;
            ECPoint point = ECCMath.PointAdd(ECCMath.ScalarMult(signature[1], ECCurve.G),
                ECCMath.ScalarMult(t, publicKey));
            BigInteger R = ECCMath.MathMod((e + point.X), ECCurve.n);
            return R == signature[0];
        }

        
    }
}