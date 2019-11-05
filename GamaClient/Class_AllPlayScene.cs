using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_AllPlayScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        byte[] tBuffer = new byte[1024];
        tBuffer[0] = (byte)PROTOCOL.REQ_QUIT_GAME;

        Class_NetworkClient.GetInst().Send(tBuffer, tBuffer.Length);
    }
}
