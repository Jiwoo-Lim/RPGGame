using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Player_0 : MonoBehaviour
{
    Class_Charactor mpCharactor = null;
    
    float mtime = 0;
    // Start is called before the first frame update
    void Start()
    {
        mpCharactor= FindObjectOfType<Class_Charactor>();
    }

    // Update is called once per frame
    void Update()
    {
        CharaTurn();
    }


    //Class_Charactor.mPlayer_0 = true일 경우 캐릭터 선택 버튼 활성화
    //Player_0 의 캐릭터의 Rotation의 Y값에 ++ 시켜 회전
    //캐릭터 선택을 누르면 Rotation의 Y값을 0으로 초기화.
    public void CharaTurn()
    {
        if (mpCharactor.mPlayer_0 == true)
        {
            mtime += Time.deltaTime;
            if (mtime > 0.1f)
            {
                this.transform.Rotate(new Vector3(0, 7.2f, 0));
                Debug.Log("Player_0 turn");
                mtime = 0.0f;
            }
        }
        else
        {
            this.transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
        }
    }
}
