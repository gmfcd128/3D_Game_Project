using UnityEngine;
using System.Collections;
using Frontend;
using Quobject.SocketIoClientDotNet.Client;
using GameStates;
using System;

public class PoolGameController : MonoBehaviour
{
    public GameObject cue;
    public GameObject cueBall;
    public GameObject redBalls;
    public GameObject mainCamera;
    public GameObject scoreBar;
    public GameObject winnerMessage;

    public float maxForce;
    public float minForce;
    public Vector3 strikeDirection;

    public const float MIN_DISTANCE = 27.5f;
    public const float MAX_DISTANCE = 32f;

    public IGameObjectState currentState;

    public Frontend.Player CurrentPlayer;
    public Frontend.Player OtherPlayer;
    public Frontend.Player mySelf;

    private bool currentPlayerContinuesToPlay = false;

    protected Socket socket;

    // This is kinda hacky but works
    static public PoolGameController GameInstance
    {
        get;
        private set;
    }

    void Start()
    {
        socket = Networking.instance.socket;
        strikeDirection = Vector3.forward;
        mySelf = new Frontend.Player(Networking.username);
        OtherPlayer = new Frontend.Player(Networking.opponentUsername);
        GameInstance = this;
        winnerMessage.GetComponent<Canvas>().enabled = false;

        currentState = new GameStates.WaitingForStrikeState(this);

        socket.On("standby", () =>
        {
            CurrentPlayer = OtherPlayer;
            currentState = new GameStates.WatchingState(this);
        });

        socket.On("yourTurn", () =>
        {
            CurrentPlayer = mySelf;
            currentState = new GameStates.WaitingForStrikeState(this);
            Debug.Log("Test2");
      
        });

    }

    void Update()
    {
        currentState.Update();
    }

    void FixedUpdate()
    {
        currentState.FixedUpdate();
    }

    void LateUpdate()
    {
        currentState.LateUpdate();
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
            Debug.Log(CurrentPlayer.Name + " continues to play");
            return;
        }
        if (CurrentPlayer == mySelf)
        {
            Networking.instance.socket.Emit("nextTurn", "");
            CurrentPlayer = OtherPlayer;
            OtherPlayer = mySelf;
        }
        else
        {
            OtherPlayer = CurrentPlayer;
            CurrentPlayer = mySelf;
        }
        
        
    }

    public void EndMatch()
    {
        Frontend.Player winner = null;
        if (CurrentPlayer.Points > OtherPlayer.Points)
            winner = CurrentPlayer;
        else if (CurrentPlayer.Points < OtherPlayer.Points)
            winner = OtherPlayer;

        var msg = "Game Over\n";

        if (winner != null)
            msg += string.Format("The winner is '{0}'", winner.Name);
        else
            msg += "It was a draw!";

        var text = winnerMessage.GetComponentInChildren<UnityEngine.UI.Text>();
        text.text = msg;
        winnerMessage.GetComponent<Canvas>().enabled = true;
    }
}
