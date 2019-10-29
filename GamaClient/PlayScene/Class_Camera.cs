using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Camera : MonoBehaviour
{
    public Class_Player mpPlayer = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, mpPlayer.transform.position - Vector3.forward * 10 + Vector3.up * 10 - Vector3.right * 10, Time.deltaTime*100);

        this.transform.LookAt(mpPlayer.transform.position);
    }
}
