using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Text;

public class Class_SceneRoomSelect : MonoBehaviour
{
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
                        }   
                        break;
                    case PROTOCOL.ACK_JOIN_ROOM:
                        {
                            Debug.Log("ACK_JOIN_ROOM");

                            int tRoomId                             = (int)tBuffer[1];                            
                            int tMasterIdLength                 = (int)tBuffer[3];
                            int tOffset                                 = 4;
                            string tMasterId                        = Encoding.UTF8.GetString(tBuffer, tOffset, tMasterIdLength);
                            tOffset                                     = tOffset + tMasterIdLength;
                            int tRoomNameLength                 = (int)tBuffer[tOffset];
                            tOffset                                     += 1;
                            string tRoomName                     = Encoding.UTF8.GetString(tBuffer, tOffset, tRoomNameLength);

                            int tUserCount = (int)tBuffer[2];

                            Debug.Log("방장 Name : " + tMasterId);
                            Debug.Log("방 Name : " + tRoomName);

                            //=====
                            int tInitPos = 4 + tMasterIdLength;
                            tOffset = tInitPos;
                            for (int ti = 0; ti < tUserCount; ti++)
                            {
                                int tLength = (int)tBuffer[tOffset];
                                tOffset = tOffset + 1;

                                string tUserName = Encoding.UTF8.GetString(tBuffer, tOffset, tLength);
                                tOffset = tOffset + tLength;

                                Debug.Log("UserName_"+ti+" : " + tUserName);
                            }
                            //=====

                            Class_NetworkClient.GetInst().mRoomId = tRoomId;
                            Class_NetworkClient.GetInst().mRoomMaster = tMasterId;

                            SceneManager.LoadScene("RoomScene");
                        }
                        break;
                    case PROTOCOL.ACK_JOIN_ROOM_FAIL:
                        {
                            Debug.Log("Join Room Fail..");
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
