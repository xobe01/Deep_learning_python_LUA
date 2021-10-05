using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [SerializeField] int difficulty;
    [SerializeField] int levelCount;
    [SerializeField] bool generateFromLevels;
    [SerializeField] GameObject[] maps;
    [SerializeField] Transform canvas;

    List<string> numbers;
    List<string> highlighted1Numbers;
    List<string> highlighted2Numbers;
    List<List<string>> centers;
    List<List<string>> combinations;
    List<string> chosenCombinations;
    List<string> centrumNumbers;
    List<List<string>> levels;
    List<List<string>> previousBaseLevels;

    int centersCount;

    void Awake()
    {
        previousBaseLevels = ReadFile(difficulty, true);
        if (generateFromLevels) GenerateFromLevels();
        else GenerateLevels();
    }

    void GenerateFromLevels()
    {
        List<List<string>> previousLevels = ReadFile(difficulty, false);
        List<List<string>> generatedLevels = new List<List<string>>();
        List<string> solutions = new List<string>();
        for (int i = previousLevels.Count; i < previousLevels.Count + levelCount; i++)
        {
            List<string> level = DataStorage.GenerateLevel(difficulty, previousBaseLevels[i]);
            solutions.Add(level[level.Count - 1]);
            level.RemoveAt(level.Count - 1);
            generatedLevels.Add(level);
        }
        SaveData(generatedLevels, false);
        SaveSolutions(solutions);
    }    

    void GenerateLevels()
    {
        levels = new List<List<string>>();
        numbers = new List<string>();
        for (int j = 0; j < 6; j++) numbers.Add((j + 1).ToString());
        switch (difficulty)
        {
            case 0:
                centersCount = 7;
                break;
            case 1:
                centersCount = 13;
                break;
            case 2:
                centersCount = 19;
                break;
            case 3:
                centersCount = 31;
                break;
        }
        Instantiate(maps[difficulty], canvas);
        List<string> level = new List<string>();
        for (int i = 0; i < levelCount; i++)
        {
            level = new List<string>();
            ResetData();
            int counter = 0;
            switch (difficulty)
            {
                case 0:
                    while (((level = CreateBeginnerLevel()).Count == 0 || IsAlreadyInList(level, levels)) && counter < 20)
                    {
                        counter++;
                        ResetData();
                    }
                    break;
                case 1:
                    while (((level = CreateIntermediateLevel()).Count == 0 || IsAlreadyInList(level, levels)) && counter < 20)
                    {
                        counter++;
                        ResetData();
                    }
                    break;
                case 2:
                    while (((level = CreateExpertLevel()).Count == 0 || IsAlreadyInList(level, levels)) && counter < 20)
                    {
                        counter++;
                        ResetData();
                    }
                    break;
                case 3:
                    while (((level = CreateMasterLevel()).Count == 0 || IsAlreadyInList(level, levels)) && counter < 20)
                    {
                        counter++;
                        ResetData();
                    }
                    break;
            }
            for (int j = 0; j < 2; j++)
            {
                if (level.Count > 0 && !IsAlreadyInList(level, levels) && !IsAlreadyInList(level, previousBaseLevels))
                {
                    levels.Add(level);
                }
            }

        }
        if (level.Count > 0)
        {
            FindObjectOfType<MapController>().SetMap(level, new List<string>());
        }
        SaveData(levels, true);
    }
    bool IsAlreadyInList(List<string> level, List<List<string>> testList)
    {
        foreach(List<string> l in testList)
        {
            bool isInList = true;
            for (int i = 0; i < l.Count; i++)
            {
                if(level[i] != l[i])
                {
                    isInList = false;
                    break;
                }
            }
            if (isInList) return true;
        }
        return false;
    }

    List<List<string>> ReadFile(int type, bool fromBase)
    {
        string fileName = Application.dataPath + "/Resources/" + type + (fromBase ? "_base" : "") + "_levels.txt";
        System.IO.StreamReader reader = new System.IO.StreamReader(fileName, true);
        List<string> data = new List<string>();
        string line;
        while ((line = reader.ReadLine()) != null) data.Add(line);
        reader.Close();
        List<List<string>> levels = new List<List<string>>();
        foreach (string s in data)
        {
            List<string> level = new List<string>();
            string[] singles = s.Split(';');
            for (int i = 0; i < singles.Length - 1; i++) level.Add(singles[i]);
            levels.Add(level);
        }
        return levels;
    }

    void SaveData(List<List<string>> levels, bool toBase)
    {
        List<string> data = new List<string>();
        foreach(List<string> l in levels)
        {
            string helper = "";
            foreach (string s in l) helper += s + ";";
            data.Add(helper);
        }
        string fileName = Application.dataPath + "/Resources/" + difficulty + (toBase ? "_base" : "") + "_levels.txt";
        System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, true);
        foreach (string s in data) writer.WriteLine(s);
        writer.Close();
    }

    void SaveSolutions(List<string> solutions)
    {
        string fileName = Application.dataPath + "/Resources/" + difficulty + "_solutions.txt";
        System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, true);
        foreach (string s in solutions) writer.WriteLine(s);
        writer.Close();
    }

    void ResetData()
    {
        chosenCombinations = new List<string>();
        combinations = new List<List<string>>();
        centers = new List<List<string>>();
        for (int j = 0; j < centersCount; j++) centers.Add(new List<string>());
        highlighted1Numbers = new List<string>();
        highlighted2Numbers = new List<string>();
        centrumNumbers = new List<string>();
    }

    public List<string> CreateBeginnerLevel() 
    {
        int exitCounter = 0;
        int counter = 0;
        List<string> returnedList;
        while (exitCounter < 1000 && counter<6)
        {
            if(combinations.Count == counter)
            {
                List<string> initialList = new List<string>();
                initialList.Add("");
                returnedList = FindCombinationsBeginner(counter, initialList);
                if (returnedList.Count == 0)
                {
                    counter--;
                    DeleteChosenNumbersBeginner(counter);
                    continue;
                }
                else
                {
                    combinations.Add(returnedList);
                }
            }
            if (combinations[counter].Count == 0)
            {
                combinations.RemoveAt(counter);
                if (counter == 0) break;
                counter--;
                DeleteChosenNumbersBeginner(counter);
            }
            else
            {
                ChooseCombinationBeginner(counter);
                counter++;
            }            
            exitCounter++;
        }
        for (int i = 0; i < centers.Count - 1; i++)
        {
            List<string> tempList = new List<string>(numbers);
            foreach (string s in centers[i]) tempList.Remove(s);
            centers[i].Add(tempList[0]);
            chosenCombinations[i] += tempList[0];
            string cc = chosenCombinations[i];
            chosenCombinations[i] = cc[0].ToString() + cc[3].ToString() + cc[2].ToString() + cc[1].ToString();
        }
        return counter == 6 ? chosenCombinations : new List<string>();
    }

    List<string> FindCombinationsBeginner(int counter, List<string> chosenNumbers)
    {
        int type = chosenNumbers[0].Length;
        List<string> returnList = new List<string>();
        foreach (string s in chosenNumbers)
        {
            List<string> tempNumbers = new List<string>(numbers);
            foreach (string i in centers[counter]) tempNumbers.Remove(i);
            for (int z = 0; z < s.Length; z++) tempNumbers.Remove(s[z].ToString());
            foreach (string st in tempNumbers)
            {
                if ((type == 0 && !highlighted1Numbers.Contains(st)) ||
                    (type == 1 && !centers[6].Contains(st) && !centers[(counter + 1) % 6].Contains(st)) ||
                    (type == 2 && !centers[(counter + 1) % 6].Contains(st)))
                {
                    returnList.Add(s + st);
                }
            }
        }
        if (returnList.Count == 0 || type == 2) return returnList;
        else return FindCombinationsBeginner(counter, returnList);
    }

    void ChooseCombinationBeginner(int counter)
    {
        string chosenCombination = combinations[counter][Random.Range(0, combinations[counter].Count)];
        highlighted1Numbers.Add(chosenCombination[0].ToString());
        centers[6].Add(chosenCombination[1].ToString());
        centers[(counter + 1) % 6].Add(chosenCombination[1].ToString());
        centers[(counter + 1) % 6].Add(chosenCombination[2].ToString());
        for (int i = 0; i < 3; i++) centers[counter].Add(chosenCombination[i].ToString());
        combinations[counter].Remove(chosenCombination);
        chosenCombinations.Add(chosenCombination);
    }

    void DeleteChosenNumbersBeginner(int counter)
    {
        chosenCombinations.RemoveAt(chosenCombinations.Count - 1);
        highlighted1Numbers.RemoveAt(highlighted1Numbers.Count - 1);
        centers[6].RemoveAt(centers[6].Count - 1);
        centers[(counter + 1) % 6].RemoveAt(centers[(counter + 1) % 6].Count - 1);
        centers[(counter + 1) % 6].RemoveAt(centers[(counter + 1) % 6].Count - 1);
        for (int i = 0; i < 3; i++) centers[counter].RemoveAt(centers[counter].Count - 1);
    }

    public List<string> CreateIntermediateLevel()
    {
        int exitCounter = 0;
        int counter = 0;
        List<string> returnedList;
        while (exitCounter < 10000 && counter < 6)
        {
            if (combinations.Count == counter)
            {
                List<string> initialList = new List<string>();
                initialList.Add("");
                returnedList = FindCombinationsIntermediate(counter, initialList);
                if (returnedList.Count == 0)
                {
                    counter--;
                    DeleteChosenNumbersIntermediate(counter);
                    continue;
                }
                else
                {
                    combinations.Add(returnedList);
                }
            }
            if (combinations[counter].Count == 0)
            {
                combinations.RemoveAt(counter);
                if (counter == 0) break;
                counter--;
                DeleteChosenNumbersIntermediate(counter);
            }
            else
            {
                ChooseCombinationIntermediate(counter);
                counter++;;
                if (counter == 6)
                {
                    List<string> tempList = new List<string>();
                    for (int i = 6; i < 12; i++)
                    {
                        for (int j= 1; j < 7; j++)
                        {
                            if(!centers[i].Contains(j.ToString()) && !centers[i + 1 > 11 ? 6 : i + 1].Contains(j.ToString()))
                            {
                                if (tempList.Contains(j.ToString())) break;
                                tempList.Add(j.ToString());
                                break;
                            }
                        }
                        if(tempList.Count < i - 5)
                        {
                            break;
                        }
                    }
                    if (tempList.Count == 6)
                    {
                        string combination = "";
                        for (int i = 0; i < tempList.Count; i++) combination += tempList[i];
                        chosenCombinations.Add(combination);
                    }
                    else
                    {
                        counter--;
                        DeleteChosenNumbersIntermediate(counter);
                    }
                }
            }
            exitCounter++;
        }
        if(counter == 6)
        {
            for (int i = 0; i < 6; i++)
            {
                List<string> tempList = new List<string>(numbers);
                foreach (string s in centers[i]) tempList.Remove(s);
                centers[i].Add(tempList[0]);
                chosenCombinations[i] += tempList[0];
                string cc = chosenCombinations[i];
                chosenCombinations[i] = cc[0].ToString() + cc[5].ToString() + cc[1].ToString() + cc[4].ToString() + cc[2].ToString() + cc[3].ToString();
            }
        }        
        return counter == 6 ? chosenCombinations : new List<string>();
    }

    List<string> FindCombinationsIntermediate(int counter, List<string> chosenNumbers)
    {
        int type = chosenNumbers[0].Length;
        List<string> returnList = new List<string>();
        foreach (string s in chosenNumbers)
        {
            List<string> tempNumbers = new List<string>(numbers);
            for (int z = 0; z < s.Length; z++) tempNumbers.Remove(s[z].ToString());
            foreach (string st in tempNumbers)
            {
                if ((type == 0 && !highlighted1Numbers.Contains(st)) ||
                    (type == 1 && !highlighted2Numbers.Contains(st)) ||
                    (type == 2 && !centers[counter + 6].Contains(st) && !centers[counter + 7 > 11 ? 6 : counter + 7].Contains(st)) ||
                    (type == 3 && !centers[counter + 6].Contains(st)) ||
                     type == 4 && !centers[counter + 7 > 11 ? 6 : counter + 7].Contains(st))
                {
                    returnList.Add(s + st);
                }
            }
        }
        if (returnList.Count == 0 || type == 4)
        {
            if (counter > 1)
            {
                List<string> tempList = new List<string>();
                foreach (string s in returnList)
                {
                    bool isGood = false;
                    List<string> innerCenter = new List<string>(centers[counter + 6]);
                    innerCenter.Add(s[2].ToString());
                    innerCenter.Add(s[3].ToString());
                    for (int i = 1; i < 7; i++)
                    {
                        if(!innerCenter.Contains(i.ToString()) && !centers[counter + 5].Contains(i.ToString()) && !centrumNumbers.Contains(i.ToString()))
                        {
                            isGood = true;
                        }
                        if(!innerCenter.Contains(i.ToString()) && centrumNumbers.Contains(i.ToString()))
                        {
                            isGood = false;
                            break;
                        }
                    }
                    if (isGood)
                    {
                        tempList.Add(s);
                    }
                }
                returnList = new List<string>(tempList);
            }            
            return returnList;
        }
        else return FindCombinationsIntermediate(counter, returnList);
    }

    void ChooseCombinationIntermediate(int counter)
    {
        string chosenCombination = combinations[counter][Random.Range(0, combinations[counter].Count)];
        highlighted1Numbers.Add(chosenCombination[0].ToString());
        highlighted2Numbers.Add(chosenCombination[1].ToString());
        centers[counter + 6].Add(chosenCombination[2].ToString());
        centers[counter + 7 > 11 ? 6 : counter + 7].Add(chosenCombination[2].ToString());
        centers[counter + 6].Add(chosenCombination[3].ToString());
        centers[counter + 7 > 11 ? 6 : counter + 7].Add(chosenCombination[4].ToString());
        for (int i = 0; i < 5; i++) centers[counter].Add(chosenCombination[i].ToString());
        combinations[counter].Remove(chosenCombination);
        chosenCombinations.Add(chosenCombination);
        if(counter > 1)
        {
            for (int i = 1; i < 7; i++)
            {
                if(!centers[counter + 6].Contains(i.ToString()) && !centers[counter + 5].Contains(i.ToString()))
                {
                    centrumNumbers.Add(i.ToString());
                    break;
                }
            }
        }
    }

    void DeleteChosenNumbersIntermediate(int counter)
    {
        chosenCombinations.RemoveAt(chosenCombinations.Count - 1);
        highlighted1Numbers.RemoveAt(highlighted1Numbers.Count - 1);
        highlighted2Numbers.RemoveAt(highlighted2Numbers.Count - 1);
        for (int i = 0; i < 2; i++)
        {
            centers[counter + 6].RemoveAt(centers[counter + 6].Count - 1);
            centers[counter + 7 > 11 ? 6 : counter + 7].RemoveAt(centers[counter + 7 > 11 ? 6 : counter + 7].Count - 1);
        }
        for (int i = 0; i < 5; i++) centers[counter].RemoveAt(centers[counter].Count - 1);
        if (centrumNumbers.Count > 0) centrumNumbers.RemoveAt(centrumNumbers.Count - 1);
    }

    public List<string> CreateExpertLevel()
    {
        int exitCounter = 0;
        int counter = 0;
        List<string> returnedList;
        while (exitCounter < 10000 && counter < 12)
        {
            if (combinations.Count == counter)
            {
                List<string> initialList = new List<string>();
                initialList.Add("");
                returnedList = FindCombinationsExpert(counter, initialList);
                if (returnedList.Count == 0)
                {
                    counter--;
                    DeleteChosenNumbersExpert(counter);
                    continue;
                }
                else
                {
                    combinations.Add(returnedList);
                }
            }
            if (combinations[counter].Count == 0)
            {
                combinations.RemoveAt(counter);
                if (counter == 0) break;
                counter--;
                DeleteChosenNumbersExpert(counter);
            }
            else
            {
                ChooseCombinationExpert(counter);
                counter++;
                if (counter == 12)
                {
                    List<string> tempList = new List<string>();
                    for (int i = 12; i < 18; i++)
                    {
                        for (int j = 1; j < 7; j++)
                        {
                            if (!centers[i].Contains(j.ToString()) && !centers[i + 1 > 17 ? 12 : i + 1].Contains(j.ToString()))
                            {
                                if (tempList.Contains(j.ToString())) break;
                                tempList.Add(j.ToString());
                                break;
                            }
                        }
                        if (tempList.Count < i - 11)
                        {
                            break;
                        }
                    }
                    if (tempList.Count == 6)
                    {
                        string combination = "";
                        for (int i = 0; i < tempList.Count; i++) combination += tempList[i];
                        chosenCombinations.Add(combination);
                    }
                    else
                    {
                        counter--;
                        DeleteChosenNumbersExpert(counter);
                    }
                }
            }
            exitCounter++;
        }
        if(counter == 12)
        {
            for (int i = 0; i < 12; i++)
            {
                List<string> tempList = new List<string>(numbers);
                foreach (string s in centers[i]) tempList.Remove(s);
                centers[i].Add(tempList[0]);
                chosenCombinations[i] += tempList[0];
                string cc = chosenCombinations[i];
                chosenCombinations[i] = i % 2 == 0 ? cc[0].ToString() + cc[3].ToString() + cc[2].ToString() + cc[1].ToString() :
                    cc[3].ToString() + cc[1].ToString() + cc[0].ToString() + cc[2].ToString();
            }
        }        
        return counter == 12 ? chosenCombinations : new List<string>();
    }

    List<string> FindCombinationsExpert(int counter, List<string> chosenNumbers)
    {
        int type = chosenNumbers[0].Length;
        List<string> returnList = new List<string>();
        foreach (string s in chosenNumbers)
        {
            List<string> tempNumbers = new List<string>(numbers);
            for (int z = 0; z < s.Length; z++) tempNumbers.Remove(s[z].ToString());
            foreach (string st in centers[counter]) tempNumbers.Remove(st);
            foreach (string st in tempNumbers)
            {
                if(counter % 2 == 0)
                {
                    if ((type == 0 && !highlighted1Numbers.Contains(st)) ||
                    (type == 1 && !centers[12 + counter/2].Contains(st)) ||
                    type == 2)
                    {
                        returnList.Add(s + st);
                    }
                }
                else
                {
                    if ((type == 0 && ((counter == 11 && !centers[0].Contains(st)) || counter != 11) && 
                        !centers[counter < 11 ? 13 + counter / 2 : 12].Contains(st)) ||
                    (type == 1 && ((counter == 11 && !centers[0].Contains(st)) || counter != 11)) ||
                    type == 2 && !centers[counter < 11 ? 13 + counter / 2 : 12].Contains(st) &&
                    !centers[12 + counter / 2].Contains(st))
                    {
                        returnList.Add(s + st);
                    }
                }
            }
        }
        if (returnList.Count == 0 || type == 2)
        {
            if (counter > 4 && counter % 2 == 1)
            {
                List<string> tempList = new List<string>();
                foreach (string s in returnList)
                {
                    bool isGood = false;
                    List<string> innerCenter = new List<string>(centers[12 + counter / 2]);
                    innerCenter.Add(s[2].ToString());
                    for (int i = 1; i < 7; i++)
                    {
                        if (!innerCenter.Contains(i.ToString()) && !centers[12 + counter / 2 - 1].Contains(i.ToString()) && 
                            !centrumNumbers.Contains(i.ToString()))
                        {
                            isGood = true;
                        }
                        if (!innerCenter.Contains(i.ToString()) && centrumNumbers.Contains(i.ToString()))
                        {
                            isGood = false;
                            break;
                        }
                    }
                    if(counter == 11)
                    {
                        if (s[0].ToString() == centrumNumbers[0] || s[2].ToString() == centrumNumbers[0]) isGood = false;
                    }
                    if (isGood)
                    {
                        tempList.Add(s);
                    }
                }
                returnList = new List<string>(tempList);
            }
            return returnList;
        }
        else return FindCombinationsExpert(counter, returnList);
    }

    void ChooseCombinationExpert(int counter)
    {
        string chosenCombination = combinations[counter][Random.Range(0, combinations[counter].Count)];
        if(counter % 2 == 0)
        {
            highlighted1Numbers.Add(chosenCombination[0].ToString());
            centers[counter + 1].Add(chosenCombination[1].ToString());
            centers[12 + counter / 2].Add(chosenCombination[1].ToString());
            centers[counter + 1].Add(chosenCombination[2].ToString());
            for (int i = 0; i < 3; i++) centers[counter].Add(chosenCombination[i].ToString());
        }
        else
        {
            centers[(counter + 1) % 12].Add(chosenCombination[0].ToString());
            centers[counter < 11 ? 13 + counter / 2 : 12].Add(chosenCombination[0].ToString());
            centers[(counter + 1) % 12].Add(chosenCombination[1].ToString());
            centers[counter < 11 ? 13 + counter / 2 : 12].Add(chosenCombination[2].ToString());
            centers[12 + counter / 2].Add(chosenCombination[2].ToString());
            for (int i = 0; i < 3; i++) centers[counter].Add(chosenCombination[i].ToString());
        }
        combinations[counter].Remove(chosenCombination);
        chosenCombinations.Add(chosenCombination);
        if (counter > 4 && counter % 2 == 1)
        {
            for (int i = 1; i < 7; i++)
            {
                if (!centers[12 + counter / 2].Contains(i.ToString()) && !centers[12 + counter / 2 - 1].Contains(i.ToString()))
                {
                    centrumNumbers.Add(i.ToString());
                    break;
                }
                
            }
            if(counter == 5)
            {
                for (int i = 1; i < 7; i++)
                {
                    if (!centers[12 + counter / 2 - 1].Contains(i.ToString()) && !centrumNumbers.Contains(i.ToString()))
                    {
                        centrumNumbers.Insert(0, i.ToString());
                        break;
                    }
                }
            }   
        }
    }

    void DeleteChosenNumbersExpert(int counter)
    {
        chosenCombinations.RemoveAt(chosenCombinations.Count - 1);
        if (counter % 2 == 0)
        {
            highlighted1Numbers.RemoveAt(highlighted1Numbers.Count - 1);
            for (int i = 0; i < 2; i++) centers[counter + 1].RemoveAt(centers[counter + 1].Count - 1);
            centers[12 + counter / 2].RemoveAt(centers[12 + counter / 2].Count - 1);
            for (int i = 0; i < 3; i++) centers[counter].RemoveAt(centers[counter].Count - 1);
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                centers[(counter + 1) % 12].RemoveAt(centers[(counter + 1) % 12].Count - 1);
                centers[counter < 11 ? 13 + counter / 2 : 12].RemoveAt(centers[counter < 11 ? 13 + counter / 2 : 12].Count - 1);
            }
            centers[12 + counter / 2].RemoveAt(centers[12 + counter / 2].Count - 1);
            for (int i = 0; i < 3; i++) centers[counter].RemoveAt(centers[counter].Count - 1);
            if (centrumNumbers.Count > 0) centrumNumbers.RemoveAt(centrumNumbers.Count - 1);
            if (centrumNumbers.Count == 1) centrumNumbers.RemoveAt(centrumNumbers.Count - 1);
        }
    }

    public List<string> CreateMasterLevel()
    {
        int exitCounter = 0;
        int counter = 0;
        List<string> returnedList;
        while (exitCounter < 5000 && counter < 18)
        {
            if (combinations.Count == counter)
            {
                List<string> initialList = new List<string>();
                initialList.Add("");
                returnedList = FindCombinationsMaster(counter, initialList);
                if (returnedList.Count == 0)
                {
                    counter--;
                    DeleteChosenNumbersMaster(counter);
                    continue;
                }
                else
                {
                    combinations.Add(returnedList);
                }
            }
            if (combinations[counter].Count == 0)
            {
                combinations.RemoveAt(counter);
                if (counter == 0) break;
                counter--;
                DeleteChosenNumbersMaster(counter);
            }
            else
            {
                ChooseCombinationMaster(counter);
                counter++;
            }
            exitCounter++;
        }
        if (chosenCombinations.Count == 18)
        {
            for (int i = 0; i < 6; i++) chosenCombinations.Add("");
            for (int i = 0; i < 18; i++)
            {
                if (i % 3 != 2)
                {
                    List<string> tempList = new List<string>(numbers);
                    foreach (string s in centers[i]) tempList.Remove(s);
                    centers[i].Add(tempList[0]);
                    chosenCombinations[i] += tempList[0];
                }
            }
            for (int i = 0; i < 18; i++)
            {
                string cc = chosenCombinations[i];
                if (i % 3 == 0)
                {
                    chosenCombinations[i] = cc[0].ToString() + cc[3].ToString() + cc[2].ToString() + cc[1].ToString();
                    if (cc.Length == 6) chosenCombinations[i] += cc[4].ToString() + cc[5].ToString();
                }
                else if (i % 3 == 1)
                {
                    chosenCombinations[i] = cc[0].ToString() + cc[4].ToString() + cc[2].ToString() + cc[1].ToString();
                    chosenCombinations[i > 2 ? 17 + i / 3 : 23] += cc[3].ToString();
                }
                else
                {
                    chosenCombinations[i + 1 < 18 ? i + 1 : 0] += cc[0].ToString() + cc[1].ToString();
                    chosenCombinations[18 + i / 3] += cc[2].ToString();
                    if(i == 17)
                    {
                        string cc2 = chosenCombinations[18 + i / 3];
                        chosenCombinations[18 + i / 3] = cc2[3].ToString() + cc2[0].ToString() + cc2[1].ToString() + cc2[2].ToString();
                    }
                    chosenCombinations[i > 2 ? 17 + i / 3 : 23] += cc[3].ToString() + cc[4].ToString();
                }
            }
            List<string> tempList2 = new List<string>();
            for (int i = 0; i < chosenCombinations.Count; i++)
            {
                if (i % 3 != 2 || i > 17) tempList2.Add(chosenCombinations[i]);
            }
            chosenCombinations = tempList2;            
            return chosenCombinations;
        }
        else
        {
            return new List<string>();
        }
    }

    List<string> FindCombinationsMaster(int counter, List<string> chosenNumbers)
    {
        int type = chosenNumbers[0].Length;
        List<string> returnList = new List<string>();
        foreach (string s in chosenNumbers)
        {
            List<string> tempNumbers = new List<string>(numbers);
            for (int z = 0; z < s.Length; z++) tempNumbers.Remove(s[z].ToString());
            foreach (string st in centers[counter + (counter >= 18 ? 6 : 0)]) tempNumbers.Remove(st);
            foreach (string st in tempNumbers)
            {
                if (counter % 3 == 0)
                {
                    if ((type == 0 && !highlighted1Numbers.Contains(st)) ||
                    (type == 1 && !centers[18 + counter / 3].Contains(st)) ||
                    type == 2)
                    {
                        returnList.Add(s + st);
                    }
                }
                else if (counter % 3 == 1)
                {
                    if ((type == 0 && !highlighted2Numbers.Contains(st)) ||
                    (type == 1 && !centers[18 + counter / 3].Contains(st)) && !centers[counter + 1].Contains(st) ||
                    type == 2 && !centers[counter + 1].Contains(st))
                    {
                        returnList.Add(s + st);
                    }
                }
                else
                {
                    if (type == 0 && ((counter == 17 && !centers[0].Contains(st) && !centers[18].Contains(st)) || counter != 17) ||
                            type == 1 && ((counter == 17 && !centers[0].Contains(st)) || counter != 17))
                    {
                        returnList.Add(s + st);
                    }
                }
            }
        }
        if (returnList.Count == 0 || ((counter % 3 == 2 && type == 1) || (counter % 3 != 2 && type == 2)))
        {
            if (counter % 3 != 0)
            {
                List<string> tempList = new List<string>();
                for (int z = 0; z < returnList.Count; z++)
                {
                    bool isGood = false;
                    if (counter % 3 == 1)
                    {
                        List<string> innerCenter = new List<string>(centers[18 + counter / 3]);
                        innerCenter.Add(returnList[z][1].ToString());
                        for (int i = 1; i < 7; i++)
                        {
                            if (!innerCenter.Contains(i.ToString()) && !centers[counter > 2 ? counter - 2 : 17].Contains(i.ToString()) &&
                                !centers[23 + counter / 3].Contains(i.ToString()))
                            {
                                returnList[z] += i.ToString();
                                isGood = true;
                                break;
                            }
                        }

                    }
                    else if(counter % 3 == 2)
                    {
                        List<string> innerCenter = new List<string>(centers[counter]);
                        innerCenter.Add(returnList[z][0].ToString());
                        innerCenter.Add(returnList[z][1].ToString());
                        for (int i = 1; i < 7; i++)
                        {
                            if (!innerCenter.Contains(i.ToString()) && !centers[18 + counter / 3].Contains(i.ToString()) &&
                                !centers[24 + counter / 3].Contains(i.ToString()))
                            {
                                List<string> tempList2 = new List<string>(numbers);
                                foreach (string str in centers[18 + counter / 3]) tempList2.Remove(str);
                                tempList2.Remove(i.ToString());
                                if (!centers[counter > 2 ? 23 + counter / 3 : 29].Contains(tempList2[0]) && 
                                    !centers[24 + counter / 3].Contains(tempList2[0]))
                                {
                                    for (int j = 1; j < 7; j++)
                                    {
                                        if (!centers[counter > 2 ? 23 + counter / 3 : 29].Contains(j.ToString()) &&
                                            !centers[24 + counter / 3].Contains(j.ToString())
                                            && j != i && j.ToString() != tempList2[0] && !centers[30].Contains(j.ToString()))
                                        {
                                            returnList[z] += i.ToString();
                                            returnList[z] += tempList2[0];
                                            returnList[z] += j.ToString();
                                            isGood = true;
                                            break;
                                        }
                                    }
                                }                                    
                            }
                        }
                    }
                    if (isGood)
                    {
                        tempList.Add(returnList[z]);
                    }
                }
                returnList = new List<string>(tempList);
            }
            return returnList;
        }
        else return FindCombinationsMaster(counter, returnList);
    }

    void ChooseCombinationMaster(int counter)
    {
        string chosenCombination = combinations[counter][Random.Range(0, combinations[counter].Count)];
        if (counter % 3 == 0)
        {
            highlighted1Numbers.Add(chosenCombination[0].ToString());
            centers[counter + 1].Add(chosenCombination[1].ToString());
            centers[18 + counter / 3].Add(chosenCombination[1].ToString());
            centers[counter + 1].Add(chosenCombination[2].ToString());
            for (int i = 0; i < 3; i++) centers[counter].Add(chosenCombination[i].ToString());
        }
        else if (counter % 3 == 1)
        {
            highlighted2Numbers.Add(chosenCombination[0].ToString());
            centers[counter + 1].Add(chosenCombination[1].ToString());
            centers[18 + counter / 3].Add(chosenCombination[1].ToString());
            centers[counter + 1].Add(chosenCombination[2].ToString());
            for (int i = 0; i < 3; i++) centers[counter].Add(chosenCombination[i].ToString());
            centers[counter > 2 ? 23 + counter / 3 : 29].Add(chosenCombination[3].ToString());
            centers[counter > 2 ? counter - 2 : 17].Add(chosenCombination[3].ToString());
            centers[18 + counter / 3].Add(chosenCombination[3].ToString());              
        }
        else
        {
            centers[(counter + 1) % 18].Add(chosenCombination[0].ToString());
            centers[counter < 17 ? 18 + counter / 3 + 1 : 18].Add(chosenCombination[0].ToString());
            centers[(counter + 1) % 18].Add(chosenCombination[1].ToString());
            for (int i = 0; i < 2; i++) centers[counter].Add(chosenCombination[i].ToString());
            centers[24 + counter / 3].Add(chosenCombination[2].ToString());
            centers[18 + counter / 3].Add(chosenCombination[2].ToString());
            centers[counter].Add(chosenCombination[2].ToString());
            centers[counter > 2 ? 23 + counter / 3 : 29].Add(chosenCombination[3].ToString());
            centers[24 + counter / 3].Add(chosenCombination[3].ToString());
            centers[18 + counter / 3].Add(chosenCombination[3].ToString());
            centers[counter > 2 ? 23 + counter / 3 : 29].Add(chosenCombination[4].ToString());
            centers[24 + counter / 3].Add(chosenCombination[4].ToString());
            centers[30].Add(chosenCombination[4].ToString());
        }
        combinations[counter].Remove(chosenCombination);
        chosenCombinations.Add(chosenCombination);
    }

    void DeleteChosenNumbersMaster(int counter)
    {
        chosenCombinations.RemoveAt(chosenCombinations.Count - 1);
        if (counter % 3 == 0)
        {
            highlighted1Numbers.RemoveAt(highlighted1Numbers.Count - 1);
            for (int i = 0; i < 2; i++) centers[counter + 1].RemoveAt(centers[counter + 1].Count - 1);
            centers[18 + counter / 3].RemoveAt(centers[18 + counter / 3].Count - 1);
            for (int i = 0; i < 3; i++) centers[counter].RemoveAt(centers[counter].Count - 1);
        }
        else if (counter % 3 == 1)
        {
            highlighted2Numbers.RemoveAt(highlighted2Numbers.Count - 1);
            for (int i = 0; i < 2; i++) centers[counter + 1].RemoveAt(centers[counter + 1].Count - 1);
            centers[18 + counter / 3].RemoveAt(centers[18 + counter / 3].Count - 1);
            for (int i = 0; i < 3; i++) centers[counter].RemoveAt(centers[counter].Count - 1);
            centers[counter > 2 ? 23 + counter / 3 : 29].RemoveAt(centers[counter > 2 ? 23 + counter / 3 : 29].Count - 1);
            centers[counter > 2 ? counter - 2 : 17].RemoveAt(centers[counter > 2 ? counter - 2 : 17].Count - 1);
            centers[18 + counter / 3].RemoveAt(centers[18 + counter / 3].Count - 1);
        }
        else
        {
            for (int i = 0; i < 2; i++) centers[(counter + 1) % 18].RemoveAt(centers[(counter + 1) % 18].Count - 1);
            centers[counter < 17 ? 18 + counter / 3 + 1 : 18].RemoveAt(centers[counter < 17 ? 18 + counter / 3 + 1 : 18].Count - 1);
            for (int i = 0; i < 2; i++) centers[counter].RemoveAt(centers[counter].Count - 1);
            for (int i = 0; i < 3; i++) centers[24 + counter / 3].RemoveAt(centers[24 + counter / 3].Count - 1);
            for (int i = 0; i < 2; i++) centers[18 + counter / 3].RemoveAt(centers[18 + counter / 3].Count - 1);
            centers[counter].RemoveAt(centers[counter].Count - 1);
            for (int i = 0; i < 2; i++) centers[counter > 2 ? 23 + counter / 3 : 29].RemoveAt(centers[counter > 2 ? 23 + counter / 3 : 29].Count - 1);
            centers[30].RemoveAt(centers[30].Count - 1);
        }
    }
}
