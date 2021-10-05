using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : ClickButton
{
    MapController mapCont;

    private void Start()
    {
        mapCont = FindObjectOfType<MapController>();
    }

    protected override void TaskOnClick()
    {
        base.TaskOnClick();
        mapCont.Restart();
    }
}
