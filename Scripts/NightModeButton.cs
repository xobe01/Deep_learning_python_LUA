using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightModeButton : ClickButton
{
    SceneBaseFunctions sceneBase;

    protected override void Awake()
    {
        base.Awake();
        sceneBase = FindObjectOfType<SceneBaseFunctions>();
    }

    protected override void TaskOnClick()
    {
        base.TaskOnClick();
        sceneBase.ChangeMode();
    }
}
