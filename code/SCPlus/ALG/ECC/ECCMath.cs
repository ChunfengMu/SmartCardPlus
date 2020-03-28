using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace ALG_ECC
{
    public class ECCMath
    {
        /// <summary>
        /// Returns k * point computed using the double and <see cref="PointAdd"/> algorithm.
        /// </summary>
        /// <param name="k"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static ECPoint ScalarMult(BigInteger k, ECPoint point)
        {
            //if (MathMod(k, ECCurve.n) == 0 || point.IsEmpty())
            if (point.IsEmpty())
            {
                return point;
            }

            if (k < 0)
            {
                return ScalarMult(-k, NegatePoint(point));
            }

            ECPoint result = ECPoint.GetEmpty();
            ECPoint addend = point;

            while (k > 0)
            {
                if ((k & 1) > 0)
                {
                    result = PointAdd(result, addend);
                }

                addend = PointAdd(addend, addend);

                k >>= 1;
            }

            return result;
        }

        /// <summary>
        /// Returns the result of point1 + point2 according to the group law.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static ECPoint PointAdd(ECPoint point1, ECPoint point2)
        {
            if (point1.IsEmpty())
            {
                return point2;
            }

            if (point2.IsEmpty())
            {
                return point1;
            }

            BigInteger x1 = point1.X, y1 = point1.Y;
            BigInteger x2 = point2.X, y2 = point2.Y;

            if (x1 == x2 && y1 != y2)
            {
                return ECPoint.GetEmpty();
            }

            BigInteger m = BigInteger.Zero;
            if (x1 == x2)
            {
                m = (3 * x1 * x1 + ECCurve.a) * InverseMod(2 * y1, ECCurve.p);
            }
            else
            {
                m = (y1 - y2) * InverseMod(x1 - x2, ECCurve.p);
            }

            BigInteger x3 = m * m - x1 - x2;
            BigInteger y3 = y1 + m * (x3 - x1);
            ECPoint result = new ECPoint(MathMod(x3, ECCurve.p), MathMod(-y3, ECCurve.p));

            return result;
        }

        /// <summary>
        /// Returns the inverse of k modulo p.
        /// </summary>
        /// <param name="k">Must be non-zero</param>
        /// <param name="p">Must be a prime</param>
        /// <returns></returns>
        public static BigInteger InverseMod(BigInteger k, BigInteger p)
        {
            if (k == 0)
            {
                throw new DivideByZeroException("k");
            }

            if (k < 0)
            {
                return p - InverseMod(-k, p);
            }

            var result = ExtendedGcd(p, k);

            if (result[0] != 1)
            {
                throw new Exception("Gcd is not 1");
            }

            if (MathMod(k * result[1], p) != 1)
            {
                throw new Exception("(k * result.x) % p != 1");
            }

            return MathMod(result[1], p);
        }

        public static BigInteger MathMod(BigInteger a, BigInteger b)
        {
            return (BigInteger.Abs(a * b) + a) % b;
        }

        /// <summary>
        /// Extended Euclidean algorithm.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns> BigInteger gcd, BigInteger x, BigInteger y </returns>
        public static BigInteger[] ExtendedGcd(BigInteger a, BigInteger b)
        {
            BigInteger s = 0, old_s = 1;
            BigInteger t = 1, old_t = 0;
            BigInteger r = a, old_r = b;

            while (r != 0)
            {
                BigInteger q = old_r / r;
                var temp = r;
                r = old_r - q * r;
                old_r = temp;

                temp = s;
                s = old_s - q * s;
                old_s = temp;

                temp = t;
                t = old_t - q * t;
                old_t = temp;
            }

            BigInteger[] bigInteger = new BigInteger[3];
            bigInteger[0] = old_r;
            bigInteger[1] = old_s;
            bigInteger[2] = old_t;
            return bigInteger;
        }

        /// <summary>
        /// Returns -point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static ECPoint NegatePoint(ECPoint point)
        {
            if (point.IsEmpty())
            {
                return point;
            }

            BigInteger x = point.X;
            BigInteger y = point.Y;
            ECPoint result = new ECPoint(x, MathMod(-y, ECCurve.p));

            return result;
        }

        /// <summary>
        /// Returns <c>true</c> if the given point lies on the elliptic curve.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool IsOnCurve(ECPoint point)
        {
            if (point.IsEmpty())
            {
                return true;
            }

            BigInteger x = point.X;
            BigInteger y = point.Y;

            return MathMod(y * y - x * x * x - ECCurve.a * x - ECCurve.b, ECCurve.p) == BigInteger.Zero;
        }


        public static bool IsInfinityPoint(ECPoint point)
        {
            if (point.IsEmpty())
            {
                return true;
            }
            return false;
        }
        
        public static BigInteger RandomIntegerBelow(BigInteger N)
        {
            byte[] bytes = N.ToByteArray();
            BigInteger R;
            Random random = new Random();
            do
            {
                random.NextBytes(bytes);
                bytes[bytes.Length - 1] &= (byte)0x7F;
                R = new BigInteger(bytes);
            } while (R >= N && R >= 1);

            return R;
        }

        public static bool RangeBetween(BigInteger integer, BigInteger M, BigInteger N)
        {
            bool ret = false;
            if (integer >= M && integer <= N)
                ret = true;
            return ret;
        }
        
    }
}
