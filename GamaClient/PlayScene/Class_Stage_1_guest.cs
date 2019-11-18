using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Stage_1_guest : MonoBehaviour
{
    private Class_Player mpPlayer = null;
    private Class_ReceivePlayer mpReceivePlayer = null;
    private Class_PlayScene mpPlayScene = null;

    public GameObject mBridge = null;

    // Start is called before the first frame update
    void Start()
    {
        mpPlayer = FindObjectOfType<Class_Player>();
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
            this.gameObject.SetActive(false);
            mBridge.gameObject.SetActive(true);

            OnStageClear();

            mpPlayScene.mStage = Class_PlayScene.STAGE.Stage_2;
        }
        else if (tCollider.CompareTag("TagOtherPlayer"))
        {
            this.gameObject.SetActive(false);
            mBridge.gameObject.SetActive(true);

            mpPlayScene.mStage = Class_PlayScene.STAGE.Stage_2;
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
