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
            //AudioSource.PlayClipAtPoint(PoolAudio.instance.ballCollisionAudio[2], collision.contacts[0].point, 1.0F * (velocity / 100));
            ballAudioSource.PlayOneShot(PoolAudio.instance.ballCollisionAudio[2], 1.0F * (velocity / 100));
        }
        else if (velocity > ballHitTriggerVelocity)
        {
            int sampleIndex = Random.Range(0, 1);
            //AudioSource.PlayClipAtPoint(PoolAudio.instance.ballCollisionAudio[sampleIndex], collision.contacts[0].point, 1.0F * (velocity / 100));
            ballAudioSource.PlayOneShot(PoolAudio.instance.ballCollisionAudio[sampleIndex], 1.0F * (velocity / 100));
        }
        else
        {
            int sampleIndex = Random.Range(0, 1);
            //AudioSource.PlayClipAtPoint(PoolAudio.instance.clackAudio[sampleIndex], collision.contacts[0].point, 1.0F * (velocity / 100));
            ballAudioSource.PlayOneShot(PoolAudio.instance.clackAudio[sampleIndex], 1.0F * (velocity / 100));
        }
    }

    public void PlayStrikeSound(Vector3 position, Vector3 force)
    {
        AudioSource.PlayClipAtPoint(PoolAudio.instance.cueAudio, position, 1.0F * (force.magnitude / 100));
    }
}
