using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using SFB;
using System;

public class Avatar : MonoBehaviour, IPointerClickHandler
{
    public Image avatar;
    public void OnPointerClick(PointerEventData eventData)
    {
        string[] files = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);

        string path = files[0];
        if (path.Length != 0)
        {
            StartCoroutine(uploadAvatar(path, downloadAvatar));
        }
    }

    private void downloadAvatar() {
        StartCoroutine(fetchAvatarFromServer());
    }

    IEnumerator uploadAvatar(string path, Action onComplete)
    {

        byte[] photoByte = File.ReadAllBytes(path);
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("upload", photoByte, path, null));
        string authorization = Networking.instance.GetAuthRequestString();
        UnityWebRequest www = UnityWebRequest.Post("http://" + Networking.url + ":3000/avatar", formData);
        www.SetRequestHeader("Cookie", Networking.sessionCookie);
        www.useHttpContinue = false;
        www.chunkedTransfer = false;

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
            onComplete();
        }
    }

    IEnumerator fetchAvatarFromServer()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(Networking.url + ":3000/avatar?username=" + Networking.username);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D avatarImage = DownloadHandlerTexture.GetContent(www); ;
            avatar.sprite = Sprite.Create(avatarImage, new Rect(0f, 0f, avatarImage.width, avatarImage.height), new Vector2(0.5f, 0.5f));
            Debug.Log("Avatar download completed.");
        }
    }

    public void Start()
    {
        avatar = gameObject.GetComponent<Image>();
        StartCoroutine(fetchAvatarFromServer());
    }



}
