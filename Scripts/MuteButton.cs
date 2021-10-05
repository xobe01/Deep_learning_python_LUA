using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : ClickButton
{
    [SerializeField] Sprite mutedSprite;
    [SerializeField] Sprite unmutedSprite;
    [SerializeField] Sprite mutedSpriteNight;
    [SerializeField] Sprite unmutedSpriteNight;

    Image image;
    NightModeAdapter nightModeAdapter;

    bool isMuted;

    private void Start()
    {
        nightModeAdapter = GetComponent<NightModeAdapter>();
        isMuted = DataStorage.IsMuted;
        image = GetComponent<Image>();
        if (isMuted) image.sprite = mutedSprite;
        audioCont = FindObjectOfType<AudioController>();
        CheckNightMode(true);
    }

    protected override void TaskOnClick()
    {
        base.TaskOnClick();
        DataStorage.IsMuted = !isMuted;
        isMuted = !isMuted;
        audioCont.SetIsMuted(isMuted);
        image.sprite = DataStorage.NightMode ? (isMuted ? mutedSpriteNight : unmutedSpriteNight) : (isMuted ? mutedSprite : unmutedSprite);
        CheckNightMode(false);
    }

    void CheckNightMode(bool isInitial)
    {
        if (nightModeAdapter != null) nightModeAdapter.SetTexture((!DataStorage.NightMode || isInitial) ? (isMuted ? mutedSpriteNight : unmutedSpriteNight) :
             (isMuted ? mutedSprite : unmutedSprite), isInitial);
    }
}
