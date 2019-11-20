using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_Singleton_Sound
{
    private static Class_Singleton_Sound mInstance = null;

    public Dictionary<string, AudioSource> mSoundDictionary = new Dictionary<string, AudioSource>();

    protected Class_Singleton_Sound()
    {
        mInstance = null;
    }

    public static Class_Singleton_Sound GetInst()
    {
        if(mInstance==null)
        {
            mInstance = new Class_Singleton_Sound();
        }
        return mInstance;
    }

    public void DoRegist(AudioSource tAS)
    {
        mSoundDictionary.Add(tAS.clip.name, tAS);
    }

    public void Play(string tString)
    {
        mSoundDictionary[tString].Play();
    }

    public void Stop(string tString)
    {
        mSoundDictionary[tString].Stop();
    }
}
