using System;
using System.Linq;
using System.Numerics;

namespace ALG_ECC
{
    public class Base58
    {
        protected const int CheckSumSizeInBytes = 4;
        //protected const string Hexdigits = "0123456789abcdefABCDEF";
        //private const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        private const string Digits_BitCoin = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        private const string Digits_Monero = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        private const string Digits_Ripple = "rpshnaf39wBUDNEGHJKLM4PQRST7VWXYZ2bcdeCg65jkm8oFqi1tuvAxyz";
        private const string Digits_Flickr = "123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";     

        public static string Base58Encode_BitCoin(string str, bool isChecked = false)//bitcoin, monero
        {
            if (isChecked)                        
            {
                string sha256 = "";
                sha256 = ALG.Hash.HashSHA256(str);
                sha256 = ALG.Hash.HashSHA256(sha256);
                str = str + sha256.Substring(0, 8);
            }
            return Encode(str, Digits_BitCoin);
        }
        public static string Base58Decode_BitCoin(string str, bool isChecked = false)
        {
            string res = Decode(str, Digits_BitCoin);
            if (isChecked)
            {               
                string sha256 = "";
                sha256 = ALG.Hash.HashSHA256(res.Substring(0, res.Length-8));
                sha256 = ALG.Hash.HashSHA256(sha256);
                sha256 = sha256.Substring(0, 8);

                if(sha256.ToUpper() != res.Substring(res.Length-8).ToUpper())
                {
                    //return null;
                    throw new Exception("Base58 CheckSum Failed");
                }
                res = res.Substring(0, res.Length - 8);
            }
            return res;
        }

        public static string Base58Encode_Ripple(string str, bool isChecked = false)
        {
           if (isChecked)
            {
                string sha256 = "";
                sha256 = ALG.Hash.HashSHA256(str);
                sha256 = ALG.Hash.HashSHA256(sha256);
                str = str + sha256.Substring(0, 8);
            }
            return Encode(str, Digits_Ripple);
        }
        public static string Base58Decode_Ripple(string str, bool isChecked = false)
        {
            string res = Decode(str, Digits_Ripple);
            if (isChecked)
            {
                string sha256 = "";
                sha256 = ALG.Hash.HashSHA256(res.Substring(0, res.Length - 8));
                sha256 = ALG.Hash.HashSHA256(sha256);
                sha256 = sha256.Substring(0, 8);

                if (sha256.ToUpper() != res.Substring(res.Length - 8).ToUpper())
                {
                    //return null;
                    throw new Exception("Base58 CheckSum Failed");
                }
                res = res.Substring(0, res.Length - 8);
            }
            return res;
        }

        private static string Encode(string str, string Digits)
        {
            str = xTool.ConvertTool.RemoveSpace(str);

            byte []data = xTool.ConvertTool.String2Bytes(str);
            // Decode byte[] to BigInteger
            BigInteger intData = 0;
            for (var i = 0; i < data.Length; i++)
            {
                intData = intData * 256 + data[i];
            }

            // Encode BigInteger to Base58 string
            var result = "";
            while (intData > 0)
            {
                var remainder = (int)(intData % 58);
                intData /= 58;
                result = Digits[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (var i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }
            return result;
        }

        private static string Decode(string base58, string Digits)
        {
            base58 = xTool.ConvertTool.RemoveSpace(base58);

            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;
            for (var i = 0; i < base58.Length; i++)
            {
                var digit = Digits.IndexOf(base58[i]); //Slow
                if (digit < 0)
                    throw new FormatException("Invalid Base58 Character");
                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            var leadingZeroCount = base58.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
                intData.ToByteArray()
                .Reverse()// to big endian
                .SkipWhile(b => b == 0);//strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();

            return xTool.ConvertTool.Bytes2String(result);
            //return result;
        }
    }
}