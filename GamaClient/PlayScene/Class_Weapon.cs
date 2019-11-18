using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Weapon : MonoBehaviour
{
    private Class_Player mpPlayer = null;

    void Start()
    {
        mpPlayer = FindObjectOfType<Class_Player>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider tCollider)
    {
        if (mpPlayer.tSpace == 1)
        {
            if (tCollider.CompareTag("TagEnemy"))
            {
                tCollider.GetComponent<Class_Enemy>().mEnemyHP -= Class_NetworkClient.GetInst().mMyUserInfo.mAP;
            }
        }
    }
}
