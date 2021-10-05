using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreePlayButton : SceneChangerButton
{
    private void Start()
    {
        if (DataStorage.FreePlayLevel.Count > 0) ChangeToGame();
    }

    public void ChangeToGame()
    {
        SetGoalScene("04_Game");
    }

    protected override void TaskOnClick()
    {
        PlayerPrefs.SetInt("isFreePlay", 2);
        base.TaskOnClick();
    }
}
