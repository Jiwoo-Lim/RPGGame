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
    private Class_ReceiveWarrior mpReceiveWarrior = null;
    private Class_ReceiveWizard mpReceiveWizard = null;
    private Class_PlayerUI mpPlayerUI = null;
    private Class_ReceivePlayerUI mpReceivePlayerUI = null;
    private Class_PlayScene mpPlayScene = null;

    public Class_EnemyJon mpEnemySpawn = null;
    public Class_ItemSpawn mpItemSpawn = null;

    private static int localPort;

    private string IP;
    public int port;

    IPEndPoint remoteEndPoint;
    UdpClient client;

    string strMessage = "";

    public float tHorizontal = 0.0f;
    public float tVertical = 0.0f;
    public float mSpeedScalar = 5.0f;
    public int tSpace = 0;
    public int tCount = 5;
    public int tEnemyCount = 10;

    void Awake()
    {
        mpItemSpawn = FindObjectOfType<Class_ItemSpawn>();
        mpEnemySpawn = FindObjectOfType<Class_EnemyJon>();
        mpPlayScene = FindObjectOfType<Class_PlayScene>();

        mpWizard = this.GetComponentInChildren<Class_Wizard>();
        mpWarrior = this.GetComponentInChildren<Class_Warrior>();
        mpReceiveWizard = this.GetComponentInChildren<Class_ReceiveWizard>();
        mpReceiveWarrior = this.GetComponentInChildren<Class_ReceiveWarrior>();
        mpPlayerUI = this.GetComponentInChildren<Class_PlayerUI>();
        mpReceivePlayerUI = this.GetComponentInChildren<Class_ReceivePlayerUI>();

        Destroy(mpReceivePlayerUI);

        if (Class_NetworkClient.GetInst().mMyUserInfo.mOccupation == "Warrior")
        {
            Destroy(mpWizard.gameObject);
            Destroy(mpReceiveWarrior);
        }
        else
        {
            Destroy(mpWarrior.gameObject);
            Destroy(mpReceiveWizard);
        }
    }

    void Start()
    {
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
            this.transform.forward = Vector3.Lerp(this.transform.forward, mMoveDir, Time.deltaTime * 10);
        }

        if (tCount <= 0 && mpPlayScene.mStage==Class_PlayScene.STAGE.Stage_1)
        {
            mpItemSpawn.StartSpawnItem(false);
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
            tSendPlayerPos.tHorizontal = tHorizontal;
            tSendPlayerPos.tVertical = tVertical;
            tSendPlayerPos.Space = tSpace;
            tSendPlayerPos.tCount = tCount;
            tSendPlayerPos.tEnemyCount = tEnemyCount;

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

    private void OnTriggerEnter(Collider tCollider)
    {
        if (tCollider.CompareTag("TagItem"))
        {
            tCount -= 1;
            Destroy(tCollider.gameObject);
        }
    }
}
