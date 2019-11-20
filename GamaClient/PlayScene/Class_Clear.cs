using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Class_Clear : MonoBehaviour
{
    public Button mBntClear = null;
    public Button mBtnFail = null;
    public Text mCountTxt = null;

    private Class_Player mpPlayer = null;
    private Class_ReceivePlayer mpReceivePlayer = null;
    private Class_PlayScene mpPlayScene = null;

    private bool BtnOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        mpPlayer = FindObjectOfType<Class_Player>();
        mpReceivePlayer = FindObjectOfType<Class_ReceivePlayer>();
        mpPlayScene = FindObjectOfType<Class_PlayScene>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var u in Class_NetworkClient.GetInst().mUserInfoes)
        {
            if (u.mUserName != Class_NetworkClient.GetInst().mMyUserInfo.mUserName)
            {
                if (Class_NetworkClient.GetInst().mMyUserInfo.mOccupation == "Wizard" && Class_NetworkClient.GetInst().mMyUserInfo.mHP <= 0&&BtnOpen==false)
                {
                    StageFail();
                }
                else if (u.mOccupation == "Wizard" && u.mHP <= 0)
                {
                    StageFail();
                }                
            }
        }
        switch (mpPlayScene.mStage)
        {
            case Class_PlayScene.STAGE.Stage_1:
                {
                    if (Class_NetworkClient.GetInst().mMyUserInfo.mMyTurn == true)
                    {
                        mCountTxt.text = "남은 아이템 : " + mpPlayer.tCount;
                    }
                    else
                    {
                        mCountTxt.text = "남은 아이템 : " + mpReceivePlayer.tCount;
                    }
                }
                break;
            case Class_PlayScene.STAGE.Stage_2:
                {
                    if (Class_NetworkClient.GetInst().mMyUserInfo.mMyTurn == true)
                    {
                        mCountTxt.text = "남은 적 수 : " + mpPlayer.tEnemyCount;
                    }
                    else
                    {
                        mCountTxt.text = "남은 적 수 : " + mpReceivePlayer.tEnemyCount;
                    }
                }
                break;
            case Class_PlayScene.STAGE.Stage_3:
                break;
        }
    }

    private void OnTriggerEnter(Collider tCollider)
    {
        if (tCollider.CompareTag("TagPlayer"))
        {
            Class_Singleton_Sound.GetInst().Play("GameClear");
            mBntClear.gameObject.SetActive(true);
            Time.timeScale = 0.0f;
        }
        else if(tCollider.CompareTag("TagOtherPlayer"))
        {
            Class_Singleton_Sound.GetInst().Play("GameClear");
            mBntClear.gameObject.SetActive(true);
            Time.timeScale = 0.0f;
        }
    }

    public void OnClickBtnClick()
    {
        Class_Singleton_Sound.GetInst().Play("ClickSound_2");
        Debug.Log("Game Clear");
        Time.timeScale = 1.0f;
        byte[] tBuffer = new byte[1024];

        tBuffer[0] = (byte)PROTOCOL.REQ_GAME_CLEAR;

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }

    public void OnClickBtnFail()
    {
        Class_Singleton_Sound.GetInst().Play("ClickSound_2");
        Debug.Log("Game Fail");
        Time.timeScale = 1.0f;
        byte[] tBuffer = new byte[1024];

        tBuffer[0] = (byte)PROTOCOL.REQ_GAME_FAIL;

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }

    public void StageFail()
    {
        mBtnFail.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
        BtnOpen = true;
    }
}