using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : SceneChangerButton
{
    [SerializeField] Transform line;
    [SerializeField] Sprite filledSprite;
    [SerializeField] Sprite filledSpriteNight;
    [SerializeField] Sprite LockedSprite;
    [SerializeField] Sprite LockedSpriteNight;
    [SerializeField] Sprite nextLevelSprite;

    Text text;
    int difficulty;
    int levelNumber;

    public enum LevelButtonType
    {
        Completed, 
        Unlocked,
        Locked
    }

    public void SetData(int difficulty, int levelNumber, int completedLevels, bool isInLevelSelector, bool isNightMode)
    {
        text = GetComponentInChildren<Text>();
        this.difficulty = difficulty;
        this.levelNumber = levelNumber;
        Image image = GetComponent<Image>();
        if (isInLevelSelector)
        {
            if (isNightMode) text.color = Color.white;
            text.text = (levelNumber + 1).ToString();
            LevelButtonType type;
            if (completedLevels < levelNumber + 1) type = LevelButtonType.Locked;
            else if (completedLevels == levelNumber + 1) type = LevelButtonType.Unlocked;
            else type = LevelButtonType.Completed;
            if (type == LevelButtonType.Completed)
            {
                image.color = isNightMode ? new Color(0, .4f, 1) : new Color(0, 1, 1);
                line.GetComponentInChildren<Image>().sprite = isNightMode ? filledSpriteNight : filledSprite;
            }
            else if (type == LevelButtonType.Unlocked) image.color = new Color(1, 0.5f, 0);
            else
            {
                GetComponent<Button>().interactable = false;
                image.sprite = isNightMode ? LockedSpriteNight : LockedSprite;
            }
            if (levelNumber == DataStorage.LevelData[difficulty].Count - 1) Destroy(line.gameObject);
            else if ((levelNumber + 1) % 4 == 0) line.rotation = Quaternion.Euler(0, 0, 90);
            else if ((levelNumber / 4) % 2 == 1) line.rotation = Quaternion.Euler(0, 0, 180); 
        }
        else
        {
            text.text = "";
            image.sprite = nextLevelSprite;
            image.transform.rotation = Quaternion.Euler(0, 0, 180);
            line.gameObject.SetActive(false);
        }
    }

    protected override void TaskOnClick()
    {
        PlayerPrefs.SetInt("difficulty", difficulty);
        PlayerPrefs.SetInt("levelNumber", levelNumber);
        base.TaskOnClick();
    }
}
