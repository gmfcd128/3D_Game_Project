using Quobject.SocketIoClientDotNet.Client;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    Socket socket;
    [SerializeField]
    GameObject dialogPrefab;
    [SerializeField]
    GameObject canvas;
    [SerializeField]
    Button quitGameButton;
    void Start()
    {
        socket = Networking.instance.socket;
        Action onOpponentQuit = opponentQuitHandler;
        socket.On("opponentQuit", onOpponentQuit);
        quitGameButton.onClick.AddListener(() =>
        {
            socket.Emit("quitGame", "");
            SceneManager.LoadScene("Lobby");
        });
    }

    void opponentQuitHandler()
    {
        GameObject dialog = Instantiate(dialogPrefab, canvas.transform);
        Text messsage = dialog.transform.Find("Message").gameObject.GetComponent<Text>();
        Button button1 = dialog.transform.Find("ButtonGroup").gameObject.transform.GetChild(0).gameObject.GetComponent<Button>();
        Button button2 = dialog.transform.Find("ButtonGroup").gameObject.transform.GetChild(1).gameObject.GetComponent<Button>();
        messsage.text = "對手已中離";
        button2.gameObject.SetActive(false);
        button1.GetComponentInChildren<Text>().text = "回大廳";
        button1.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Lobby");
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
