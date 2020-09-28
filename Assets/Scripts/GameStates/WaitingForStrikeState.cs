using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;

namespace GameStates {
	public class WaitingForStrikeState  : AbstractGameObjectState {
		private GameObject cue;
		private GameObject cueBall;
		private GameObject mainCamera;
		SerializedTransform cameraTrans;
		SerializedTransform cueTrans;

		private PoolGameController gameController;

		public WaitingForStrikeState(MonoBehaviour parent) : base(parent) { 
			gameController = (PoolGameController)parent;
			cue = gameController.cue;
			cueBall = gameController.cueBall;
			mainCamera = gameController.mainCamera;
			cameraTrans = new SerializedTransform();
			cueTrans = new SerializedTransform();

			cue.GetComponent<Renderer>().enabled = true;
			Debug.Log("WaitingForStrike state enteted.");
		}

		public override void Update() {
			var x = Input.GetAxis("Horizontal");
			
			if (x != 0) {
				var angle = x * 75 * Time.deltaTime;
				gameController.strikeDirection = Quaternion.AngleAxis(angle, Vector3.up) * gameController.strikeDirection;
				mainCamera.transform.RotateAround(cueBall.transform.position, Vector3.up, angle);
				//Unity的transform屬性無法被序列化以透過網路傳送，故須經過此轉換
				cameraTrans.SetValue(mainCamera.transform);
				Networking.instance.socket.Emit("CameraPositionChange", JsonConvert.SerializeObject(cameraTrans));
				cue.transform.RotateAround(cueBall.transform.position, Vector3.up, angle);
				cueTrans.SetValue(cue.transform);
				Networking.instance.socket.Emit("CuePositionChange", JsonConvert.SerializeObject(cueTrans));
			}
			Debug.DrawLine(cueBall.transform.position, cueBall.transform.position + gameController.strikeDirection * 10);

			if (Input.GetButtonDown("Fire1")) {
				Networking.instance.socket.Emit("CueReleased", "");
				gameController.currentState = new GameStates.StrikingState(gameController);
			}
		}
	}
}