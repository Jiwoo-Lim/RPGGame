using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Player_1 : MonoBehaviour
{
    Class_Charactor mpCharactor = null;

    float mtime = 0;
    // Start is called before the first frame update
    void Start()
    {
        mpCharactor = FindObjectOfType<Class_Charactor>();
    }

    // Update is called once per frame
    void Update()
    {
        CharaTurn();
    }

    public void CharaTurn()
    {
        if (mpCharactor.mPlayer_1 == true)
        {
            mtime += Time.deltaTime;
            if (mtime > 0.1f)
            {
                this.transform.Rotate(new Vector3(0, 7.2f, 0));
                Debug.Log("Player_1 turn");
                mtime = 0.0f;
            }
        }
        else
        {
            this.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
}
