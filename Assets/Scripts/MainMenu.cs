using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject selectLevelPanel;

    public Text nameLevelText;
    public Text infoLevelText;

    public Transform contentNewObject;

    public string currentLevelToLoad;

    public void PlayBtn()
    {
        selectLevelPanel.SetActive(true);
    }

    public void BtnLevelOnClick(InfoLevel level)
    {
        currentLevelToLoad = level.name;

        nameLevelText.text = level.nameLevel;
        infoLevelText.text = level.infoLevel;

        foreach (Transform item in contentNewObject)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in level.newObjects)
        {
            GameObject g = Instantiate(Resources.Load("Slot New Object") as GameObject, contentNewObject.transform);
        }
    }

    public void GoPlayBtn()
    {
        if (currentLevelToLoad != "")
        {
            SceneManager.LoadScene(currentLevelToLoad);
        }
    }
}
