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
		private bool initialized = false;
		SerializedTransform cameraTrans;
		SerializedTransform cueTrans;
		SerializableVector3 strikeDir;

		private PoolGameController gameController;

		public WaitingForStrikeState(MonoBehaviour parent) : base(parent) {
			Debug.Log("Oops!!");
			gameController = (PoolGameController)parent;
			cue = gameController.cue;
			cueBall = gameController.cueBall;
			mainCamera = gameController.mainCamera;
			cameraTrans = new SerializedTransform();
			cueTrans = new SerializedTransform();
			strikeDir = new SerializableVector3();
			if (cue.GetComponent<Renderer>().enabled == false)
			{
				cue.GetComponent<Renderer>().enabled = true;
			}
			InvertCameraPosition();
			Debug.Log("WaitingForStrike state enteted.");
		}

		public override void Update() {
			var x = Input.GetAxis("Horizontal");
			var y = Input.GetAxis("Vertical");
			if (x != 0) {
				var angle = x * 75 * Time.deltaTime;
				gameController.strikeDirection = Quaternion.AngleAxis(angle, Vector3.up) * gameController.strikeDirection;
				Debug.Log(gameController.strikeDirection);
				strikeDir = (SerializableVector3)gameController.strikeDirection;
				Debug.Log(strikeDir);
				Networking.instance.socket.Emit("StrikeDirectionChange", JsonConvert.SerializeObject(strikeDir));
				mainCamera.transform.RotateAround(cueBall.transform.position, Vector3.up, angle);
				//Unity的transform屬性無法被序列化以透過網路傳送，故須經過此轉換
				cameraTrans.SetValue(mainCamera.transform);
				Networking.instance.socket.Emit("CameraPositionChange", JsonConvert.SerializeObject(cameraTrans));
				cue.transform.RotateAround(cueBall.transform.position, Vector3.up, angle);
				cueTrans.SetValue(cue.transform);
				Networking.instance.socket.Emit("CuePositionChange", JsonConvert.SerializeObject(cueTrans));
			}
			Debug.DrawLine(cueBall.transform.position, cueBall.transform.position + gameController.strikeDirection * 10);
			
			if (y != 0)
			{
				var angle2 = y * 50 * Time.deltaTime;
				gameController.strikeDirection = Quaternion.AngleAxis(angle2, Vector3.forward) * gameController.strikeDirection;
				strikeDir = gameController.strikeDirection;
				Networking.instance.socket.Emit("StrikeDirectionChange", JsonConvert.SerializeObject(strikeDir));
				mainCamera.transform.RotateAround(cueBall.transform.position, Vector3.forward, angle2);
				//Unity的transform屬性無法被序列化以透過網路傳送，故須經過此轉換
				cameraTrans.SetValue(mainCamera.transform);
				Networking.instance.socket.Emit("CameraPositionChange", JsonConvert.SerializeObject(cameraTrans));
				cue.transform.RotateAround(cueBall.transform.position, Vector3.forward, angle2);
				cueTrans.SetValue(cue.transform);
				Networking.instance.socket.Emit("CuePositionChange", JsonConvert.SerializeObject(cueTrans));
			}
			if (Input.GetButtonDown("Fire1")) {
				Networking.instance.socket.Emit("CueReleased", "");
				gameController.currentState = new GameStates.StrikingState(gameController);
			}
		}

		private void InvertCameraPosition()
		{
			mainCamera.transform.position = new Vector3(cueBall.transform.position.x + (cueBall.transform.position.x - mainCamera.transform.position.x),
														mainCamera.transform.position.y,
														cueBall.transform.position.z + (cueBall.transform.position.z - mainCamera.transform.position.z));
			Vector3 cameraRot = mainCamera.transform.rotation.eulerAngles;
			cameraRot = new Vector3(cameraRot.x, cameraRot.y + 180, cameraRot.z);
			mainCamera.transform.rotation = Quaternion.Euler(cameraRot);
		}
	}
}
