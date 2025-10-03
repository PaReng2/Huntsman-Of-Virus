using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public GameObject portalPrefab;

    private int totalEnemyCount = 0; // 실제 스폰된 적 수
    private int deadEnemyCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void SetTotalEnemyCount(int count)
    {
        totalEnemyCount = count;
        deadEnemyCount = 0;
    }
    // 적이 스폰될 때마다 호출
    public void OnEnemySpawned()
    {
        totalEnemyCount++;
    }

    // 적이 죽을 때마다 호출
    public void OnEnemyKilled()
    {
        deadEnemyCount++;

        if (deadEnemyCount >= totalEnemyCount && totalEnemyCount > 0)
        {
            OpenPortal();
        }
    }

    private void OpenPortal()
    {
        Instantiate(portalPrefab, new Vector3(10, 1, 1), Quaternion.identity);    }

    // 스테이지 초기화용
    public void Reset()
    {
        totalEnemyCount = 0;
        deadEnemyCount = 0;
    }
}
