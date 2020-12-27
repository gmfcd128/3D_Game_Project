using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollisionAudio : MonoBehaviour
{
    public float ballHitTriggerVelocity;
    public float ballHitHardTriggerVelocity;
    public AudioSource ballAudioSource;
    public static BallCollisionAudio instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
    }


    public void PlayBallCollisionSound(Collision collision)
    {
        float velocity = collision.relativeVelocity.magnitude;
        if (velocity > ballHitHardTriggerVelocity)
        {
            ballAudioSource.PlayOneShot(PoolAudio.instance.ballCollisionAudio[2], 1.0F * (velocity / 100));
        }
        else if (velocity > ballHitTriggerVelocity)
        {
            int sampleIndex = Random.Range(0, 1);
            ballAudioSource.PlayOneShot(PoolAudio.instance.ballCollisionAudio[sampleIndex], 1.0F * (velocity / 100));
        }
        else
        {
            int sampleIndex = Random.Range(0, 1);
            ballAudioSource.PlayOneShot(PoolAudio.instance.clackAudio[sampleIndex], 1.0F * (velocity / 100));
        }
    }

    public void PlayStrikeSound(Vector3 position, float relativeDistance)
    {
        Debug.Log("Strike Audio Volume : " + relativeDistance);
        AudioSource.PlayClipAtPoint(PoolAudio.instance.cueAudio, position, 1.0F * relativeDistance);
    }

    public float Saturate(float value, int threshold)
    {
        if (value > threshold)
        {
            return threshold;
        }
        else
        {
            return value;
        }
    }
}
