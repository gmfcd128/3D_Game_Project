using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player
{
    public string socketID;
    public string userName;
}
public class RoomScrollList : MonoBehaviour
{
    public List<Player> rooms;
    public Transform contentPanel;
    public SimpleObjectPool buttonObjectPool;
    // Start is called before the first frame update
    void Start()
    {
        rooms = new List<Player>();
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

    public void addItemToList(Player item) 
    {
        rooms.Add(item);
        refreshDisplay();
    } 

    private void addButtons() 
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            Player item = rooms[i];
            GameObject newButton = buttonObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel);
            RoomListItem roomListItem = newButton.GetComponent<RoomListItem>();
            roomListItem.Setup(item, this);
        }
    }
}
