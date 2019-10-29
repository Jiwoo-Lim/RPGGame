using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Class_Player : MonoBehaviour
{
    float tHorizontal = 0.0f;
    float tVertical = 0.0f;
    public float mSpeedScalar = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tHorizontal = Input.GetAxis("Horizontal");
        tVertical = Input.GetAxis("Vertical");

        Vector3 mMoveDir = new Vector3();
        mMoveDir = tHorizontal * Vector3.right + tVertical * Vector3.forward;

        this.transform.Translate(mMoveDir * Time.deltaTime * mSpeedScalar, Space.World);
    }
}
