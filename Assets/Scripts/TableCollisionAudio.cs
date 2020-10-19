using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCollisionAudio : MonoBehaviour
{
    public AudioSource tableAudioSource;
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        Debug.Log("Table Collision Audio Triggered.");
        tableAudioSource.PlayOneShot(PoolAudio.instance.cushionCollisionAudio, 1.0f * (rb.velocity.magnitude / 100));
    }
}
