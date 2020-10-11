using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioClip defaultBGM;
    public AudioClip gameBGM;
    public AudioClip mouseOverAudio;
    public AudioClip mouseClickAudio;
    public AudioSource backgroundMusic;
    public AudioSource mouseAudio;
    public AudioMixerSnapshot scilence;
    public AudioMixerSnapshot normal;

    public void Awake()
    {
        instance = this;
        DontDestroyOnLoad(instance.gameObject);
    }

    public IEnumerator PlayDefaultMusic()
    {
        Debug.Log(backgroundMusic.clip);
        scilence.TransitionTo(0.5f);
        yield return new WaitForSeconds(0.7f);
        backgroundMusic.Stop();
        backgroundMusic.clip = defaultBGM;
        backgroundMusic.Play();
        normal.TransitionTo(0.5f);
        yield return new WaitForSeconds(0.7f);
        yield break;
    }

    public IEnumerator PlayGameMusic()
    {
        scilence.TransitionTo(0.5f);
        yield return new WaitForSeconds(0.7f);
        backgroundMusic.Stop();
        backgroundMusic.clip = gameBGM;
        backgroundMusic.Play();
        normal.TransitionTo(0.5f);
        yield return new WaitForSeconds(0.7f);
        yield break;
    }

    public void PlayMouseClickAudio()
    {
        mouseAudio.PlayOneShot(mouseClickAudio);
    }

    public void PlayMouseOverAudio()
    {
        mouseAudio.PlayOneShot(mouseOverAudio);
    }
}
