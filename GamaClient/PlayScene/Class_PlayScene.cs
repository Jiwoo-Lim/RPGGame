using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Class_PlayScene : MonoBehaviour
{
    public Class_ItemSpawn mpItemSpawn = null;
    private Class_EnemyJon mpEnemySpawn = null;
    public Class_MazeJon mpMazeSpawn = null;
    public Class_Stage_1 mpStage_1 = null;
    public Class_Stage_1_guest mpStage_1_guest = null;
    public Class_Stage_2 mpStage_2 = null;
    public Class_Stage_2_guest mpStage_2_guest = null;

    private Class_Player mpPlayer = null;
    private Class_ReceivePlayer mpReceivePlayer = null;

    public enum STAGE
    {
        Stage_1 = 0,
        Stage_2,
        Stage_3
    }
    public STAGE mStage = STAGE.Stage_1;

    void Awake()
    {
        mpItemSpawn = FindObjectOfType<Class_ItemSpawn>();
        mpEnemySpawn= FindObjectOfType<Class_EnemyJon>();
        mpMazeSpawn = FindObjectOfType<Class_MazeJon>();

        if (Class_NetworkClient.GetInst().mMyUserInfo.mUserName==Class_NetworkClient.GetInst().mRoomMaster)
        {
            GameObject PFPlayer_0 = Resources.Load<GameObject>("Prefabs/PFPlayer_0");
            GameObject PFPlayer_1 = Resources.Load<GameObject>("Prefabs/PFPlayer_1");
            GameObject MyPlayer = Instantiate<GameObject>(PFPlayer_0, PFPlayer_0.transform.position, Quaternion.identity);
            GameObject OtherPlayer = Instantiate<GameObject>(PFPlayer_1, PFPlayer_1.transform.position, Quaternion.identity);

            MyPlayer.AddComponent<Class_Player>();
            OtherPlayer.AddComponent<Class_ReceivePlayer>();

            mpItemSpawn.StartSpawnItem(true);

            //TCP 게임시작 요청
            byte[] tBuffer = new byte[1024];

            tBuffer[0] = (byte)PROTOCOL.REQ_START_GAME;

            Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
        }
        else
        {
            GameObject PFPlayer_0 = Resources.Load<GameObject>("Prefabs/PFPlayer_0");
            GameObject PFPlayer_1 = Resources.Load<GameObject>("Prefabs/PFPlayer_1");
            GameObject OtherPlayer = Instantiate<GameObject>(PFPlayer_0, PFPlayer_0.transform.position, Quaternion.identity);
            GameObject MyPlayer = Instantiate<GameObject>(PFPlayer_1, PFPlayer_1.transform.position, Quaternion.identity);

            MyPlayer.AddComponent<Class_Player>();
            OtherPlayer.AddComponent<Class_ReceivePlayer>();
        }
    }

    void Start()
    {
        StartCoroutine("UpdateFromNetwork");

        mpPlayer = FindObjectOfType<Class_Player>();
        mpReceivePlayer = FindObjectOfType<Class_ReceivePlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (mStage)
        {
            case STAGE.Stage_1:
                {
                    if (Class_NetworkClient.GetInst().mMyUserInfo.mUserName == Class_NetworkClient.GetInst().mRoomMaster)
                    {
                        if (mpPlayer.tCount <= 0 && mpStage_1.mActive == false) 
                        {
                            mpStage_1.gameObject.SetActive(true);
                            mpStage_1.mActive = true;
                        }
                        else if (mpReceivePlayer.tCount <= 0)
                        {
                            mpStage_1_guest.gameObject.SetActive(true);

                            mStage = Class_PlayScene.STAGE.Stage_2;
                        }
                    }
                    else
                    {
                        if (mpPlayer.tCount <= 0)
                        {
                            mpStage_1_guest.gameObject.SetActive(true);

                            mStage = Class_PlayScene.STAGE.Stage_2;
                        }
                        else if (mpReceivePlayer.tCount <= 0 && mpStage_1.mActive == false)
                        {
                            mpStage_1.gameObject.SetActive(true);
                            mpStage_1.mActive = true;
                        }
                    }
                }
                break;
            case STAGE.Stage_2:
                {
                    if (Class_NetworkClient.GetInst().mMyUserInfo.mUserName == Class_NetworkClient.GetInst().mRoomMaster)
                    {
                        if (mpPlayer.tEnemyCount <= 0 && mpStage_2.mActive == false)
                        {
                            mpStage_2.gameObject.SetActive(true);
                            mpStage_2.mActive = true;
                        }
                        else if (mpReceivePlayer.tEnemyCount <= 0)
                        {
                            mpStage_2_guest.gameObject.SetActive(true);

                            mStage = STAGE.Stage_3;
                        }
                    }
                    else
                    {
                        if (mpPlayer.tEnemyCount <= 0)
                        {
                            mpStage_2_guest.gameObject.SetActive(true);

                            mStage = STAGE.Stage_3;
                        }
                        else if (mpReceivePlayer.tEnemyCount <= 0 && mpStage_2.mActive == false) 
                        {
                            mpStage_2.gameObject.SetActive(true);
                            mpStage_2.mActive = true;
                        }
                    }
                }
                break;
            case STAGE.Stage_3:

                break;
        }
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
                    case PROTOCOL.ACK_START_GAME:
                        {
                            Debug.Log("ACK_START_GAME");

                            int tUserKey= tBuffer[1];

                            //방장 차례
                            if (tUserKey == Class_NetworkClient.GetInst().mMyUserInfo.mUserKey)
                            {
                                Class_NetworkClient.GetInst().CleanTurn();
                                Class_NetworkClient.GetInst().mMyUserInfo.mMyTurn = true;
                            }
                            else
                            {
                                Class_NetworkClient.GetInst().CleanTurn();
                            }
                        }
                        break;
                    case PROTOCOL.ACK_TURN_OVER:
                        {
                            Debug.Log("ACK_TURN_OVER");

                            int tUserKey = tBuffer[1];

                            if(tUserKey == Class_NetworkClient.GetInst().mMyUserInfo.mUserKey)
                            {
                                Class_NetworkClient.GetInst().CleanTurn();
                            }
                            else
                            {
                                Class_NetworkClient.GetInst().CleanTurn();
                                Class_NetworkClient.GetInst().mMyUserInfo.mMyTurn = true;

                                switch (mStage)
                                {
                                    case STAGE.Stage_1:
                                        if (Class_NetworkClient.GetInst().mRoomMaster != Class_NetworkClient.GetInst().mMyUserInfo.mUserName)
                                        {
                                            if (mpItemSpawn.mSpawn == false)
                                            {
                                                mpItemSpawn.StartSpawnItem(true);
                                            }
                                        }
                                        break;
                                    case STAGE.Stage_2:
                                        {
                                            if (mpEnemySpawn.mSpawn == false)
                                            {
                                                mpEnemySpawn.SpawnEnemy();
                                            }
                                        }
                                            break;
                                    case STAGE.Stage_3:
                                        {
                                            if(mpMazeSpawn.mSpawn==false)
                                            {
                                                mpMazeSpawn.SpawnMaze();
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        break;
                    case PROTOCOL.ACK_GAME_CLEAR:
                        {
                            Debug.Log("ACK_GAME_CLEAR");

                            //서버에서 받은 데이터 입력
                            Class_Singleton_User tFirstUser = new Class_Singleton_User();

                            tFirstUser.mClearCount = tBuffer[1];
                            int tHPLength = tBuffer[2];

                            int tOffset = 3;
                            tFirstUser.mHP = BitConverter.ToInt32(tBuffer, tOffset);
                            tOffset += sizeof(int);

                            int tAPLength = tBuffer[tOffset];
                            tOffset += 1;
                            tFirstUser.mAP = BitConverter.ToInt32(tBuffer, tOffset);
                            tOffset += sizeof(int);

                            int tNameLength = tBuffer[tOffset];
                            tOffset += 1;
                            tFirstUser.mUserName = Encoding.UTF8.GetString(tBuffer, tOffset, tNameLength);

                            tOffset += tNameLength;
                            int tOccupationLength = tBuffer[tOffset];
                            tOffset += 1;
                            tFirstUser.mOccupation = Encoding.UTF8.GetString(tBuffer, tOffset, tOccupationLength);

                            Class_NetworkClient.GetInst().mUserInfoes.Add(tFirstUser);
                            //서버에서 받은 데이터 입력

                            SceneManager.LoadScene("ClearScene");
                            SceneManager.LoadScene("AllPlayScene", LoadSceneMode.Additive);
                        }
                        break;
                    case PROTOCOL.ACK_GAME_FAIL:
                        {
                            Debug.Log("ACK_GAME_FAIL");

                            SceneManager.LoadScene("FailScene");
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
}