using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    public Button button;
    public Text roomNameLabel;
    public Text roomHostLabel;
    private Player item;
    private RoomScrollList scrollList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Setup(Player currentItem, RoomScrollList currentScrollList) 
    {
        item = currentItem;
        roomNameLabel.text = item.userName;
        scrollList = currentScrollList;
    }

}
