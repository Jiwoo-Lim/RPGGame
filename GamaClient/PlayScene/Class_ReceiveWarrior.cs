using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_ReceiveWarrior : MonoBehaviour
{
    private Animator anim;

    private Class_ReceivePlayer mpPlayer = null;

    void Start()
    {
        anim = GetComponent<Animator>();

        mpPlayer = FindObjectOfType<Class_ReceivePlayer>();
    }

    void Update()
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

        if (mpPlayer.tSpace == 1) 
        {
            mpPlayer.tSpace = 1;
            anim.SetBool("Attack", true);
        }
        else
        {
            mpPlayer.tSpace = 0;
            anim.SetBool("Attack", false);
        }

        foreach (var u in Class_NetworkClient.GetInst().mUserInfoes)
        {
            if (u.mUserName != Class_NetworkClient.GetInst().mMyUserInfo.mUserName)
            {
                if (u.mHP <= 0)
                {
                    anim.SetBool("Death", true);
                }
            }
        }
    }
}
