using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public GameObject portalPrefab;   // Æ÷Å» ÇÁ¸®ÆÕ
    private int totalEnemyCount = 0;  // ÃÑ ½ºÆùµÉ Àû ¼ö
    private int killedEnemyCount = 0; // Ã³Ä¡µÈ Àû ¼ö
    private bool portalSpawned = false;

    public int TotalEnemyCount;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetTotalEnemyCount(int count)
    {
        totalEnemyCount = count;
        killedEnemyCount = 0;
        portalSpawned = false;
    }

    public void OnEnemySpawned()
    {

    }

    public void OnEnemyKilled()
    {
        killedEnemyCount++;

        if (!portalSpawned && killedEnemyCount >= totalEnemyCount)
        {
            OpenPortal();
        }
    }

    private void OpenPortal()
    {
        portalSpawned = true;
        Instantiate(portalPrefab, new Vector3(10, 1, 1), Quaternion.identity);
    }
}
