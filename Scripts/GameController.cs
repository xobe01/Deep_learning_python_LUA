using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] bool isSerialized;
    [SerializeField] GameObject[] maps;
    [SerializeField] Transform mapParent;
    [SerializeField] GameObject continueTextBox;
    [SerializeField] GameObject levelCompleted;
    [SerializeField] GameObject watchAdFilter;
    [SerializeField] AudioClip completedClip;
    [SerializeField] Text levelText;
    [SerializeField] GameObject timeIndicator;
    [SerializeField] RectTransform[] stars;
    [SerializeField] Canvas numbersCanvas;

    AudioController audioCont;
    LevelButton levelButton;
    TipButton tipButton;
    MapController mapCont;
    RectTransform completedRestartButton;
    TutorialObjectController tutorialObject;

    List<string> levelData;

    int difficulty;
    int levelNumber;
    int freePlayTime;
    string solution;
    bool isFreePlay;
    bool isLevelCompleted;
    bool isTutorial;
    bool isPaused;
    float floatTime;

    void Awake()
    {
        RewardedAdController.SetGameController(this);
        bool isNewFreePlay = DataStorage.FreePlayFilledFields.Count == 0;
        tutorialObject = FindObjectOfType<TutorialObjectController>();
        completedRestartButton = levelCompleted.transform.Find("Restart").GetComponent<RectTransform>();
        audioCont = FindObjectOfType<AudioController>();
        levelButton = levelCompleted.GetComponentInChildren<LevelButton>();
        levelData = new List<string>();
        tipButton = FindObjectOfType<TipButton>();
        difficulty = PlayerPrefs.GetInt("difficulty");
        levelNumber = PlayerPrefs.GetInt("levelNumber");
        int freePlayInt = PlayerPrefs.GetInt("isFreePlay");
        FindObjectOfType<InformationButton>().SetDifficulty(difficulty);
        isFreePlay = freePlayInt > 0;
        levelText.text = isFreePlay ? "FREE PLAY" : "LEVEL " + (levelNumber + 1);
        timeIndicator.SetActive(isFreePlay);
        isTutorial = !DataStorage.IsTutorialCompleted[difficulty]; 
        if (freePlayInt == 2)
        {
            continueTextBox.SetActive(true);
            difficulty = DataStorage.FreePlayDifficulty;
        }
        freePlayTime = DataStorage.FreePlayTime;
        CreateMap(isNewFreePlay);
    }

    IEnumerator IndicateTime()
    {
        Text timeText = timeIndicator.GetComponentInChildren<Text>();
        if(timeIndicator.gameObject.activeSelf)
        {
            while (!isLevelCompleted && !isPaused)
            {
                floatTime += Time.deltaTime;
                if (floatTime > 1)
                {
                    DataStorage.FreePlayTime = freePlayTime;
                    SaveFreePlayTime();
                    freePlayTime++;
                    floatTime -= 1f;
                }
                timeText.text = TransformToTime(freePlayTime);
                yield return null;
            }
        }        
    }

    string TransformToTime(int time)
    {
        int hour = time / 3600;
        int min = time % 3600 / 60;
        int sec = time % 60;
        return (hour > 0 ? hour + ":" : "") + (min > 9 ? min.ToString() : "0" + min) + ":" + (sec > 9 ? sec.ToString() : "0" + sec);
    }

    private void Start()
    {
        if (isTutorial)
        {
            tutorialObject.StartTutorial(difficulty);            
        }
        else tutorialObject.FilterEnabled = false;
        tipButton.SetEnabled((isFreePlay ? DataStorage.GetTipState(-1, -1, true) :
            DataStorage.GetTipState(difficulty, levelNumber, false)) != SerializableData.TipState.InUse && RewardedAdController.IsReady());
    }

    void CreateMap(bool isNewFreePlay)
    {
        List<string> filledFields = new List<string>();
        if (!isFreePlay)
        {
            filledFields = DataStorage.GetFilledFields(difficulty)[levelNumber];
            levelData = DataStorage.LevelData[difficulty][levelNumber];
        }
        else
        {
            if(DataStorage.FreePlayLevel.Count > 0)
            {
                levelData = DataStorage.FreePlayLevel;
                solution = DataStorage.FreePlaySolution;
                filledFields = DataStorage.FreePlayFilledFields;
            }
            else
            {
                levelData = DataStorage.GetFreePlayLevel(difficulty);
                solution = levelData[levelData.Count - 1];
                levelData.RemoveAt(levelData.Count - 1);
                for (int i = 0; i < solution.Length; i++) filledFields.Add("0");
                DataStorage.FreePlayLevel = levelData;
                DataStorage.FreePlaySolution = solution;
                DataStorage.FreePlayFilledFields = filledFields;
                DataStorage.FreePlayDifficulty = difficulty;
                DataStorage.Save();
            }            
        }
        mapCont = Instantiate(maps[difficulty], mapParent).GetComponent<MapController>();
        mapCont.SetMap(levelData, filledFields);
        if (isFreePlay)
        {
            if (isNewFreePlay && !isTutorial) StopStartTime(false);
            else timeIndicator.GetComponentInChildren<Text>().text = TransformToTime(DataStorage.FreePlayTime);
            FindObjectOfType<SceneChangerButton>().SetGoalScene("01_MainMenu");
        }
        else if (!isTutorial) StopStartTime(false);
    }

    public void SaveFill(List<string> filledFields)
    {
        DataStorage.SaveFill(filledFields, isFreePlay, difficulty, levelNumber);
    }

    void GiveTip()
    {
        tipButton.SetEnabled(false);
        StopStartTime(false);
        List<string> values = new List<string>();
        List<string> filledFields = isFreePlay ? DataStorage.FreePlayFilledFields : DataStorage.GetFilledFields(difficulty)[levelNumber];
        if(!isFreePlay) solution = DataStorage.Solutions[difficulty][levelNumber];
        for (int i = 0; i < filledFields.Count; i++)
        {
            values.Add(i % 3 == 1 ? solution[i].ToString() : filledFields[i]);
        }
        if (isFreePlay) DataStorage.SetTipState(SerializableData.TipState.InUse, -1, -1, true);
        else DataStorage.SetTipState(SerializableData.TipState.InUse, difficulty, levelNumber, false);
        SaveFill(values);
        mapCont.SetMap(levelData, values);
    }

    public void Restart()
    {
        if(isLevelCompleted && isFreePlay)
        {
            SceneManager.LoadScene("02_DifficultySelectorFreePlay");
        }
        else
        {
            if (isLevelCompleted && levelNumber == 0) tutorialObject.StartTutorial(difficulty);
            levelCompleted.SetActive(false);
            foreach (RectTransform r in stars) r.localScale = Vector2.zero;
            List<string> restartedFilledFields = new List<string>();
            foreach (string s in isFreePlay ? DataStorage.FreePlayFilledFields : DataStorage.GetFilledFields(difficulty)[levelNumber])
            {
                if ((s == "g" || s == "h" || s == "i" || s == "j" || s == "k" || s == "l") && !isLevelCompleted) restartedFilledFields.Add(s);
                else restartedFilledFields.Add("0");
            }
            if (isLevelCompleted)
            {
                tipButton.SetEnabled(true);
                StopStartTime(true);
            }
            mapCont.SetMap(levelData, restartedFilledFields);
        }        
    }

    public void LevelCompleted(int difficulty, bool isInitial)
    {
        if (!isInitial) audioCont.PlaySound(completedClip);
        isLevelCompleted = true;
        if(levelNumber == DataStorage.GetFilledFields(difficulty).Count - 1 || isFreePlay) DataStorage.LevelCompleted(difficulty, isFreePlay);
        levelCompleted.SetActive(true);
        if (levelNumber + 1 == DataStorage.LevelData[difficulty].Count || isFreePlay) levelButton.gameObject.SetActive(false);
        if (isFreePlay)
        {
            completedRestartButton.localPosition = new Vector2(0, completedRestartButton.localPosition.y);
            levelButton.transform.parent.parent.gameObject.SetActive(false);
            DataStorage.SetTipState(SerializableData.TipState.Unlocked, -1, -1, true);
        }
        else
        {
            levelButton.SetData(isFreePlay ? -1 : difficulty, levelNumber + 1, -1, false, DataStorage.NightMode);
            DataStorage.SetTipState(SerializableData.TipState.Unlocked, difficulty, levelNumber, false);
        }
        PlaceStars(isInitial);
        StopStartTime(true);
    }

    void PlaceStars(bool isInitial)
    {
        for (int i = 0; i < difficulty + 1; i++)
        {
            if (isInitial) stars[i].localScale = Vector2.one;
            else StartCoroutine(GrowStar(stars[i], i * 0.1f));
        }
        switch (difficulty)
        {
            case 0:
                stars[0].localPosition = new Vector2(0, stars[0].localPosition.y);
                break;
            case 1:
                stars[0].localPosition = new Vector2(-120, stars[0].localPosition.y);
                stars[1].localPosition = new Vector2(120, stars[0].localPosition.y);
                break;
            case 2:
                stars[0].localPosition = new Vector2(-240, stars[0].localPosition.y);
                stars[1].localPosition = new Vector2(0, stars[0].localPosition.y);
                stars[2].localPosition = new Vector2(240, stars[0].localPosition.y);
                break;
        }
    }

    IEnumerator GrowStar(RectTransform rect, float delay)
    {
        yield return new WaitForSeconds(delay);
        for (float i = 0; i < 0.2f; i+=Time.deltaTime)
        {
            rect.localScale = Vector2.one * i * 5;
            rect.localRotation = Quaternion.Euler(0, 0, i * 5 * 360);
            yield return null;
        }
        rect.localScale = Vector2.one;
        rect.localRotation = Quaternion.Euler(0, 0, 360);
    }

    public void CloseContinueTextBox(bool continueGame)
    {
        if (continueGame)
        {
            continueTextBox.SetActive(false);
            StopStartTime(false);
        }
        else
        {
            DataStorage.ResetFreePlayData();
            SceneManager.LoadScene("02_DifficultySelectorFreePlay");
        }
    }

    public void TipButtonPressed(bool value)
    {
        if (DataStorage.GetTipState(difficulty, levelNumber, isFreePlay) == SerializableData.TipState.Unlocked) GiveTip();
        else
        {
            watchAdFilter.SetActive(value);
            StopStartTime(value);
        }
    }

    public void ShowRewardedAd()
    {
        RewardedAdController.ShowAd();
    }

    public void RewardedAdFinished()
    {
        GiveTip();
        watchAdFilter.SetActive(false);
    }

    public void SaveFreePlayTime()
    {
        DataStorage.FreePlayTime = freePlayTime;
        DataStorage.Save();
    }

    public void BackButtonPressed()
    {
        if (continueTextBox.activeSelf) CloseContinueTextBox(true);
        else if (watchAdFilter.activeSelf) TipButtonPressed(false);
        else if (tutorialObject.FilterEnabled) { if (!isTutorial) tutorialObject.FilterEnabled = false; }
        else SceneManager.LoadScene(isFreePlay ? "01_MainMenu" : "03_LevelSelector");
    }

    public void StopStartTime(bool stop)
    {
        isPaused = stop;
        numbersCanvas.sortingOrder = stop ? 0 : 1;
        mapCont.SetFilter(!stop);
        if (!stop) StartCoroutine(IndicateTime());
    }

    public void TutorialCompleted()
    {
        isTutorial = false;
        DataStorage.IsTutorialCompleted[difficulty] = true;
    }
}
