using System.Security.Cryptography;
using System.IO;
using xTool;

namespace ALG
{
    public class Hash
    {

        public static string HashSHA1(string data)
        {
            
            SHA1 sha = SHA1Managed.Create();
            string res = "";
            if (data.Contains(":"))
            {
                using (FileStream fs = new FileStream(data, FileMode.Open))
                {
                    res = ConvertTool.Bytes2String(sha.ComputeHash(fs));
                }
            }
            else
            {
                data = ConvertTool.RemoveSpace(data);
                res = ConvertTool.Bytes2String(sha.ComputeHash(ConvertTool.String2Bytes(data)));
            }

            return res;

        }


        public static string HashSHA224(string data)
        {

            SHA224 sha = new SHA224Managed();

            string res = "";
            if (data.Contains(":"))
            {
                using (FileStream fs = new FileStream(data, FileMode.Open))
                {
                    res = ConvertTool.Bytes2String(sha.ComputeHash(fs));
                }
            }
            else
            {
                data = ConvertTool.RemoveSpace(data);
                res = ConvertTool.Bytes2String(sha.ComputeHash(ConvertTool.String2Bytes(data)));
            }

            return res;


        }


        public static string HashSHA256(string data)
        {

            SHA256 sha = SHA256Managed.Create();

            string res = "";
            if (data.Contains(":"))
            {
                using (FileStream fs = new FileStream(data, FileMode.Open))
                {
                    res = ConvertTool.Bytes2String(sha.ComputeHash(fs));
                }
            }
            else
            {
                data = ConvertTool.RemoveSpace(data);
                res = ConvertTool.Bytes2String(sha.ComputeHash(ConvertTool.String2Bytes(data)));
            }

            return res;

        }


        public static string HashSHA384(string data)
        {

            SHA384 sha = SHA384Managed.Create();

            string res = "";
            if (data.Contains(":"))
            {
                using (FileStream fs = new FileStream(data, FileMode.Open))
                {
                    res = ConvertTool.Bytes2String(sha.ComputeHash(fs));
                }
            }
            else
            {
                data = ConvertTool.RemoveSpace(data);
                res = ConvertTool.Bytes2String(sha.ComputeHash(ConvertTool.String2Bytes(data)));
            }

            return res;

        }

        public static string HashSHA512(string data)
        {

            SHA512 sha = SHA512Managed.Create();

            string res = "";
            if (data.Contains(":"))
            {
                using (FileStream fs = new FileStream(data, FileMode.Open))
                {
                    res = ConvertTool.Bytes2String(sha.ComputeHash(fs));
                }
            }
            else
            {
                data = ConvertTool.RemoveSpace(data);
                res = ConvertTool.Bytes2String(sha.ComputeHash(ConvertTool.String2Bytes(data)));
            }

            return res;

        }

        public static string MD5(string data)
        {

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            string res = "";
            if (data.Contains(":"))
            {
                using (FileStream fs = new FileStream(data, FileMode.Open))
                {
                    res = ConvertTool.Bytes2String(md5.ComputeHash(fs));
                }
            }
            else
            {
                data = ConvertTool.RemoveSpace(data);
                res = ConvertTool.Bytes2String(md5.ComputeHash(ConvertTool.String2Bytes(data)));
            }

            return res;

        }
    }
}
