﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAudioStatus : MonoBehaviour
{
    // reference : https://stackoverflow.com/questions/41290547/detect-collision-colliding-only-once
    // Start is called before the first frame update
    bool detectedBefore = false;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            //Exit if we have already done some damage
            if (detectedBefore)
            {
                return;
            }

            //Set the other detectedBefore variable to true
            CollisionAudioStatus stat = collision.gameObject.GetComponent<CollisionAudioStatus>();
            if (stat)
            {
                stat.detectedBefore = true;
            }

            // Put damage/or code to run once below
            Debug.Log(collision.relativeVelocity.magnitude);
            BallCollisionAudio.instance.PlayBallCollisionSound(collision);
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ball")
        {
            //Reset on exit?
            detectedBefore = false;
        }
    }
}
