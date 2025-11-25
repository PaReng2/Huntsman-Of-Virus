using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolTip : MonoBehaviour
{
    public GameObject moveKey;
    public GameObject attackKey;
    public GameObject toolTipActive;
    public bool isTutorial;
    public bool tooltipActive;
    private int curSceneNum;
    private void Awake()
    {
        Scene curScene = SceneManager.GetActiveScene();

        curSceneNum = curScene.buildIndex;
    }

    private void Update()
    {
        if (curSceneNum == 1)
        {
            isTutorial = true;
        }
        else
        {
            isTutorial= false;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.K))
        {
            if (!tooltipActive)
            {
                moveKey.SetActive(true);
                attackKey.SetActive(true);
                toolTipActive.SetActive(true);
                tooltipActive = true;
            }
            else
            {
                moveKey.SetActive(false);
                attackKey.SetActive(false);
                toolTipActive.SetActive(false);
                tooltipActive = false;
            }
                
        }
        
    }
}
