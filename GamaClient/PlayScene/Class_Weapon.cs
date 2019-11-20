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
                Class_Singleton_Sound.GetInst().Play("Damage");
                tCollider.GetComponent<Class_Enemy>().DamageStart();
                tCollider.GetComponent<Class_Enemy>().mEnemyHP -= Class_NetworkClient.GetInst().mMyUserInfo.mAP;
            }
        }
    }

    //private void OnCollisionEnter(Collision tCollision)
    //{
    //    if(tCollision.gameObject.CompareTag("TagEnemy"))
    //    {
    //        tCollision.gameObject.GetComponent<Class_Enemy>().mEnemyHP -= Class_NetworkClient.GetInst().mMyUserInfo.mAP;
    //    }
    //}
}
