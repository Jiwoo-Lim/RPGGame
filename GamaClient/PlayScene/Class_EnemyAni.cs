using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_EnemyAni : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DamageEnd()
    {
        this.GetComponent<Animator>().SetBool("Damage", false);
    }
}
