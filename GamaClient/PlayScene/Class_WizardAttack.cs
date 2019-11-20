using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_WizardAttack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider tCollider)
    {
        if(tCollider.CompareTag("TagEnemy"))
        {
            Class_Singleton_Sound.GetInst().Play("Damage");

            tCollider.GetComponent<Class_Enemy>().DamageStart();

            tCollider.GetComponent<Class_Enemy>().mEnemyHP -= Class_NetworkClient.GetInst().mMyUserInfo.mAP;

            Destroy(this.gameObject);
        }

        if(tCollider.CompareTag("TagWall"))
        {
            Destroy(this.gameObject);
        }
    }
}
