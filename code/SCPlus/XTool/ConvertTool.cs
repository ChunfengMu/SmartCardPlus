using System;
using System.Text;
using System.Diagnostics.Contracts;

namespace xTool
{
    public class ConvertTool
    {
        public static string RemoveSpace(string str)
        {
            if (str != null && str !="")
            {
                str = str.Replace(" ", "");
                //str = str.Replace("\t", "");
            }
            return str;
        }
        /// <summary>bytes array to hex string.</summary>
        public static string Bytes2String(byte[] data)
        {
            string hex = BitConverter.ToString(data);
            return hex.Replace("-", "");
        }
        /// <summary>hex string to bytes array.</summary>
        public static byte[] String2Bytes(string hex)
        {
            if (hex.Length % 2 != 0 || hex.Length == 0)
            {
                throw new ArgumentException("Invalid Hex String, Not 2*n bytes");
            }
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            int i = 0;
            try
            {
                for (i = 0; i < NumberChars; i += 2)
                    bytes[i / 2] = System.Convert.ToByte(hex.Substring(i, 2), 16);
            }
            catch (System.FormatException)
            {
                throw new Exception("Invalid Hex Character:\'" + hex.Substring(i, 2) + "\'");
            }

            return bytes;
        }

        //PBOC Debit -> 50424F43204465626974
        public static string String2Ascii(string strEncode)
        {
            string strReturn = "";

            foreach (byte shortx in strEncode.ToCharArray())
            {
                strReturn += shortx.ToString("X2");
            }
            return strReturn;
        }

        //50424F43204465626974 -> PBOC Debit 
        public static string Ascii2String(string strDecode, bool isReturnNull = false)
        {
            string res = "";
            strDecode = strDecode.Replace(" ", "");
            byte[] array = new byte[1];
            for (int i = 0; i < strDecode.Length; i = i + 2)
            {
                array[0] = (byte)(Convert.ToInt32(strDecode.Substring(i, 2), 16));
                if (array[0] <= 127 && array[0] >= 32)
                    res += Convert.ToString(System.Text.Encoding.ASCII.GetString(array));
                else if (isReturnNull)
                    return null;
                else
                    res += ".";
            }
            return res;
        }


        public static string Base64Decode(string value)
        {
            if (value == null || value == "")
            {
                return "";
            }
            byte[] bytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string Base64Encode(string value)
        {
            if (value == null || value == "")
            {
                return "";
            }
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }
       
    }
}
