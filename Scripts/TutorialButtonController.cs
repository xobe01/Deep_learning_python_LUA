using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButtonController : ClickButton
{
    [SerializeField] Sprite[] beginnerSprites;
    [SerializeField] Sprite[] beginnerSpritesNight;
    [SerializeField] string[] beginnerStrings;

    [SerializeField] Sprite[] intermediateSprites;
    [SerializeField] Sprite[] intermediateSpritesNight;
    [SerializeField] string[] intermediateStrings;

    [SerializeField] Sprite[] expertSprites;
    [SerializeField] Sprite[] expertSpritesNight;
    [SerializeField] string[] expertStrings;

    [SerializeField] Sprite[] masterSprites;
    [SerializeField] Sprite[] masterSpritesNight;
    [SerializeField] string[] masterStrings;

    [SerializeField] Image image;
    [SerializeField] Text text;

    TutorialObjectController tutorialObject;
    GameController gameCont;
    Sprite[] currentSprites;

    string[] currentStrings;
    int imageNumber;
    bool isNightMode;

    protected override void Awake()
    {
        base.Awake();
        isNightMode = DataStorage.NightMode;
        tutorialObject = FindObjectOfType<TutorialObjectController>();
        text.color = isNightMode ? Color.white : Color.black;
        gameCont = FindObjectOfType<GameController>();
    }

    public void StartTutorial(int difficulty)
    {
        switch (difficulty)
        {
            case 0:
                currentSprites = isNightMode ? beginnerSpritesNight : beginnerSprites;
                currentStrings = beginnerStrings;
                break;
            case 1:
                currentSprites = isNightMode ? intermediateSpritesNight : intermediateSprites;
                currentStrings = intermediateStrings;
                break;
            case 2:
                currentSprites = isNightMode ? expertSpritesNight : expertSprites;
                currentStrings = expertStrings;
                break;
            case 3:
                currentSprites = isNightMode ? masterSpritesNight : masterSprites;
                currentStrings = masterStrings;
                break;
        }
        image.sprite = currentSprites[0];
        text.text = currentStrings[0];
        CheckSubStrings();
    }

    void CheckSubStrings()
    {
        text.text = text.text.Replace("CENTER", "<color=#FF0000>" + "CENTER" + "</color>");
        text.text = text.text.Replace("BLUE", "<color=#" + (isNightMode ? "00FFFF" : "00C1C1") + ">" + "BLUE" + "</color>");
        text.text = text.text.Replace("ORANGE", "<color=#FF9900>" + "ORANGE" + "</color>");
    }

    protected override void TaskOnClick()
    {
        base.TaskOnClick();
        imageNumber++;
        if(imageNumber == currentSprites.Length)
        {
            image.sprite = currentSprites[0];
            text.text = currentStrings[0];
            tutorialObject.FilterEnabled = false;
            imageNumber = 0;
            gameCont.StopStartTime(false);
            gameCont.TutorialCompleted();
        }
        else
        {
            image.sprite = currentSprites[imageNumber];
            text.text = currentStrings[imageNumber];
        }
        CheckSubStrings();
    }
}
