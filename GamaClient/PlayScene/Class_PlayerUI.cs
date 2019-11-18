using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Class_PlayerUI : MonoBehaviour
{
    public Text mpPlayerName = null;
    public Text mpPlayerHPTxt = null;

    public Slider mpPlayerHPbar = null;
    private int MaxHp = 0;
    // Start is called before the first frame update
    void Start()
    {
        mpPlayerName.text = Class_NetworkClient.GetInst().mMyUserInfo.mUserName;
        MaxHp = Class_NetworkClient.GetInst().mMyUserInfo.mHP;
    }

    // Update is called once per frame
    void Update()
    {        
        mpPlayerHPTxt.text = Class_NetworkClient.GetInst().mMyUserInfo.mHP.ToString();
        mpPlayerHPbar.value = (float)Class_NetworkClient.GetInst().mMyUserInfo.mHP / (float)MaxHp;
    }
}
