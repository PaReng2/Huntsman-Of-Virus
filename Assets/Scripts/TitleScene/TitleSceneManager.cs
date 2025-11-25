using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    public GameObject Title;
    public GameObject StartButton;
    public GameObject SettingButton;

    public void DisAbleObject()
    {
        Title.SetActive(false);
        StartButton.SetActive(false);
        SettingButton.SetActive(false);
    }
    public void EnAbleObject()
    {
        Title.SetActive(true);
        StartButton.SetActive(true);
        SettingButton.SetActive(true);
    }
}
