using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Networking;
using SFB;

public class Avatar : MonoBehaviour, IPointerClickHandler
{
    public Image avatar;
    public void OnPointerClick(PointerEventData eventData)
    {
        string[] files = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);

        string path = files[0];
        if (path.Length != 0)
        {
            Networking.instance.UpdateAvatar(path, downloadAvatar);
        }
    }

    private void downloadAvatar() {
        StartCoroutine(fetchAvatarFromServer());
    }

    IEnumerator fetchAvatarFromServer()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("http://" + Networking.instance.url + "/avatar");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myAvatar = ((DownloadHandlerTexture)www.downloadHandler).texture;
            gameObject.GetComponent<Image>().image = myAvatar;
        }
    }

    public void Start()
    {
        avatar = gameObject.GetComponent<Image>();
    }

}
