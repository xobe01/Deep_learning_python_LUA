using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] bool containsImage;

    protected AudioController audioCont;
    protected AudioClip clickSound;
    protected Button button;
    Color disabledColor;
    Color pressedColor;
    Color releasedColor;

    protected virtual void Awake()
    {
        audioCont = FindObjectOfType<AudioController>();
        clickSound = Resources.Load("Click_UI") as AudioClip;
        button = GetComponent<Button>();
        button.onClick.AddListener(TaskOnClick);
        var colors = button.colors;
        disabledColor = colors.disabledColor;
        pressedColor = new Color(.7f, .7f, .7f);
        releasedColor = colors.normalColor;
    }

    protected void SetInteractable(bool value)
    {
        button.interactable = value;
        if (containsImage)
        {
            foreach (Image i in GetComponentsInChildren<Image>()) i.color = disabledColor;
            foreach (Text t in GetComponentsInChildren<Text>()) t.color = disabledColor;
        }
    }

    virtual protected void TaskOnClick()
    {
        audioCont.PlaySound(clickSound);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (containsImage && button.interactable)
        {
            foreach (Image i in GetComponentsInChildren<Image>()) StartCoroutine(FadeImage(true, i, null));
            foreach (Text t in GetComponentsInChildren<Text>()) StartCoroutine(FadeImage(true, null, t));
        }
    }

    IEnumerator FadeImage(bool fade, Image image, Text text)
    {
        Color startColor = fade ? releasedColor : pressedColor;
        Color goalColor = fade ? pressedColor : releasedColor;
        float time = 0.05f;
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            if (text != null) text.color = startColor - (startColor - goalColor) * i / time;
            if (image != null) image.color = startColor - (startColor - goalColor) * i / time;
            yield return null;
        }
        if (text != null) text.color = goalColor;
        if (image != null) image.color = goalColor;
        yield return null;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (containsImage && button.interactable)
        {
            foreach (Image i in GetComponentsInChildren<Image>()) StartCoroutine(FadeImage(false, i, null));
            foreach (Text t in GetComponentsInChildren<Text>()) StartCoroutine(FadeImage(false, null, t));
        }
    }
}
