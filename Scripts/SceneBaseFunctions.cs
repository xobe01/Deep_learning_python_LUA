using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBaseFunctions : MonoBehaviour
{
    NightModeAdapter[] nightModeAdapters;
    Camera camera;
    bool isNightMode;

    private void Awake()
    {
        nightModeAdapters = FindObjectsOfType<NightModeAdapter>();
        camera = FindObjectOfType<Camera>();
    }

    private void Start()
    {
        GameObject.Find("Banner").GetComponent<BannerController>().RequestBanner();
        if (DataStorage.NightMode)
        {
            isNightMode = true;
            camera.backgroundColor = Color.black;
        }
    }

    public void ChangeMode()
    {
        isNightMode = !isNightMode;
        foreach (NightModeAdapter n in nightModeAdapters) n.ChangeTexture(isNightMode);
        camera.backgroundColor = isNightMode ? Color.black : Color.white;
        DataStorage.NightMode = isNightMode;
        DataStorage.Save();
    }
}
