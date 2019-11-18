using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace RPGGameServer
{
    public class Class_NetworkServer
    {
        private Socket mSocketForListen = null;

        public List<Class_User> mUsers = new List<Class_User>();
        protected bool mThreadLoop = false;
        protected Thread mThread = null;
        
        //락,임계영역 자물쇠( mutex role )
        public Object lockObject = new Object();

        public bool StartServer(int port, int tConnectionNum)
        {
            Console.WriteLine("         StartServer");
            try
            {
                //소켓생성
                mSocketForListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //사용할 포트 번호를 할당.
                //mSocketForListen.Bind(new IPEndPoint(IPAddress.Loopback, port));

                IPAddress ipaddr = IPAddress.Parse("192.168.0.11");

                mSocketForListen.Bind(new IPEndPoint(ipaddr, port));
                //대기를 시작.
                mSocketForListen.Listen(tConnectionNum);
            }
            catch
            {
                Console.WriteLine("StartServer fail");

                return false;
            }
            return LaunchThread();
        }

        public void StopServer()
        {
            //Disconnect();

            if (mSocketForListen != null)
            {
                mSocketForListen.Close();
                mSocketForListen = null;
            }
            Console.WriteLine("Server stopped.");
        }

        //클라이언트 접속
        void AcceptClient()
        {
            if (mSocketForListen != null && mSocketForListen.Poll(0, SelectMode.SelectRead))
            {
                //mUser.mSocketForClient = mSocketForListen.Accept();
                Socket tSocket = mSocketForListen.Accept();

                Console.WriteLine("Connected from client.");

                byte[] tBuffer = new byte[1];
                tBuffer[0] = (byte)PROTOCOL.ACK_CONNECT;
                tSocket.Send(tBuffer, SocketFlags.None);

                Class_User tUser = new Class_User();
                tUser.mSocketForClient = tSocket;
                mUsers.Add(tUser);
            }
        }

        public void Disconnect(Class_User tUser)
        {
            tUser.Disconnect();
        }

        bool LaunchThread()
        {
            try
            {
                mThreadLoop = true;
                mThread = new Thread(new ThreadStart(Dispatch));

                mThread.Start();
            }
            catch
            {
                Console.WriteLine("Cannot launch thread.");
                return false;
            }
            return true;
        }

        public void Dispatch()
        {
            Console.WriteLine("    Start Dispatch thread");

            while (mThreadLoop)
            {
                AcceptClient();

                //mutex
                lock (this.lockObject)
                {
                    foreach (var tUser in mUsers)
                    {
                        if (tUser != null)
                        {
                            if (tUser.mSocketForClient != null)
                            {
                                DispatchSend(tUser);

                                DispatchReceive(tUser);
                            }
                        }
                    }
                }
                Thread.Sleep(1);
            }
            Console.WriteLine("Dispatch thread ended.");
        }

        void DispatchSend(Class_User tUser)
        {
            tUser.DispatchSend();
        }


        void DispatchReceive(Class_User tUser)
        {
            tUser.DispatchReceive();
        }
    }
}
