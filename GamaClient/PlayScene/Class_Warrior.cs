using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Warrior : MonoBehaviour
{
    private Animator anim;

    private Class_Player mpPlayer = null;

    void Start()
    {
        anim = GetComponent<Animator>();

        mpPlayer = FindObjectOfType<Class_Player>();
    }

    void Update()
    {
        if (Class_NetworkClient.GetInst().mMyUserInfo.mMyTurn == true)
        {
            if (Mathf.Abs(mpPlayer.tHorizontal) > 0.0f || Mathf.Abs(mpPlayer.tVertical) > 0.0f)
            {
                anim.SetBool("Run", true);
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


    void DamageStart()
    {
        anim.SetBool("Damage", true);
    }

    void DamageEnd()
    {
        anim.SetBool("Damage", false);
    }
}
