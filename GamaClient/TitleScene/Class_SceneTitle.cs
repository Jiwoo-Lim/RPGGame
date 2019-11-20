using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;
using System;

public class Class_SceneTitle : MonoBehaviour
{
    private string mIdComment = "";
    private string mPasswordComment = "";

    public InputField InputId = null;
    public InputField InputPassword = null;

    public Class_Singleton_User tUser = null;

    // Start is called before the first frame update
    void Start()
    {
        Class_NetworkClient.GetInst().CreateRyu();
        bool tResult = Class_NetworkClient.GetInst().Connect(Class_NetworkClient.GetInst().mServerIPAddress, Class_NetworkClient.GetInst().mPort);
        if (true == tResult)
        {
            StartCoroutine("UpdateFromNetwork");
        }
        else
        {
            Debug.Log("cannot connect");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnClickBtnLogin();
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
                    case PROTOCOL.ACK_CONNECT:
                        {
                            Debug.Log("ACK_CONNECT");
                        }
                        break;
                    case PROTOCOL.ACK_LOGIN:
                        {
                            Debug.Log("ACK_LOGIN");
                            int tUserKey = tBuffer[1];
                            int tHPLength = tBuffer[2];
                            int tOffset = 3;
                            int tHP = BitConverter.ToInt32(tBuffer, tOffset);
                            tOffset += sizeof(int);

                            int tAPLength = tBuffer[tOffset];
                            tOffset += 1;
                            int tAP = BitConverter.ToInt32(tBuffer, tOffset);
                            tOffset += sizeof(int);

                            int tOccupationlength = tBuffer[tOffset];
                            tOffset += 1;
                            string tOccupation = Encoding.UTF8.GetString(tBuffer, tOffset, tOccupationlength);

                            tUser.mUserKey = tUserKey;
                            tUser.mHP = tHP;
                            tUser.mAP = tAP;
                            tUser.mOccupation = tOccupation;

                            Class_NetworkClient.GetInst().mMyUserInfo = tUser;
                            Class_NetworkClient.GetInst().mUserInfoes.Add(tUser);

                            SceneManager.LoadScene("RoomSelectScene");
                            SceneManager.LoadScene("AllPlayScene", LoadSceneMode.Additive);
                        }
                        break;
                    case PROTOCOL.ACK_LOGIN_FAIL:
                        {
                            Debug.Log("ACK_LOGIN_FAIL");

                            Debug.Log("Password is wrong");
                        }
                        break;
                    case PROTOCOL.ACK_CREATE_CHAR:
                        {
                            Debug.Log("ACK_CREATE_CHAR");
                            tUser.mUserKey = tBuffer[1];
                            Class_NetworkClient.GetInst().mMyUserInfo = tUser;

                            SceneManager.LoadScene("CharactorScene");
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

    public void OnClickBtnLogin()
    {
        Class_Singleton_Sound.GetInst().Play("ClickSound_2");
        mIdComment = InputId.text;
        mPasswordComment =InputPassword.text;

        tUser.mUserName = mIdComment;

        byte[] tBuffer = new byte[1024];
        byte tProtocolID = (byte)PROTOCOL.REQ_LOGIN;

        byte tIdLength = (byte)mIdComment.Length;
        byte[] tId = Encoding.UTF8.GetBytes(mIdComment);

        byte tPasswordLength = (byte)mPasswordComment.Length;
        byte[] tPassword = Encoding.UTF8.GetBytes(mPasswordComment);

        tBuffer[0] = tProtocolID;
        tBuffer[1] = tIdLength;
        tId.CopyTo(tBuffer, 2);
        tBuffer[tBuffer[1] + 2] = tPasswordLength;
        tPassword.CopyTo(tBuffer, tBuffer[1] + 3);

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
        
    }
}
