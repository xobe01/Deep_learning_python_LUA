using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SerializableData {

    public enum TipState
    {
        Locked,
        Unlocked,
        InUse
    }

    //--v1
    public int version;
    public List<List<List<string>>> filledFields;
    public List<List<TipState>> tips;
    public List<string> freePlayFilledFields;
    public List<string> freePlayLevel;
    public string freePlaySolution;
    public TipState freePlayTipState;
    public int freePlayDifficulty;
    public int stars;
    public bool isMuted;
    public int freePlayTime;
    public bool nightMode;

    //--v2

    public List<bool> isTutorialCompleted;
}
