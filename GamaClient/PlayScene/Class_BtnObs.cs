using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_BtnObs : MonoBehaviour
{
    Class_Obstacle mpObstacle = null;

    GameObject PFBtnRed = null;

    GameObject tBtnRed = null;

    bool mTagPlayer = false;

    int mTimeCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        PFBtnRed = Resources.Load<GameObject>("Prefabs/Button Platform 01 Red");
        mpObstacle = FindObjectOfType<Class_Obstacle>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mTagPlayer)
        {
            if (tBtnRed == null)
            {
                tBtnRed = Instantiate(PFBtnRed, this.transform.position, Quaternion.identity);
            }
        }
        else if (mTagPlayer == false)
        {
            if (tBtnRed != null)
            {
                Destroy(tBtnRed);
            }
        }

        if (mTimeCount > 0)
        {
            mTimeCount--;
        }
        else if (mTimeCount == 0 && mTagPlayer == true) 
        {
            mTagPlayer = false;
            mpObstacle.DoDown();
        }
    }

    private void OnTriggerEnter(Collider tCollider)
    {
        if (tCollider.CompareTag("TagPlayer"))
        {
            if (tBtnRed == null)
            {
                mTimeCount = 120;
                Debug.Log("TagPlayer");
                mTagPlayer = true;
                mpObstacle.DoUp();
            }
        }
    }
}
