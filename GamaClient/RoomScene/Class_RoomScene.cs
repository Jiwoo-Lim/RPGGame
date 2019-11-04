using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Class_RoomScene : MonoBehaviour
{
    public Text mRoomName = null;
    public Text mMatserName = null;

    public Button mBeginPlayBtn = null;
    public Button mReadyBtn = null;
    // Start is called before the first frame update
    void Start()
    {
        mRoomName.text = "Master : " + Class_NetworkClient.GetInst().mRoomName;
        mMatserName.text = "RoomName : " + Class_NetworkClient.GetInst().mRoomMaster;
        StartCoroutine("UpdateFromNetwork");

        if (Class_Singleton_User.GetInst().mUserName == Class_NetworkClient.GetInst().mRoomMaster) 
        {
            mReadyBtn.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator UpdateFromNetwork()
    {
        while(true)
        {
            byte[] tBuffer = new byte[1024];
            int tRecvSize = Class_NetworkClient.GetInst().GetFromQueue(ref tBuffer, tBuffer.Length);

            if (tRecvSize > 0)
            {
                PROTOCOL tProtocolID = 0;
                tProtocolID = (PROTOCOL)tBuffer[0];

                switch (tProtocolID)
                {
                    case PROTOCOL.ACK_READY:
                        {
                            Debug.Log("ACK_READY");
                        }
                        break;
                    case PROTOCOL.ACK_ALL_READY:
                        {
                            mBeginPlayBtn.gameObject.SetActive(true);

                            Debug.Log("ACK_ALL_READY");
                        }
                        break;
                    case PROTOCOL.ACK_BEGIN_PLAY:
                        {
                            Debug.Log("ACK_BEGIN_PLAY");

                            SceneManager.LoadScene("PlayScene_1");
                        }
                        break;
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnClickBtnReady()
    {
        mReadyBtn.gameObject.SetActive(false);

        byte[] tBuffer = new byte[1024];

        byte tProtocolID = (byte)PROTOCOL.REQ_READY;

        tBuffer[0] = tProtocolID;

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }

    public void OnClickBtnBeginPlay()
    {
        byte[] tBuffer = new byte[1024];

        byte tProtocolID = (byte)PROTOCOL.REQ_BEGIN_PLAY;

        tBuffer[0] = tProtocolID;

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }
}