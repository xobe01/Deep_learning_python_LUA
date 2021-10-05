using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorController : MonoBehaviour
{
    [SerializeField] GameObject levelButton;
    [SerializeField] Transform scroll;
    [SerializeField] Text difficultyText;

    List<GameObject> buttons;
    RectTransform scrollRect;

    int difficulty;
    int completedLevelCount;
    int currentTopRow;
    int levelCount;
    bool isNightMode;

    void Awake()
    {
        isNightMode = DataStorage.NightMode;
        buttons = new List<GameObject>();
        difficulty = PlayerPrefs.GetInt("difficulty");
        completedLevelCount = DataStorage.GetFilledFields(difficulty).Count;
        levelCount = DataStorage.LevelData[difficulty].Count;
        bool isScrollRectOnTop = completedLevelCount / 4 >= levelCount / 4 - 7;
        currentTopRow = isScrollRectOnTop ? levelCount / 4 - 10 : Mathf.Max(0, (int)Mathf.Floor((completedLevelCount - 1) / 4) - 2);
        switch (difficulty)
        {
            case 0:
                difficultyText.text = "BEGINNER";
                break;
            case 1:
                difficultyText.text = "INTERMEDIATE";
                break;
            case 2:
                difficultyText.text = "EXPERT";
                break;
            case 3:
                difficultyText.text = "MASTER";
                break;
        }
        scrollRect = scroll.GetComponent<RectTransform>();
        scrollRect.sizeDelta = new Vector2(1000, levelCount * 250 / 4 - 200);
        scroll.localPosition = new Vector3(0, (scrollRect.sizeDelta.y - scroll.parent.GetComponent<RectTransform>().sizeDelta.y) / 2
            - Mathf.Floor((completedLevelCount - 1) / 4) * 250, 0);
        int initialRowCount = isScrollRectOnTop ? 10 : 12 - Mathf.Max(0, 2 - (int)Mathf.Floor((completedLevelCount - 1) / 4))
            - Mathf.Max(0, 2 - (levelCount/4 - 8 - (int)Mathf.Floor((completedLevelCount - 1) / 4)));
        for (int i = 0; i < initialRowCount; i++) CreateRow(true, true);
    }

    public void CreateRow(bool toTop, bool isInitial)
    {
        if((currentTopRow < levelCount/4 && toTop) || (currentTopRow > 12 && !toTop))
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject instance = Instantiate(levelButton, scroll);
                instance.GetComponent<RectTransform>().localPosition = new Vector3(-375 + j * 250 + ((toTop ? currentTopRow :
                    (currentTopRow - buttons.Count / 4 - 1)) % 2 == 1 ? 3 - 2 * j : 0) * 250, (toTop ? currentTopRow :
                    (currentTopRow - buttons.Count/4 - 1)) * 250 - scrollRect.sizeDelta.y / 2, 0);
                instance.GetComponentInChildren<LevelButton>().SetData(difficulty, (toTop ? currentTopRow : (currentTopRow - 
                    buttons.Count / 4 - 1)) * 4 + j, completedLevelCount, true, isNightMode);
                if (toTop) buttons.Add(instance);
                else
                {
                    buttons.Insert(0, instance);
                    instance.transform.SetSiblingIndex(j);
                }
            }
            if (toTop) currentTopRow++;
        }         
        if (!isInitial && (buttons.Count == 52 || (currentTopRow >= levelCount / 4 && toTop) || (currentTopRow <= 12 && !toTop)))
        {
            if (!toTop) currentTopRow--;
            for (int i = 0; i < 4; i++)
            {
                int index = toTop ? 0 : buttons.Count - 1;
                Destroy(buttons[index]);
                buttons.RemoveAt(index);
            }            
        }
    }
}
