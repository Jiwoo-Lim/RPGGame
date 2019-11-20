using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using System;

public class Class_RoomScene : MonoBehaviour
{
    public Text mRoomName = null;
    public Text mMatserName = null;

    public Button mBeginPlayBtn = null;
    public Button mReadyBtn = null;

    public Image mGuestImg = null;
    // Start is called before the first frame update
    void Start()
    {
        mRoomName.text = "Master : " + Class_NetworkClient.GetInst().mRoomName;
        mMatserName.text = "RoomName : " + Class_NetworkClient.GetInst().mRoomMaster;
        StartCoroutine("UpdateFromNetwork");

        if (Class_NetworkClient.GetInst().mMyUserInfo.mUserName == Class_NetworkClient.GetInst().mRoomMaster) 
        {
            mReadyBtn.gameObject.SetActive(false);
            mGuestImg.gameObject.SetActive(false);
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
                    case PROTOCOL.ACK_JOIN_ROOM:
                        {
                            Debug.Log("ACK_JOIN_ROOM");

                            int tUserCount = tBuffer[2];
                            int tMasterIdLength = tBuffer[3];
                            int tOffset = 4;
                            tOffset += tMasterIdLength;
                            int tRoomNameLength = tBuffer[tOffset];
                            tOffset += 1;

                            //=====
                            int tInitPos = tOffset + tRoomNameLength;
                            tOffset = tInitPos;
                            for (int ti = 0; ti < tUserCount; ti++)
                            {
                                int tLength = (int)tBuffer[tOffset];
                                tOffset = tOffset + 1;

                                string tUserName = Encoding.UTF8.GetString(tBuffer, tOffset, tLength);
                                tOffset = tOffset + tLength;

                                Debug.Log("UserName_" + ti + " : " + tUserName);

                                if (tUserName != Class_NetworkClient.GetInst().mMyUserInfo.mUserName)
                                {
                                    Class_Singleton_User tUser = new Class_Singleton_User();
                                    tUser.mUserName = tUserName;

                                    Class_NetworkClient.GetInst().mUserInfoes.Add(tUser);
                                }

                                mGuestImg.gameObject.SetActive(true);
                            }
                        }
                        break;
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

                            //송신받은 게스트/방장의 정보를 상대유저정보에 입력
                            foreach(var o in Class_NetworkClient.GetInst().mUserInfoes)
                            {
                                if(o.mUserName != Class_NetworkClient.GetInst().mMyUserInfo.mUserName)
                                {
                                    o.mUserKey = tBuffer[1];
                                    int tHPLength = tBuffer[2];
                                    int tOffset = 3;
                                    o.mHP = BitConverter.ToInt32(tBuffer, tOffset);
                                    tOffset += sizeof(int);
                                    int tAPLength = tBuffer[tOffset];
                                    tOffset += 1;
                                    o.mAP = BitConverter.ToInt32(tBuffer, tOffset);
                                    tOffset += sizeof(int);
                                    int tOccupationLength = tBuffer[tOffset];
                                    tOffset += 1;
                                    o.mOccupation = Encoding.UTF8.GetString(tBuffer, tOffset, tOccupationLength);
                                }
                            }

                            SceneManager.LoadScene("MapScene");
                            SceneManager.LoadScene("AllPlayScene", LoadSceneMode.Additive);
                        }
                        break;
                    case PROTOCOL.ACK_QUIT_GAME:
                        {
                            #if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
                            #else
                            Application.Quit();
                            #endif
                        }
                        break;
                }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnClickBtnReady()
    {
        Class_Singleton_Sound.GetInst().Play("ClickSound_2");
        mReadyBtn.gameObject.SetActive(false);

        byte[] tBuffer = new byte[1024];

        byte tProtocolID = (byte)PROTOCOL.REQ_READY;

        tBuffer[0] = tProtocolID;

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }

    public void OnClickBtnBeginPlay()
    {
        Class_Singleton_Sound.GetInst().Play("ClickSound_2");

        byte[] tBuffer = new byte[1024];

        byte tProtocolID = (byte)PROTOCOL.REQ_BEGIN_PLAY;

        tBuffer[0] = tProtocolID;

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }
}