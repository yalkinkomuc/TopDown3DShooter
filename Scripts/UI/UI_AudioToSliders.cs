using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_AudioToSliders : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    [Header("Audio")]
    [SerializeField] private AudioSource buttonHoverSFX;
    [SerializeField] private AudioSource buttonPressedSFX;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(buttonPressedSFX != null) 
            buttonPressedSFX.Play();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonHoverSFX != null)
            buttonHoverSFX.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}
