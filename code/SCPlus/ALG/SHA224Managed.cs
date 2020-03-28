using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
    [ComVisible(true)]
    public class SHA224Managed : SHA224
    {
        public byte[] HashValueSHA224;

        private byte[] _buffer;

        private long _count;

        private uint[] _stateSHA224;

        private uint[] _W;
   

        private static readonly uint[] _K = new uint[]
		{
			1116352408u,
			1899447441u,
			3049323471u,
			3921009573u,
			961987163u,
			1508970993u,
			2453635748u,
			2870763221u,
			3624381080u,
			310598401u,
			607225278u,
			1426881987u,
			1925078388u,
			2162078206u,
			2614888103u,
			3248222580u,
			3835390401u,
			4022224774u,
			264347078u,
			604807628u,
			770255983u,
			1249150122u,
			1555081692u,
			1996064986u,
			2554220882u,
			2821834349u,
			2952996808u,
			3210313671u,
			3336571891u,
			3584528711u,
			113926993u,
			338241895u,
			666307205u,
			773529912u,
			1294757372u,
			1396182291u,
			1695183700u,
			1986661051u,
			2177026350u,
			2456956037u,
			2730485921u,
			2820302411u,
			3259730800u,
			3345764771u,
			3516065817u,
			3600352804u,
			4094571909u,
			275423344u,
			430227734u,
			506948616u,
			659060556u,
			883997877u,
			958139571u,
			1322822218u,
			1537002063u,
			1747873779u,
			1955562222u,
			2024104815u,
			2227730452u,
			2361852424u,
			2428436474u,
			2756734187u,
			3204031479u,
			3329325298u
		};

        public SHA224Managed()
        {
            if (CryptoConfig.AllowOnlyFipsAlgorithms)
            {
                throw new InvalidOperationException("Cryptography_NonCompliantFIPSAlgorithm");
            }
            this.HashSizeValue = 224;
            this._stateSHA224 = new uint[8];
            this._buffer = new byte[64];
            this._W = new uint[64];
            this.InitializeState();
        }

        public override void Initialize()
        {
            this.InitializeState();
            Array.Clear(this._buffer, 0, this._buffer.Length);
            Array.Clear(this._W, 0, this._W.Length);
        }

        protected override void HashCore(byte[] rgb, int ibStart, int cbSize)
        {
            this._HashData(rgb, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            return this._EndHash();
        }

        private void InitializeState()
        {
            this._count = 0L;
            this._stateSHA224[0] = 0xc1059ed8u;
            this._stateSHA224[1] = 0x367cd507u;
            this._stateSHA224[2] = 0x3070dd17u;
            this._stateSHA224[3] = 0xf70e5939u;
            this._stateSHA224[4] = 0xffc00b31u;
            this._stateSHA224[5] = 0x68581511u;
            this._stateSHA224[6] = 0x64f98fa7u;
            this._stateSHA224[7] = 0xbefa4fa4u;
        }

        [SecuritySafeCritical]
        private unsafe void _HashData(byte[] partIn, int ibStart, int cbSize)
        {
            int i = cbSize;
            int num = ibStart;
            int num2 = (int)(this._count & 63L);
            this._count += (long)i;
            fixed (uint* stateSHA = this._stateSHA224)
            {
                fixed (byte* buffer = this._buffer)
                {
                    fixed (uint* w = this._W)
                    {
                        if (num2 > 0 && num2 + i >= 64)
                        {
                            Buffer.BlockCopy(partIn, num, this._buffer, num2, 64 - num2);
                            num += 64 - num2;
                            i -= 64 - num2;
                            SHA224Managed.SHATransform(w, stateSHA, buffer);
                            num2 = 0;
                        }
                        while (i >= 64)
                        {
                            Buffer.BlockCopy(partIn, num, this._buffer, 0, 64);
                            num += 64;
                            i -= 64;
                            SHA224Managed.SHATransform(w, stateSHA, buffer);
                        }
                        if (i > 0)
                        {
                            Buffer.BlockCopy(partIn, num, this._buffer, num2, i);
                        }
                    }
                }
            }
        }

        [SecurityCritical]
        internal unsafe static void DWORDFromBigEndian(uint* x, int digits, byte* block)
        {
            int i = 0;
            int num = 0;
            while (i < digits)
            {
                x[i] = (uint)((int)block[num] << 24 | (int)block[num + 1] << 16 | (int)block[num + 2] << 8 | (int)block[num + 3]);
                i++;
                num += 4;
            }
        }

        internal static void DWORDToBigEndian(byte[] block, uint[] x, int digits)
        {
            int i = 0;
            int num = 0;
            while (i < digits)
            {
                block[num] = (byte)(x[i] >> 24 & 255u);
                block[num + 1] = (byte)(x[i] >> 16 & 255u);
                block[num + 2] = (byte)(x[i] >> 8 & 255u);
                block[num + 3] = (byte)(x[i] & 255u);
                i++;
                num += 4;
            }
        }


        private byte[] _EndHash()
        {
            byte[] array = new byte[28];
            int num = 64 - (int)(this._count & 63L);
            if (num <= 8)
            {
                num += 64;
            }
            byte[] array2 = new byte[num];
            array2[0] = 128;
            long num2 = this._count * 8L;
            array2[num - 8] = (byte)(num2 >> 56 & 255L);
            array2[num - 7] = (byte)(num2 >> 48 & 255L);
            array2[num - 6] = (byte)(num2 >> 40 & 255L);
            array2[num - 5] = (byte)(num2 >> 32 & 255L);
            array2[num - 4] = (byte)(num2 >> 24 & 255L);
            array2[num - 3] = (byte)(num2 >> 16 & 255L);
            array2[num - 2] = (byte)(num2 >> 8 & 255L);
            array2[num - 1] = (byte)(num2 & 255L);
            this._HashData(array2, 0, array2.Length);
            DWORDToBigEndian(array, this._stateSHA224, 7);
            this.HashValue = array;
            HashValueSHA224 = array;
            return array;
        }

        [SecurityCritical]
        private unsafe static void SHATransform(uint* expandedBuffer, uint* state, byte* block)
        {
            uint num = *state;
            uint num2 = state[1];
            uint num3 = state[2];
            uint num4 = state[3];
            uint num5 = state[4];
            uint num6 = state[5];
            uint num7 = state[6];
            uint num8 = state[7];
            DWORDFromBigEndian(expandedBuffer, 16, block);
            SHA224Managed.SHA256Expand(expandedBuffer);
            for (int i = 0; i < 64; i++)
            {
                uint num9 = num8 + SHA224Managed.Sigma_1(num5) + SHA224Managed.Ch(num5, num6, num7) + SHA224Managed._K[i] + expandedBuffer[i];
                uint num10 = num4 + num9;
                uint num11 = num9 + SHA224Managed.Sigma_0(num) + SHA224Managed.Maj(num, num2, num3);
                i++;
                num9 = num7 + SHA224Managed.Sigma_1(num10) + SHA224Managed.Ch(num10, num5, num6) + SHA224Managed._K[i] + expandedBuffer[i];
                uint num12 = num3 + num9;
                uint num13 = num9 + SHA224Managed.Sigma_0(num11) + SHA224Managed.Maj(num11, num, num2);
                i++;
                num9 = num6 + SHA224Managed.Sigma_1(num12) + SHA224Managed.Ch(num12, num10, num5) + SHA224Managed._K[i] + expandedBuffer[i];
                uint num14 = num2 + num9;
                uint num15 = num9 + SHA224Managed.Sigma_0(num13) + SHA224Managed.Maj(num13, num11, num);
                i++;
                num9 = num5 + SHA224Managed.Sigma_1(num14) + SHA224Managed.Ch(num14, num12, num10) + SHA224Managed._K[i] + expandedBuffer[i];
                uint num16 = num + num9;
                uint num17 = num9 + SHA224Managed.Sigma_0(num15) + SHA224Managed.Maj(num15, num13, num11);
                i++;
                num9 = num10 + SHA224Managed.Sigma_1(num16) + SHA224Managed.Ch(num16, num14, num12) + SHA224Managed._K[i] + expandedBuffer[i];
                num8 = num11 + num9;
                num4 = num9 + SHA224Managed.Sigma_0(num17) + SHA224Managed.Maj(num17, num15, num13);
                i++;
                num9 = num12 + SHA224Managed.Sigma_1(num8) + SHA224Managed.Ch(num8, num16, num14) + SHA224Managed._K[i] + expandedBuffer[i];
                num7 = num13 + num9;
                num3 = num9 + SHA224Managed.Sigma_0(num4) + SHA224Managed.Maj(num4, num17, num15);
                i++;
                num9 = num14 + SHA224Managed.Sigma_1(num7) + SHA224Managed.Ch(num7, num8, num16) + SHA224Managed._K[i] + expandedBuffer[i];
                num6 = num15 + num9;
                num2 = num9 + SHA224Managed.Sigma_0(num3) + SHA224Managed.Maj(num3, num4, num17);
                i++;
                num9 = num16 + SHA224Managed.Sigma_1(num6) + SHA224Managed.Ch(num6, num7, num8) + SHA224Managed._K[i] + expandedBuffer[i];
                num5 = num17 + num9;
                num = num9 + SHA224Managed.Sigma_0(num2) + SHA224Managed.Maj(num2, num3, num4);
            }
            *state += num;
            state[1] += num2;
            state[2] += num3;
            state[3] += num4;
            state[4] += num5;
            state[5] += num6;
            state[6] += num7;
            state[7] += num8;
        }

        private static uint RotateRight(uint x, int n)
        {
            return x >> n | x << 32 - n;
        }

        private static uint Ch(uint x, uint y, uint z)
        {
            return (x & y) ^ ((x ^ 4294967295u) & z);
        }

        private static uint Maj(uint x, uint y, uint z)
        {
            return (x & y) ^ (x & z) ^ (y & z);
        }

        private static uint sigma_0(uint x)
        {
            return SHA224Managed.RotateRight(x, 7) ^ SHA224Managed.RotateRight(x, 18) ^ x >> 3;
        }

        private static uint sigma_1(uint x)
        {
            return SHA224Managed.RotateRight(x, 17) ^ SHA224Managed.RotateRight(x, 19) ^ x >> 10;
        }

        private static uint Sigma_0(uint x)
        {
            return SHA224Managed.RotateRight(x, 2) ^ SHA224Managed.RotateRight(x, 13) ^ SHA224Managed.RotateRight(x, 22);
        }

        private static uint Sigma_1(uint x)
        {
            return SHA224Managed.RotateRight(x, 6) ^ SHA224Managed.RotateRight(x, 11) ^ SHA224Managed.RotateRight(x, 25);
        }

        [SecurityCritical]
        private unsafe static void SHA256Expand(uint* x)
        {
            for (int i = 16; i < 64; i++)
            {
                x[i] = SHA224Managed.sigma_1(x[i - 2]) + x[i - 7] + SHA224Managed.sigma_0(x[i - 15]) + x[i - 16];
            }
        }
    }
}
