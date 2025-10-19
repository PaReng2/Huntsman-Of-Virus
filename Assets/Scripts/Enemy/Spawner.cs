using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;      // �� ������
    public float spawnInterval = 3f;    // �� ���� ����
    public float spawnRange = 5f;       // ���� ����
    public int maxSpawnCount = 5;       // �ִ� �� ��

    public float activationRange = 5f;

    private int currentSpawnCount = 0;  // ���� ������ �� ��
    private float timer = 0f;           // �ð� ������

    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;


        // StageManager�� �� �� �� ��� (�� ����)
        if (StageManager.Instance != null && StageManager.Instance.TotalEnemyCount == 0)
        {
            StageManager.Instance.SetTotalEnemyCount(maxSpawnCount);
        }
    }

    void Update()
    {
        // �÷��̾� �Ÿ� Ȯ��
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // �÷��̾ activationRange �̳��� ������ ���� ���� ����
        if (distance > activationRange)
        {
            timer = 0f;
            return;
        }

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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}
