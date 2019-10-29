using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class Class_SceneTitle : MonoBehaviour
{
    private string mIdComment = "";
    private string mPasswordComment = "";

    //[SerializeField]
    //private Text mpIdTxt = null;

    //[SerializeField]
    //private Text mpPasswordTxt = null;

    public InputField InputId = null;
    public InputField InputPassword = null;

    // Start is called before the first frame update
    void Start()
    {
        Class_NetworkClient.GetInst().CreateRyu();
        bool tResult = Class_NetworkClient.GetInst().Connect("127.0.0.1", 50765);
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

                            
                        }
                        break;
                }
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    public void OnClickBtnLogin()
    {
        mIdComment =InputId.text;
        mPasswordComment =InputPassword.text;


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
