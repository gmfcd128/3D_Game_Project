using UnityEngine;
public class PoolGameController : MonoBehaviour
{
    public GameObject cue;
    public GameObject cueBall;
    public GameObject redBalls;
    public GameObject mainCamera;
    public GameObject scoreBar;

    public float maxForce;
    public float minForce;
    public Vector3 strikeDirection;

    public const float MIN_DISTANCE = 27.5f;
    public const float MAX_DISTANCE = 32f;

    public IGameObjectState currentState;

    public Frontend.Player CurrentPlayer;
    public Frontend.Player IdlePlayer;
    public Frontend.Player mySelf;
    public Frontend.Player opponent;

    private bool currentPlayerContinuesToPlay = false;
    private bool firstEntry = true;
    private bool gameFinished = false;


    // This is kinda hacky but works
    static public PoolGameController GameInstance
    {
        get;
        private set;
    }

    void Start()
    {
        strikeDirection = Vector3.forward;
        mySelf = new Frontend.Player(Networking.username);
        opponent = new Frontend.Player(Networking.opponentUsername);
        IdlePlayer = new Frontend.Player(Networking.opponentUsername);
        GameInstance = this;
        StartCoroutine(AudioManager.instance.PlayGameMusic());

        currentState = new GameStates.WaitingForStrikeState(this);
    }

    public void standby()
    {
        CurrentPlayer = opponent;
        IdlePlayer = mySelf;
        if (firstEntry)
        {
            firstEntry = false;
        }
        currentState = new GameStates.WatchingState(this);
    }

    public void myTurn()
    {
        CurrentPlayer = mySelf;
        IdlePlayer = opponent;
        Debug.Log(currentState.GetType());
        currentState = new GameStates.WaitingForStrikeState(this);
        if (firstEntry)
        {
            firstEntry = false;
        }
        else
        {
            InvertCameraPosition();
        }
    }

    void Update()
    {
        currentState.Update();
        if (gameFinished) {
            EndMatch();
            gameFinished = false;
        }
    }

    void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    void LateUpdate()
    {
        currentState.LateUpdate();
    }

    public void OnCuePositionChange(string data)
    {
        currentState.OnSocketEvent("CuePositionChange", @data);
    }

    public void OnCameraPositionChange(string data)
    {
        currentState.OnSocketEvent("CameraPositionChange", @data);
    }

    public void OnStrikeDirectionChange(string data)
    {
        currentState.OnSocketEvent("StrikeDirectionChange", @data);
    }

    public void OnCueBallStriked(string data)
    {
        currentState.OnSocketEvent("CueBallStriked", @data);
    }

    public void OnContinue()
    {
        currentState.OnSocketEvent("continue", "");
    }


    public void BallPocketed(int ballNumber)
    {
        currentPlayerContinuesToPlay = true;
        CurrentPlayer.Collect(ballNumber);
    }

    public void NextPlayer()
    {
        if (currentPlayerContinuesToPlay)
        {
            currentPlayerContinuesToPlay = false;
            WebGLPluginJS.SocketEmit("continue", "");
            Debug.Log(CurrentPlayer.Name + " continues to play");
        }
        else
        {
            Debug.Log("CurrentPlayer: " + Networking.username);
            Debug.Log("Opponent: " + Networking.opponentUsername);
            if (CurrentPlayer.Equals(mySelf))
            {
                WebGLPluginJS.SocketEmit("nextTurn", "");
            }
        }


    }

    public void InvertCameraPosition()
    {
        Debug.Log("InvertCameraPosition");
        mainCamera.transform.position = new Vector3(cueBall.transform.position.x + (cueBall.transform.position.x - mainCamera.transform.position.x),
                                                    mainCamera.transform.position.y,
                                                    cueBall.transform.position.z + (cueBall.transform.position.z - mainCamera.transform.position.z));
        Vector3 cameraRot = mainCamera.transform.rotation.eulerAngles;
        cameraRot = new Vector3(cameraRot.x, cameraRot.y + 180, cameraRot.z);
        mainCamera.transform.rotation = Quaternion.Euler(cameraRot);
    }

    public void InvertCameraPosition(Transform currentPlayerCamera)
    {
        Debug.Log("InvertCameraPosition");
        mainCamera.transform.position = new Vector3(cueBall.transform.position.x + (cueBall.transform.position.x - currentPlayerCamera.position.x),
                                                    currentPlayerCamera.transform.position.y,
                                                    cueBall.transform.position.z + (cueBall.transform.position.z - currentPlayerCamera.position.z));
        Vector3 cameraRot = currentPlayerCamera.transform.rotation.eulerAngles;
        cameraRot = new Vector3(cameraRot.x, cameraRot.y + 180, cameraRot.z);
        mainCamera.transform.rotation = Quaternion.Euler(cameraRot);
    }

    public void EndMatch()
    {
        Frontend.Player winner = null;
        if (CurrentPlayer.Points > IdlePlayer.Points)
            winner = CurrentPlayer;
        else if (CurrentPlayer.Points < IdlePlayer.Points)
            winner = IdlePlayer;

        var msg = "遊戲結束\n";

        if (winner != null)
            msg += string.Format("'{0}' 贏了", winner.Name);
        else
            msg += "平手.";

        WebGLPluginJS.SocketEmit("endMatch", "");


        GameManager.instance.displayMatchResult(msg);
    }


}
