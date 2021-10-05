using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyButton : SceneChangerButton
{
    [SerializeField] int difficulty;
    [SerializeField] int isFreePlay;
    [SerializeField] GameObject lockedObjects;

    protected override void Awake()
    {
        int unlockBoundary = 0;
        switch (difficulty)
        {
            case 1:
                unlockBoundary = 5;
                break;
            case 2:
                unlockBoundary = 15;
                break;
            case 3:
                unlockBoundary = 30;
                break;
        }
        base.Awake();
        if (DataStorage.Stars < unlockBoundary)
        {
            SetInteractable(false);
            transform.Find("Image").gameObject.SetActive(false);
            lockedObjects.SetActive(true);
            lockedObjects.transform.Find("Number").GetComponent<UnityEngine.UI.Text>().text = (unlockBoundary - DataStorage.Stars).ToString();
        }
    }

    protected override void TaskOnClick()
    {
        PlayerPrefs.SetInt("difficulty", difficulty);
        PlayerPrefs.SetInt("isFreePlay", isFreePlay);
        base.TaskOnClick();
    }
}
