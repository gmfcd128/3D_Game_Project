using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    private string playerListData = null;
    private bool playerStatChanged = false;
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
        var jobj = new JObject();
        jobj.Add("username", Networking.username);
        socket.On("playerListUpdate", (data) =>
        {
            Debug.Log("QQ");
            playerListData = data.ToString();
            playerStatChanged = true;
        }
       );
        socket.Emit("joinGame", jobj);
    }

    void updatePlayerList()
    {
        Debug.Log(@playerListData);
        Dictionary<string, string> onlinePlayers = JsonConvert.DeserializeObject<Dictionary<string, string>>(@playerListData);
        scrollList.rooms.Clear();
        scrollList.refreshDisplay();
        foreach (var player in onlinePlayers)
        {
            Debug.Log("Key = " + player.Key);
            if (player.Value != Networking.username)
            {
                Player onlinePlayer = new Player();
                onlinePlayer.socketID = player.Key;
                onlinePlayer.userName = player.Value;
                scrollList.addItemToList(onlinePlayer);
            }
        }
        Debug.Log("addRoom function entered!");
    }

    public void createPopup(Player player)
    {
        GameObject dialog = Instantiate(dialogPrefab, canvas.transform);
        Text messsage = dialog.transform.Find("Message").gameObject.GetComponent<Text>();
        messsage.text = "正在等候" + player.userName + "...";
        Button button = dialog.transform.Find("Button").gameObject.GetComponent<Button>();
        button.onClick.AddListener(() =>destroyGameObject(dialog));
        button.GetComponentInChildren<Text>().text = "取消"
        socket.Emit("requestChallenge", player.socketID);
    }

    void destroyGameObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (socket != null)
        {
            socket.Disconnect();
            socket = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerStatChanged)
        {
            playerStatChanged = false;
            updatePlayerList();
        }
    }
}
