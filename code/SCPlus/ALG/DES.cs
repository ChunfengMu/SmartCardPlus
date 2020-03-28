using System;
using System.Reflection;
using System.Security.Cryptography;
using xTool;
namespace ALG
{
    public class DES
    {
        public static string DesECBEn(string plaintext, string key)
        {
            plaintext = ConvertTool.RemoveSpace(plaintext);
            key = ConvertTool.RemoveSpace(key);

            if (key.Length != 16)
                throw new Exception("Invalid Key, Not 8 bytes");
            if (plaintext.Length % 16 != 0 || plaintext.Length == 0)
                throw new Exception("Invalid Data, Not 8*n bytes");

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Padding = PaddingMode.None;
            Type t = Type.GetType("System.Security.Cryptography.CryptoAPITransformMode");
            object obj = t.GetField("Encrypt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(t);

            MethodInfo mi = des.GetType().GetMethod("_NewEncryptor", BindingFlags.Instance | BindingFlags.NonPublic);
            ICryptoTransform desCrypt = (ICryptoTransform)mi.Invoke(des, new object[] { ConvertTool.String2Bytes(key), CipherMode.ECB, null, 0, obj });

            byte[] result = desCrypt.TransformFinalBlock(ConvertTool.String2Bytes(plaintext), 0, (ConvertTool.String2Bytes(plaintext)).Length);
            return BitConverter.ToString(result).Replace("-", "");

            /*
                        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                        byte[] inputByteArray = ConvertTool.String2Bytes(plaintext);
                        des.Key = ConvertTool.String2Bytes(key);
                        des.Mode = CipherMode.ECB;
                        des.Padding = System.Security.Cryptography.PaddingMode.None;
                        MemoryStream ms = new MemoryStream();
                        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        StringBuilder ret = new StringBuilder();
                        foreach (byte b in ms.ToArray())
                            ret.AppendFormat("{0:X2}", b);

                        return ret.ToString();*/
        }

        //no padding
        public static string TriDesMAC(string data, string key, string icv = "0000000000000000")
        {
            data = ConvertTool.RemoveSpace(data);
            key = ConvertTool.RemoveSpace(key);
            icv = ConvertTool.RemoveSpace(icv);

            if (key.Length != 32)
                throw new Exception("Invalid Key, Not 16 bytes");
            if (data.Length % 16 != 0 || data.Length == 0)
                throw new Exception("Invalid Data, Not 8*n bytes");
            if (icv.Length != 16)
                throw new Exception("Invalid IV, , Not 8 bytes");

            byte[] inputByteArray = ConvertTool.String2Bytes(data);
            string KeyA = key.Substring(0, 16);
            string KeyB = key.Substring(16, 16);
            int i;
            byte[] data1 = new byte[8];
            byte[] data2 = ConvertTool.String2Bytes(icv);
            byte[] xorres = new byte[8];
            for (i = 0; i < inputByteArray.Length; i += 8)
            {
                Array.Copy(inputByteArray, i, data1, 0, 8);
                for (int k = 0; k < 8; k++)
                    xorres[k] = (byte)(data1[k] ^ data2[k]);

                data2 = ConvertTool.String2Bytes(DesECBEn(ConvertTool.Bytes2String(xorres), KeyA));
            }

            return DesECBEn(DesECBDe(ConvertTool.Bytes2String(data2), KeyB), KeyA);
        }

        public static string DesMAC(string data, string key, string icv = "0000000000000000")
        {
            string res = DesCBCEn(data, key, icv);
            res = res.Substring(data.Length - 16);
            return res;
        }

        public static string DesECBDe(string cipher, string key)
        {
            cipher = ConvertTool.RemoveSpace(cipher);
            key = ConvertTool.RemoveSpace(key);
           
            if (key.Length != 16)
                throw new Exception("Invalid Key, Not 8 bytes");
            if (cipher.Length % 16 != 0 || cipher.Length == 0)
                throw new Exception("Invalid Cipher, Not 8*n bytes");

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Padding = PaddingMode.None;

            Type t = Type.GetType("System.Security.Cryptography.CryptoAPITransformMode");
            object obj = t.GetField("Decrypt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(t);

            MethodInfo mi = des.GetType().GetMethod("_NewEncryptor", BindingFlags.Instance | BindingFlags.NonPublic);
            ICryptoTransform desCrypt = (ICryptoTransform)mi.Invoke(des, new object[] { ConvertTool.String2Bytes(key), CipherMode.ECB, null, 0, obj });

            byte[] result = desCrypt.TransformFinalBlock(ConvertTool.String2Bytes(cipher), 0, ConvertTool.String2Bytes(cipher).Length);
            return BitConverter.ToString(result).Replace("-", "");

            /*            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                        byte[] inputByteArray = ConvertTool.String2Bytes(cipher);
                        des.Key = ConvertTool.String2Bytes(key);
                        des.Mode = CipherMode.ECB;
                        des.Padding = System.Security.Cryptography.PaddingMode.None;
                        MemoryStream ms = new MemoryStream();
                        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        StringBuilder ret = new StringBuilder();
                        foreach (byte b in ms.ToArray())
                            ret.AppendFormat("{0:X2}", b);

                        return ret.ToString();*/
        }

        public static string TriDesECBEn(string plaintext, string key)
        {
            plaintext = ConvertTool.RemoveSpace(plaintext);
            key = ConvertTool.RemoveSpace(key);

            if (key.Length != 32 && key.Length != 48)
                throw new Exception("Invalid Key, Not 16 or 24 bytes");
            if (plaintext.Length % 16 != 0 || plaintext.Length == 0)
                throw new Exception("Invalid Data, Not 8*n bytes");          

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Padding = PaddingMode.None;
            Type t = Type.GetType("System.Security.Cryptography.CryptoAPITransformMode");
            object obj = t.GetField("Encrypt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(t);

            MethodInfo mi = des.GetType().GetMethod("_NewEncryptor", BindingFlags.Instance | BindingFlags.NonPublic);
            ICryptoTransform desCrypt = (ICryptoTransform)mi.Invoke(des, new object[] { ConvertTool.String2Bytes(key), CipherMode.ECB, null, 0, obj });

            byte[] result = desCrypt.TransformFinalBlock(ConvertTool.String2Bytes(plaintext), 0, (ConvertTool.String2Bytes(plaintext)).Length);
            return BitConverter.ToString(result).Replace("-", "");

            /*            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                        byte[] inputByteArray = ConvertTool.String2Bytes(plaintext);
                        des.Key = ConvertTool.String2Bytes(key);
                        des.Mode = CipherMode.ECB;
                        des.Padding = System.Security.Cryptography.PaddingMode.None;
                        MemoryStream ms = new MemoryStream();
                        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        StringBuilder ret = new StringBuilder();
                        foreach (byte b in ms.ToArray())
                            ret.AppendFormat("{0:X2}", b);

                        return ret.ToString();*/
        }


        public static string TriDesECBDe(string cipher, string key)
        {
            cipher = ConvertTool.RemoveSpace(cipher);
            key = ConvertTool.RemoveSpace(key);

            if (key.Length != 32 && key.Length != 48)
                throw new Exception("Invalid Key, Not 16 or 24 bytes");
            if (cipher.Length % 16 != 0 || cipher.Length == 0)
                throw new Exception("Invalid Cipher, Not 8*n bytes");

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Padding = PaddingMode.None;

            Type t = Type.GetType("System.Security.Cryptography.CryptoAPITransformMode");
            object obj = t.GetField("Decrypt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(t);

            MethodInfo mi = des.GetType().GetMethod("_NewEncryptor", BindingFlags.Instance | BindingFlags.NonPublic);
            ICryptoTransform desCrypt = (ICryptoTransform)mi.Invoke(des, new object[] { ConvertTool.String2Bytes(key), CipherMode.ECB, null, 0, obj });

            byte[] result = desCrypt.TransformFinalBlock(ConvertTool.String2Bytes(cipher), 0, ConvertTool.String2Bytes(cipher).Length);
            return BitConverter.ToString(result).Replace("-", "");

            /*            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                        byte[] inputByteArray = ConvertTool.String2Bytes(cipher);
                        des.Key = ConvertTool.String2Bytes(key);
                        des.Mode = CipherMode.ECB;
                        des.Padding = System.Security.Cryptography.PaddingMode.None;
                        MemoryStream ms = new MemoryStream();
                        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        StringBuilder ret = new StringBuilder();
                        foreach (byte b in ms.ToArray())
                            ret.AppendFormat("{0:X2}", b);

                        return ret.ToString();*/
        }


        public static string DesCBCEn(string plaintext, string key, string icv = "0000000000000000")
        {
            plaintext = ConvertTool.RemoveSpace(plaintext);
            key = ConvertTool.RemoveSpace(key);
            icv = ConvertTool.RemoveSpace(icv);
            
            if (key.Length != 16)
                throw new Exception("Invalid Key, Not 8 bytes");
            if (plaintext.Length % 16 != 0 || plaintext.Length == 0)
                throw new Exception("Invalid Data, Not 8*n bytes");
            if (icv.Length != 16)
                throw new Exception("Invalid IV, Not 8 bytes");

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Padding = PaddingMode.None;
            Type t = Type.GetType("System.Security.Cryptography.CryptoAPITransformMode");
            object obj = t.GetField("Encrypt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(t);

            MethodInfo mi = des.GetType().GetMethod("_NewEncryptor", BindingFlags.Instance | BindingFlags.NonPublic);
            ICryptoTransform desCrypt = (ICryptoTransform)mi.Invoke(des, new object[] { ConvertTool.String2Bytes(key), CipherMode.CBC, ConvertTool.String2Bytes(icv), 0, obj });

            byte[] result = desCrypt.TransformFinalBlock(ConvertTool.String2Bytes(plaintext), 0, (ConvertTool.String2Bytes(plaintext)).Length);
            return BitConverter.ToString(result).Replace("-", "");
            /*
                        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                        byte[] inputByteArray = ConvertTool.String2Bytes(plaintext);
                        des.Key = ConvertTool.String2Bytes(key);
                        des.IV = ConvertTool.String2Bytes(icv);
                        des.Mode = CipherMode.CBC;
                        des.Padding = System.Security.Cryptography.PaddingMode.None;
                        MemoryStream ms = new MemoryStream();
                        CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        StringBuilder ret = new StringBuilder();
                        foreach (byte b in ms.ToArray())
                            ret.AppendFormat("{0:X2}", b);

                        return ret.ToString();*/
        }


        public static string DesCBCDe(string cipher, string key, string icv = "0000000000000000")
        {
            cipher = ConvertTool.RemoveSpace(cipher);
            key = ConvertTool.RemoveSpace(key);
            icv = ConvertTool.RemoveSpace(icv);

            if (key.Length != 16)
                throw new Exception("Invalid Key, Not 8 bytes");
            if (cipher.Length % 16 != 0 || cipher.Length == 0)
                throw new Exception("Invalid Cipher, Not 8*n bytes");
            if (icv.Length != 16)
                throw new Exception("Invalid IV, Not 8 bytes");

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.Padding = PaddingMode.None;

            Type t = Type.GetType("System.Security.Cryptography.CryptoAPITransformMode");
            object obj = t.GetField("Decrypt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(t);

            MethodInfo mi = des.GetType().GetMethod("_NewEncryptor", BindingFlags.Instance | BindingFlags.NonPublic);
            ICryptoTransform desCrypt = (ICryptoTransform)mi.Invoke(des, new object[] { ConvertTool.String2Bytes(key), CipherMode.CBC, ConvertTool.String2Bytes(icv), 0, obj });

            byte[] result = desCrypt.TransformFinalBlock(ConvertTool.String2Bytes(cipher), 0, ConvertTool.String2Bytes(cipher).Length);
            return BitConverter.ToString(result).Replace("-", "");

            /*          DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                      byte[] inputByteArray = ConvertTool.String2Bytes(cipher);
                      des.Key = ConvertTool.String2Bytes(key);
                      des.IV = ConvertTool.String2Bytes(icv);
                      des.Mode = CipherMode.CBC;
                      des.Padding = System.Security.Cryptography.PaddingMode.None;
                      MemoryStream ms = new MemoryStream();
                      CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                      cs.Write(inputByteArray, 0, inputByteArray.Length);
                      cs.FlushFinalBlock();
                      StringBuilder ret = new StringBuilder();
                      foreach (byte b in ms.ToArray())
                          ret.AppendFormat("{0:X2}", b);

                      return ret.ToString();*/
        }

        public static string TriDesCBCEn(string plaintext, string key, string icv = "0000000000000000")
        {
            plaintext = ConvertTool.RemoveSpace(plaintext);
            key = ConvertTool.RemoveSpace(key);
            icv = ConvertTool.RemoveSpace(icv);

            if (key.Length != 32 && key.Length != 48)
                throw new Exception("Invalid Key, Not 16 or 24 bytes");
            if (plaintext.Length % 16 != 0 || plaintext.Length == 0)
                throw new Exception("Invalid Data, Not 8*n bytes");
            if (icv.Length % 16 != 0)
                throw new Exception("Invalid IV, Not 8 bytes");

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Padding = PaddingMode.None;
            Type t = Type.GetType("System.Security.Cryptography.CryptoAPITransformMode");
            object obj = t.GetField("Encrypt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(t);

            MethodInfo mi = des.GetType().GetMethod("_NewEncryptor", BindingFlags.Instance | BindingFlags.NonPublic);
            ICryptoTransform desCrypt = (ICryptoTransform)mi.Invoke(des, new object[] { ConvertTool.String2Bytes(key), CipherMode.CBC, ConvertTool.String2Bytes(icv), 0, obj });

            byte[] result = desCrypt.TransformFinalBlock(ConvertTool.String2Bytes(plaintext), 0, (ConvertTool.String2Bytes(plaintext)).Length);
            return BitConverter.ToString(result).Replace("-", "");

            /*         TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

                     byte[] inputByteArray = ConvertTool.String2Bytes(plaintext);
                     des.Key = ConvertTool.String2Bytes(key);
                     des.IV = ConvertTool.String2Bytes(icv);
                     des.Mode = CipherMode.CBC;
                     des.Padding = System.Security.Cryptography.PaddingMode.None;
                     MemoryStream ms = new MemoryStream();
                     CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                     cs.Write(inputByteArray, 0, inputByteArray.Length);
                     cs.FlushFinalBlock();
                     StringBuilder ret = new StringBuilder();
                     foreach (byte b in ms.ToArray())
                         ret.AppendFormat("{0:X2}", b);

                     return ret.ToString();*/
        }


        public static string TriDesCBCDe(string cipher, string key, string icv = "0000000000000000")
        {
            cipher = ConvertTool.RemoveSpace(cipher);
            key = ConvertTool.RemoveSpace(key);
            icv = ConvertTool.RemoveSpace(icv);

            if (key.Length != 32 && key.Length != 48)
                throw new Exception("Invalid Key, Not 16 or 24 bytes");
            if (cipher.Length % 16 != 0 || cipher.Length == 0)
                throw new Exception("Invalid Cipher, Not 8*n bytes");
            if (icv.Length % 16 != 0)
                throw new Exception("Invalid IV, Not 8 bytes");

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Padding = PaddingMode.None;

            Type t = Type.GetType("System.Security.Cryptography.CryptoAPITransformMode");
            object obj = t.GetField("Decrypt", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(t);

            MethodInfo mi = des.GetType().GetMethod("_NewEncryptor", BindingFlags.Instance | BindingFlags.NonPublic);
            ICryptoTransform desCrypt = (ICryptoTransform)mi.Invoke(des, new object[] { ConvertTool.String2Bytes(key), CipherMode.CBC, ConvertTool.String2Bytes(icv), 0, obj });

            byte[] result = desCrypt.TransformFinalBlock(ConvertTool.String2Bytes(cipher), 0, ConvertTool.String2Bytes(cipher).Length);
            return BitConverter.ToString(result).Replace("-", "");

            /*            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
                        byte[] inputByteArray = ConvertTool.String2Bytes(cipher);
                        des.Key = ConvertTool.String2Bytes(key);
                        des.IV = ConvertTool.String2Bytes(icv);
                        des.Mode = CipherMode.CBC;
                        des.Padding = System.Security.Cryptography.PaddingMode.None;
                        MemoryStream ms = new MemoryStream();
                        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        StringBuilder ret = new StringBuilder();
                        foreach (byte b in ms.ToArray())
                            ret.AppendFormat("{0:X2}", b);

                        return ret.ToString();*/
        }

    }
}
