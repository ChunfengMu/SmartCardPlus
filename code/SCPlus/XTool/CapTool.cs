using System;
using System.IO;
using System.Security.Cryptography;
namespace xTool
{
    public class CapTool
    {
        public static string PackageAID;
        public static string[] ClassAID;
        public static string SHA1;

        private static string Hash_SHA1(string data)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            return ConvertTool.Bytes2String(sha.ComputeHash(ConvertTool.String2Bytes(data)));
        }

        public static string GetComponent(string sourceFilePath)
        {
            ClassAID = null;
            int offset = 0;
            int length = 0;
            string component = null;
            //int[] sequence = new int[] { 1, 2, 4, 3, 6, 7, 8, 10, 5, 9, 11, 12 };
            int[] sequence = new int[] { 1, 2, 4, 3, 6, 7, 8, 10, 5, 9 };
            FileStream fs = new FileStream(sourceFilePath, FileMode.Open);

            int capSize = Convert.ToInt32(fs.Length);
            byte[] byteCapData = new byte[capSize];
            fs.Read(byteCapData, offset, capSize);
            fs.Close();

            string strCapData = ConvertTool.Bytes2String(byteCapData);

            for (int i = 0; i < sequence.Length; i++)
            {
                offset = strCapData.IndexOf("2E636170" + sequence[i].ToString("X2"));
                if (offset > -1)
                {
                    length = System.Convert.ToInt32(strCapData.Substring(offset + 10, 4), 16);
                    component += strCapData.Substring(offset + 8, (length + 3) * 2);

                    if (sequence[i] == 1) {  //package aid
                        string temp = strCapData.Substring(offset + 8, (length + 3) * 2);
                        string len = temp.Substring(24,2);
                        PackageAID = temp.Substring(26, (System.Convert.ToInt32(len, 16))*2);
                    }

                    if (sequence[i] == 3) //class aid
                    {
                        string temp = strCapData.Substring(offset + 8, (length + 3) * 2);
                        string count = temp.Substring(6, 2);
                        int max = System.Convert.ToInt32(count,16);
                        ClassAID = new string [max];

                        int aidOffset = 4;
                        int aidLen = 0;

                        for (int k = 0; k < max; k++)
                        {
                            aidOffset = aidOffset + 6 + aidLen * 2;
                            aidLen = System.Convert.ToInt32( temp.Substring(aidOffset-2, 2), 16 );
                            ClassAID[k] = temp.Substring(aidOffset, aidLen * 2);
                        }
                        
                    }
                }
            }

           
            SHA1 = Hash_SHA1(component);

            string TL = "C4";
            string capLen = System.Convert.ToString(component.Length / 2, 16);
            if (capLen.Length % 2 != 0)
                capLen = "0" + capLen;
            if (component.Length > 510)
                TL += "82" + capLen;
            else
                TL += "81" + capLen;

            return TL + component;
        }
    }
}
