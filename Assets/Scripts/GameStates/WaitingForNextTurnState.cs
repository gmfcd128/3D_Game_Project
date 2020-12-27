﻿using UnityEngine;
using Newtonsoft.Json;
namespace GameStates
{
    public class WaitingForNextTurnState : AbstractGameObjectState
    {
        private PoolGameController gameController;
        private GameObject cue;
        private GameObject cueBall;
        private GameObject redBalls;
        private GameObject mainCamera;
        private SerializedTransform cameraTrans;
        private SerializedTransform cueTrans;
        private SerializedTransform redBallTransform;

        private Vector3 cameraOffset;
        private Vector3 cueOffset;
        private Quaternion cameraRotation;
        private Quaternion cueRotation;

        public WaitingForNextTurnState(MonoBehaviour parent) : base(parent)
        {
            gameController = (PoolGameController)parent;

            cue = gameController.cue;
            cueBall = gameController.cueBall;
            redBalls = gameController.redBalls;
            mainCamera = gameController.mainCamera;
            cueTrans = new SerializedTransform();
            cameraTrans = new SerializedTransform();
            redBallTransform = new SerializedTransform();

            cameraOffset = cueBall.transform.position - mainCamera.transform.position;
            cameraRotation = mainCamera.transform.rotation;
            cueOffset = cueBall.transform.position - cue.transform.position;
            cueRotation = cue.transform.rotation;
            Debug.Log("WaitingForNextTurn...");
        }

        public override void FixedUpdate()
        {
            //Debug.Log(redBalls.GetComponentsInChildren<Transform>().Length);
            if (redBalls.GetComponentsInChildren<Transform>().Length == 1)
            {
                gameController.EndMatch();
            }
            else
            {
                var cueBallBody = cueBall.GetComponent<Rigidbody>();
                if (!(cueBallBody.IsSleeping() || cueBallBody.velocity == Vector3.zero))
                    return;
                /*JObject redBallsTransform = new JObject();
                foreach (Transform child in redBalls.transform)
                {
                    redBallTransform.SetValue(child.transform);
                    redBallsTransform.Add(new JProperty(child.gameObject.name, JsonConvert.SerializeObject(redBallTransform)));
                }
                socket.Emit("redBallsTransform", redBallsTransform.ToString());
                */

                foreach (var rigidbody in redBalls.GetComponentsInChildren<Rigidbody>())
                {
                    if (!(rigidbody.IsSleeping() || rigidbody.velocity == Vector3.zero))
                        return;
                }

                gameController.NextPlayer();
                // If all balls are sleeping, time for the next turn
                // This is kinda hacky but gets the job done
                gameController.currentState = new WaitingForStrikeState(gameController);
            }
        }

        public override void LateUpdate()
        {
            mainCamera.transform.position = cueBall.transform.position - cameraOffset;
            mainCamera.transform.rotation = cameraRotation;
            cameraTrans.SetValue(mainCamera.transform);
            WebGLPluginJS.SocketEmit("CameraPositionChange", JsonConvert.SerializeObject(cameraTrans));
            cue.transform.position = cueBall.transform.position - cueOffset;
            cue.transform.rotation = cueRotation;
            cueTrans.SetValue(cue.transform);
            WebGLPluginJS.SocketEmit("CuePositionChange", JsonConvert.SerializeObject(cueTrans));
        }
    }
}