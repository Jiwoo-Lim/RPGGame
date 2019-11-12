using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Class_ClearScene : MonoBehaviour
{
    public Text mpFirstUserInfoTxt = null;

    private Class_Singleton_User tFirstUser = new Class_Singleton_User();
    // Start is called before the first frame update
    void Start()
    {
        tFirstUser = Class_NetworkClient.GetInst().mUserInfoes.Find(f => f.mClearCount > 0);
    }

    // Update is called once per frame
    void Update()
    {
        mpFirstUserInfoTxt.text = "1위 : <color='red'>" + tFirstUser.mUserName + "</color>\n직업 : <color='red'>" + tFirstUser.mOccupation + "</color>\n클리어 횟수 : <color='red'>" + tFirstUser.mClearCount + "</color>";
    }
}
