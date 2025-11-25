using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    public GameObject optionPanel;

    public GameObject title;
    public GameObject buttonLayout;

    private void Start()
    {
        optionPanel.SetActive(false);
    }

    public void DisAbleObject()
    {
        title.SetActive(false);
        buttonLayout.SetActive(false);
    }
    public void EnAbleObject()
    {
        title.SetActive(true);
        buttonLayout.SetActive(true);
    }

    public void OpenPanel()
    {
        optionPanel.SetActive(true);
    }
    public void ClosePanel()
    {
        optionPanel.SetActive(false);
    }
}
