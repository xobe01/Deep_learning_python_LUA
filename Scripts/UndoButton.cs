using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoButton : ClickButton
{
    MapController mapCont;

    protected override void Awake()
    {
        base.Awake();
        mapCont = FindObjectOfType<MapController>();
    }

    protected override void TaskOnClick()
    {
        base.TaskOnClick();
        mapCont.Undo();
    }

    public void SetEnabled(bool value)
    {
        button.interactable = value;
    }
}
