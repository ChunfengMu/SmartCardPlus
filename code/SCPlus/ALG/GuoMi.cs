using System.Text;
using xTool;
using System;
using System.IO;

namespace ALG
{
    public class GuoMi
    {
        public static string SM4EncryptCBC(string input, string iv, string key)
        {
            input = ConvertTool.RemoveSpace(input);
            iv = ConvertTool.RemoveSpace(iv);
            key = ConvertTool.RemoveSpace(key);

            if (key.Length != 32)
                throw new Exception("Invalid Key, Not 16 bytes");
            if (input.Length % 32 != 0 || input.Length==0)
                throw new Exception("Invalid Data, Not 16*n bytes");
            if (iv.Length != 32)
                throw new Exception("Invalid IV, Not 16*n bytes");

            byte[] output = SM4.Encrypt_CBC(ConvertTool.String2Bytes(input), ConvertTool.String2Bytes(key), ConvertTool.String2Bytes(iv));

            return ConvertTool.Bytes2String(output);
           
        }

        public static string SM4EncryptECB(string input, string key)
        {
            input = ConvertTool.RemoveSpace(input);
            key = ConvertTool.RemoveSpace(key);

            if (key.Length != 32)
                throw new Exception("Invalid Key, Not 16 bytes");
            if (input.Length % 32 != 0 || input.Length == 0)
                throw new Exception("Invalid Data, Not 16*n bytes");

            byte[] output = SM4.Encrypt_ECB(ConvertTool.String2Bytes(input), ConvertTool.String2Bytes(key));

            return ConvertTool.Bytes2String(output);
        }

        public static string SM4DecryptCBC(string input, string iv, string key)
        {
            input = ConvertTool.RemoveSpace(input);
            key = ConvertTool.RemoveSpace(key);
            iv = ConvertTool.RemoveSpace(iv);

            if (key.Length != 32)
                throw new Exception("Invalid Key, Not 16 bytes");
            if (input.Length % 32 != 0 || input.Length == 0)
                throw new Exception("Invalid Cipher, Not 16*n bytes");
            if (iv.Length != 32)
                throw new Exception("Invalid IV, Not 16 bytes");

            byte[] output = SM4.Decrypt_CBC(ConvertTool.String2Bytes(input), ConvertTool.String2Bytes(key), ConvertTool.String2Bytes(iv));

            return ConvertTool.Bytes2String(output);
        }

        public static string SM4DecryptECB(string input, string key)
        {
            input = ConvertTool.RemoveSpace(input);
            key = ConvertTool.RemoveSpace(key);
            
            if (key.Length != 32)
                throw new Exception("Invalid Key, Not 16 bytes");
            if (input.Length % 32 != 0 || input.Length == 0)
                throw new Exception("Invalid Cipher, Not 16*n bytes");


            byte[] output = SM4.Decrypt_ECB(ConvertTool.String2Bytes(input), ConvertTool.String2Bytes(key));

            return ConvertTool.Bytes2String(output);
        }

        public static string SM4MAC(string input, string iv, string key)
        {
            input = ConvertTool.RemoveSpace(input);
            key = ConvertTool.RemoveSpace(key);
            iv = ConvertTool.RemoveSpace(iv);

            if (key.Length != 32)
                throw new Exception("Invalid Key, Not 16 bytes");
            if (input.Length % 32 != 0 || input.Length == 0)
                throw new Exception("Invalid Cipher, Not 16*n bytes");
            if (iv.Length != 32)
                throw new Exception("Invalid IV, Not 16 bytes");

            byte[] data_I = ConvertTool.String2Bytes(iv);
            for (int i = 0; i < input.Length; i = i + 32)
            {
                string temp = input.Substring(i, 32);
                byte[] data1 = ConvertTool.String2Bytes(temp);
                for (int j = 0; j < 16; j++)
                    data_I[j] = (byte)(data1[j] ^ data_I[j]);

                string res;
                res = SM4EncryptECB(ConvertTool.Bytes2String(data_I), key);

                data_I = ConvertTool.String2Bytes(res);
            }

            return ConvertTool.Bytes2String(data_I);
        }

        
        public static string SM3(string data)
        {
            data = ConvertTool.RemoveSpace(data);
           
            string res = "";         
            SM3Cng sm3 = new SM3Cng();

            if (data.Contains(":"))
            {
                using (FileStream fs = new FileStream(data, FileMode.Open))
                {
                    res = ConvertTool.Bytes2String(sm3.ComputeHash(fs));
                }
            }
            else
            {
                //if (data.Length % 2 != 0)
                //    throw new Exception("Invalid Data, Not 2*n bytes");
                res = ConvertTool.Bytes2String(sm3.ComputeHash(ConvertTool.String2Bytes(data)));
            }

            return res;        
        }

       
    }
}
