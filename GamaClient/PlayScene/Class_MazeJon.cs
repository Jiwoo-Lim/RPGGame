using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_MazeJon : MonoBehaviour
{
    GameObject PFMaze = null;

    public bool mSpawn = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnMaze()
    {
        int tA = 0;
        tA = Random.Range(0, 2);
        if (Class_NetworkClient.GetInst().mMyUserInfo.mUserName == Class_NetworkClient.GetInst().mRoomMaster)
        {
            PFMaze = Resources.Load<GameObject>("Prefabs/PFMaze_" + tA);
            GameObject tMaze = Instantiate<GameObject>(PFMaze, PFMaze.transform.position, PFMaze.transform.rotation);
        }
        else
        {
            PFMaze = Resources.Load<GameObject>("Prefabs/PFMaze_" + tA + "_guest");
            GameObject tMaze = Instantiate<GameObject>(PFMaze, PFMaze.transform.position, PFMaze.transform.rotation);

        }
    }
}
