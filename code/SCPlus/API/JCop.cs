using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
namespace PCSC
{
    /// <summary>jcop debug</summary>
    public  class JCOP
    {
        private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);  

        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   
        private byte[] atr;
        private byte[] buffer;
        private int port;

        /// <summary>construction function</summary>
        public JCOP()
        {
            this.buffer = new byte[600];
        }
     
        private void TestSocket() {          
            TimeoutObject.Reset();
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port), CallBackMethod, socket);
                      
            if (TimeoutObject.WaitOne(300, false))
            {
                socket.Close();
            }
            else
            {
                socket.Close();
                throw new PCSCException(SCardError.JCOPConnectFailed, "JCOP Connect Failed, You can try:\n" +
                    "  1. set JCOP Remote Port in SmartCard Plus Config, you can get the port through:\n" +
                    "     try execute '/terminal' on JCOP Shell or \n" +
                    "     'Run/Debug As' -> 'Run/Debug Configurations' -> \n" +
                    "     Select a Java Card Application -> 'Target' -> \n" +
                    "     'Configure' -> 'Listen on default port 8050'.\n" +
                    "  2. execute '/close' on JCOP Shell.\n" );  
            }  
        }
        private void CallBackMethod(IAsyncResult asyncresult)
        {                
            TimeoutObject.Set();
        }

        /// <summary>connect port 8050</summary>
        public void connect(int iport)
        {
            this.port = iport;
            TestSocket();
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.LingerState.Enabled = false;
            this.socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
 
            byte[] rst = { 0, 33, 0, 4 };
            socket.Send(rst);
            int len = receive(0);
            this.atr = new byte[len];
            System.Array.Copy(this.buffer, 0, this.atr, 0, len);         
        }

        /// <summary>close port 8050</summary>
        public void stop()
        {
            if(this.socket != null)
            {
                this.socket.Close();
            }
                      
        }

        private void receive(byte[] s, int off, int len)
        {       
            if (this.socket.Receive(s, off, len, SocketFlags.None) < 0)
                throw new PCSCException(SCardError.JCOPReceiveFailed, "JCOP: 0x" + ((int)SCardError.JCOPReceiveFailed).ToString("X8") + ", CUSTOM_ERROR:JCOP Receive Failed.");
        }

        private int receive(int tag)
        {
            receive(this.buffer, 0, 4);
            if ((this.buffer[0] != tag) || (this.buffer[1] != 0))
                throw new PCSCException(SCardError.JCOPResponseFormatError, "JCOP: 0x" + ((int)SCardError.JCOPResponseFormatError).ToString("X8") + ", CUSTOM_ERROR:JCOP Response Format Error.");
          
            int len = ((this.buffer[2] & 0xFF) << 8) + (this.buffer[3] & 0xFF);
            receive(this.buffer, 0, len);
            return len;
        }

        /// <summary>send apdu</summary>
        public byte[] send(byte[] apdu)
        {
            byte[] head = { 1, 0, (byte)(apdu.Length >> 8), (byte)apdu.Length };
            byte[] fapdu = new byte[head.Length + apdu.Length];
            head.CopyTo(fapdu, 0);
            apdu.CopyTo(fapdu, head.Length);
            socket.Send(fapdu);
            int len = receive(1);
            byte[] temp = new byte[len];
            System.Array.Copy(this.buffer, 0, temp, 0, len);
            return temp;
        }

        /// <summary>reset jcop</summary>
        public byte[] reset(int iport)
        {
            if(this.socket !=null)
                stop();
            
            connect(iport);
            return this.atr;
        }
    }
}
