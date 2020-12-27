using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Networking;
using SFB;
using System;

public class Avatar : MonoBehaviour, IPointerDownHandler, IPointerClickHandler
{
    public Image avatar;
    // download avatar from browser environment before upload to bypass security limitation of webgl
    // https://forum.unity.com/threads/webgl-builds-and-streamingassetspath.366346/
    private byte[] localAvatarToUpload;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", ".png, .jpg", false);
    }
    public void OnPointerClick(PointerEventData eventData) {
        //not needed in webgl
    }
    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(GetLocalAvatar(url));
    }
#else
    public void OnPointerClick(PointerEventData eventData)
    {
        string[] files = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);

        string path = files[0];
        if (path.Length != 0)
        {
            StartCoroutine(GetLocalAvatar(path));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // only works in webgl.
    }
#endif

    private void downloadAvatar() {
        StartCoroutine(fetchAvatarFromServer());
    }

    IEnumerator GetLocalAvatar(string path)
    {
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            localAvatarToUpload = www.downloadHandler.data;
            Debug.Log("local avatar data length: " + localAvatarToUpload.Length);
            StartCoroutine(uploadAvatar(path, downloadAvatar));
        }
    }

    IEnumerator uploadAvatar(string path, Action onComplete)
    {

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormFileSection("upload", localAvatarToUpload, Path.GetFileName(path), null));
        string authorization = Networking.instance.GetAuthRequestString();
        UnityWebRequest www = UnityWebRequest.Post("http://" + Networking.url + ":3000/avatar", formData);
        //setting cookie is not needed in web gl because this part is handled by the browser
        //www.SetRequestHeader("Cookie", Networking.sessionCookie);
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
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("http://" + Networking.url + ":3000/avatar?username=" + Networking.username);
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
