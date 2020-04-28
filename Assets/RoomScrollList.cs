using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item
{
    public string roomName;
    public string hostName;
}
public class RoomScrollList : MonoBehaviour
{
    public Transform contentPanel;
    public SimpleObjectPool roomListObjectPool;
    // Start is called before the first frame update
    void Start()
    {
        refreshDisplay();
    }

    public void refreshDisplay()
    {
        addButtons();
    }

    private void addButtons() 
    {
    
    }
}
