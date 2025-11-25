using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneButton : MonoBehaviour
{
    string titleTag;
    public GameObject optionPanel;
    bool isSetting;
    [SerializeField]
    private SceneController _sceneController;

    private TitleSceneManager manager;


    private void Start()
    {
        titleTag = gameObject.tag;
    }

    private void Awake()
    {
        optionPanel.SetActive(false);
        isSetting = false;
        manager = FindAnyObjectByType<TitleSceneManager>();

    }

    public void StartButton()
    {
        _sceneController.LoadScene(1);
    }

    public void OptionTab()
    {
        optionPanel.SetActive(true);
        isSetting = true;
        manager.DisAbleObject();
    }

    public void CloseOptionTab()
    {
        optionPanel.SetActive(false);
        isSetting = false;
        manager.EnAbleObject();
    }


}