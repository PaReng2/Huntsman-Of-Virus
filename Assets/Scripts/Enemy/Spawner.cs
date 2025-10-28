using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // �ν����Ϳ��� ������ ���̺� ���
    public Wave[] waves;

    public float timeBetweenWaves = 5f; // ���̺� ������ ��� �ð�
    public float activationRange = 15f; // ������ Ȱ��ȭ ����
    public float spawnRange = 5f;       // ���� ����

    private Transform player;
    private int currentWaveIndex = 0;         // ���� ���� ���� ���̺� �ε���
    private int totalEnemiesSpawnedSoFar = 0; // ���ݱ��� ������ ��� ���� �� ��
    private bool isSpawningWave = false;      // ���� ���̺갡 ���� ������ ����
    private bool wavesStarted = false;        // ���̺� �������� ���۵Ǿ����� ����

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        // StageManager�� ��� ���̺��� �� �� ���� ����Ͽ� ���
        int totalEnemiesInStage = 0;
        foreach (Wave wave in waves)
        {
            totalEnemiesInStage += wave.count;
        }

        if (StageManager.Instance != null)
        {
            StageManager.Instance.SetTotalEnemyCount(totalEnemiesInStage);
        }
    }

    void Update()
    {
        if (player == null) return;
        if (wavesStarted && !isSpawningWave) // ���̺갡 ���۵Ǿ���, ���� ���� ���� �ƴ� ��
        {
            // ������� ������ ��� ���� óġ�Ǿ����� Ȯ��
            int currentKills = StageManager.Instance.GetKilledEnemyCount();

            if (currentKills == totalEnemiesSpawnedSoFar)
            {
                // ���� ���� ���̺갡 �����ִٸ�
                if (currentWaveIndex < waves.Length)
                {
                    // ���� ���̺� ����
                    StartCoroutine(SpawnNextWave());
                }
                // (��� ���̺갡 �����ٸ� Spawner�� �� ���� ��Ĩ�ϴ�)
            }
        }
        else if (!wavesStarted) // ���̺갡 ���� ���۵��� �ʾҴٸ�
        {
            // �÷��̾ Ȱ��ȭ ���� �ȿ� ���Դ��� Ȯ��
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= activationRange)
            {
                // ù ��° ���̺� ����
                wavesStarted = true;
                StartCoroutine(SpawnNextWave());
            }
        }
    }

    IEnumerator SpawnNextWave()
    {
        isSpawningWave = true;
        Wave wave = waves[currentWaveIndex];

        Debug.Log((currentWaveIndex + 1) + "��° ���̺� ����!");

        // ù ��° ���̺갡 �ƴ϶�� ���̺� ���� ��� �ð� ����
        if (currentWaveIndex > 0)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        // ���� ���̺��� �� ����
        for (int i = 0; i < wave.count; i++)
        {
            // ���� ��ġ ���
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0f,
                Random.Range(-spawnRange, spawnRange)
            );

            // �� ����
            Instantiate(wave.enemyPrefab, spawnPos, Quaternion.identity);

            // ���������� �� ���� �� ī��Ʈ ����
            totalEnemiesSpawnedSoFar++;

            // ���� �� �������� ���
            yield return new WaitForSeconds(wave.rate);
        }

        currentWaveIndex++; // ���� ���̺� �ε�����
        isSpawningWave = false;
        Debug.Log("���̺� ���� �Ϸ�. �� óġ ��� ��...");
    }


    // (���� OnDrawGizmosSelected �ڵ�� ����뿡 �����ϹǷ� �״�� �Ӵϴ�)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange * 2, 0.1f, spawnRange * 2));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}