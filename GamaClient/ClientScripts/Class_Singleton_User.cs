using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Class_Singleton_User
{
    //private static Class_Singleton_User mInstance = null;

    public int mUserKey = 0;
    public string mUserName = "";
    public string mOccupation = "";
    public int mHP = 0;
    public int mAP = 0;

    //private Class_Singleton_User()
    //{
    //    mInstance = null;
    //}

    //public static Class_Singleton_User GetInst()
    //{
    //    if(mInstance==null)
    //    {
    //        mInstance = new Class_Singleton_User();
    //    }

    //    return mInstance;
    //}

    //public void CreateUser()
    //{

    //}
}
