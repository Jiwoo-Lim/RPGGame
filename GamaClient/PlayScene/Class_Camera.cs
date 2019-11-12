using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Camera : MonoBehaviour
{
    private Class_Player mpPlayer = null;
    private Class_ReceivePlayer mpOtherPlayer = null;

    public float mCameraX = -10.0f;
    public float mCameraY = 10.0f;
    public float mCameraZ = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        mpPlayer = FindObjectOfType<Class_Player>();
        mpOtherPlayer = FindObjectOfType<Class_ReceivePlayer>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Class_NetworkClient.GetInst().mMyUserInfo.mMyTurn == true)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, mpPlayer.transform.position - Vector3.forward * mCameraZ + Vector3.up * mCameraY - Vector3.right * mCameraX, Time.deltaTime * 60);

            this.transform.LookAt(mpPlayer.transform.position);
        }
        else
        {
            this.transform.position = Vector3.Lerp(this.transform.position, mpOtherPlayer.transform.position - Vector3.forward * mCameraZ + Vector3.up * mCameraY - Vector3.right * mCameraX, Time.deltaTime * 60);

            this.transform.LookAt(mpOtherPlayer.transform.position);
        }
    }
}
