using UnityEngine;
using System.Collections;
using Socket.Quobject.SocketIoClientDotNet.Client;

public class PocketsController : MonoBehaviour {
	public GameObject redBalls;
	public GameObject cueBall;
	public AudioSource ballAudioSource;
	private Vector3 originalCueBallPosition;
	private QSocket socket;

	void Start() {
		originalCueBallPosition = cueBall.transform.position;
	}

	void OnCollisionEnter(Collision collision) {
		foreach (var transform in redBalls.GetComponentsInChildren<Transform>()) {
			if (transform.name == collision.gameObject.name) {
				var objectName = collision.gameObject.name;
				GameObject.Destroy(collision.gameObject);
				var ballNumber = int.Parse(objectName.Replace("Ball", ""));
				PoolGameController.GameInstance.BallPocketed(ballNumber);
				ballAudioSource.PlayOneShot(PoolAudio.instance.ballSinkAudio);
			}
		}

		if (cueBall.transform.name == collision.gameObject.name) {
			cueBall.transform.position = originalCueBallPosition;
		}
	}
}
