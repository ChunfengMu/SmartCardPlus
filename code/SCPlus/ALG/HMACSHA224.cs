
using System.Security.Cryptography;

namespace ALG
{
    public class HMACSHA224:HMACSHA224Base
    {
        public HMACSHA224(byte[] key)
		{		
			base.InitializeKey(key);
		}
    }
}
