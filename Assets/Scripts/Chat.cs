using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    [SerializeField]
    private InputField messageInput;
    [SerializeField]
    private GameObject chatDisplay;
    [SerializeField]
    private GameObject messagePrefab;
    bool allowEnter;

    private bool hasNewMessage;
    private string msg;
    private Socket socket;
    // Start is called before the first frame update
    void Start()
    {
        hasNewMessage = false;
        socket = Networking.instance.socket;
        socket.On("newMessage", (data) =>
        {
            hasNewMessage = true;
            msg = data.ToString();
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (allowEnter && (messageInput.text.Length > 0) && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
        {
            socket.Emit("newMessage", messageInput.text);
            allowEnter = false;
            messageInput.text = "";
        }
        else
        {
            allowEnter = messageInput.isFocused || messageInput.isFocused;
        }

        if (hasNewMessage)
        {
            hasNewMessage = false;
            updateChat(msg);
        }
    }

    void updateChat(string jsonString)
    {
        Debug.Log("You've got message!!");
        Dictionary<string, string> message = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
        GameObject messageText = Instantiate(messagePrefab, chatDisplay.transform);
        messageText.GetComponent<Text>().text = message["speaker"] + ":" + message["message"];
        if (message["speaker"] != Networking.username)
        {
            messageText.GetComponent<Text>().color = new Color32(0, 64, 0, 255);
        }
        else
        {
            messageText.GetComponent<Text>().color = new Color32(64, 0, 0, 255);
        }
        messageText.transform.SetParent(chatDisplay.transform);
    }
}
