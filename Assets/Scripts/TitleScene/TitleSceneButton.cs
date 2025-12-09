using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneButton : MonoBehaviour
{

   

    [SerializeField]
    private SceneController _sceneController;

    private TitleSceneManager manager;


    private void Awake()
    {

        manager = FindAnyObjectByType<TitleSceneManager>();

    }


    public void StartButton()
    {
        _sceneController.LoadScene(1);
        manager.buttonSound.Play();
    }

    public void OpenOptionTab()
    {
        manager.OpenPanel();
        manager.DisAbleObject();
        manager.buttonSound.Play();
    }

    public void CloseOptionTab()
    {
        manager.ClosePanel();
        manager.EnAbleObject();
        manager.closeButtonSound.Play();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}