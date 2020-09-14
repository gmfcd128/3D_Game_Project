using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quobject.SocketIoClientDotNet.Client;

namespace GameStates
{
    public class Test :AbstractGameObjectState
    {
        private GameObject cue;
        private GameObject cueBall;
        private GameObject mainCamera;
        SerializedTransform cameraTrans;
        SerializedTransform cueTrans;
        private PoolGameController gameController;
        Socket socket;
        public Test(MonoBehaviour parent) : base(parent)
        {
            gameController = (PoolGameController)parent;
            cue = gameController.cue;
            cueBall = gameController.cueBall;
            mainCamera = gameController.mainCamera;
            socket = Networking.instance.socket;
            socket.On("test", () => { Debug.Log("FUCK"); });
        }
    }

}

