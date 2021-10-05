using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] CenterController[] centerControllers;
    [SerializeField] FieldController[] highlighted1Fields;
    [SerializeField] FieldController[] highlighted2Fields;
    [SerializeField] int difficulty;

    FieldController[] fieldControllers;
    List<FieldController> fillableFields;
    FieldController activeField;
    NumberButtonsController numberButtonsCont;
    GameController gameCont;
    UndoButton undoButton;
    PinchableScrollRect scrollRect;
    GameObject filter;

    List<Step> steps;
    List<string> filledFields;

    public int Difficulty { get { return difficulty; } }

    class Step
    {
        int iD;
        string previousValue;

        public int ID { get { return iD; } }
        public string PreviousValue { get { return previousValue; } }

        public Step(int iD, string previousValue)
        {
            this.iD = iD;
            this.previousValue = previousValue;
        }
    }

    private void Awake()
    {
        filter = transform.Find("Scroll").Find("Filter").gameObject;
        scrollRect = GetComponentInChildren<PinchableScrollRect>();
        undoButton = FindObjectOfType<UndoButton>();
        gameCont = FindObjectOfType<GameController>();
        fieldControllers = FindObjectsOfType<FieldController>();
        numberButtonsCont = FindObjectOfType<NumberButtonsController>();
        //StartCoroutine(temp());
    }

    public void SetMap(List<string> level, List<string> filledFields)
    {
        scrollRect.IsLevelCompleted = false;
        this.filledFields = filledFields;
        if (steps != null)
        {
            activeField = null;
            foreach (FieldController f in fieldControllers) f.SetIsSelected(activeField);
            undoButton.SetEnabled(false);
            numberButtonsCont.EnableButtons(false, "0");
        }
        steps = new List<Step>();
        fillableFields = new List<FieldController>();        
        List<string> forwardedLevel = new List<string>(level);
        int[] fillableFieldsCount = new int[level.Count];
        int counter = 0;
        for (int j = 0; j < level.Count; j++)
        {
            string helper = "";
            if (j < fillableFieldsCount.Length - 1) fillableFieldsCount[j + 1] = fillableFieldsCount[j];
            for (int i = 0; i < level[j].Length; i++)
            {
                if(level[j][i] == '0')
                {
                    if (j < fillableFieldsCount.Length - 1) fillableFieldsCount[j + 1]++;
                    if (filledFields.Count == counter) filledFields.Add("0");
                    helper += ConvertToABC(filledFields[counter]);
                    counter++;
                }
                else helper += level[j][i];                
            }
            forwardedLevel[j] = helper;
        }
        if (difficulty < 3) for (int i = 0; i < 6; i++) centerControllers[i].SetNumbers(forwardedLevel[i], fillableFieldsCount[i], this);
        if (difficulty == 1) centerControllers[12].SetNumbers(forwardedLevel[6], fillableFieldsCount[6], this);
        if(difficulty == 2)
        {
            for (int j = 6; j < 12; j++) centerControllers[j].SetNumbers(forwardedLevel[j], fillableFieldsCount[j], this);
            centerControllers[18].SetNumbers(forwardedLevel[12], fillableFieldsCount[12], this);
        }
        if(difficulty == 3)
        {
            int counter2 = 0;
            for (int i = 0; i < 18; i++)
            {
                if (counter2 % 3 == 2 && counter2 < 18) counter2++;
                if (counter2 == 18) counter2 += 6;
                centerControllers[counter2].SetNumbers(forwardedLevel[i], fillableFieldsCount[i], this);
                counter2++;
            }
        }
        foreach (CenterController c in centerControllers) c.GainFieldNumbers();
        CheckIfCompleted(true);
        gameCont.SaveFill(filledFields);
    }

    private void Start()
    {
        undoButton.SetEnabled(false);
    }

    public void AddFillableField(FieldController fieldCont)
    {
        fillableFields.Add(fieldCont);
    }

    public void Restart()
    {
        gameCont.Restart();
    }

    string ConvertToABC(string number)
    {
        switch (number)
        {
            case "1":
                return "a";
            case "2":
                return "b";
            case "3":
                return "c";
            case "4":
                return "d";
            case "5":
                return "e";
            case "6":
                return "f";
        }
        return number;
    }

    public void SetActiveField(FieldController fieldCont, string number)
    {
        activeField = fieldCont;
        numberButtonsCont.EnableButtons(true, number);
        foreach(FieldController f in fieldControllers) f.SetIsSelected(activeField);
    }

    public void SetActiveFieldNumber(string number, bool isUndo)
    {
        if (!isUndo)
        {
            steps.Add(new Step(activeField.FieldID, activeField.CurrentNumber));
            undoButton.SetEnabled(true);
        }
        string realNumber = number == activeField.CurrentNumber ? "0" : number;
        filledFields[activeField.FieldID] = realNumber;
        if (gameCont != null) gameCont.SaveFill(filledFields);
        activeField.CurrentNumber = realNumber;
        foreach (CenterController c in centerControllers) c.SetFieldNumber(activeField, realNumber);
        CheckIfCompleted(false);
        //activeField = null;
    }

    public void Undo()
    {
        Step step = steps[steps.Count - 1];
        activeField = fillableFields[step.ID];
        numberButtonsCont.SetNumber(step.PreviousValue, true);
        steps.RemoveAt(steps.Count - 1);
        if (steps.Count == 0) undoButton.SetEnabled(false);
    }

    void CheckIfCompleted(bool isInitial)
    {
        foreach (FieldController f in fieldControllers) f.SetIsGood(true);
        bool isGood = true;
        foreach (CenterController c in centerControllers)
        {
            if (!c.IsCompleted()) isGood = false;
        }
        if (!IsHighlightedGood(true)) isGood = false;
        if ((difficulty == 1 || difficulty == 3) && !IsHighlightedGood(false)) isGood = false;
        if (isGood) LevelCompleted(isInitial);
    }

    bool IsHighlightedGood(bool isHighlighted1)
    {
        FieldController[] highlightedList = isHighlighted1 ? highlighted1Fields : highlighted2Fields;
        bool isGood = true;
        for (int i = 0; i < highlightedList.Length; i++)
        {
            for (int j = 0; j < highlightedList.Length; j++)
            {
                if (i != j && highlightedList[i].CurrentNumber == highlightedList[j].CurrentNumber && highlightedList[i].CurrentNumber != "0")
                {
                    isGood = false;
                    highlightedList[i].SetIsGood(false);
                    highlightedList[j].SetIsGood(false);
                }
                if (highlightedList[i].CurrentNumber == "0") isGood = false;
            }
        }
        if (isGood) foreach (FieldController f in isHighlighted1 ? highlighted1Fields : highlighted2Fields) f.SetHighlightedToGood();
        return isGood;
    }

    void LevelCompleted(bool isInitial)
    {
        scrollRect.IsLevelCompleted = true;
        gameCont.LevelCompleted(difficulty, isInitial);
    }

    IEnumerator temp()
    {
        for (int i = 0; i < centerControllers.Length; i++)
        {
            StartCoroutine(centerControllers[i].temp());
            yield return new WaitForSeconds(2f);
        }
    }

    public void SetFilter(bool value)
    {
        filter.SetActive(value);
    }
}
