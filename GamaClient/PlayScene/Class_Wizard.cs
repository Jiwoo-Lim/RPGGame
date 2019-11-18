using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Wizard : MonoBehaviour
{
    private Animator anim;

    private Class_Player mpPlayer = null;

    private Class_WizardAttack PFAttack = null;

    public GameObject mpAttackPos = null;

    void Start()
    {
        anim = GetComponent<Animator>();

        mpPlayer = FindObjectOfType<Class_Player>();
        PFAttack = Resources.Load<Class_WizardAttack>("Prefabs/PFAttack");
    }

    void Update()
    {
        if (Class_NetworkClient.GetInst().mMyUserInfo.mMyTurn == true)
        {
            if (Mathf.Abs(mpPlayer.tHorizontal) > 0.0f || Mathf.Abs(mpPlayer.tVertical) > 0.0f)
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    mpPlayer.tSpace = 1;
                    anim.SetBool("Attack", false);
                    anim.SetBool("Run", false);
                    anim.SetBool("RunAttack", true);
                }
                else
                {
                    mpPlayer.tSpace = 0;
                    anim.SetBool("RunAttack", false);
                    anim.SetBool("Run", true);
                }
            }
            else
            {
                anim.SetBool("Run", false);
            }

            //test
            //if (Input.GetKey(KeyCode.B))
            //{
            //    anim.SetBool("Damage", true);
            //}

            if (Input.GetKey(KeyCode.Space))
            {
                mpPlayer.tSpace = 1;
                if (mpPlayer.tHorizontal == 0 && mpPlayer.tVertical == 0)
                {
                    anim.SetBool("RunAttack", false);
                    anim.SetBool("Attack", true);
                }
            }
            else
            {
                mpPlayer.tSpace = 0;
                anim.SetBool("Attack", false);
            }
        }
    }

    void DamageStart()
    {
        anim.SetBool("Damage", true);
    }

    void DamageEnd()
    {
        anim.SetBool("Damage", false);
    }

    void DoAttack()
    {
        Class_WizardAttack tAttack = Instantiate(PFAttack, mpAttackPos.transform.position, Quaternion.identity);

        tAttack.GetComponent<Rigidbody>().AddForce(this.transform.forward * 30, ForceMode.Impulse);

        Destroy(tAttack.gameObject, 3);
    }

}

