using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hint : MonoBehaviour
{
    public GameObject uiPanel;
    public GameObject scoreBarPanel;
    public GameObject Controller;
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(delegate
        {
            startGame();
        });
    }

    // Update is called once per frame
    void startGame()
    {
        WebGLPluginJS.PlayerReady();
        //WebGLPluginJS.SocketEmit("playerReady", "");
        Controller.SetActive(true);
        uiPanel.SetActive(true);
        scoreBarPanel.SetActive(true);
        GameManager.instance.playerReady = true;
        gameObject.SetActive(false);
        
    }
}
