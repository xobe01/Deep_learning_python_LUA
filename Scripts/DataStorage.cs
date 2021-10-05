using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

static class DataStorage
{
    static List<List<List<string>>> levelBaseData;
    static List<List<List<string>>> levelData;
    static List<List<string>> solutions;
    static bool isReadingDone;
    static SerializableData data;

    public static List<List<List<string>>> LevelBaseData { get { return levelBaseData; } }
    public static List<List<List<string>>> LevelData { get { return levelData; } }
    public static List<List<string>> Solutions { get { return solutions; } }
    public static bool IsReadingDone { get { return isReadingDone; } }
    public static SerializableData Data { get { return data; } }
    public static List<string> FreePlayLevel
    {
        get { return data.freePlayLevel; }
        set { data.freePlayLevel = value; }
    }
    public static List<string> FreePlayFilledFields
    {
        get { return data.freePlayFilledFields; }
        set { data.freePlayFilledFields = value; }
    }
    public static string FreePlaySolution
    {
        get { return data.freePlaySolution; }
        set { data.freePlaySolution = value; }
    }
    public static int FreePlayDifficulty
    {
        get { return data.freePlayDifficulty; }
        set { data.freePlayDifficulty = value; }
    }

    public static bool IsMuted
    {
        get { return data.isMuted; }
        set { data.isMuted = value; }
    }

    public static int FreePlayTime
    {
        get { return data.freePlayTime; }
        set { data.freePlayTime = value; }
    }

    public static int Stars
    {
        get { return data.stars; }
        set { data.stars = value; }
    }

    public static bool NightMode
    {
        get { return data.nightMode; }
        set { data.nightMode = value; }
    }

    public static List<bool> IsTutorialCompleted
    {
        get { return data.isTutorialCompleted; }
        set { data.isTutorialCompleted = value; }
    }

    [RuntimeInitializeOnLoadMethod]
    static void AppStartFunctions()
    {
        ReadData();
        RewardedAdController.StartRewardedAd();
        Application.targetFrameRate = 60;
    }
    
    static void ReadData()
    {
        data = SaveAndLoad.LoadData(true);
        //data = SaveAndLoad.LoadData(false);
        //SaveAndLoad.SaveData(data);
        levelData = new List<List<List<string>>>();
        for (int i = 0; i < 4; i++) 
        {
            levelData.Add(new List<List<string>>());
            levelData[i] = ReadLevels(i, false);
        }
        solutions = new List<List<string>>();
        for (int i = 0; i < 4; i++)
        {
            solutions.Add(new List<string>());
            solutions[i] = ReadSolutions(i);
        }
        levelBaseData = new List<List<List<string>>>();
        for (int i = 0; i < 4; i++)
        {
            levelBaseData.Add(new List<List<string>>());
            levelBaseData[i] = ReadLevels(i, true);
        }
        isReadingDone = true;
        GoogleMobileAds.Api.MobileAds.Initialize(initStatus => { });
    }

    static List<List<string>> ReadLevels(int type, bool isBase)
    {
        TextAsset data = Resources.Load<TextAsset>(type + (isBase ? "_base" : "") + "_levels");
        StreamReader reader = new StreamReader(new MemoryStream(data.bytes));
        List<string> lines = new List<string>();
        string line;
        while ((line = reader.ReadLine()) != null) lines.Add(line);
        reader.Close();
        List<List<string>> levels = new List<List<string>>();
        foreach(string s in lines)
        {
            List<string> level = new List<string>();
            string[] singles = s.Split(';');
            for (int i = 0; i < singles.Length - 1; i++) level.Add(singles[i]);
            levels.Add(level);
        }
        return levels;
    }

    static List<string> ReadSolutions(int type)
    {
        TextAsset data = Resources.Load<TextAsset>(type + "_solutions");
        StreamReader reader = new StreamReader(new MemoryStream(data.bytes));
        List<string> lines = new List<string>();
        string line;
        while ((line = reader.ReadLine()) != null) lines.Add(line);
        reader.Close();
        return lines;
    }

    public static List<string> GetFreePlayLevel(int difficulty)
    {
        List<string> level = GenerateLevel(difficulty, levelBaseData[difficulty][Random.Range(0, levelBaseData[difficulty].Count)]);
        return level;
    }

    static public List<string> GenerateLevel(int difficulty, List<string> level)
    {
        List<string> returnLevel = new List<string>();
        bool isGood = false;
        string solution = "";
        int counter = 0;
        while (!isGood && counter < 100)
        {
            returnLevel = new List<string>();
            solution = "";
            foreach (string s in level)
            {
                string helper = "";
                int index1 = Random.Range(0, s.Length);
                int index2 = (index1 + Random.Range(1, s.Length)) % s.Length;
                bool hasThirdChosen = false;
                for (int j = 0; j < s.Length; j++)
                {
                    float chance = 0;
                    switch (difficulty)
                    {
                        case 0:
                            chance = 0.6f;
                            break;
                        case 2:
                            chance = 0.2f;
                            break;
                        case 3:
                            chance = 0.1f;
                            break;
                    }
                    bool chooseThird = Random.Range(0f, 1f) < chance && !hasThirdChosen;
                    if (chooseThird) hasThirdChosen = true;
                    if (j != index1 && (j != index2 || difficulty != 1) && !chooseThird)
                    {
                        helper += '0';
                        solution += ConvertToSolution(s[j]);
                    }
                    else helper += s[j];
                }
                returnLevel.Add(helper);
            }
            isGood = IsLevelGood(difficulty, returnLevel);
            counter++;
        }
        returnLevel.Add(solution);
        return returnLevel;
    }

    static bool IsLevelGood(int difficulty, List<string> level)
    {
        bool isGood = false;
        int counter = 0;
        if (difficulty == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                if (level[i][0] == '0') counter++;
            }
            if (counter>=2 && counter <= 4)
            {
                counter = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (level[i][3] == '0') counter++;
                }
                if (counter >= 2) isGood = true;
            }
        }
        else if(difficulty == 1)
        {
            for (int i = 0; i < 6; i++)
            {
                if (level[i][0] == '0') counter++;
            }
            if (counter >= 2 && counter <= 4)
            {
                counter = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (level[i][2] == '0') counter++;
                }
                if (counter >= 2 && counter <= 4)
                {
                    counter = 0;
                    List<char> tempList;
                    isGood = true;
                    for (int i = 0; i < 6; i++)
                    {
                        tempList = new List<char>();
                        tempList.Add(level[i][3]);
                        tempList.Add(level[i][4]);
                        tempList.Add(level[(i + 1) % 6][4]);
                        tempList.Add(level[(i + 1) % 6][5]);
                        tempList.Add(level[6][i]);
                        tempList.Add(level[6][(i + 1) % 6]);
                        foreach (char c in tempList) if (c == '0') counter++;
                        if (counter < 2)
                        {
                            isGood = false;
                            break;
                        }
                    }
                }
            }
        }
        else if(difficulty == 2)
        {
            for (int i = 0; i < 6; i++)
            {
                if (level[i * 2][0] == '0') counter++;
            }
            if (counter >= 2 && counter <= 4)
            {
                counter = 0;
                List<char> tempList;
                isGood = true;
                for (int i = 0; i < 6; i++)
                {
                    tempList = new List<char>();
                    tempList.Add(level[(i * 2 + 1)%12][2]);
                    tempList.Add(level[(i * 2 + 1)% 12][3]);
                    tempList.Add(level[(i * 2 + 2)% 12][3]);
                    tempList.Add(level[(i * 2 + 3)% 12][3]);
                    tempList.Add(level[12][i]);
                    tempList.Add(level[12][(i + 1) % 6]);
                    foreach (char c in tempList) if (c == '0') counter++;
                    if(counter < 2)
                    {
                        isGood = false;
                        break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                if (level[i * 2][0] == '0') counter++;
            }
            if (counter >= 2 && counter <= 4)
            {
                counter = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (level[i * 2 + 1][0] == '0') counter++;
                }
                if (counter >= 2 && counter <= 4)
                {
                    isGood = true;
                    counter = 0;
                    List<char> tempList1;
                    List<char> tempList2;
                    for (int i = 0; i < 6; i++)
                    {
                        tempList1 = new List<char>();
                        tempList2 = new List<char>();
                        tempList1.Add(level[i * 2][3]);
                        tempList1.Add(level[i * 2][4]);
                        tempList1.Add(level[i * 2 + 1][3]);
                        tempList1.Add(level[12 + i][0]);
                        tempList1.Add(level[i == 0 ? 17 : 11 + i][1]);
                        tempList1.Add(level[i == 0 ? 17 : 11 + i][2]);

                        tempList2.Add(level[i * 2 + 1][2]);
                        tempList2.Add(level[i * 2 + 1][3]);
                        tempList2.Add(level[(i * 2 + 2) % 12][4]);
                        tempList2.Add(level[(i * 2 + 2) % 12][5]);
                        tempList2.Add(level[12 + i][0]);
                        tempList2.Add(level[12 + i][1]);

                        foreach (char c in tempList1) if (c == '0') counter++;
                        if (counter < 2)
                        {
                            isGood = false;
                            break;
                        }
                        counter = 0;
                        foreach (char c in tempList2) if (c == '0') counter++;
                        if (counter < 2)
                        {
                            isGood = false;
                            break;
                        }
                    }
                    if (isGood)
                    {
                        tempList1 = new List<char>();
                        for (int i = 0; i < 6; i++)
                        {
                            tempList1.Add(level[12 + i][3]);
                        }
                        foreach (char c in tempList1) if (c == '0') counter++;
                        if (counter < 2)
                        {
                            isGood = false;
                        }
                    }
                }
            }            
        }
        if (!isGood) return false;
        string[] numbers = { "1", "2", "3", "4", "5", "6" };
        List<string> numbersList = new List<string>(numbers);
        foreach(string s in level) foreach(char c in s)  if (numbersList.Contains(c.ToString())) numbersList.Remove(c.ToString());
        if (numbersList.Count > 0) return false;
        return true;
    }

    static public string ConvertToSolution(char c)
    {
        switch (c)
        {
            case '1':
                return "g";
            case '2':
                return "h";
            case '3':
                return "i";
            case '4':
                return "j";
            case '5':
                return "k";
            case '6':
                return "l";
        }
        return "0";
    }

    static public void SaveFill(List<string> filledFields, bool isFreePlay, int difficulty, int levelNumber)
    {
        if (isFreePlay) data.freePlayFilledFields = filledFields;
        else data.filledFields[difficulty][levelNumber] = filledFields;
        Save();
    }

    static public void Save()
    {
        SaveAndLoad.SaveData(data);
    }

    static public void LevelCompleted(int difficulty, bool isFreePlay)
    {
        FireBaseController.TriggerEvent(0, difficulty, data.filledFields[difficulty].Count);
        data.stars += difficulty + 1;
        if (!isFreePlay)
        {
            data.filledFields[difficulty].Add(new List<string>());
            data.tips[difficulty].Add(SerializableData.TipState.Locked);
            Save();
        }
        else
        {
            ResetFreePlayData();
        }
    }

    public static void ResetFreePlayData()
    {
        data.freePlayTime = 0;
        data.freePlaySolution = "";
        data.freePlayLevel = new List<string>();
        data.freePlayFilledFields = new List<string>();
        data.freePlayTipState = SerializableData.TipState.Locked;
        Save();
    }

    public static List<List<string>> GetFilledFields(int difficulty)
    {
        return data.filledFields[difficulty];
    }

    public static void SetTipState(SerializableData.TipState state, int difficulty, int levelNumber, bool isFreePlay)
    {
        if (isFreePlay) data.freePlayTipState = state;
        else data.tips[difficulty][levelNumber] = state;
        Save();
    }

    public static SerializableData.TipState GetTipState(int difficulty, int levelNumber, bool isFreePlay)
    {
        if (isFreePlay) return data.freePlayTipState;
        else return data.tips[difficulty][levelNumber];
    }
}
