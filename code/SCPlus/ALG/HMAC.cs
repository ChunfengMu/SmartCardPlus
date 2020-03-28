
using System.Security.Cryptography;
using xTool;
using System.IO;

namespace ALG
{
    public class HMAC
    {

        /* 
         key:        Bytes     //array of bytes
         message:    Bytes     //array of bytes to be hashed
         hash:       Function  //the hash function to use (e.g. SHA-1)
         blockSize:  Integer   //the block size of the underlying hash function (e.g. 64 bytes for SHA-1)
         outputSize: Integer   //the output size of the underlying hash function (e.g. 20 bytes for SHA-1)

       //Keys longer than blockSize are shortened by hashing them
       if (length(key) > blockSize) then
          key ← hash(key) //Key becomes outputSize bytes long

       //Keys shorter than blockSize are padded to blockSize by padding with zeros on the right
        if (length(key) < blockSize) then
          key  ← Pad(key, blockSize)  //pad key with zeros to make it blockSize bytes long


       o_key_pad = key xor[0x5c * blockSize]     //Outer padded key
       i_key_pad = key xor[0x36 * blockSize]    //Inner padded key

         return hash(o_key_pad ∥ hash(i_key_pad ∥ message)) //Where ∥ is concatenation
         */

        public static string HMAC_SHA1(string key, string data)
        {
            key = ConvertTool.RemoveSpace(key);
            data = ConvertTool.RemoveSpace(data);

            HMACSHA1 hmac = new HMACSHA1(ConvertTool.String2Bytes(key));
            byte[] hashValue;
            if (data.Contains(":"))
            {
                using (FileStream inStream = new FileStream(data, FileMode.Open))
                {
                    hashValue = hmac.ComputeHash(inStream);
                }
            }
            else
                hashValue = hmac.ComputeHash(ConvertTool.String2Bytes(data));
            return ConvertTool.Bytes2String(hashValue);
        }

        public static string HMAC_SHA224(string key, string data)
        {
            key = ConvertTool.RemoveSpace(key);
            data = ConvertTool.RemoveSpace(data);

            HMACSHA224 hmac = new HMACSHA224(ConvertTool.String2Bytes(key));
            byte[] hashValue;
            if (data.Contains(":"))
            {
                using (FileStream inStream = new FileStream(data, FileMode.Open))
                {
                    hashValue = hmac.ComputeHash(inStream);
                }
            }
            else
                hashValue = hmac.ComputeHash(ConvertTool.String2Bytes(data));
            return ConvertTool.Bytes2String(hashValue);

        }

        public static string HMAC_SHA256(string key, string data)
        {
            key = ConvertTool.RemoveSpace(key);
            data = ConvertTool.RemoveSpace(data);

            HMACSHA256 hmac = new HMACSHA256(ConvertTool.String2Bytes(key));
            byte[] hashValue;
            if (data.Contains(":"))
            {
                using (FileStream inStream = new FileStream(data, FileMode.Open))
                {
                    hashValue = hmac.ComputeHash(inStream);
                }
            }
            else
                hashValue = hmac.ComputeHash(ConvertTool.String2Bytes(data));
            return ConvertTool.Bytes2String(hashValue);

        }

        public static string HMAC_SHA384(string key, string data)
        {
            key = ConvertTool.RemoveSpace(key);
            data = ConvertTool.RemoveSpace(data);

            HMACSHA384 hmac = new HMACSHA384(ConvertTool.String2Bytes(key));
            byte[] hashValue;
            if (data.Contains(":"))
            {
                using (FileStream inStream = new FileStream(data, FileMode.Open))
                {
                    hashValue = hmac.ComputeHash(inStream);
                }
            }
            else
                hashValue = hmac.ComputeHash(ConvertTool.String2Bytes(data));
            return ConvertTool.Bytes2String(hashValue);

        }

        public static string HMAC_SHA512(string key, string data)
        {
            key = ConvertTool.RemoveSpace(key);
            data = ConvertTool.RemoveSpace(data);

            HMACSHA512 hmac = new HMACSHA512(ConvertTool.String2Bytes(key));
            byte[] hashValue;
            if (data.Contains(":"))
            {
                using (FileStream inStream = new FileStream(data, FileMode.Open))
                {
                    hashValue = hmac.ComputeHash(inStream);
                }
            }
            else
                hashValue = hmac.ComputeHash(ConvertTool.String2Bytes(data));
            return ConvertTool.Bytes2String(hashValue);

        }

        public static string HMAC_MD5(string key, string data)
        {
            key = ConvertTool.RemoveSpace(key);
            data = ConvertTool.RemoveSpace(data);

            HMACMD5 hmac = new HMACMD5(ConvertTool.String2Bytes(key));
            byte[] hashValue;
            if (data.Contains(":"))
            {
                using (FileStream inStream = new FileStream(data, FileMode.Open))
                {
                    hashValue = hmac.ComputeHash(inStream);
                }
            }
            else
                hashValue = hmac.ComputeHash(ConvertTool.String2Bytes(data));
            return ConvertTool.Bytes2String(hashValue);

        }
    }
}
