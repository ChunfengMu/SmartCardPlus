using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using xTool;

namespace ALG
{
    public class AES
    {

        /// <summary>  
        /// AES加密(无向量)  
        /// </summary>  
        /// <param name="plainBytes">被加密的明文</param>  
        /// <param name="key">密钥</param>  
        /// <returns>密文</returns>  
        public static string AesECBEn(String Data, String Key)
        {
            Data = ConvertTool.RemoveSpace(Data);
            Key = ConvertTool.RemoveSpace(Key);

            if (Key.Length != 32 && Key.Length != 48 && Key.Length != 64)
                throw new Exception("Invalid Key, Not 16 or 24 or 32 bytes");
            if (Data.Length % 32 != 0 || Data.Length == 0)
                throw new Exception("Invalid Data, Not 16*n bytes");

            MemoryStream mStream = new MemoryStream();
            RijndaelManaged aes = new RijndaelManaged();

            byte[] plainBytes = ConvertTool.String2Bytes(Data);
            byte[] bKey = ConvertTool.String2Bytes(Key);

            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.None;
            aes.KeySize = Key.Length * 4;
            aes.Key = bKey;

            CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in mStream.ToArray())
                ret.AppendFormat("{0:X2}", b);


            cryptoStream.Close();
            mStream.Close();
            aes.Clear();

            return ret.ToString();

        }


        /// <summary>  
        /// AES解密(无向量)  
        /// </summary>  
        /// <param name="encryptedBytes">被加密的明文</param>  
        /// <param name="key">密钥</param>  
        /// <returns>明文</returns>  
        public static string AesECBDe(String Data, String Key)
        {
            Data = ConvertTool.RemoveSpace(Data);
            Key = ConvertTool.RemoveSpace(Key);

            if (Key.Length != 32 && Key.Length != 48 && Key.Length != 64)
                throw new Exception("Invalid Key, Not 16 or 24 or 32 bytes");
            if (Data.Length % 32 != 0 || Data.Length == 0)
                throw new Exception("Invalid Cipher, Not 16*n bytes");

            Byte[] encryptedBytes = ConvertTool.String2Bytes(Data);
            Byte[] bKey = ConvertTool.String2Bytes(Key);

            MemoryStream mStream = new MemoryStream(encryptedBytes);

            RijndaelManaged aes = new RijndaelManaged
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None,
                KeySize = Key.Length * 4
            };
            ;
            aes.Key = bKey;

            CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Read);

            byte[] tmp = new byte[encryptedBytes.Length + 32];
            int len = cryptoStream.Read(tmp, 0, encryptedBytes.Length + 32);
            byte[] ret = new byte[len];
            Array.Copy(tmp, 0, ret, 0, len);


            cryptoStream.Close();
            mStream.Close();
            aes.Clear();

            return ConvertTool.Bytes2String(ret);

        }


        /// <summary>  
        /// AES加密  
        /// </summary>  
        /// <param name="Data">被加密的明文</param>  
        /// <param name="Key">密钥</param>  
        /// <param name="Vector">向量</param>  
        /// <returns>密文</returns>  
        public static String AesCBCEn(String Data, String Key, String Vector)
        {
            Data = ConvertTool.RemoveSpace(Data);
            Key = ConvertTool.RemoveSpace(Key);
            Vector = ConvertTool.RemoveSpace(Vector);

            if (Key.Length != 32 && Key.Length != 48 && Key.Length != 64)
                throw new Exception("Invalid Key, Not 16 or 24 or 32 bytes");
            if (Data.Length % 32 != 0 || Data.Length == 0)
                throw new Exception("Invalid Data, Not 16*n bytes");
            if (Vector.Length != 32)
                throw new Exception("Invalid IV, Not 16*n bytes");
           
            Byte[] plainBytes = ConvertTool.String2Bytes(Data);
            Byte[] bKey = ConvertTool.String2Bytes(Key);
            Byte[] bVector = ConvertTool.String2Bytes(Vector);
            Byte[] Cryptograph = null; // 加密后的密文  

            Rijndael Aes = Rijndael.Create();
            Aes.Mode = CipherMode.CBC;
            Aes.Padding = PaddingMode.None;
            Aes.KeySize = Key.Length * 4; ;

            // 开辟一块内存流  
            using (MemoryStream Memory = new MemoryStream())
            {
                // 把内存流对象包装成加密流对象  
                using (CryptoStream Encryptor = new CryptoStream(Memory,
                 Aes.CreateEncryptor(bKey, bVector),
                 CryptoStreamMode.Write))
                {
                    // 明文数据写入加密流  
                    Encryptor.Write(plainBytes, 0, plainBytes.Length);
                    Encryptor.FlushFinalBlock();

                    Cryptograph = Memory.ToArray();
                }
            }

            return ConvertTool.Bytes2String(Cryptograph);
        }

        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="Data">被解密的密文</param>  
        /// <param name="Key">密钥</param>  
        /// <param name="Vector">向量</param>  
        /// <returns>明文</returns>  
        public static String AesCBCDe(String Data, String Key, String Vector)
        {
            Data = ConvertTool.RemoveSpace(Data);
            Key = ConvertTool.RemoveSpace(Key);
            Vector = ConvertTool.RemoveSpace(Vector);

            if (Key.Length != 32 && Key.Length != 48 && Key.Length != 64)
                throw new Exception("Invalid Key, Not 16 or 24 or 32 bytes");
            if (Data.Length % 32 != 0 || Data.Length == 0)
                throw new Exception("Invalid Cipher, Not 16*n bytes");
            if (Vector.Length != 32)
                throw new Exception("Invalid IV, Not 16*n bytes");

            Byte[] encryptedBytes = ConvertTool.String2Bytes(Data);
            Byte[] bKey = ConvertTool.String2Bytes(Key);
            Byte[] bVector = ConvertTool.String2Bytes(Vector);
            Byte[] original = null; // 解密后的明文  

            Rijndael Aes = Rijndael.Create();
            Aes.Mode = CipherMode.CBC;
            Aes.Padding = PaddingMode.None;
            Aes.KeySize = Key.Length * 4; ;

            // 开辟一块内存流，存储密文  
            using (MemoryStream Memory = new MemoryStream(encryptedBytes))
            {
                // 把内存流对象包装成加密流对象  
                using (CryptoStream Decryptor = new CryptoStream(Memory,
                Aes.CreateDecryptor(bKey, bVector),
                CryptoStreamMode.Read))
                {
                    // 明文存储区  
                    using (MemoryStream originalMemory = new MemoryStream())
                    {
                        Byte[] Buffer = new Byte[1024];
                        Int32 readBytes = 0;
                        while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                        {
                            originalMemory.Write(Buffer, 0, readBytes);
                        }

                        original = originalMemory.ToArray();
                    }
                }
            }

            return ConvertTool.Bytes2String(original);
        }


    }
}
