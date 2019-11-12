using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Class_Player : MonoBehaviour
{
    private Class_Warrior mpWarrior = null;
    private Class_Wizard mpWizard = null;

    private static int localPort;

    private string IP;
    public int port;

    IPEndPoint remoteEndPoint;
    UdpClient client;

    string strMessage = "";

    float tHorizontal = 0.0f;
    float tVertical = 0.0f;
    public float mSpeedScalar = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        mpWizard = this.GetComponentInChildren<Class_Wizard>();
        mpWarrior = this.GetComponentInChildren<Class_Warrior>();
        if (Class_NetworkClient.GetInst().mMyUserInfo.mOccupation == "Warrior")
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

        StartCoroutine("UpdatePos");

        this.gameObject.tag = "TagPlayer";
    }

    // Update is called once per frame
    void Update()
    {
        if (Class_NetworkClient.GetInst().mMyUserInfo.mMyTurn == true)
        {
            tHorizontal = Input.GetAxis("Horizontal");
            tVertical = Input.GetAxis("Vertical");

            Vector3 mMoveDir = new Vector3();
            mMoveDir = tHorizontal * Vector3.right + tVertical * Vector3.forward;

            this.transform.Translate(mMoveDir * Time.deltaTime * mSpeedScalar, Space.World);
            this.transform.forward = Vector3.Lerp(this.transform.forward, mMoveDir, Time.deltaTime*10);
        }
    }

    IEnumerator UpdatePos()
    {
        for (; ; )
        {
            Class_SendPlayerPos tSendPlayerPos = new Class_SendPlayerPos();
            tSendPlayerPos.x = this.transform.position.x;
            tSendPlayerPos.y = this.transform.position.y;
            tSendPlayerPos.z = this.transform.position.z;
            tSendPlayerPos.ry = this.transform.rotation.eulerAngles.y;
            tSendPlayerPos.HP = Class_NetworkClient.GetInst().mMyUserInfo.mHP;

            strMessage = JsonUtility.ToJson(tSendPlayerPos);
            sendString(strMessage + "\n");

            yield return new WaitForSeconds(0.05f);
        }
    }

    public void init()
    {
        //Woo
        IP = "192.168.0.11";
        //Sung
        //IP = "192.168.0.144";
        if (Class_NetworkClient.GetInst().mMyUserInfo.mUserName == Class_NetworkClient.GetInst().mRoomMaster)
        {
            port = 8051;
        }
        else
        {
            port = 8052;

        }

        //Senden
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
    }

    private void sendString(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
}
