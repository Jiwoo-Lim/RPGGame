using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Camera : MonoBehaviour
{
    public Class_Player mpPlayer = null;

    public float mCameraX = -10.0f;
    public float mCameraY = 10.0f;
    public float mCameraZ = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, mpPlayer.transform.position - Vector3.forward * mCameraZ + Vector3.up * mCameraY - Vector3.right * mCameraX, Time.deltaTime*100);

        this.transform.LookAt(mpPlayer.transform.position);
    }
}
