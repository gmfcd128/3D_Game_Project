using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    private string playerListData = null;
    private bool playerStatChanged = false;
    private Socket socket;
    private ManualResetEvent ManualResetEvent = null;
    private RoomScrollList scrollList;

    // Start is called before the first frame update
    void Start()
    {
        scrollList = GameObject.Find("Canvas").GetComponent<RoomScrollList>();
        Debug.Log("LobbyUI Started.");
        socket = Networking.instance.socket;
        var jobj = new JObject();
        jobj.Add("username", Networking.username);
        socket.On("newPlayerEntered", (data) =>
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
        Dictionary<string, Player[]> roomDict = JsonConvert.DeserializeObject<Dictionary<string, Player[]>>(@playerListData);
        foreach (var OneItem in roomDict)
        {
            Debug.Log("Key = " + OneItem.Key);
            foreach (var room in OneItem.Value)
            {
                if (room.userName != Networking.username)
                {
                    scrollList.addItemToList(room);
                }

            }
        }
        Debug.Log("addRoom function entered!");
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
