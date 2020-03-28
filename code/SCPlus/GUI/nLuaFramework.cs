using GUI;
using NLua;
using System;
using System.Reflection;
namespace nLuaFramework
{
    public class NLuaFunction : Attribute
    {
        private readonly String FunctionName;

        public NLuaFunction(String strFuncName)
        {
            FunctionName = strFuncName;
        }

        public String GetFuncName()
        {
            return FunctionName;
        }
    }


    public class LuaFramework : IDisposable
    {     
        private Lua pLuaVM = new Lua();

        public void BindLuaApiClass(Object pLuaAPIClass)
        {
            foreach (MethodInfo mInfo in pLuaAPIClass.GetType().GetMethods())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                {
                    if (attr is NLuaFunction)
                    {
                        string LuaFunctionName = (attr as NLuaFunction).GetFuncName();
                        pLuaVM.RegisterFunction(LuaFunctionName, pLuaAPIClass, mInfo);
                    }
                }
            }
        }


        public void ExecuteFile(string luaFileName)
        {
            try
            {              
                pLuaVM.DoFile(luaFileName);
            }
            catch (Exception e)
            {
                string info = "";
                if(e.Source != "")
                    info = e.Source + "\n";
                if (e.Message != "")
                    info += e.Message;
                MainWindow.errPrint(info);                             
            }
        }

        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~LuaFramework()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing) {
            if (pLuaVM != null)
                pLuaVM.Dispose();
        }
    }
}

