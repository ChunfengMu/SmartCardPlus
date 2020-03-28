using System.Numerics;
using System;

namespace ALG_ECC
{
    public class ECCurve
    {
        //public const int ECCurve_Fp = 0;
        //public const int ECCurve_F2m = 1;
        //public static int ECCurve_Type;

        public const string ECCurve_SM2 = "SM2";
        //X9_62 prime
        public const string ECCurve_prime192v1 = "prime192v1";
        public const string ECCurve_prime256v1 = "prime256v1";
        //nist prime
        public const string ECCurve_P_192 = "P-192";
        public const string ECCurve_P_224 = "P-224";
        public const string ECCurve_P_256 = "P-256";
        public const string ECCurve_P_384 = "P-384";
        public const string ECCurve_P_521 = "P-521";
        //sec prime
        public const string ECCurve_secp192k1 = "secp192k1";
        public const string ECCurve_secp192r1 = "secp192r1";
        public const string ECCurve_secp224k1 = "secp224k1";
        public const string ECCurve_secp224r1 = "secp224r1";
        public const string ECCurve_secp256k1 = "secp256k1";
        public const string ECCurve_secp256r1 = "secp256r1";
        public const string ECCurve_secp384r1 = "secp384r1";
        public const string ECCurve_secp521r1 = "secp521r1";

        // Field characteristic.
        public static BigInteger p;

        // Curve coefficients.
        public static BigInteger a;
        public static BigInteger b;

        // Base point.
        public static ECPoint G;

        // Subgroup order.
        public static BigInteger n;

        // Subgroup cofactor.
        public static int h = 1;

        public static int BitLength;

        public ECCurve(string ec_name)
        {
            BigInteger P;
            BigInteger A;
            BigInteger B;
            BigInteger Gx;
            BigInteger Gy;
            BigInteger N;
            var H = 1;
            switch (ec_name)
            {               
                case ECCurve.ECCurve_secp192k1:
                    P = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFEE37", System.Globalization.NumberStyles.AllowHexSpecifier);
                    A = BigInteger.Parse("00000000000000000000000000000000000000000000000000", System.Globalization.NumberStyles.AllowHexSpecifier);
                    B = BigInteger.Parse("00000000000000000000000000000000000000000000000003", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gx = BigInteger.Parse("00DB4FF10EC057E9AE26B07D0280B7F4341DA5D1B1EAE06C7D", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gy = BigInteger.Parse("009B2F2F6D9C5628A7844163D015BE86344082AA88D95E2F9D", System.Globalization.NumberStyles.AllowHexSpecifier);
                    N = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFE26F2FC170F69466A74DEFD8D", System.Globalization.NumberStyles.AllowHexSpecifier);
                    BitLength = 192;
                   
                    break;

                case ECCurve.ECCurve_prime192v1:
                case ECCurve.ECCurve_P_192:
                case ECCurve.ECCurve_secp192r1:
                    P = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFF", System.Globalization.NumberStyles.AllowHexSpecifier);
                    A = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFC", System.Globalization.NumberStyles.AllowHexSpecifier);
                    B = BigInteger.Parse("0064210519E59C80E70FA7E9AB72243049FEB8DEECC146B9B1", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gx = BigInteger.Parse("00188DA80EB03090F67CBF20EB43A18800F4FF0AFD82FF1012", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gy = BigInteger.Parse("0007192B95FFC8DA78631011ED6B24CDD573F977A11E794811", System.Globalization.NumberStyles.AllowHexSpecifier);
                    N = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFF99DEF836146BC9B1B4D22831", System.Globalization.NumberStyles.AllowHexSpecifier);
                    BitLength = 192;
                    
                    break;

                case ECCurve.ECCurve_secp224k1:
                    P = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFE56D", System.Globalization.NumberStyles.AllowHexSpecifier);
                    A = BigInteger.Parse("00000000000000000000000000000000000000000000000000000000", System.Globalization.NumberStyles.AllowHexSpecifier);
                    B = BigInteger.Parse("00000000000000000000000000000000000000000000000000000005", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gx = BigInteger.Parse("00A1455B334DF099DF30FC28A169A467E9E47075A90F7E650EB6B7A45C", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gy = BigInteger.Parse("007E089FED7FBA344282CAFBD6F7E319F7C0B0BD59E2CA4BDB556D61A5", System.Globalization.NumberStyles.AllowHexSpecifier);
                    N = BigInteger.Parse("010000000000000000000000000001DCE8D2EC6184CAF0A971769FB1F7", System.Globalization.NumberStyles.AllowHexSpecifier);
                    BitLength = 224;
                    
                    break;

                case ECCurve.ECCurve_P_224:
                case ECCurve.ECCurve_secp224r1:
                    P = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF000000000000000000000001", System.Globalization.NumberStyles.AllowHexSpecifier);
                    A = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFE", System.Globalization.NumberStyles.AllowHexSpecifier);
                    B = BigInteger.Parse("00B4050A850C04B3ABF54132565044B0B7D7BFD8BA270B39432355FFB4", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gx = BigInteger.Parse("00B70E0CBD6BB4BF7F321390B94A03C1D356C21122343280D6115C1D21", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gy = BigInteger.Parse("00BD376388B5F723FB4C22DFE6CD4375A05A07476444D5819985007E34", System.Globalization.NumberStyles.AllowHexSpecifier);
                    N = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFF16A2E0B8F03E13DD29455C5C2A3D", System.Globalization.NumberStyles.AllowHexSpecifier);
                    BitLength = 224;
                    
                    break;

                case ECCurve.ECCurve_secp256k1:              
                    P = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFC2F", System.Globalization.NumberStyles.AllowHexSpecifier);
                    A = BigInteger.Parse("000000000000000000000000000000000000000000000000000000000000000000", System.Globalization.NumberStyles.AllowHexSpecifier);
                    B = BigInteger.Parse("000000000000000000000000000000000000000000000000000000000000000007", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gx = BigInteger.Parse("0079BE667EF9DCBBAC55A06295CE870B07029BFCDB2DCE28D959F2815B16F81798", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gy = BigInteger.Parse("00483ADA7726A3C4655DA4FBFC0E1108A8FD17B448A68554199C47D08FFB10D4B8", System.Globalization.NumberStyles.AllowHexSpecifier);
                    N = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEBAAEDCE6AF48A03BBFD25E8CD0364141", System.Globalization.NumberStyles.AllowHexSpecifier);
                    BitLength = 256;
                    
                    break;

                case ECCurve.ECCurve_prime256v1:
                case ECCurve.ECCurve_P_256:
                case ECCurve.ECCurve_secp256r1:
                    P = BigInteger.Parse("00FFFFFFFF00000001000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFF", System.Globalization.NumberStyles.AllowHexSpecifier);
                    A = BigInteger.Parse("00FFFFFFFF00000001000000000000000000000000FFFFFFFFFFFFFFFFFFFFFFFC", System.Globalization.NumberStyles.AllowHexSpecifier);
                    B = BigInteger.Parse("005AC635D8AA3A93E7B3EBBD55769886BC651D06B0CC53B0F63BCE3C3E27D2604B", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gx = BigInteger.Parse("006B17D1F2E12C4247F8BCE6E563A440F277037D812DEB33A0F4A13945D898C296", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gy = BigInteger.Parse("004FE342E2FE1A7F9B8EE7EB4A7C0F9E162BCE33576B315ECECBB6406837BF51F5", System.Globalization.NumberStyles.AllowHexSpecifier);
                    N = BigInteger.Parse("00FFFFFFFF00000000FFFFFFFFFFFFFFFFBCE6FAADA7179E84F3B9CAC2FC632551", System.Globalization.NumberStyles.AllowHexSpecifier);
                    BitLength = 256;
                    
                    break;

                case ECCurve.ECCurve_P_384:
                case ECCurve.ECCurve_secp384r1:
                    P = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFF0000000000000000FFFFFFFF", System.Globalization.NumberStyles.AllowHexSpecifier);
                    A = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFFFFF0000000000000000FFFFFFFC", System.Globalization.NumberStyles.AllowHexSpecifier);
                    B = BigInteger.Parse("00B3312FA7E23EE7E4988E056BE3F82D19181D9C6EFE8141120314088F5013875AC656398D8A2ED19D2A85C8EDD3EC2AEF", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gx = BigInteger.Parse("00AA87CA22BE8B05378EB1C71EF320AD746E1D3B628BA79B9859F741E082542A385502F25DBF55296C3A545E3872760AB7", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gy = BigInteger.Parse("003617DE4A96262C6F5D9E98BF9292DC29F8F41DBD289A147CE9DA3113B5F0B8C00A60B1CE1D7E819D7A431D7C90EA0E5F", System.Globalization.NumberStyles.AllowHexSpecifier);
                    N = BigInteger.Parse("00FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC7634D81F4372DDF581A0DB248B0A77AECEC196ACCC52973", System.Globalization.NumberStyles.AllowHexSpecifier);
                    BitLength = 384;
                    
                    break;

                case ECCurve.ECCurve_P_521:
                case ECCurve.ECCurve_secp521r1:
                    P = BigInteger.Parse("0001FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", System.Globalization.NumberStyles.AllowHexSpecifier);
                    A = BigInteger.Parse("0001FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFC", System.Globalization.NumberStyles.AllowHexSpecifier);
                    B = BigInteger.Parse("000051953EB9618E1C9A1F929A21A0B68540EEA2DA725B99B315F3B8B489918EF109E156193951EC7E937B1652C0BD3BB1BF073573DF883D2C34F1EF451FD46B503F00", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gx = BigInteger.Parse("0000C6858E06B70404E9CD9E3ECB662395B4429C648139053FB521F828AF606B4D3DBAA14B5E77EFE75928FE1DC127A2FFA8DE3348B3C1856A429BF97E7E31C2E5BD66", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gy = BigInteger.Parse("00011839296A789A3BC0045C8A5FB42C7D1BD998F54449579B446817AFBD17273E662C97EE72995EF42640C550B9013FAD0761353C7086A272C24088BE94769FD16650", System.Globalization.NumberStyles.AllowHexSpecifier);
                    N = BigInteger.Parse("0001FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFA51868783BF2F966B7FCC0148F709A5D03BB5C9B8899C47AEBB6FB71E91386409", System.Globalization.NumberStyles.AllowHexSpecifier);
                    BitLength = 528;
                    
                    break;

                case ECCurve.ECCurve_SM2:
                    P = BigInteger.Parse("00FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFF", System.Globalization.NumberStyles.AllowHexSpecifier);
                    A = BigInteger.Parse("00FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000000FFFFFFFFFFFFFFFC", System.Globalization.NumberStyles.AllowHexSpecifier);
                    B = BigInteger.Parse("0028E9FA9E9D9F5E344D5A9E4BCF6509A7F39789F515AB8F92DDBCBD414D940E93", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gx = BigInteger.Parse("0032C4AE2C1F1981195F9904466A39C9948FE30BBFF2660BE1715A4589334C74C7", System.Globalization.NumberStyles.AllowHexSpecifier);
                    Gy = BigInteger.Parse("00BC3736A2F4F6779C59BDCEE36B692153D0A9877CC62A474002DF32E52139F0A0", System.Globalization.NumberStyles.AllowHexSpecifier);
                    N = BigInteger.Parse("00FFFFFFFEFFFFFFFFFFFFFFFFFFFFFFFF7203DF6B21C6052B53BBF40939D54123", System.Globalization.NumberStyles.AllowHexSpecifier);

                    //P = BigInteger.Parse("008542D69E4C044F18E8B92435BF6FF7DE457283915C45517D722EDB8B08F1DFC3", System.Globalization.NumberStyles.AllowHexSpecifier);
                    //A = BigInteger.Parse("00787968B4FA32C3FD2417842E73BBFEFF2F3C848B6831D7E0EC65228B3937E498", System.Globalization.NumberStyles.AllowHexSpecifier);
                    //B = BigInteger.Parse("0063E4C6D3B23B0C849CF84241484BFE48F61D59A5B16BA06E6E12D1DA27C5249A", System.Globalization.NumberStyles.AllowHexSpecifier);
                    //Gx = BigInteger.Parse("00421DEBD61B62EAB6746434EBC3CC315E32220B3BADD50BDC4C4E6C147FEDD43D", System.Globalization.NumberStyles.AllowHexSpecifier);
                    //Gy = BigInteger.Parse("000680512BCBB42C07D47349D2153B70C4E5D7FDFCBFA36EA1A85841B9E46E09A2", System.Globalization.NumberStyles.AllowHexSpecifier);
                    //N = BigInteger.Parse("008542D69E4C044F18E8B92435BF6FF7DD297720630485628D5AE74EE7C32E79B7", System.Globalization.NumberStyles.AllowHexSpecifier);
                    BitLength = 256;
                    
                    break;

                default:
                    throw new Exception("Invalid Curve Name");                   
            }
            p = P;
            a = A;
            b = B;
            G = new ECPoint(Gx, Gy);
            n = N;
            h = H;
        }

    }
}