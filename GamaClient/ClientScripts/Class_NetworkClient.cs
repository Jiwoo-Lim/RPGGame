using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Class_NetworkClient
{
    private static Class_NetworkClient mInstance = null;

    //EndPoint
    private string mServerIPAddress = "";
    private int mPort = 0;

    //socket, is connect
    private Socket mSocketForClient = null;
    private bool mIsConnected = false;

    //thread
    protected bool mThreadLoop = false;
    protected Thread mThread = null;

    private static int MTU = 1400;

    //network-data queue
    private PacketQueue mSendQueue = null;
    private PacketQueue mRecvQueue = null;

    public Class_UserInfo mMyUserInfo = null;
    public List<Class_UserInfo> mUserInfoes = new List<Class_UserInfo>();

    public int mRoomId = 0;

    private Class_NetworkClient()
    {
        mInstance = null;
    }

    public static Class_NetworkClient GetInst()
    {
        if (mInstance == null)
        {
            mInstance = new Class_NetworkClient();
        }

        return mInstance;
    }

    public void CreateRyu()
    {
        mServerIPAddress = "127.0.0.1";
        mPort = 50765;

        mSendQueue = new PacketQueue();
        mRecvQueue = new PacketQueue();
    }

    public bool Connect(string address, int port)
    {
        bool tResult = false;
        try
        {
            mSocketForClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mSocketForClient.NoDelay = true;	//Nagle 알고리즘을 off한다.
            mSocketForClient.Connect(address, port);

            Debug.Log("TransportTCP connect called.]]]]]]   " + address + ", " + port);

            tResult = LaunchThread();
        }
        catch
        {
            mSocketForClient = null;
        }

        if (tResult == true)
        {
            mIsConnected = true;
            Debug.Log("Connection success.");
        }
        else
        {
            mIsConnected = false;
            Debug.Log("Connect fail");
        }

        return mIsConnected;
    }

    // 끊기. 
    public void Disconnect()
    {
        mIsConnected = false;

        if (mSocketForClient != null)
        {
            // 소켓 클로즈.
            mSocketForClient.Shutdown(SocketShutdown.Both);
            mSocketForClient.Close();
            mSocketForClient = null;
        }

    }


    public int Send(byte[] data, int size)
    {
        if (mSendQueue == null)
        {
            return 0;
        }

        int tResult = mSendQueue.Enqueue(data, size);
        return tResult;
    }

    public int GetFromQueue(ref byte[] buffer, int size)
    {
        if (mRecvQueue == null)
        {
            Debug.Log("Receive ERROR ryu.");

            return 0;
        }

        int tResult = mRecvQueue.Dequeue(ref buffer, size);
        return tResult;
    }


    public bool IsConnected()
    {
        return mIsConnected;
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
            Debug.Log("Cannot launch thread.");

            return false;
        }

        return true;
    }

    public void Dispatch()
    {
        Debug.Log("Dispatch thread started.");

        while (mThreadLoop)
        {
            if (mSocketForClient != null && mIsConnected == true)
            {
                DispatchSend();

                DispatchReceive();
            }

            Thread.Sleep(5);
        }

        Debug.Log("Dispatch thread ended.");
    }

    void DispatchSend()
    {
        try
        {
            if (mSocketForClient.Poll(0, SelectMode.SelectWrite))
            {
                byte[] buffer = new byte[MTU];

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

    void DispatchReceive()
    {
        try
        {
            while (mSocketForClient.Poll(0, SelectMode.SelectRead))
            {
                byte[] buffer = new byte[MTU];

                int recvSize = mSocketForClient.Receive(buffer, buffer.Length, SocketFlags.None);
                if (recvSize == 0)
                {
                    // 끊기.
                    Debug.Log("Disconnect recv from client.");

                    Disconnect();
                }
                else if (recvSize > 0)
                {
                    Debug.Log("recvSize " + recvSize);

                    mRecvQueue.Enqueue(buffer, recvSize);
                }
            }
        }
        catch
        {
            return;
        }
    }

    public void CleanTurn()
    {
        foreach (var u in mUserInfoes)
        {
            u.mIsMyTurn = false;
        }
    }
}