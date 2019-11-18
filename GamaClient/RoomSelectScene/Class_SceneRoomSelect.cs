using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Text;
using UnityEngine.UI;

public class Class_SceneRoomSelect : MonoBehaviour
{
    public Text mpTxt = null;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("UpdateFromNetwork");
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator UpdateFromNetwork()
    {
        while (true)
        {
            byte[] tBuffer = new byte[1024];
            int tRecvSize = Class_NetworkClient.GetInst().GetFromQueue(ref tBuffer, tBuffer.Length);

            if (tRecvSize > 0)
            {
                PROTOCOL tProtocolID = 0;
                tProtocolID = (PROTOCOL)tBuffer[0];

                switch (tProtocolID)
                {
                    case PROTOCOL.ACK_CREATE_ROOM:
                        {
                            Debug.Log("ACK_CREATE_ROOM");
                            int tRoomId = tBuffer[1];
                            int tMasterIdLength = tBuffer[2];
                            int tOffset = 3;
                            string tMasterId = Encoding.UTF8.GetString(tBuffer, tOffset, tMasterIdLength);
                            int tRoomNameLength = tBuffer[tMasterIdLength + tOffset];
                            tOffset = tOffset + tMasterIdLength + 1;
                            string tRoomName = Encoding.UTF8.GetString(tBuffer, tOffset, tRoomNameLength);

                            Class_NetworkClient.GetInst().mRoomId = tRoomId;
                            Class_NetworkClient.GetInst().mRoomMaster = tMasterId;
                            Class_NetworkClient.GetInst().mRoomName = tRoomName;
 
                            Debug.Log("mRoomMaster : " +Class_NetworkClient.GetInst().mRoomMaster);
                            Debug.Log("mRoomName : " + Class_NetworkClient.GetInst().mRoomName);

                            SceneManager.LoadScene("RoomScene");
                            SceneManager.LoadScene("AllPlayScene", LoadSceneMode.Additive);
                        }   
                        break;
                    case PROTOCOL.ACK_JOIN_ROOM:
                        {
                            Debug.Log("ACK_JOIN_ROOM");

                            int tRoomId                             = tBuffer[1];
                            int tUserCount                         = tBuffer[2];
                            int tMasterIdLength                 = tBuffer[3];
                            int tOffset                               = 4;
                            string tMasterId                      = Encoding.UTF8.GetString(tBuffer, tOffset, tMasterIdLength);
                            tOffset                                    = tOffset + tMasterIdLength;
                            int tRoomNameLength              = tBuffer[tOffset];
                            tOffset                                    += 1;
                            string tRoomName                    = Encoding.UTF8.GetString(tBuffer, tOffset, tRoomNameLength);

                            

                            Debug.Log("방장 Name : " + tMasterId);
                            Debug.Log("방 Name : " + tRoomName);

                            //=====
                            int tInitPos = tOffset + tRoomNameLength;
                            tOffset = tInitPos;
                            for (int ti = 0; ti < tUserCount; ti++)
                            {
                                int tLength = (int)tBuffer[tOffset];
                                tOffset = tOffset + 1;

                                string tUserName = Encoding.UTF8.GetString(tBuffer, tOffset, tLength);
                                tOffset = tOffset + tLength;

                                Debug.Log("UserName_"+ti+" : " + tUserName);

                                if(tUserName != Class_NetworkClient.GetInst().mMyUserInfo.mUserName)
                                {
                                    Class_Singleton_User tUser = new Class_Singleton_User();
                                    tUser.mUserName = tUserName;

                                    Class_NetworkClient.GetInst().mUserInfoes.Add(tUser);
                                }
                            }
                            //=====

                            Class_NetworkClient.GetInst().mRoomId = tRoomId;
                            Class_NetworkClient.GetInst().mRoomMaster = tMasterId;
                            Class_NetworkClient.GetInst().mRoomName = tRoomName;

                            SceneManager.LoadScene("RoomScene");
                            SceneManager.LoadScene("AllPlayScene", LoadSceneMode.Additive);
                        }
                        break;
                    case PROTOCOL.ACK_JOIN_ROOM_FAIL:
                        {
                            Debug.Log("Join Room Fail..");

                            mpTxt.gameObject.SetActive(true);

                            mpTxt.text = "<color='green'>방 입장에 실패했어요 ..ㅠㅠ</color>";
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

    public void OnClickBtnCreateRoom()
    {
        byte[] tBuffer = new byte[1024];

        byte tProtocolID = (byte)PROTOCOL.REQ_CREATE_ROOM;

        tBuffer[0] = tProtocolID;

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }

    public void OnClickBtnJoinRoom()
    {
        byte[] tBuffer = new byte[1024];

        byte tProtocolID = (byte)PROTOCOL.REQ_JOIN_ROOM;

        tBuffer[0] =tProtocolID;

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }
}
