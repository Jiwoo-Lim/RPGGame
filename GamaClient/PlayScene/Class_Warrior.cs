using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Warrior : MonoBehaviour
{
    private Animator anim;

    private Class_Player mpPlayer = null;

    private Class_Clear mpClear = null;

    void Start()
    {
        anim = GetComponent<Animator>();

        mpPlayer = FindObjectOfType<Class_Player>();

        mpClear = FindObjectOfType<Class_Clear>();
    }

    void Update()
    {
        if (Class_NetworkClient.GetInst().mMyUserInfo.mMyTurn == true)
        {
            if (anim.GetBool("Attack") == false)
            {
                if (Mathf.Abs(mpPlayer.tHorizontal) > 0.0f || Mathf.Abs(mpPlayer.tVertical) > 0.0f)
                {
                    anim.SetBool("Run", true);
                }
                else
                {
                    anim.SetBool("Run", false);
                }
            }

            if (Input.GetKey(KeyCode.Space))
            {
                mpPlayer.tSpace = 1;
                anim.SetBool("Run", false);
                anim.SetBool("Attack", true);
            }
            else
            {
                mpPlayer.tSpace = 0;
                anim.SetBool("Attack", false);
            }

            if (Class_NetworkClient.GetInst().mMyUserInfo.mHP <= 0)
            {
                anim.SetBool("Death", true);
            }
        }
    }


    public void DamageStart()
    {
        Class_Singleton_Sound.GetInst().Play("Damage");
        anim.SetBool("Damage", true);
    }

    public void DamageEnd()
    {
        anim.SetBool("Damage", false);
    }

    public void DeathEnd()
    {
        mpClear.StageFail();
    }

    public void AttackSound()
    {
        Class_Singleton_Sound.GetInst().Play("WarriorAttack");
    }
}
