using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipButton : ClickButton
{
    [SerializeField] bool openTipFilter;
    GameController gameCont;

    protected override void Awake()
    {
        base.Awake();
        gameCont = FindObjectOfType<GameController>();
    }

    public void SetEnabled(bool value)
    {
        button.interactable = value;
    }

    protected override void TaskOnClick()
    {
        base.TaskOnClick();
        gameCont.TipButtonPressed(openTipFilter);
    }
}
