using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangerButton : ClickButton
{
    [SerializeField] string goalScene;

    protected override void TaskOnClick()
    {
        base.TaskOnClick();
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(goalScene);
    }

    public void SetGoalScene(string sceneName)
    {
        goalScene = sceneName;
    }
}
