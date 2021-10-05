using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationButton : ClickButton
{
    TutorialObjectController tutorialObject;
    GameController gameCont;
    int difficulty;

    protected override void Awake()
    {
        base.Awake();
        gameCont = FindObjectOfType<GameController>();
        tutorialObject = FindObjectOfType<TutorialObjectController>();
    }

    protected override void TaskOnClick()
    {
        base.TaskOnClick();
        tutorialObject.StartTutorial(difficulty);
        gameCont.StopStartTime(true);
    }

    public void SetDifficulty(int difficulty)
    {
        this.difficulty = difficulty;
    }
}
