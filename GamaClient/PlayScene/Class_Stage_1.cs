using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Stage_1 : MonoBehaviour
{
    private Class_Player mpPlayer = null;
    private Class_ReceivePlayer mpReceivePlayer = null;

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
        }
        else if(tCollider.CompareTag("TagOtherPlayer"))
        {
            this.gameObject.SetActive(false);
            mBridge.gameObject.SetActive(true);
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
