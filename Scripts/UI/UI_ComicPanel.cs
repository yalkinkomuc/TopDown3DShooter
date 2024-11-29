using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ComicPanel : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] private Image[] comicPanels;
    private Image myImage;
     private int imageIndex;
    [SerializeField] private GameObject buttonToEnable;
     private bool comicShowOver;
    

    private void Start()
    {
        myImage = GetComponent<Image>();
        ShowNextImage();
    }
    private void ShowNextImage()
    {
        if(comicShowOver)
            return;

        StartCoroutine(ChangeImageAlpha(1,1.5f,ShowNextImage));
    }

    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, System.Action onComplete)
    {
        float time = 0;
        Color currentColor = comicPanels[imageIndex].color;
        float startAlpha = currentColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            comicPanels[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        comicPanels[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        imageIndex++;

        if(imageIndex >= comicPanels.Length)
        {
            FinishComicShow();
        }

        onComplete?.Invoke();
    }

    private void FinishComicShow()
    {
        StopAllCoroutines();
        comicShowOver = true;
        buttonToEnable.SetActive(true); 
        myImage.raycastTarget = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        ShowNextImageOnClick();
    }

    private void ShowNextImageOnClick()
    {
        comicPanels[imageIndex].color = Color.white;
        imageIndex++;

        if (imageIndex >= comicPanels.Length)
            FinishComicShow();

        if(comicShowOver)
            return;

        ShowNextImage();
    }
}
