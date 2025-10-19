using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;      // 적 프리팹
    public float spawnInterval = 3f;    // 적 생성 간격
    public float spawnRange = 5f;       // 스폰 범위
    public int maxSpawnCount = 5;       // 최대 적 수

    public float activationRange = 5f;

    private int currentSpawnCount = 0;  // 현재 스폰된 적 수
    private float timer = 0f;           // 시간 누적용

    private Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;


        // StageManager에 총 적 수 등록 (한 번만)
        if (StageManager.Instance != null && StageManager.Instance.TotalEnemyCount == 0)
        {
            StageManager.Instance.SetTotalEnemyCount(maxSpawnCount);
        }
    }

    void Update()
    {
        // 플레이어 거리 확인
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // 플레이어가 activationRange 이내로 들어왔을 때만 스폰 시작
        if (distance > activationRange)
        {
            timer = 0f;
            return;
        }

        if (currentSpawnCount >= maxSpawnCount) return; // 5마리 넘으면 멈춤


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
