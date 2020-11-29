using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socket.Quobject.SocketIoClientDotNet.Client;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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
			Debug.Log("WaitingForStrike state enteted.");
		}

		public override void Update() {
			var x = Input.GetAxis("Horizontal");
			var y = Input.GetAxis("Vertical");
			strikeDir = (SerializableVector3)gameController.strikeDirection;
			Debug.Log(strikeDir);
			Networking.instance.socket.Emit("StrikeDirectionChange", JsonConvert.SerializeObject(strikeDir));
			cueTrans.SetValue(cue.transform);
			Networking.instance.socket.Emit("CuePositionChange", JsonConvert.SerializeObject(cueTrans));
			//Unity的transform屬性無法被序列化以透過網路傳送，故須經過此轉換
			cameraTrans.SetValue(mainCamera.transform);
			Networking.instance.socket.Emit("CameraPositionChange", JsonConvert.SerializeObject(cameraTrans));
			if (x != 0) {
				var angle = x * 75 * Time.deltaTime;
				gameController.strikeDirection = Quaternion.AngleAxis(angle, Vector3.up) * gameController.strikeDirection;
				Debug.Log(gameController.strikeDirection);
				mainCamera.transform.RotateAround(cueBall.transform.position, Vector3.up, angle);
				cue.transform.RotateAround(cueBall.transform.position, Vector3.up, angle);
			}
			Debug.DrawLine(cueBall.transform.position, cueBall.transform.position + gameController.strikeDirection * 10);
			
			if (y != 0)
			{
				var angle2 = y * 50 * Time.deltaTime;
				gameController.strikeDirection = Quaternion.AngleAxis(angle2, Vector3.forward) * gameController.strikeDirection;
				mainCamera.transform.RotateAround(cueBall.transform.position, Vector3.forward, angle2);
				cue.transform.RotateAround(cueBall.transform.position, Vector3.forward, angle2);
			}
			if (Input.GetButtonDown("Fire1") && (IsPointerOverUIObject() == false)) {
				Networking.instance.socket.Emit("CueReleased", "");
				gameController.currentState = new GameStates.StrikingState(gameController);
			}
		}

		public static bool IsPointerOverUIObject()
		{
			PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
			return results.Count > 0;
		}
	}
}
