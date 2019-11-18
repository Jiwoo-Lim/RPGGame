using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Class_ReceivePlayerUI : MonoBehaviour
{
    Class_Singleton_User mpReceivePlayer = null;
    public Text mpPlayerName = null;
    public Text mpPlayerHPTxt = null;

    public Slider mpPlayerHPbar = null;
    private int MaxHp = 0;

    // Start is called before the first frame update
    void Start()
    {
        mpReceivePlayer = Class_NetworkClient.GetInst().mUserInfoes.Find(u => u.mUserName != Class_NetworkClient.GetInst().mMyUserInfo.mUserName);

        mpPlayerName.text = mpReceivePlayer.mUserName;
        MaxHp = mpReceivePlayer.mHP;
    }

    // Update is called once per frame
    void Update()
    {

        mpPlayerHPTxt.text = mpReceivePlayer.mHP.ToString();

        mpPlayerHPbar.value = (float)mpReceivePlayer.mHP / (float)MaxHp;
    }
}
