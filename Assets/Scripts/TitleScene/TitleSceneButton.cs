using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneButton : MonoBehaviour
{
    public GameObject optionPanel;

    private void Awake()
    {
        optionPanel.SetActive(false);
    }
    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OptionTab()
    {
        optionPanel.SetActive(true);
    }
}
