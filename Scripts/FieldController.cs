using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FieldController : ClickButton
{
    [SerializeField] int highlightedType;
    [SerializeField] Sprite lockedTexture;

    GameObject selectedFrame;

    MapController mapcont;
    Text text;
    Image image;

    Color normalColor;
    Color wrongColor;
    Color wrongLockedColor;
    Color lockedColor;
    Color highlighted1Color;
    Color lockedHighlighted1Color;
    Color highlighted2Color;
    Color lockedHighlighted2Color;
    Color tipColor;
    Color goodColor;
    Color goodLockedColor;

    string currentNumber;
    int fieldID;
    bool isLocked;
    bool isTip;
    bool isNightMode;
    bool isGood;

    public int FieldID
    {
        get { return fieldID; }
    }

    public string CurrentNumber
    {
        get { return currentNumber; }
        set 
        {
            text.text = value == "0" ? "" : value;
            currentNumber = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        selectedFrame = transform.Find("SelectedFrame").gameObject;
        if (highlightedType == 1) selectedFrame.GetComponent<Image>().color = Color.white;
        clickSound = Resources.Load("Click_field") as AudioClip;
        mapcont = FindObjectOfType<MapController>();
        image = GetComponent<Image>();
        image.alphaHitTestMinimumThreshold = 0.5f;
        text = GetComponentInChildren<Text>();
        isNightMode = DataStorage.NightMode;        
        normalColor = isNightMode ? Color.black : Color.white;
        wrongColor = Color.red;
        wrongLockedColor = new Color(0.8f, 0, 0);
        lockedColor = isNightMode ? new Color(.2f, .2f, .2f) : new Color(.8f, .8f, .8f);
        highlighted1Color = isNightMode ? new Color(0, .4f, 1) : new Color(0, 1, 1);
        lockedHighlighted1Color = isNightMode ? new Color(0, .2f, .5f) : new Color(0, .7f, .7f);
        highlighted2Color = new Color(1f, 0.627451f, 0f);
        lockedHighlighted2Color = new Color(.8f, .5f, .0f);
        tipColor = new Color(1, 1, 0);
        goodColor = Color.green;
        goodLockedColor = new Color(0, 0.8f, 0);

        if (highlightedType == 1) image.color = highlighted1Color;
        else if (highlightedType == 2) image.color = highlighted2Color;
    }

    public void SetNumber(string number, int fieldID)
    {
        this.fieldID = fieldID;
        bool isFilled = false;
        isTip = false;
        isLocked = false;
        if (number != ConvertToNumber(number))
        {
            number = ConvertToNumber(number);
            if(int.Parse(number) < 7) isFilled = true;
            else
            {
                number = (int.Parse(number) - 6).ToString();
                isTip = true;
            }
        }
        text.text = number == "0" ? "" : number;
        currentNumber = number;
        if(number != "0" && !isFilled)
        {
            button.interactable = false;
            isLocked = true;
        }
        if (!isLocked && !isTip) button.interactable = true;
        if (isNightMode && !isTip) text.color = Color.white;
    }

    string ConvertToNumber(string number)
    {
        switch (number)
        {
            case "a":
                return "1";
            case "b":
                return "2";
            case "c":
                return "3";
            case "d":
                return "4";
            case "e":
                return "5";
            case "f":
                return "6";
            case "g":
                return "7";
            case "h":
                return "8";
            case "i":
                return "9";
            case "j":
                return "10";
            case "k":
                return "11";
            case "l":
                return "12";
        }
        return number;
    }

    public IEnumerator temp()
    {
        yield return new WaitForSeconds(0.1f);
        image.color = Color.black;
        yield return new WaitForSeconds(0.3f);
        image.color = Color.white;
    }

    protected override void TaskOnClick()
    {
        base.TaskOnClick();
        mapcont.SetActiveField(this, currentNumber);

    }
    
    public void SetIsSelected(FieldController fieldCont)
    {
        selectedFrame.SetActive(fieldCont == this);
    }

    public void SetIsGood(bool isGood)
    {
        this.isGood = isGood;
        Color color;
        if (isTip)
        {
            color = tipColor;
            if (isNightMode) text.color = Color.black;
        }
        else
        {
            if (isGood)
            {
                if (isLocked)
                {
                    if (highlightedType == 1) color = lockedHighlighted1Color;
                    else if (highlightedType == 2) color = lockedHighlighted2Color;
                    else color = lockedColor;
                }
                else
                {
                    if (highlightedType == 1) color = highlighted1Color;
                    else if (highlightedType == 2) color = highlighted2Color;
                    else color = normalColor;
                }
            }
            else
            {
                if (isLocked) color = wrongLockedColor;
                else color = wrongColor;
            }
        }
        image.color = color;
    }

    public void SetHighlightedToGood()
    {
        if (isGood && !isTip) image.color = isLocked ? (isNightMode ? new Color(0, .4f, 0) : goodLockedColor) :
                 (isNightMode ? new Color(0, .7f, 0) : goodColor);
    }
}
