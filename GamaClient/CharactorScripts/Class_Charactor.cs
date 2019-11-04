using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;
public class Class_Charactor : MonoBehaviour
{
    public bool mPlayer_0 = false;
    public bool mPlayer_1 = false;

    public Button mpOkBtn_0 = null;
    public Button mpOkBtn_1 = null;


    public Class_Occupation mpOccupation = null;

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
                    case PROTOCOL.ACK_LOGIN:
                        {
                            Debug.Log("ACK_LOGIN");
                            SceneManager.LoadScene("PlayScene_1");
                        }
                        break;
                }
            }


            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnClickCharactor()
    {
        Debug.Log("1P");
        mPlayer_0 = true;
        mPlayer_1 = false;
        //SceneManager.LoadScene("PlayScene_1");

        mpOkBtn_0.gameObject.SetActive(true);
        mpOkBtn_1.gameObject.SetActive(false);
    }

    public void OnClickBtnCharactor()
    {
        Debug.Log("2P");
        mPlayer_1 = true;
        mPlayer_0 = false;

        mpOkBtn_1.gameObject.SetActive(true);
        mpOkBtn_0.gameObject.SetActive(false);
    }

    public void OnClickBtnGoPlayScene_0()
    {
        mpOccupation.mOccupation = "Warrior";
        mpOccupation.mHP = 1000;
        mpOccupation.mAP = 500;


        Debug.Log("mOccupation : " + mpOccupation.mOccupation + "\n");
        Debug.Log("mHP : " + mpOccupation.mHP + "\n");
        Debug.Log("mAP : " + mpOccupation.mAP + "\n");

        byte[] tBuffer = new byte[1024];
        byte tProtocolID = (byte)PROTOCOL.REQ_CREATE_CHAR;

        int tOccupationlength = mpOccupation.mOccupation.Length;
        byte[] tOccupation = Encoding.UTF8.GetBytes(mpOccupation.mOccupation);

        byte[] tHP = BitConverter.GetBytes(mpOccupation.mHP);
        int tHPLength = tHP.Length;
        byte[] tAP = BitConverter.GetBytes(mpOccupation.mAP);
        int tAPLength = tAP.Length;

        Class_Singleton_User.GetInst().mHP = mpOccupation.mHP;
        Class_Singleton_User.GetInst().mAP = mpOccupation.mAP;
        Class_Singleton_User.GetInst().mOccupation = mpOccupation.mOccupation;

        tBuffer[0] = tProtocolID;
        tBuffer[1] = (byte)tHPLength;
        int tOffset = 2;
        tHP.CopyTo(tBuffer, tOffset);
        tOffset += tHPLength;
        tBuffer[tOffset] = (byte)tAPLength;
        tOffset += 1;
        tAP.CopyTo(tBuffer, tOffset);
        tOffset += tAPLength;
        tBuffer[tOffset] = (byte)tOccupationlength;
        tOffset += 1;
        tOccupation.CopyTo(tBuffer, tOffset);

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }

    public void OnClickBtnGoPlayScene_1()
    {
        mpOccupation.mOccupation = "Magition";
        mpOccupation.mHP = 500;
        mpOccupation.mAP = 1000;


        byte[] tBuffer = new byte[1024];
        byte tProtocolID = (byte)PROTOCOL.REQ_CREATE_CHAR;

        int tOccupationlength = mpOccupation.mOccupation.Length;
        byte[] tOccupation = Encoding.UTF8.GetBytes(mpOccupation.mOccupation);

        byte[] tHP = BitConverter.GetBytes(mpOccupation.mHP);
        int tHPLength = tHP.Length;
        byte[] tAP = BitConverter.GetBytes(mpOccupation.mAP);
        int tAPLength = tAP.Length;

        Class_Singleton_User.GetInst().mHP = mpOccupation.mHP;
        Class_Singleton_User.GetInst().mAP = mpOccupation.mAP;
        Class_Singleton_User.GetInst().mOccupation = mpOccupation.mOccupation;

        tBuffer[0] = tProtocolID;
        tBuffer[1] = (byte)tHPLength;
        int tOffset = 2;
        tHP.CopyTo(tBuffer, tOffset);
        tOffset += tHPLength;
        tBuffer[tOffset] = (byte)tAPLength;
        tOffset += 1;
        tAP.CopyTo(tBuffer, tOffset);
        tOffset += tAPLength;
        tBuffer[tOffset] = (byte)tOccupationlength;
        tOffset += 1;
        tOccupation.CopyTo(tBuffer, tOffset);

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }
}