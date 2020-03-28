using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
    [ComVisible(true)]
    public abstract class SHA224 : HashAlgorithm
    {
        protected SHA224()
        {
            this.HashSizeValue = 224;
        }
       
    }
}
