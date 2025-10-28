using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public GameObject portalPrefab;
    private int totalEnemyCount = 0;
    private int killedEnemyCount = 0;
    private bool portalSpawned = false;

    //���� Spawner ��ũ��Ʈ�� Start()���� �� ������ ����ϹǷ� ���ܵ�
    // WaveSpawner������ ���X
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
        portalPrefab.SetActive(false);
    }

    public void SetTotalEnemyCount(int count)
    {
        totalEnemyCount = count;
        killedEnemyCount = 0;
        portalSpawned = false;
        Debug.Log("�� ���������� �� �� ��: " + totalEnemyCount);
    }

    public void OnEnemySpawned()
    {
        // (�ʿ�� ���)
    }

    public void OnEnemyKilled()
    {
        killedEnemyCount++;
        Debug.Log("�� óġ: " + killedEnemyCount + " / " + totalEnemyCount);

        // ��� ���̺��� ��� ���� �� óġ�ߴ��� Ȯ��
        if (!portalSpawned && killedEnemyCount >= totalEnemyCount)
        {
            OpenPortal();
        }
    }

    // WaveSpawner�� ���� óġ�� �� ���� Ȯ���� �� �ֵ��� Getter�� �߰��մϴ�.
    public int GetKilledEnemyCount()
    {
        return killedEnemyCount;
    }

    private void OpenPortal()
    {
        portalSpawned = true;
        portalPrefab.SetActive(true);
        Debug.Log("��Ż�� ���Ƚ��ϴ�!");
    }
}