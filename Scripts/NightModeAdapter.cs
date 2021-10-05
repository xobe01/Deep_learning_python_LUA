using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightModeAdapter : MonoBehaviour
{
    [SerializeField] Sprite nightModeTexture;

    Sprite originalTexture;
    Image image;

    bool muteButtonSignalArrived;

    void Awake()
    {
        image = GetComponent<Image>();
        if(image != null) originalTexture = image.sprite;
    }

    private void Start()
    {
        if (DataStorage.NightMode)
        {
            if (GetComponent<MuteButton>() == null) ChangeTexture(true);
            else StartCoroutine(WaitForSignal());
        }
    }

    IEnumerator WaitForSignal()
    {
        while (!muteButtonSignalArrived) yield return null;
        //ChangeTexture(true);
    }

    public void ChangeTexture(bool toNightMode)
    {
        if (nightModeTexture == null)
        {
            if (image != null) image.color = Color.black;
            else GetComponent<Text>().color = new Color(0, 0.4f, 1);
        }
        else image.sprite = toNightMode ? nightModeTexture : originalTexture;
        if (GetComponent<Shadow>() != null) GetComponent<Shadow>().effectColor = toNightMode ? new Color(0, 0.2f, 0.5f)
                : new Color(.7f, .4f, 0);
        if (GetComponent<Outline>() != null) GetComponent<Outline>().effectColor = Color.white;

    }

    public void SetTexture(Sprite newTexture, bool isInitial)
    {
        if (!DataStorage.NightMode || (DataStorage.NightMode && isInitial))
        {
            nightModeTexture = newTexture;
            originalTexture = image.sprite;
            if (DataStorage.NightMode) ChangeTexture(true);
        }
        else
        {
            nightModeTexture = image.sprite;
            originalTexture = newTexture;
            ChangeTexture(true);
        }
        muteButtonSignalArrived = true;
    }
}
