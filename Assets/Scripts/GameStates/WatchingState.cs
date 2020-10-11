using Quobject.SocketIoClientDotNet.Client;
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
        protected Socket socket;
        private bool cueBallHit;
        private bool cueStickEnable;
        private Vector3 cueBallForce;

        private SerializedTransform cueTransSerialized;
        private SerializedTransform cameraTransSerialized;
        private SerializableVector3 strikeDir;

        private PoolGameController gameController;
        public WatchingState(MonoBehaviour parent) : base(parent)
        {
            socket = Networking.instance.socket;
            Debug.Log("Watching state entered.");
            gameController = (PoolGameController)parent;
            cue = gameController.cue;
            cueBall = gameController.cueBall;
            mainCamera = gameController.mainCamera;

            socket.On("CuePositionChange", OnCuepositionChange);
            socket.On("CameraPositionChange", OnCameraPositionChange);
            socket.On("StrikeDirectionChange", OnStrikeDirectionChange);
            
            socket.On("CueBallStriked", CueBallHit);
            socket.On("continue", () => { cueStickEnable = true; });
            cueBallHit = false;
        }

        protected void OnCuepositionChange(object data)
        {
            cueTransSerialized = JsonConvert.DeserializeObject<SerializedTransform>(data.ToString()); 
        }

        void OnCameraPositionChange(object data)
        {
            cameraTransSerialized = JsonConvert.DeserializeObject<SerializedTransform>(data.ToString()); 
        }

        void OnStrikeDirectionChange(object data)
        { 
            strikeDir = JsonConvert.DeserializeObject<SerializableVector3>(data.ToString()); 
        }

        void CueBallHit(object data)
        {
            Debug.Log("The cue ball is hit.");
            cueBallForce = JsonConvert.DeserializeObject<SerializableVector3>(data.ToString());
            cueBallHit = true;
        }

        public override void Update()
        {
            if (cameraTransSerialized != null && cueTransSerialized != null)
            {
                DeserialTransform(mainCamera.transform, serializedTransform: cameraTransSerialized);
                DeserialTransform(cue.transform, cueTransSerialized);
            }

            if(strikeDir.x != 0 || strikeDir.y != 0 || strikeDir.z != 0)
            {
                gameController.strikeDirection = (Vector3)strikeDir;
                Debug.Log(strikeDir);
            }

            if (cueBallHit)
            {
                cue.GetComponent<Renderer>().enabled = false;
                cueBall.GetComponent<Rigidbody>().AddForce(cueBallForce);
                cueBallHit = false;
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
