using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextChapter : MonoBehaviour
{
    [Header("넘어갈 챕터 작성 // 1 - 튜토리얼, 2 - 챕터1, 3 - 상점")]
    public int chapterNum;      
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(chapterNum);
        }
    }
}
