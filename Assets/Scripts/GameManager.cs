using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    Socket socket;
    [SerializeField]
    GameObject dialogPrefab;
    [SerializeField]
    GameObject canvas;
    [SerializeField]
    Button quitGameButton;
    [SerializeField]
    Text hud;

    private bool isPlaying = false;
    private bool opponentQuit = false;
    private int remainingTime = 0;
    void Start()
    {
        socket = Networking.instance.socket;
        Action onServerReady = respondReadyState;
        socket.On("opponentQuit", () => {opponentQuit = true; });
        quitGameButton.onClick.AddListener(() =>
        {
            socket.Emit("quitGame", "");
            SceneManager.LoadScene("Lobby");
        });
        socket.On("yourTurn", () => { isPlaying = true; });
        socket.On("standby", () => { isPlaying = false; });
        socket.On("serverReady", onServerReady);
        socket.On("timer", (data) => { remainingTime = Int32.Parse(data.ToString()); });
        
    }

    void respondReadyState()
    {
        socket.Emit("playerReady", "");
    }

    void opponentQuitHandler()
    {
        GameObject dialog = Instantiate(dialogPrefab, canvas.transform);
        Text messsage = dialog.transform.Find("Message").gameObject.GetComponent<Text>();
        Button button1 = dialog.transform.Find("ButtonGroup").gameObject.transform.GetChild(0).gameObject.GetComponent<Button>();
        Button button2 = dialog.transform.Find("ButtonGroup").gameObject.transform.GetChild(1).gameObject.GetComponent<Button>();
        messsage.text = "對手已中離";
        button2.gameObject.SetActive(false);
        button1.GetComponentInChildren<Text>().text = "回大廳";
        button1.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Lobby");
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaying)
        {
            hud.text = "觀戰狀態";
        }
        else
        {
            hud.text = "輪到你了";
            if (remainingTime > 0)
            {
                hud.text += "(剩下" + remainingTime + "秒)";
            }
        }
       
        if (opponentQuit)
        {
            opponentQuit = false;
            opponentQuitHandler();
        }
    }
}
