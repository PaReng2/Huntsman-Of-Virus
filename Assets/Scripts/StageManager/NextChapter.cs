using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextChapter : MonoBehaviour
{
    [Header("�Ѿ é�� �ۼ� // 1 - Ʃ�丮��, 2 - é��1, 3 - ����")]
    public int chapterNum;      
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(chapterNum);
        }
    }
}
