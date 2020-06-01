using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
public class Networking : MonoBehaviour
{
    [SerializeField]
    private string url = "localhost";
    public static string username { get; private set; }
    public static string password { get; private set; }
    private string sessionCookie;
    public Socket socket { get; set; }

    private static Networking _instance;
    public static Networking instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Networking instance is null.");
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        Debug.Log("Socket test.");
        socket = IO.Socket("http://" + url + ":3000");
        socket.On(Socket.EVENT_CONNECT, () =>
        {
            Debug.Log("SocketIO Connected!!");

        });
    }

    public void Authenticate(string username, string password)
    {
        Networking.username = username;
        Networking.password = password;
        StartCoroutine(Login());
    }

    public void Register(string username, string password)
    {
        Networking.username = username;
        Networking.password = password;
        StartCoroutine(SignUp());
    }

    string GetAuthRequestString()
    {
        string auth = username + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
    }

    IEnumerator Login()
    {
        string authorization = GetAuthRequestString();
        UnityWebRequest request = new UnityWebRequest("http://" + url + ":3000/users/login", "POST");
        Debug.Log(url + ":3000/users/login");
        request.useHttpContinue = false;
        request.chunkedTransfer = false;
        request.SetRequestHeader("AUTHORIZATION", authorization);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.isNetworkError)

        {
            Debug.Log("http error:" + request.error);
        }
        else
        {
            string result = request.downloadHandler.text;
            if (request.GetResponseHeaders().ContainsKey("set-cookie"))
            {
                sessionCookie = request.GetResponseHeader("set-cookie");
                Debug.Log("Got cookies!!");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
                foreach (var OneItem in request.GetResponseHeaders())
                {
                    Debug.Log("Key = " + OneItem.Key + ", Value = " + OneItem.Value);
                }
            }
            //UIManager.instance.ShowMessage(result);
            UIManager.instance.GoToLobby();
            

        }
    }

    IEnumerator SignUp()
    {
        UnityWebRequest request = new UnityWebRequest(url + ":3000/users/signup", "POST");
        JObject credentials = new JObject();
        credentials["username"] = username;
        credentials["password"] = password;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(credentials.ToString());
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.isNetworkError)

        {
            Debug.Log("http error:" + request.error);
        }
        else
        {
            string result = request.downloadHandler.text;
            Debug.Log(result);
            UIManager.instance.ShowMessage(result);
        }

    }


}
