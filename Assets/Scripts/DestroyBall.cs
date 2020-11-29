using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBall : MonoBehaviour
{
    public GameObject cueBall;

    private Vector3 originalCueBallPosition;
    void Start()
    {
        originalCueBallPosition = cueBall.transform.position;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            if (cueBall.transform.name == collision.gameObject.name)
            {
                cueBall.transform.position = originalCueBallPosition;
            }
            else 
            {
                Debug.Log("Destroyed a ball in house");
                Destroy(collision.gameObject);
            }
        }
    }
}
