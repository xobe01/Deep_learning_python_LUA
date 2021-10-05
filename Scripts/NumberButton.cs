using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberButton : ClickButton
{
    [SerializeField] string number;

    NumberButtonsController numberButtonsCont;
    Text text;
    GameObject cross;

    AudioClip fillSound;
    AudioClip removeSound;

    bool isCrossActive;

    protected override void Awake()
    {
        base.Awake();
        fillSound = Resources.Load("Click_fill") as AudioClip;
        removeSound = Resources.Load("Click_remove") as AudioClip;
        text = GetComponentInChildren<Text>();
        numberButtonsCont = FindObjectOfType<NumberButtonsController>();
        cross = transform.Find("Cross").gameObject;
        if (DataStorage.NightMode)
        {
            GetComponent<Image>().color = new Color(0, .4f, 1);
            text.color = Color.white;
        }
    }

    public void SetButton(bool enable, string number)
    {
        button.interactable = enable;
        text.enabled = number != this.number;
        isCrossActive = number == this.number;
        cross.SetActive(isCrossActive);
    }

    protected override void TaskOnClick()
    {
        clickSound = isCrossActive ? removeSound : fillSound;
        base.TaskOnClick();
        numberButtonsCont.SetNumber(number, false);
    }
}
