using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_ReceiveWizard : MonoBehaviour
{
    private Animator anim;

    private Class_ReceivePlayer mpPlayer = null;

    private Class_WizardAttack PFAttack = null;

    public GameObject mpAttackPos = null;

    void Start()
    {
        anim = GetComponent<Animator>();
        
        mpPlayer = FindObjectOfType<Class_ReceivePlayer>();
        PFAttack = Resources.Load<Class_WizardAttack>("Prefabs/PFAttack");
    }

    void Update()
    {
        if (Mathf.Abs(mpPlayer.tHorizontal) > 0.0f || Mathf.Abs(mpPlayer.tVertical) > 0.0f)
        {
            if (mpPlayer.tSpace == 1) 
            {
                anim.SetBool("Attack", false);
                anim.SetBool("Run", false);
                anim.SetBool("RunAttack", true);
            }
            else
            {
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

        if (mpPlayer.tSpace == 1) 
        {
            if (mpPlayer.tHorizontal == 0 && mpPlayer.tVertical == 0)
            {
                anim.SetBool("RunAttack", false);
                anim.SetBool("Attack", true);
            }
        }
        else
        {
            anim.SetBool("Attack", false);
        }
    }

    void DoAttack()
    {
        Class_Singleton_Sound.GetInst().Play("WizardAttack");
        Class_WizardAttack tAttack = Instantiate(PFAttack, mpAttackPos.transform.position, Quaternion.identity);

        tAttack.GetComponent<Rigidbody>().AddForce(this.transform.forward * 30, ForceMode.Impulse);

        Destroy(tAttack.gameObject, 3);
    }
}

