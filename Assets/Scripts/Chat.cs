using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
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

    public static Chat instance { get; private set; }

    private void Start()
    {
        if (instance == null) {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (allowEnter && (messageInput.text.Length > 0) && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
        {
            WebGLPluginJS.SocketEmit("newMessage", messageInput.text);
            allowEnter = false;
            messageInput.text = "";
        }
        else
        {
            allowEnter = messageInput.isFocused || messageInput.isFocused;
        }
    }

    public void UpdateChat(string jsonString)
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

    public bool InputFocused()
    {
        return messageInput.isFocused;
    }
}
