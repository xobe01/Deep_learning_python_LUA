using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonController : MonoBehaviour
{
    [SerializeField] string sceneName;

    bool isPressed;
    bool isReady;

    private void Start()
    {
        isReady = true;
    }

    void Update()
    {
        isPressed = Input.GetKey(KeyCode.Escape);
        if (isPressed && isReady)
        {
            isReady = false;
            if (sceneName == "")
            {
                if (FindObjectOfType<GameController>() != null) FindObjectOfType<GameController>().BackButtonPressed();
            }
            else SceneManager.LoadScene(sceneName);
        }
        else if (!isPressed) isReady = true;
    }
}
