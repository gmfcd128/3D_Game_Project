using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socket.Quobject.SocketIoClientDotNet.Client;

namespace GameStates
{
    public class StrikeState : AbstractGameObjectState
    {
        private PoolGameController gameController;

        private GameObject cue;
        private GameObject cueBall;

        private SerializedTransform cueTrans;
        private SerializableVector3 cueBallForce;

        private float speed = 30f;
        private float force = 0f;
        private float relativeDistance;

        public StrikeState(MonoBehaviour parent) : base(parent)
        {
            Debug.Log("StrikeState constructor entered.");
            gameController = (PoolGameController)parent;
            cue = gameController.cue;
            cueBall = gameController.cueBall;
            cueTrans = new SerializedTransform();
            var forceAmplitude = gameController.maxForce - gameController.minForce;
            relativeDistance = (Vector3.Distance(cue.transform.position, cueBall.transform.position) - PoolGameController.MIN_DISTANCE) / (PoolGameController.MAX_DISTANCE - PoolGameController.MIN_DISTANCE);
            force = forceAmplitude * relativeDistance + gameController.minForce;
            Debug.Log("Strike state enteted.");
        }

        public override void FixedUpdate()
        {
            var distance = Vector3.Distance(cue.transform.position, cueBall.transform.position);
            if (distance < PoolGameController.MIN_DISTANCE)
            {
                cueBall.GetComponent<Rigidbody>().AddForce(gameController.strikeDirection * force);
                BallCollisionAudio.instance.PlayStrikeSound(cueBall.transform.position, relativeDistance);
                cue.GetComponent<Renderer>().enabled = false;
                cueBallForce = gameController.strikeDirection * force;
                Debug.Log(gameController.strikeDirection);
                Debug.Log(force);
                WebGLPluginJS.SocketEmit("CueBallStriked", JsonConvert.SerializeObject(cueBallForce));
                cue.transform.Translate(Vector3.down * speed * Time.fixedDeltaTime);
                cueTrans.SetValue(cue.transform);
                WebGLPluginJS.SocketEmit("CuePositionChange", JsonConvert.SerializeObject(cueTrans));
                gameController.currentState = new GameStates.WaitingForNextTurnState(gameController);
            }
            else
            {
                cue.transform.Translate(Vector3.down * speed * -1 * Time.fixedDeltaTime);
                cueTrans.SetValue(cue.transform);
                WebGLPluginJS.SocketEmit("CuePositionChange", JsonConvert.SerializeObject(cueTrans));
            }
        }
    }
}