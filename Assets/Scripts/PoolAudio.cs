using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolAudio : MonoBehaviour
{
    public AudioClip cueAudio;
    public AudioClip ballSinkAudio;
    public AudioClip[] clackAudio;
    public AudioClip[] ballCollisionAudio;
    public AudioClip cushionCollisionAudio;
    public static PoolAudio instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }


}
