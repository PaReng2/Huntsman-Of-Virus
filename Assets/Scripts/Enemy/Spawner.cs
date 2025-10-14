using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;      // �� ������
    public float spawnInterval = 3f;    // �� ���� ����
    public float spawnRange = 5f;       // ���� ����
    public int maxSpawnCount = 5;       // �ִ� �� ��

    private int currentSpawnCount = 0;  // ���� ������ �� ��
    private float timer = 0f;           // �ð� ������

    void Start()
    {
        // StageManager�� �� �� �� ��� (�� ����)
        if (StageManager.Instance != null && StageManager.Instance.TotalEnemyCount == 0)
        {
            StageManager.Instance.SetTotalEnemyCount(maxSpawnCount);
        }
    }

    void Update()
    {
        if (currentSpawnCount >= maxSpawnCount) return; // 5���� ������ ����

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0f,
                Random.Range(-spawnRange, spawnRange)
            );

            Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            currentSpawnCount++;
            timer = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange * 2, 0.1f, spawnRange * 2));
    }
}
