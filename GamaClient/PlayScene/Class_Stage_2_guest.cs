using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Stage_2_guest : MonoBehaviour
{
    public GameObject mBridge = null;
    public GameObject mBtn = null;

    private Class_Player mpPlayer = null;
    private Class_PlayScene mpPlayScene = null;
    private Class_ReceivePlayer mpReceivePlayer = null;

    // Start is called before the first frame update
    void Start()
    {
        mpPlayer = FindObjectOfType<Class_Player>();
        mpPlayScene = FindObjectOfType<Class_PlayScene>();
        mpReceivePlayer = FindObjectOfType<Class_ReceivePlayer>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter(Collider tCollider)
    {
        if (tCollider.CompareTag("TagPlayer"))
        {
            Class_Singleton_Sound.GetInst().Play("StageClear");

            this.gameObject.SetActive(false);
            mBridge.gameObject.SetActive(true);
            mBtn.gameObject.SetActive(true);

            OnStageClear();            
        }
        else if (tCollider.CompareTag("TagOtherPlayer"))
        {
            Class_Singleton_Sound.GetInst().Play("StageClear");

            this.gameObject.SetActive(false);
            mBridge.gameObject.SetActive(true);
            mBtn.gameObject.SetActive(true);
        }
    }

    public void OnStageClear()
    {
        if (Class_NetworkClient.GetInst().mMyUserInfo.mMyTurn == true)
        {
            byte[] tBuffer = new byte[1024];

            tBuffer[0] = (byte)PROTOCOL.REQ_TURN_OVER;

            Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
        }
    }
}
