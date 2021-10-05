using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : ClickButton
{
    [SerializeField] bool continueGame;

    GameController gameCont;

    protected override void Awake()
    {
        gameCont = FindObjectOfType<GameController>();    
        base.Awake();
    }

    protected override void TaskOnClick()
    {
        base.TaskOnClick();
        gameCont.CloseContinueTextBox(continueGame);
    }
}
