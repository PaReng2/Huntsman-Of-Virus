using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public GameObject portalPrefab;

    private int totalEnemyCount = 0; // ���� ������ �� ��
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
    // ���� ������ ������ ȣ��
    public void OnEnemySpawned()
    {
        totalEnemyCount++;
    }

    // ���� ���� ������ ȣ��
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

    // �������� �ʱ�ȭ��
    public void Reset()
    {
        totalEnemyCount = 0;
        deadEnemyCount = 0;
    }
}
