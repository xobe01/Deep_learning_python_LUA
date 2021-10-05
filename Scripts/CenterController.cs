using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CenterController : MonoBehaviour
{
    [SerializeField] FieldController[] fields;
    RawImage image;
    MapController mapCont;

    string[] currentNumbers;

    private void Awake()
    {
        image = GetComponentInChildren<RawImage>();
        currentNumbers = new string[6];
    }

    public void SetNumbers(string numbers, int fillableFieldsCount, MapController mapCont)
    {
        this.mapCont = mapCont;
        int counter = fillableFieldsCount;
        for (int i = 0; i < numbers.Length; i++)
        {
            bool isFillable = numbers[i].ToString() != ConvertToNumber(numbers[i].ToString()) || numbers[i] == '0';
            fields[i].SetNumber(numbers[i].ToString(), isFillable ? counter : -1);
            if (isFillable)
            {
                mapCont.AddFillableField(fields[i]);
                counter++;
            }
        }
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
                return "1";
            case "h":
                return "2";
            case "i":
                return "3";
            case "j":
                return "4";
            case "k":
                return "5";
            case "l":
                return "6";
        }
        return number;
    }

    public void GainFieldNumbers()
    {
        for (int i = 0; i < fields.Length; i++) currentNumbers[i] = fields[i].CurrentNumber;
    }

    public void SetFieldNumber(FieldController field, string number)
    {
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i] == field)
            {
                currentNumbers[i] = number;
            }
        }
    }

    public bool IsCompleted()
    {
        bool isGood = true;
        for (int i = 0; i < currentNumbers.Length; i++)
        {
            for (int j = 0; j < currentNumbers.Length; j++)
            {
                if (i != j && currentNumbers[i] == currentNumbers[j] && currentNumbers[i] != "0")
                {
                    isGood = false;
                    fields[i].SetIsGood(false);
                    fields[j].SetIsGood(false);
                }
                if (currentNumbers[i] == "0") isGood = false;
            }
        }
        image.color = isGood ? Color.green : Color.red;
        return isGood;
    }

    public IEnumerator temp()
    {
        for (int i = 0; i < fields.Length; i++)
        {
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(fields[i].temp());
        }
    }
}
