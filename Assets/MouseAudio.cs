using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAudio : MonoBehaviour
{
    public AudioClip mouseOverAudio;
    public AudioClip mouseClickAudio;
    public static MouseAudio instance;
    public AudioSource mouseAudio;

    public void Awake()
    {
        instance = this;
    }

    public void mouseClick()
    {
        mouseAudio.PlayOneShot(mouseClickAudio);
    }

    public void OnMouseOver()
    {
        mouseAudio.PlayOneShot(mouseOverAudio);
    }
}
