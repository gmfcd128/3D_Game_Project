using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAudio : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public void OnPointerEnter(PointerEventData ped)
    {
        AudioManager.instance.PlayMouseOverAudio();
    }

    public void OnPointerDown(PointerEventData ped)
    {
        AudioManager.instance.PlayMouseClickAudio();
    }
}
