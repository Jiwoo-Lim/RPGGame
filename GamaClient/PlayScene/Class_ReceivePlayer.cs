using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Class_ReceivePlayer : MonoBehaviour
{
    private Class_Warrior mpWarrior = null;
    private Class_Wizard mpWizard = null;

    //receiving Thread
    Thread receiveThread;

    //udpclient object
    UdpClient client;

    private int port;

    Vector3 tReceivePlayerPos = Vector3.zero;
    Quaternion tReceivePlayerRotation = Quaternion.identity;

    void Start()
    {
        mpWizard = this.GetComponentInChildren<Class_Wizard>();
        mpWarrior = this.GetComponentInChildren<Class_Warrior>();
        string tOtherPlayerOccupation = "";
        foreach (var u in Class_NetworkClient.GetInst().mUserInfoes)
        {
            if (u.mUserName != Class_NetworkClient.GetInst().mMyUserInfo.mUserName)
            {
                tOtherPlayerOccupation = u.mOccupation;
            }
        }

        if (tOtherPlayerOccupation == "Warrior")
        {
            mpWizard.gameObject.SetActive(false);
            Destroy(mpWizard);
        }
        else
        {
            mpWarrior.gameObject.SetActive(false);
            Destroy(mpWarrior);
        }

        init();

        if (Class_NetworkClient.GetInst().mMyUserInfo.mUserName == Class_NetworkClient.GetInst().mRoomMaster)
        {
            tReceivePlayerPos = new Vector3(4f, 1f, -1f);
        }
        else
        {
            tReceivePlayerPos = new Vector3(-4f, 1f, -1f);
        }

        this.gameObject.tag = "TagOtherPlayer";
    }

    void Update()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, tReceivePlayerPos, Time.deltaTime * 10.0f);
        this.transform.localRotation = Quaternion.Lerp(this.transform.rotation, tReceivePlayerRotation, Time.deltaTime * 10.0f);
    }

    private void init()
    {
        print("UDPSend.init()");

        if (Class_NetworkClient.GetInst().mMyUserInfo.mUserName == Class_NetworkClient.GetInst().mRoomMaster)
        {
            port = 8052;
        }
        else
        {
            port = 8051;
        }

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, port);
                byte[] data = client.Receive(ref anyIP);

                string text = Encoding.UTF8.GetString(data);

                Class_ReceivePlayerPos tPlayerPos = JsonUtility.FromJson<Class_ReceivePlayerPos>(text);

                Thread.Sleep(5);

                float tX = tPlayerPos.x;
                float tY = tPlayerPos.y;
                float tZ = tPlayerPos.z;
                float tRY = tPlayerPos.ry;
                foreach(var u in Class_NetworkClient.GetInst().mUserInfoes)
                {
                    if (u.mUserName != Class_NetworkClient.GetInst().mMyUserInfo.mUserName)
                    {
                        u.mHP = tPlayerPos.HP;
                    }
                }

                tReceivePlayerPos = new Vector3(tX, tY, tZ);
                tReceivePlayerRotation = Quaternion.Euler(0f, tRY, 0f);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
}
