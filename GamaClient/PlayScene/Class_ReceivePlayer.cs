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
    private Class_ReceiveWarrior mpReceiveWarrior = null;
    private Class_ReceiveWizard mpReceiveWizard = null;
    private Class_Warrior mpWarrior = null;
    private Class_Wizard mpWizard = null;
    private Class_PlayerUI mpPlayerUI = null;
    private Class_ReceivePlayerUI mpReceivePlayerUI = null;

    //receiving Thread
    Thread receiveThread;

    //udpclient object
    UdpClient client;

    private int port;

    Vector3 tReceivePlayerPos = Vector3.zero;
    Quaternion tReceivePlayerRotation = Quaternion.identity;

    public float tHorizontal = 0.0f;
    public float tVertical = 0.0f;
    public int tSpace = 0;
    public int tCount = 0;
    public int tEnemyCount = 0;

    void Awake()
    {
        mpReceiveWizard = this.GetComponentInChildren<Class_ReceiveWizard>();
        mpReceiveWarrior = this.GetComponentInChildren<Class_ReceiveWarrior>();
        mpWizard = this.GetComponentInChildren<Class_Wizard>();
        mpWarrior = this.GetComponentInChildren<Class_Warrior>();
        mpPlayerUI = this.GetComponentInChildren<Class_PlayerUI>();
        mpReceivePlayerUI = this.GetComponentInChildren<Class_ReceivePlayerUI>();

        Destroy(mpPlayerUI);

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
            Destroy(mpReceiveWizard.gameObject);
            Destroy(mpWarrior);
        }
        else
        {
            Destroy(mpReceiveWarrior.gameObject);
            Destroy(mpWizard);
        }
    }

    void Start()
    {
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

                tReceivePlayerPos = new Vector3(tPlayerPos.x, tPlayerPos.y, tPlayerPos.z);
                tReceivePlayerRotation = Quaternion.Euler(0f, tPlayerPos.ry, 0f);
                foreach (var u in Class_NetworkClient.GetInst().mUserInfoes)
                {
                    if (u.mUserName != Class_NetworkClient.GetInst().mMyUserInfo.mUserName)
                    {
                        u.mHP = tPlayerPos.HP;
                    }
                }

                tHorizontal = tPlayerPos.tHorizontal;
                tVertical = tPlayerPos.tVertical;

                tSpace = tPlayerPos.Space;

                tCount = tPlayerPos.tCount;

                tEnemyCount = tPlayerPos.tEnemyCount;
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
}
