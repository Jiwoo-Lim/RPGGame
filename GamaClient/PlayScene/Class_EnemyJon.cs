using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_EnemyJon : MonoBehaviour
{
    private Class_Enemy mEnemy;
    public List<Class_Enemy> mEnemys = null;

    public bool mSpawn = false;

    void Start()
    {
        mEnemy = Resources.Load<Class_Enemy>("Prefabs/PFEnemy");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnEnemy()
    {
        mSpawn = true;
        int i, j;
        if (Class_NetworkClient.GetInst().mMyUserInfo.mUserName == Class_NetworkClient.GetInst().mRoomMaster)
        {
            for (i = -52; i <= -50; i++)
            {
                for (j = 45; j < 55; j++)
                {
                    Class_Enemy tEnemy = Instantiate<Class_Enemy>(mEnemy, new Vector3(i, 0, j), Quaternion.identity);
                    mEnemys.Add(tEnemy);
                    j++;
                }
                i++;
            }
        }
        else
        {
            for (i = 33; i <= 35; i++)
            {
                for (j = 40; j < 50; j++)
                {
                    Class_Enemy tEnemy = Instantiate<Class_Enemy>(mEnemy, new Vector3(j, 0, i), Quaternion.identity);
                    j++;
                }
                i++;
            }
        }
    }
}
