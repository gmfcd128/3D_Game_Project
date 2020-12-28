using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject dialogPrefab;
    [SerializeField]
    GameObject canvas;
    [SerializeField]
    Button quitGameButton;
    [SerializeField]
    Text hud;
    [SerializeField]
    Image myAvatar;
    [SerializeField]
    Image opponentAvatar;

    public bool serverReady = false;
    public bool playerReady = false;
    private int remainingTime = 0;
    public static GameManager instance { get; private set; }
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        StartCoroutine(downloadAvatar(Networking.username, myAvatar));
        StartCoroutine(downloadAvatar(Networking.opponentUsername, opponentAvatar));
        quitGameButton.onClick.AddListener(() =>
        {
            WebGLPluginJS.SocketEmit("quitGame", "");
            SceneManager.LoadScene("Lobby");
        });

    }

    public void updateTimer(int time)
    {
        remainingTime = time;
    }

    public void OnServerReady()
    {
        serverReady = true;
    }

    public void UpdateHUD(int isPlaying)
    {
        if (isPlaying == 1)
        {
            //玩家正在玩
            hud.text = "輪到你了";
            myAvatar.gameObject.GetComponent<Outline>().enabled = true;
            opponentAvatar.gameObject.GetComponent<Outline>().enabled = false;
            if (remainingTime > 0)
            {
                hud.text += "(剩下" + remainingTime + "秒)";
            }
        }
        else if (isPlaying == 0)
        {
            hud.text = "觀戰狀態";
            myAvatar.gameObject.GetComponent<Outline>().enabled = false;
            opponentAvatar.gameObject.GetComponent<Outline>().enabled = true;
        }
    }

    public void OnOpponentQuit()
    {
        displayMatchResult("對手已中離");
    }


    IEnumerator downloadAvatar(string username, Image target)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("http://" + Networking.url + ":3000/avatar?username=" + username);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D avatarImage = DownloadHandlerTexture.GetContent(www); ;
            target.sprite = Sprite.Create(avatarImage, new Rect(0f, 0f, avatarImage.width, avatarImage.height), new Vector2(0.5f, 0.5f));
            Debug.Log("Avatar download completed.");
        }
    }

    public void displayMatchResult(string msg)
    {
        GameObject dialog = Instantiate(dialogPrefab, canvas.transform);
        Text messsage = dialog.transform.Find("Message").gameObject.GetComponent<Text>();
        Button button1 = dialog.transform.Find("ButtonGroup").gameObject.transform.GetChild(0).gameObject.GetComponent<Button>();
        Button button2 = dialog.transform.Find("ButtonGroup").gameObject.transform.GetChild(1).gameObject.GetComponent<Button>();
        messsage.text = msg;
        button2.gameObject.SetActive(false);
        button1.GetComponentInChildren<Text>().text = "回大廳";
        button1.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Lobby");
        });
    }
}
