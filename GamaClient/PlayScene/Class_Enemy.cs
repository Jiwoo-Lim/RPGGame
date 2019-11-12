using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Enemy : MonoBehaviour
{

    public int mEnemyHP = 50;
    public int mEnemyAP = 30;
    Class_Player tPlayer = null;
    // Start is called before the first frame update
    void Start()
    {
        tPlayer = FindObjectOfType<Class_Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
