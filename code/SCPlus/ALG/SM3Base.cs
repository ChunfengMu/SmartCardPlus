using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ALG
{


    public class SM3Base
    {
        /*public static final byte[] iv = { 0x2C, (byte) 0x91, (byte) 0xB4, 0x01,
                (byte) 0xFC, 0x64, (byte) 0xB2, (byte) 0xCE, 0x7C, 0x4E,
                (byte) 0xAE, (byte) 0xFB, (byte) 0xB1, 0x3B, (byte) 0xB6,
                (byte) 0xD3, 0x17, 0x60, (byte) 0xB6, 0x35, (byte) 0xF3, 0x6F,
                0x13, (byte) 0xEB, (byte) 0xC8, 0x77, (byte) 0xE9, (byte) 0xA0,
                (byte) 0xC2, 0x76, (byte) 0xA8, 0x17 };*/

        public static byte[] iv = { 0x73, (byte) 0x80, 0x16, 0x6f, 0x49,
			0x14, (byte) 0xb2, (byte) 0xb9, 0x17, 0x24, 0x42, (byte) 0xd7,
			(byte) 0xda, (byte) 0x8a, 0x06, 0x00, (byte) 0xa9, 0x6f, 0x30,
			(byte) 0xbc, (byte) 0x16, 0x31, 0x38, (byte) 0xaa, (byte) 0xe3,
			(byte) 0x8d, (byte) 0xee, 0x4d, (byte) 0xb0, (byte) 0xfb, 0x0e,
			0x4e };

        public static int[] Tj = { 
                                 0x79cc4519, 0x79cc4519, 0x79cc4519, 0x79cc4519,
                                 0x79cc4519, 0x79cc4519, 0x79cc4519, 0x79cc4519,
                                 0x79cc4519, 0x79cc4519, 0x79cc4519, 0x79cc4519,
                                 0x79cc4519, 0x79cc4519, 0x79cc4519, 0x79cc4519,

                                 0x7a879d8a, 0x7a879d8a, 0x7a879d8a, 0x7a879d8a,
                                 0x7a879d8a, 0x7a879d8a, 0x7a879d8a, 0x7a879d8a,
                                 0x7a879d8a, 0x7a879d8a, 0x7a879d8a, 0x7a879d8a,
                                 0x7a879d8a, 0x7a879d8a, 0x7a879d8a, 0x7a879d8a,
                                 0x7a879d8a, 0x7a879d8a, 0x7a879d8a, 0x7a879d8a,
                                 0x7a879d8a, 0x7a879d8a, 0x7a879d8a, 0x7a879d8a,
                                 0x7a879d8a, 0x7a879d8a, 0x7a879d8a, 0x7a879d8a,
                                 0x7a879d8a, 0x7a879d8a, 0x7a879d8a, 0x7a879d8a,
                                 0x7a879d8a, 0x7a879d8a, 0x7a879d8a, 0x7a879d8a,
                                 0x7a879d8a, 0x7a879d8a, 0x7a879d8a, 0x7a879d8a,
                                 0x7a879d8a, 0x7a879d8a, 0x7a879d8a, 0x7a879d8a,
                                 0x7a879d8a, 0x7a879d8a, 0x7a879d8a, 0x7a879d8a,
                             };

        //static
        //{
        //    for (int i = 0; i < 16; i++)
        //    {
        //        Tj[i] = 0x79cc4519;
        //    }

        //    for (int i = 16; i < 64; i++)
        //    {
        //        Tj[i] = 0x7a879d8a;
        //    }
        //}

        public static byte[] CF(byte[] V, byte[] B)
        {
            int[] v, b;
            v = convert(V);
            b = convert(B);
            return convert(CF(v, b));
        }

        private static int[] convert(byte[] arr)
        {
            int[] RES = new int[arr.Length / 4];
            //byte[] tmp = new byte[4];
            //for (int i = 0; i < arr.Length; i += 4)
            //{
            //    Buffer.BlockCopy(arr, i, tmp, 0, 4);
            //    RES[i / 4] = bigEndianByteToInt(tmp);
            //}
            //return RES;

            DWORDFromBigEndian(RES, arr.Length / 4, arr);
            return RES;
        }

        private static byte[] convert(int[] arr)
        {
            byte[] RES = new byte[arr.Length * 4];
            //byte[] tmp = null;
            //for (int i = 0; i < arr.Length; i++)
            //{
            //    tmp = bigEndianIntToByte(arr[i]);
            //    Buffer.BlockCopy(tmp, 0, RES, i * 4, 4);
            //}
            //return RES;

            DWORDToBigEndian(RES, arr, arr.Length);
            return RES;
        }

        public static int[] CF(int[] V, int[] B)
        {
            int a, b, c, d, e, f, g, h;
            int ss1, ss2, tt1, tt2;
            a = V[0];
            b = V[1];
            c = V[2];
            d = V[3];
            e = V[4];
            f = V[5];
            g = V[6];
            h = V[7];

            int[][] arr = expand(B);
            int[] w = arr[0];
            int[] w1 = arr[1];

            for (int j = 0; j < 64; j++)
            {
                ss1 = (bitCycleLeft(a, 12) + e + bitCycleLeft(Tj[j], j));
                ss1 = bitCycleLeft(ss1, 7);
                ss2 = ss1 ^ bitCycleLeft(a, 12);
                tt1 = FFj(a, b, c, j) + d + ss2 + w1[j];
                tt2 = GGj(e, f, g, j) + h + ss1 + w[j];
                d = c;
                c = bitCycleLeft(b, 9);
                b = a;
                a = tt1;
                h = g;
                g = bitCycleLeft(f, 19);
                f = e;
                e = P0(tt2);

            }

            int[] RES = new int[8];
            RES[0] = a ^ V[0];
            RES[1] = b ^ V[1];
            RES[2] = c ^ V[2];
            RES[3] = d ^ V[3];
            RES[4] = e ^ V[4];
            RES[5] = f ^ V[5];
            RES[6] = g ^ V[6];
            RES[7] = h ^ V[7];

            return RES;
        }

        private static int[][] expand(int[] B)
        {
            int[] W = new int[68];
            int[] W1 = new int[64];
            for (int i = 0; i < B.Length; i++)
            {
                W[i] = B[i];
            }

            for (int i = 16; i < 68; i++)
            {
                W[i] = P1(W[i - 16] ^ W[i - 9] ^ bitCycleLeft(W[i - 3], 15))
                        ^ bitCycleLeft(W[i - 13], 7) ^ W[i - 6];
            }

            for (int i = 0; i < 64; i++)
            {
                W1[i] = W[i] ^ W[i + 4];
            }

            int[][] arr = new int[][] { W, W1 };
            return arr;
        }



        //[SecurityCritical]
        internal unsafe static void DWORDFromBigEndian(int[] x, int digits, byte[] block)
        {
            int i = 0;
            int num = 0;
            while (i < digits)
            {
                x[i] = (int)((int)block[num] << 24 | (int)block[num + 1] << 16 | (int)block[num + 2] << 8 | (int)block[num + 3]);
                i++;
                num += 4;
            }
        }

        internal static void DWORDToBigEndian(byte[] block, int[] x, int digits)
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




        private static byte[] bigEndianIntToByte(int num)
        {
            return back(intToBytes(num));
        }

        private static int bigEndianByteToInt(byte[] bytes)
        {
            return byteToInt(back(bytes));
        }

        private static int FFj(int X, int Y, int Z, int j)
        {
            if (j >= 0 && j <= 15)
            {
                return FF1j(X, Y, Z);
            }
            else
            {
                return FF2j(X, Y, Z);
            }
        }

        private static int GGj(int X, int Y, int Z, int j)
        {
            if (j >= 0 && j <= 15)
            {
                return GG1j(X, Y, Z);
            }
            else
            {
                return GG2j(X, Y, Z);
            }
        }

        // 逻辑位运算函数
        private static int FF1j(int X, int Y, int Z)
        {
            int tmp = X ^ Y ^ Z;
            return tmp;
        }

        private static int FF2j(int X, int Y, int Z)
        {
            int tmp = ((X & Y) | (X & Z) | (Y & Z));
            return tmp;
        }

        private static int GG1j(int X, int Y, int Z)
        {
            int tmp = X ^ Y ^ Z;
            return tmp;
        }

        private static int GG2j(int X, int Y, int Z)
        {
            int tmp = (X & Y) | (~X & Z);
            return tmp;
        }

        private static int P0(int X)
        {
            int y = rotateLeft(X, 9);
            y = bitCycleLeft(X, 9);
            int z = rotateLeft(X, 17);
            z = bitCycleLeft(X, 17);
            int t = X ^ y ^ z;
            return t;
        }

        private static int P1(int X)
        {
            int t = X ^ bitCycleLeft(X, 15) ^ bitCycleLeft(X, 23);
            return t;
        }

        /**
         * 对最后一个分组字节数据padding
         *
         * @param in
         * @param bLen
         *            分组个数
         * @return
         */
        public static byte[] padding(byte[] data, int bLen)
        {
            int k = 448 - (8 * data.Length + 1) % 512;
            if (k < 0)
            {
                k = 960 - (8 * data.Length + 1) % 512;
            }
            k += 1;
            byte[] padd = new byte[k / 8];
            padd[0] = (byte)0x80;
            long n = data.Length * 8 + bLen * 512;
            byte[] RES = new byte[data.Length + k / 8 + 64 / 8];
            int pos = 0;
            Buffer.BlockCopy(data, 0, RES, 0, data.Length);
            pos += data.Length;
            Buffer.BlockCopy(padd, 0, RES, pos, padd.Length);
            pos += padd.Length;
            byte[] tmp = back(longToBytes(n));
            Buffer.BlockCopy(tmp, 0, RES, pos, tmp.Length);
            return RES;
        }

        /**
         * 字节数组逆序
         *
         * @param in
         * @return
         */
        private static byte[] back(byte[] data)
        {
            Array.Reverse(data);
            
            return data; ;
        }

        public static int rotateLeft(int x, int n)
        {
            return (x << n) | (x >> (32 - n));
        }

        private static int bitCycleLeft(int n, int bitLen)
        {
            bitLen %= 32;
            byte[] tmp = bigEndianIntToByte(n);
            int byteLen = bitLen / 8;
            int len = bitLen % 8;
            if (byteLen > 0)
            {
                tmp = byteCycleLeft(tmp, byteLen);
            }

            if (len > 0)
            {
                tmp = bitSmall8CycleLeft(tmp, len);
            }

            return bigEndianByteToInt(tmp);
        }

        private static byte[] bitSmall8CycleLeft(byte[] data, int len)
        {
            byte[] tmp = new byte[data.Length];
            int t1, t2, t3;
            for (int i = 0; i < tmp.Length; i++)
            {
                t1 = (byte)((data[i] & 0x000000ff) << len);
                t2 = (byte)((data[(i + 1) % tmp.Length] & 0x000000ff) >> (8 - len));
                t3 = (byte)(t1 | t2);
                tmp[i] = (byte)t3;
            }

            return tmp;
        }

        private static byte[] byteCycleLeft(byte[] data, int byteLen)
        {
            byte[] tmp = new byte[data.Length];
            Buffer.BlockCopy(data, byteLen, tmp, 0, data.Length - byteLen);
            Buffer.BlockCopy(data, 0, tmp, data.Length - byteLen, byteLen);
            return tmp;
        }

        public static byte[] intToBytes(int num)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(0xff & (num >> 0));
            bytes[1] = (byte)(0xff & (num >> 8));
            bytes[2] = (byte)(0xff & (num >> 16));
            bytes[3] = (byte)(0xff & (num >> 24));
            return bytes;
        }

        /**
         * 四个字节的字节数据转换成一个整形数据
         *
         * @param bytes 4个字节的字节数组
         * @return 一个整型数据
         */
        public static int byteToInt(byte[] bytes)
        {
            int num = 0;
            int temp;
            temp = (0x000000ff & (bytes[0])) << 0;
            num = num | temp;
            temp = (0x000000ff & (bytes[1])) << 8;
            num = num | temp;
            temp = (0x000000ff & (bytes[2])) << 16;
            num = num | temp;
            temp = (0x000000ff & (bytes[3])) << 24;
            num = num | temp;
            return num;
        }

        public static byte[] longToBytes(long num)
        {
            byte[] bytes = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                bytes[i] = (byte)(0xff & (num >> (i * 8)));
            }

            return bytes;
        }
    }

}
