using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RPGGameServer
{
    public enum READY_PLAY
    {
        NOT_READY = 0,

        READY = 1
    };

    public class Class_User
    {
        public Socket mSocketForClient = null;

        public PacketQueue mSendQueue = null;
        public PacketQueue mRecvQueue = null;

        public int mId = 0;

        public READY_PLAY mReadyPlay = READY_PLAY.NOT_READY;

        public Class_Room mpRoom = null;

        public Class_User()
        {
            CreateRyu();
        }

        void CreateRyu()
        {
            //송수신 패킷 큐를 생성.
            mSendQueue = new PacketQueue();
            mRecvQueue = new PacketQueue();
        }

        public int Send(byte[] data, int size)
        {
            if (mSendQueue == null)
            {
                return 0;
            }

            int tResult = mSendQueue.Enqueue(data, size);
            Console.WriteLine("Send: " + tResult.ToString());

            return tResult;
        }

        //수신처리
        public int GetFromQueue(ref byte[] buffer, int size)
        {
            if (mRecvQueue == null)
            {
                Console.WriteLine("Receive ERROR ryu.");

                return 0;
            }
            int tResult = mRecvQueue.Dequeue(ref buffer, size);
            return tResult;
        }
        public void DispatchSend()
        {
            try
            {
                if (mSocketForClient.Poll(0, SelectMode.SelectWrite))
                {
                    byte[] buffer = new byte[Config.MTU];

                    int sendSize = mSendQueue.Dequeue(ref buffer, buffer.Length);
                    while (sendSize > 0)
                    {
                        mSocketForClient.Send(buffer, sendSize, SocketFlags.None);
                        sendSize = mSendQueue.Dequeue(ref buffer, buffer.Length);
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public void DispatchReceive()
        {
            try
            {
                while (mSocketForClient.Poll(0, SelectMode.SelectRead))
                {
                    byte[] buffer = new byte[Config.MTU];

                    int recvSize = mSocketForClient.Receive(buffer, buffer.Length, SocketFlags.None);
                    if (recvSize == 0)
                    {
                        Console.WriteLine("Disconnect recv from client.");
                        Disconnect();
                    }
                    else if (recvSize > 0)
                    {
                        mRecvQueue.Enqueue(buffer, recvSize);
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public void Disconnect()
        {
            if (mSocketForClient != null)
            {
                mSocketForClient.Shutdown(SocketShutdown.Both);
                mSocketForClient.Close();

                mSocketForClient = null;
            }
        }
    }
}
