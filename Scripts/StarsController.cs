using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarsController : MonoBehaviour
{
    void Start()
    {
        int stars = DataStorage.Stars;
        GetComponent<Text>().text = (stars < 10 ? "0" : "") + (stars < 100 ? "0" : "") + (stars < 1000 ? "0" : "")
            + Mathf.Clamp(stars, 0, 9999).ToString();
    }
}
