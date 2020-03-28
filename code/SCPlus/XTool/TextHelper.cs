using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace xTool
{
    public static class TextHelper
    {
        public static string GetCurrentDir(string file)
        {
            return file.Substring(0, file.LastIndexOf("\\"));
        }

        public static string getScript(string file)
        {
            string str_script = string.Empty;
            string result;
            try
            {               
                str_script = File.ReadAllText(file);
            }
            catch (Exception)
            {
                str_script = "-";
            }
            result = str_script;
            return result;
        }

        public static string GetAbsolutePath(string casepath, string includepath)
        {
            string dir = TextHelper.GetCurrentDir(casepath);
            string temppath = includepath.Replace("/", "\\");
            dir = dir + "\\" + temppath;
            FileInfo fi = new FileInfo(dir);
            if (fi.Exists)
            {
                return fi.FullName;
            }
            throw new Exception("include " + includepath + " failed");
        }

        public static string GetLocalFilePath(string casepath, string includepath)
        {
            string localpath = string.Empty;
            casepath = TextHelper.GetCurrentDir(casepath);
            string[] SplitPath = includepath.Replace("/", "\\").Split(new string[]
			{
				"..\\"
			}, StringSplitOptions.None);
            localpath = SplitPath[SplitPath.Length - 1];
            FileInfo fi = null;
            for (int i = SplitPath.Length; i > 0; i--)
            {
                fi = new FileInfo(casepath + "\\" + localpath);
                if (fi.Exists)
                {
                    break;
                }
                localpath = "..\\" + localpath;
            }
            if (fi != null)
            {
                return fi.FullName;
            }
            throw new Exception("include " + includepath + " failed");
        }

        
    }
}
