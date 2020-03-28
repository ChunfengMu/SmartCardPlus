using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALG
{
    public class SM4
    {
        public static byte[] Encrypt_ECB(byte[] plainBytes, byte[] keyBytes)
        {
            SM4_Context ctx = new SM4_Context();

            SM4Base sm4 = new SM4Base();
            sm4.sm4_setkey_enc(ctx, keyBytes);
            byte[] encrypted = sm4.sm4_crypt_ecb(ctx, plainBytes);
            return encrypted;
        }

        public static byte[] Decrypt_ECB(byte[] plainBytes, byte[] keyBytes)
        {
            SM4_Context ctx = new SM4_Context();

            SM4Base sm4 = new SM4Base();
            sm4.sm4_setkey_dec(ctx, keyBytes);
            byte[] encrypted = sm4.sm4_crypt_ecb(ctx, plainBytes);
            return encrypted;
        }

        public static byte[] Encrypt_CBC(byte[] plainBytes, byte[] keyBytes, byte[] ivBytes)
        {
            SM4_Context ctx = new SM4_Context();

            SM4Base sm4 = new SM4Base();
            sm4.sm4_setkey_enc(ctx, keyBytes);
            byte[] encrypted = sm4.sm4_crypt_cbc(ctx, ivBytes, plainBytes);
            return encrypted;
        }

        public static byte[] Decrypt_CBC(byte[] plainBytes, byte[] keyBytes, byte[] ivBytes)
        {
            SM4_Context ctx = new SM4_Context();

            SM4Base sm4 = new SM4Base();
            sm4.sm4_setkey_dec(ctx, keyBytes);
            byte[] encrypted = sm4.sm4_crypt_cbc(ctx, ivBytes, plainBytes);
            return encrypted;
        }

    }
}
