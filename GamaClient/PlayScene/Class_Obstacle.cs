using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Obstacle : MonoBehaviour
{
    Vector3 mOriginPos = new Vector3();
    Vector3 mTargetPos = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        mOriginPos = this.transform.position;
        mTargetPos = this.transform.position + Vector3.up * 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoUp()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, this.transform.position + Vector3.up * 2, Time.deltaTime * 66);
        //this.transform.Translate(Vector3.up * 2,Space.World);
        //this.transform.position = Vector3.MoveTowards(mOriginPos, mTargetPos, Time.deltaTime * 50);
    }

    public void DoDown()
    {
        this.transform.position = mOriginPos;
    }
}
