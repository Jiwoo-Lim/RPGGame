using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Class_Clear : MonoBehaviour
{
    public Button mBntClear = null;

    public Button mBtnFail = null;

    //public Text mpCountTxt = null;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Class_NetworkClient.GetInst().mMyUserInfo.mHP < 0)
        {
            mBtnFail.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider tCollider)
    {
        if (tCollider.CompareTag("TagPlayer"))
        {
            mBntClear.gameObject.SetActive(true);
        }
        else if(tCollider.CompareTag("TagOtherPlayer"))
        {
            mBntClear.gameObject.SetActive(true);
        }
    }

    public void OnClickBtnClick()
    {
        Debug.Log("Game Clear");
        byte[] tBuffer = new byte[1024];

        tBuffer[0] = (byte)PROTOCOL.REQ_GAME_CLEAR;

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }

    public void OnClickBtnFail()
    {
        Debug.Log("Game Fail");
        byte[] tBuffer = new byte[1024];

        tBuffer[0] = (byte)PROTOCOL.REQ_GAME_FAIL;

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }
}
