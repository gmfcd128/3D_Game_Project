using Socket.Quobject.SocketIoClientDotNet.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GameStates
{
    public class WatchingState : AbstractGameObjectState
    {
        private GameObject cue;
        private GameObject cueBall;
        private GameObject mainCamera;
        private bool cueStickEnable;
        private Vector3 cueBallForce;

        private SerializedTransform cueTransSerialized;
        private SerializedTransform cameraTransSerialized;
        private SerializableVector3 strikeDir;

        Transform currentPlayerCamera;

        private PoolGameController gameController;
        public WatchingState(MonoBehaviour parent) : base(parent)
        {
            Debug.Log("Watching state entered.");
            gameController = (PoolGameController)parent;
            cue = gameController.cue;
            cueBall = gameController.cueBall;
            mainCamera = gameController.mainCamera;
            currentPlayerCamera = mainCamera.transform;
        }

        

        public override void OnSocketEvent(string eventName, string data)
        {
            if (eventName.Equals("CuePositionChange"))
            {
                cueTransSerialized = JsonConvert.DeserializeObject<SerializedTransform>(@data);
            }
            else if (eventName.Equals("CameraPositionChange"))
            {
                cameraTransSerialized = JsonConvert.DeserializeObject<SerializedTransform>(@data);
            }
            else if (eventName.Equals("StrikeDirectionChange"))
            {
                strikeDir = JsonConvert.DeserializeObject<SerializableVector3>(@data);
            }
            else if (eventName.Equals("CueBallStriked"))
            {
                Debug.Log("The cue ball is hit.");
                cueBallForce = JsonConvert.DeserializeObject<SerializableVector3>(@data);
                cue.GetComponent<Renderer>().enabled = false;
                cueBall.GetComponent<Rigidbody>().AddForce(cueBallForce);
            }
            else if (eventName.Equals("continue"))
            {
                cueStickEnable = true;
            }
            
        }

        public override void Update()
        {
            if (cameraTransSerialized != null && cueTransSerialized != null)
            {
                DeserialTransform(currentPlayerCamera, serializedTransform: cameraTransSerialized);
                DeserialTransform(cue.transform, cueTransSerialized);
                gameController.InvertCameraPosition(currentPlayerCamera);
            }

            if(strikeDir.x != 0 || strikeDir.y != 0 || strikeDir.z != 0)
            {
                gameController.strikeDirection = (Vector3)strikeDir;
                Debug.Log(strikeDir);
            }

            if (cueStickEnable)
            {
                cue.GetComponent<Renderer>().enabled = true;
                cueStickEnable = false;
            }
        }

        public static void DeserialTransform(Transform _transform, SerializedTransform serializedTransform)
        {
            _transform.position = new Vector3(serializedTransform._position[0], serializedTransform._position[1], serializedTransform._position[2]);
            _transform.rotation = new Quaternion(serializedTransform._rotation[0], serializedTransform._rotation[1], serializedTransform._rotation[2], serializedTransform._rotation[3]);
            _transform.localScale = new Vector3(serializedTransform._scale[0], serializedTransform._scale[1], serializedTransform._scale[2]);
        }
    }
}
