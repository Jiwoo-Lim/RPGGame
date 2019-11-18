using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Class_ClearScene : MonoBehaviour
{
    public Text mpFirstUserInfoTxt = null;

    private Class_Singleton_User tFirstUser = new Class_Singleton_User();
    // Start is called before the first frame update
    void Start()
    {
        tFirstUser = Class_NetworkClient.GetInst().mUserInfoes.Find(f => f.mClearCount > 0);
        mpFirstUserInfoTxt.text = "1위 : <color='red'>" + tFirstUser.mUserName + "</color>\n직업 : <color='red'>" + tFirstUser.mOccupation + "</color>\n클리어 횟수 : <color='red'>" + tFirstUser.mClearCount + "</color>";

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
