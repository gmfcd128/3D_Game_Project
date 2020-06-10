using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player {
    public string username;
    public bool isAvailable;
}

public class RoomScrollList : MonoBehaviour
{
    public Dictionary<string, Player> players;
    public Transform contentPanel;
    public SimpleObjectPool buttonObjectPool;
    // Start is called before the first frame update
    void Start()
    {
        players = new Dictionary<string, Player>();
        Debug.Log("ready to refresh display.");
        refreshDisplay();
    }

    public void refreshDisplay()
    {
        RemoveButtons();
        addButtons();
    }
    private void RemoveButtons()
    {
        while (contentPanel.childCount > 0)
        {
            GameObject toRemove = contentPanel.GetChild(0).gameObject;
            buttonObjectPool.ReturnObject(toRemove);
        }
    }

    public void addItemToList(string socketID, Player player) 
    {
        players[socketID] = player;
        refreshDisplay();
    } 

    private void addButtons() 
    {
        foreach (var player in players)
        {
            GameObject newButton = buttonObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel);
            RoomListItem roomListItem = newButton.GetComponent<RoomListItem>();
            roomListItem.Setup(player.Key, player.Value.username, this);
        }
    }
}
