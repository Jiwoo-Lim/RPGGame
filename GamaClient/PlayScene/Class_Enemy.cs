using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Enemy : MonoBehaviour
{
    public int mEnemyHP = 2500;

    public Class_EnemyJon mpEnemyJon = null;

    private Class_Player mpPlayer = null;

    void Start()
    {
        mpPlayer = FindObjectOfType<Class_Player>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] tHitCollider = Physics.OverlapSphere(this.transform.position, 10f);

        foreach (Collider tCollider in tHitCollider)
        {
            if (tCollider.CompareTag("TagPlayer"))
            {
                Vector3 tDir = Vector3.zero;
                tDir = tCollider.transform.position - this.transform.position;

                this.transform.forward = tDir;

                this.transform.position = Vector3.MoveTowards(this.transform.position, tCollider.transform.position, 3 * Time.deltaTime);
            }
        }

        //죽을 경우
        if (mEnemyHP <= 0) 
        {
            mpEnemyJon.mEnemys.Remove(this);
            Destroy(this.gameObject);
            mpPlayer.tEnemyCount -= 1;
        }
    }

    private void OnCollisionEnter(Collision tCollision)
    {
        if(tCollision.collider.CompareTag("TagPlayer"))
        {
            Class_NetworkClient.GetInst().mMyUserInfo.mHP -= 50;
        }
    }
}
