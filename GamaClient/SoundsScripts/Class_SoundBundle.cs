using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Class_SoundBundle : MonoBehaviour
{
    public List<AudioSource> mSoundList = new List<AudioSource>();

    private void Awake()
    {
        foreach (var S in mSoundList)
        {
            Class_Singleton_Sound.GetInst().DoRegist(S);
        }

        GameObject.DontDestroyOnLoad(this.gameObject);
    }
}
