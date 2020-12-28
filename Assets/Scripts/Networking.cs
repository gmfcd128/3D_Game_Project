using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Socket.Quobject.SocketIoClientDotNet.Client;
using System;
using System.IO;

public class Networking : MonoBehaviour
{
    [SerializeField]
    public static string url = "ma302.ddns.net";
    public static string username;
    public static string opponentUsername;
    public static string password;
    public static string sessionCookie { get; private set; }
    public QSocket socket { get; set; }

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
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (this != _instance)
        {
            Destroy(this.gameObject);
        }
        Debug.Log("Socket test.");
        socket = IO.Socket("http://" + url + ":3000");
        socket.On(QSocket.EVENT_CONNECT, () =>
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


    public string GetAuthRequestString()
    {
        string auth = username + ":" + password;
        auth = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(auth));
        auth = "Basic " + auth;
        return auth;
    }

    IEnumerator Login()
    {
        string authorization = GetAuthRequestString();
        UnityWebRequest request = new UnityWebRequest("http://" + url + ":3000/login", "POST");
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
            UIManager.instance.GoToLobby();


        }
    }



    IEnumerator SignUp()
    {
        string result;
        UnityWebRequest request = new UnityWebRequest("http://" + url + ":3000/signup", "POST");
        request.useHttpContinue = false;
        JObject credentials = new JObject();
        credentials["username"] = username;
        credentials["password"] = password;
        byte[] bodyRaw = Encoding.UTF8.GetBytes(credentials.ToString());
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            result = request.error;
        }
        else
        {
            result = request.downloadHandler.text;
        }
        Debug.Log(result);
        UIManager.instance.ShowMessage(result);

    }

    //close the connection when exit the lobby scene, but not when switch to game.
    private void OnDestroy()
    {
        if (!IsEditor)
            return;

        if (socket != null)
        {
            socket.Disconnect();
            socket = null;
        }
    }

    private bool IsEditor
    {
        get { return !Application.isPlaying; }

    }


}
