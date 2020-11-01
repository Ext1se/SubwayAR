using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeveloperMode : MonoBehaviour
{
    private List<GameObject> UiList = new List<GameObject>();
    bool developMode = true;
    private void Start()
    {
        SceneManager.sceneLoaded += RememberObjects;
        SceneManager.activeSceneChanged += ActivateAgain;
    }

    public void ChangeMode()
    {
        developMode = !developMode;
        foreach(GameObject ui in UiList)
        {
            ui.SetActive(!ui.activeSelf);
        }
    }

    public void ActivateAgain(Scene active, Scene newScene)
    {
        if (developMode)
        {
            ChangeMode();
        }
    }

    public void RememberObjects(Scene scene, LoadSceneMode mode)
    {
        developMode = true;
        UiList.Clear();
        UiList.AddRange(GameObject.FindGameObjectsWithTag("Develop"));
    }
}
