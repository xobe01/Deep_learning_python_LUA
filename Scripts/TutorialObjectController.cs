using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectController : MonoBehaviour
{
    GameObject tutorialFilter;
    TutorialButtonController tutorialButtonCont;
    
    public bool FilterEnabled
    {
        get { return isFilterActive; }
        set
        {
            isFilterActive = value;
            tutorialFilter.SetActive(value);
        }
    }
    bool isFilterActive;

    void Awake()
    {
        tutorialFilter = transform.Find("TutorialFilter").gameObject;
        tutorialButtonCont = FindObjectOfType<TutorialButtonController>();
        FilterEnabled = false;
    }

    public void StartTutorial(int difficulty)
    {
        FilterEnabled = true;
        tutorialButtonCont.StartTutorial(difficulty);
    }
}