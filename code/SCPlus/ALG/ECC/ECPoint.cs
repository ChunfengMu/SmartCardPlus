using System.Numerics;

namespace ALG_ECC
{
    public struct ECPoint
    {
        public BigInteger X;
        public BigInteger Y;

        public ECPoint(BigInteger x, BigInteger y)
        {
            X = x;
            Y = y;
        }

        public static ECPoint GetEmpty()
        {
            return new ECPoint(0, 0);
        }

        public bool IsEmpty(){
           return X == BigInteger.Zero && Y == BigInteger.Zero;
        } 
    }
}
