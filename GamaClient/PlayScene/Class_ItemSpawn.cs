using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_ItemSpawn : MonoBehaviour
{
    GameObject PFItem = null;

    public bool mSpawn = false;

    // Start is called before the first frame update
    void Start()
    {
        PFItem = Resources.Load<GameObject>("Prefabs/PFItem");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSpawnItem(bool Spawn)
    {
        if(Spawn==true)
        {
            InvokeRepeating("SpawnItem", 3, 2);
            mSpawn = true;
        }
        else if(Spawn == false)
        {
            CancelInvoke("SpawnItem");
            mSpawn = false;
        }
    }

    public void SpawnItem()
    {
        int i, j;
        if (Class_NetworkClient.GetInst().mMyUserInfo.mUserName == Class_NetworkClient.GetInst().mRoomMaster)
        {

            i = Random.Range(-23, -7);
            j = Random.Range(42, 58);

            GameObject tItem = Instantiate<GameObject>(PFItem, new Vector3(i, 0, j), Quaternion.identity);

            Destroy(tItem, 2);
        }
        else
        {
            i = Random.Range(7, 23);
            j = Random.Range(42, 58);

            GameObject tItem = Instantiate<GameObject>(PFItem, new Vector3(i, 0, j), Quaternion.identity);

            Destroy(tItem, 2);
        }
    }
}
