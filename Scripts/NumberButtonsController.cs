using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberButtonsController : MonoBehaviour
{
    [SerializeField] NumberButton[] numberButtons;
    MapController mapCont;

    string currentNumber;

    private void Start()
    {
        mapCont = FindObjectOfType<MapController>();
    }

    public void EnableButtons(bool enable, string number)
    {
        currentNumber = number;
        foreach (NumberButton n in numberButtons) n.SetButton(enable, number);
    }

    public void SetNumber(string number, bool isUndo)
    {
        foreach (NumberButton n in numberButtons) n.SetButton(true, number == currentNumber ? "" : number);
        mapCont.SetActiveFieldNumber(number, isUndo);
        currentNumber = currentNumber == number ? "0" : number;
    }
}
