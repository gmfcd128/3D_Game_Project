using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    private string playerListData = null;
    private string challengeMessage = null;
    private bool playerStatChanged = false;
    private bool newChallenge = false;
    private bool requestAccepted = false;
    Dictionary<string, Player> onlinePlayers;
    private Socket socket;
    private ManualResetEvent ManualResetEvent = null;
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
        Debug.Log("LobbyUI Started.");
        socket = Networking.instance.socket;
        socket.On("playerListUpdate", (data) =>
        {
            playerListData = data.ToString();
            playerStatChanged = true;
        }
       );
        socket.On("requestChallenge", (data) =>
        {
            challengeMessage = data.ToString();
            newChallenge = true;
        });
        socket.Emit("joinGame", Networking.username);
        AudioManager.instance.PlayDefaultMusic();
    }

    void updatePlayerList()
    {
        Debug.Log("Player List Update:");
        Debug.Log(@playerListData);
        onlinePlayers = JsonConvert.DeserializeObject<Dictionary<string, Player>>(@playerListData);
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

    private void challengeHandler()
    {
        Debug.Log(challengeMessage);
        createPopup(challengeMessage, onlinePlayers[challengeMessage].username, true);
    }

    public void createPopup(string socketID, string messageUsername, bool notifyChallenge = false)
    {
        GameObject dialog = Instantiate(dialogPrefab, canvas.transform);
        Text messsage = dialog.transform.Find("Message").gameObject.GetComponent<Text>();
        Button button1 = dialog.transform.Find("ButtonGroup").gameObject.transform.GetChild(0).gameObject.GetComponent<Button>();
        Button button2 = dialog.transform.Find("ButtonGroup").gameObject.transform.GetChild(1).gameObject.GetComponent<Button>();
        if (notifyChallenge)
        {
            Networking.opponentUsername = messageUsername;
            messsage.text = "" + messageUsername + "想要挑戰你";
            button1.GetComponentInChildren<Text>().text = "同意";
            button1.onClick.AddListener(() =>
            {
                socket.Emit("acceptChallenge", socketID);
                SceneManager.LoadScene("Game");
            });

            button2.GetComponentInChildren<Text>().text = "取消";
            button2.onClick.AddListener(() =>
            {
                socket.Emit("denyRequest", socketID);
                socket.Emit("joinGame", Networking.username);
                destroyGameObject(dialog);
            });
        }
        else
        {
            Networking.opponentUsername = messageUsername;
            button2.gameObject.SetActive(false);
            messsage.text = "正在等候" + messageUsername + "...";
            button1.onClick.AddListener(() =>
            {
                socket.Emit("joinGame", Networking.username);
                destroyGameObject(dialog);
            });
            button1.GetComponentInChildren<Text>().text = "取消";
            socket.Emit("requestChallenge", socketID);
            Action onRequestDenied = () => { messsage.text = "對方已拒絕對戰請求"; };
            socket.On("requestDenied", () =>
            {
                onRequestDenied();
            });
            socket.On("acceptChallenge", () =>
            {
                requestAccepted = true;
            });
        }
        socket.Emit("playerUnavailable", "");

    }

    void destroyGameObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }

    
    //close the connection when exit the lobby scene, but not when switch to game.
    private void OnDestroy()
    {
        if (!IsEditor)
            return;

        if (socket != null)
        {
            socket.Disconnect();
            socket = null;
        }
    }

    private bool IsEditor
    {
        get { return !Application.isPlaying; }

    }

    // Update is called once per frame
    void Update()
    {
        if (playerStatChanged)
        {
            playerStatChanged = false;
            updatePlayerList();
        }
        if (newChallenge)
        {
            newChallenge = false;
            challengeHandler();
        }
        if (requestAccepted)
        {
            requestAccepted = false;
            Debug.Log("Challenge accepted.");
            SceneManager.LoadScene("Game");
        }
    }
}
