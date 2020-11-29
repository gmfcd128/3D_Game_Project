using Newtonsoft.Json;
using Socket.Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    private Text message = null;
    private bool requestAccepted = false;
    Dictionary<string, Player> onlinePlayers;
    private QSocket socket;
    private GameObject canvas;
    private RoomScrollList scrollList;
    public GameObject dialogPrefab;

    private static LobbyUIManager _instance;
    public static LobbyUIManager instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("LobbyUIManager instance is null.");
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
        canvas = GameObject.Find("Canvas");
        scrollList = canvas.GetComponent<RoomScrollList>();
        WebGLPluginJS.Init(Networking.username);
        Debug.Log("LobbyUI Started.");
        /*socket.On("requestChallenge", (data) =>
        {
            challengeMessage = data.ToString();
            newChallenge = true;
        });*/
        AudioManager.instance.PlayDefaultMusic();
    }

    public void updatePlayerList(string data)
    {
        Debug.Log("Player List Update:");
        Debug.Log(@data);
        onlinePlayers = JsonConvert.DeserializeObject<Dictionary<string, Player>>(@data);
        Debug.Log("scroll list refresh");
        scrollList.players.Clear();
        scrollList.refreshDisplay();
        foreach (var player in onlinePlayers)
        {
            Debug.Log("Key = " + player.Key);
            if ((player.Value.username != Networking.username) && player.Value.isAvailable)
            {
                scrollList.addItemToList(player.Key, player.Value);
            }
        }
        Debug.Log("addRoom function entered!");
    }

   

    public void createPopup(string socketID, string messageUsername, bool notifyChallenge = false)
    {
        GameObject dialog = Instantiate(dialogPrefab, canvas.transform);
        message = dialog.transform.Find("Message").gameObject.GetComponent<Text>();
        Button button1 = dialog.transform.Find("ButtonGroup").gameObject.transform.GetChild(0).gameObject.GetComponent<Button>();
        Button button2 = dialog.transform.Find("ButtonGroup").gameObject.transform.GetChild(1).gameObject.GetComponent<Button>();
        if (notifyChallenge)
        {
            Networking.opponentUsername = messageUsername;
            message.text = "" + messageUsername + "想要挑戰你";
            button1.GetComponentInChildren<Text>().text = "同意";
            button1.onClick.AddListener(() =>
            {
                socket.Emit("acceptChallenge", socketID);
                SceneManager.LoadScene("Game");
            });

            button2.GetComponentInChildren<Text>().text = "取消";
            button2.onClick.AddListener(() =>
            {
                WebGLPluginJS.DenyRequest(socketID);
                destroyGameObject(dialog);
            });
        }
        else
        {
            Networking.opponentUsername = messageUsername;
            button2.gameObject.SetActive(false);
            message.text = "正在等候" + messageUsername + "...";
            WebGLPluginJS.RequestChallenge(socketID);
            button1.onClick.AddListener(() =>
            {
                socket.Emit("joinGame", Networking.username);
                destroyGameObject(dialog);
            });
            button1.GetComponentInChildren<Text>().text = "取消";
            Action onRequestDenied = () => { message.text = "對方已拒絕對戰請求"; };
            socket.On("requestDenied", () =>
            {
                onRequestDenied();
            });
            socket.On("acceptChallenge", () =>
            {
                requestAccepted = true;
            });
        }

    }

    void destroyGameObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }

    

    // Update is called once per frame
    void Update()
    {
        if (requestAccepted)
        {
            requestAccepted = false;
            Debug.Log("Challenge accepted.");
            SceneManager.LoadScene("Game");
        }
    }
}
