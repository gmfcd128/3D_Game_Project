using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

namespace GameStates {
	public class StrikingState : AbstractGameObjectState {
		private PoolGameController gameController;

		private GameObject cue;
		private GameObject cueBall;
		SerializedTransform cueTrans;

		private float cueDirection = -1;
		private float speed = 7;

		public StrikingState(MonoBehaviour parent) : base(parent) { 
			gameController = (PoolGameController)parent;
			cue = gameController.cue;
			cueBall = gameController.cueBall;
			cueTrans = new SerializedTransform();
		}

		public override void Update() {
			if (Input.GetButtonUp("Fire1")) {
				gameController.currentState = new GameStates.StrikeState(gameController);
				WebGLPluginJS.SocketEmit("ballStriked", "");
			}
		}

		public override void FixedUpdate () {
			var distance = Vector3.Distance(cue.transform.position, cueBall.transform.position);
			if (distance < PoolGameController.MIN_DISTANCE || distance > PoolGameController.MAX_DISTANCE)
				cueDirection *= -1;
			cue.transform.Translate(Vector3.down * speed * cueDirection * Time.fixedDeltaTime);
			cueTrans.SetValue(cue.transform);
			WebGLPluginJS.SocketEmit("CuePositionChange", JsonConvert.SerializeObject(cueTrans));
		}
	}
}