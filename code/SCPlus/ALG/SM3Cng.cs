using System;


namespace ALG
{
    class SM3Cng : System.Security.Cryptography.HashAlgorithm
    {
        /** SM3值的长度 */
        private const int BYTE_LENGTH = 32;

        /** SM3分组长度 */
        private const int BLOCK_LENGTH = 64;

        /** 缓冲区长度 */
        private const int BUFFER_LENGTH = BLOCK_LENGTH * 1;

        /** 缓冲区 */
        private byte[] xBuf = new byte[BUFFER_LENGTH];

        /** 缓冲区偏移量 */
        private int xBufOff;

        /** 初始向量 */
        private byte[] V = { 0x73, (byte) 0x80, 0x16, 0x6f, 0x49,
			0x14, (byte) 0xb2, (byte) 0xb9, 0x17, 0x24, 0x42, (byte) 0xd7,
			(byte) 0xda, (byte) 0x8a, 0x06, 0x00, (byte) 0xa9, 0x6f, 0x30,
			(byte) 0xbc, (byte) 0x16, 0x31, 0x38, (byte) 0xaa, (byte) 0xe3,
			(byte) 0x8d, (byte) 0xee, 0x4d, (byte) 0xb0, (byte) 0xfb, 0x0e,
			0x4e };

        private int cntBlock = 0;

        public SM3Cng()
        {
        }

        public SM3Cng(SM3Cng t)
        {
            Buffer.BlockCopy(t.xBuf, 0, this.xBuf, 0, t.xBuf.Length);
            this.xBufOff = t.xBufOff;
            Buffer.BlockCopy(t.V, 0, this.V, 0, t.V.Length);
        }

        public override void Initialize()
        {
            xBufOff = 0;
            cntBlock = 0;
            V = SM3Base.iv;
            HashValue = null;
        }

        protected override byte[] HashFinal()
        {
            byte[] B = new byte[BLOCK_LENGTH];
            byte[] buffer = new byte[xBufOff];
            Buffer.BlockCopy(xBuf, 0, buffer, 0, buffer.Length);
            byte[] tmp = SM3Base.padding(buffer, cntBlock);
            for (int i = 0; i < tmp.Length; i += BLOCK_LENGTH)
            {
                Buffer.BlockCopy(tmp, i, B, 0, B.Length);
                doHash(B);
            }
            return V;
        }
        
        public void reset()
        {
            xBufOff = 0;
            cntBlock = 0;
            V = SM3Base.iv;
        }

        /**
         * 明文输入
         *
         * @param in
         *            明文输入缓冲区
         * @param inOff
         *            缓冲区偏移量
         * @param len
         *            明文长度
         */
        protected override void HashCore(byte[] data, int inOff, int len)
        {
            int partLen = BUFFER_LENGTH - xBufOff;
            int inputLen = len;
            int dPos = inOff;
            if (partLen < inputLen)
            {
                Buffer.BlockCopy(data, dPos, xBuf, xBufOff, partLen);
                inputLen -= partLen;
                dPos += partLen;
                doUpdate();
                while (inputLen > BUFFER_LENGTH)
                {
                    Buffer.BlockCopy(data, dPos, xBuf, 0, BUFFER_LENGTH);
                    inputLen -= BUFFER_LENGTH;
                    dPos += BUFFER_LENGTH;
                    doUpdate();
                }
            }

            Buffer.BlockCopy(data, dPos, xBuf, xBufOff, inputLen);
            xBufOff += inputLen;
        }

        private void doUpdate()
        {
            byte[] B = new byte[BLOCK_LENGTH];
            for (int i = 0; i < BUFFER_LENGTH; i += BLOCK_LENGTH)
            {
                Buffer.BlockCopy(xBuf, i, B, 0, B.Length);
                doHash(B);
            }
            xBufOff = 0;
        }

        private void doHash(byte[] B)
        {
            byte[] tmp = SM3Base.CF(V, B);
            Buffer.BlockCopy(tmp, 0, V, 0, V.Length);
            cntBlock++;
        }

        public int getDigestSize()
        {
            return BYTE_LENGTH;
        }

    }
}
