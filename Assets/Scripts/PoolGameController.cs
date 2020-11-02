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
    public Frontend.Player IdlePlayer;
    public Frontend.Player mySelf;
    public Frontend.Player opponent; 

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
        opponent = new Frontend.Player(Networking.opponentUsername);
        IdlePlayer = new Frontend.Player(Networking.opponentUsername);
        GameInstance = this;
        winnerMessage.GetComponent<Canvas>().enabled = false;
        StartCoroutine(AudioManager.instance.PlayGameMusic());

        currentState = new GameStates.WaitingForStrikeState(this);

        socket.On("standby", () =>
        {
            CurrentPlayer = opponent;
            IdlePlayer = mySelf;
            currentState = new GameStates.WatchingState(this);
        });

        socket.On("yourTurn", () =>
        {
            CurrentPlayer = mySelf;
            IdlePlayer = opponent;
            currentState = new GameStates.WaitingForStrikeState(this);
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
            Networking.instance.socket.Emit("continue", "");
            Debug.Log(CurrentPlayer.Name + " continues to play");
            return;
        }
        else
        {
            Debug.Log("CurrentPlayer: " + Networking.username);
            Debug.Log("Opponent: " + Networking.opponentUsername);
            if (CurrentPlayer.Equals(mySelf))
            {
                Networking.instance.socket.Emit("nextTurn", "");

            }
        }


    }

    public void EndMatch()
    {
        Frontend.Player winner = null;
        if (CurrentPlayer.Points > IdlePlayer.Points)
            winner = CurrentPlayer;
        else if (CurrentPlayer.Points < IdlePlayer.Points)
            winner = IdlePlayer;

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
