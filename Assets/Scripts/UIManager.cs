using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private InputField usernameInput;
    [SerializeField]
    private InputField passwordInput;
    [SerializeField]
    private Text message;

    private static UIManager _instance;
    public static UIManager instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("UIManager instance is null.");
            }
            return _instance;
        }
    }
  
    void Awake()
    {
        _instance = this;
        StartCoroutine(AudioManager.instance.PlayDefaultMusic());
    }


    public void OnLoginButtonClicked()
    {
        Networking.instance.Authenticate(usernameInput.text, passwordInput.text);
    }

    public void OnSignUpButtonClicked()
    {
        Networking.instance.Register(usernameInput.text, passwordInput.text);
    }

    public void ShowMessage(string msg) 
    {
        message.text = msg;
    }

    public void GoToLobby() 
    {
        SceneManager.LoadScene("Lobby");
    }
}

