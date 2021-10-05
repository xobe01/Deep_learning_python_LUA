using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipAdButton : ClickButton
{
    GameController gameCont;

    protected override void Awake()
    {
        base.Awake();
        gameCont = FindObjectOfType<GameController>();
    }

    protected override void TaskOnClick()
    {
        base.TaskOnClick();
        gameCont.ShowRewardedAd();
    }
}
