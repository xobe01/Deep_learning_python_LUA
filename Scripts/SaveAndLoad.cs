using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class SaveAndLoad : MonoBehaviour
{
    public static void SaveData(SerializableData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/data.fun";
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SerializableData LoadData(bool isSerialized)
    {
        string path = Application.persistentDataPath + "/data.fun";
        if (File.Exists(path) && isSerialized)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SerializableData data = formatter.Deserialize(stream) as SerializableData;
            VersionControl(data, data.version);
            stream.Close();
            return data;
        }
        else
        {
            return GiveInitialValues();
        }        
    }

    static void VersionControl(SerializableData data, int currentVersion)
    {
        if (currentVersion < 2) 
        {
            data.isTutorialCompleted = new List<bool>();
            for (int i = 0; i < 4; i++) data.isTutorialCompleted.Add(false);
        }
        data.version = 2;
    }

    static SerializableData GiveInitialValues()
    {
        SerializableData data = new SerializableData();

        //--v1
        data.filledFields = new List<List<List<string>>>();
        data.tips = new List<List<SerializableData.TipState>>();
        for (int i = 0; i < 4; i++)
        {
            data.filledFields.Add(new List<List<string>>());
            data.filledFields[i].Add(new List<string>());
            data.tips.Add(new List<SerializableData.TipState>());
            data.tips[i].Add(SerializableData.TipState.Locked);
        }
        data.freePlayLevel = new List<string>();
        data.freePlaySolution = "";
        data.freePlayFilledFields = new List<string>();
        data.freePlayTipState = SerializableData.TipState.Locked;
        VersionControl(data, 0);
        return data;
    }
}
