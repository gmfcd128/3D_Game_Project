using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    public Button button;
    public Text roomNameLabel;
    public Text roomHostLabel;
    private string socketID;
    private string username;
    private RoomScrollList scrollList;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(HandleClick);
    }

    public void Setup(string socketID, string username, RoomScrollList currentScrollList) 
    {
        this.socketID = socketID;
        this.username = username;
        roomNameLabel.text = this.username;
        scrollList = currentScrollList;
    }

    public void HandleClick()
    {
        LobbyUIManager.instance.requestChallenge(socketID, username);
    }

}
