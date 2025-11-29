using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 인스펙터에서 설정할 웨이브 목록
    public Wave[] waves;

    [Header("Wave Settings")]
    public float timeBetweenWaves; // 웨이브 사이의 대기 시간
    public float activationRange; // 스포너 활성화 범위
    public float spawnRange;       // 스폰 범위

    [Header("Batch Settings")]
    [Tooltip("한 번에 끊어서 스폰할 몬스터 수")]
    public int spawnBatchCount = 5;
    [Tooltip("끊어서 스폰할 때, 다음 그룹 스폰까지 대기 시간")]
    public float spawnBatchInterval = 2.0f;

    private Transform player;
    private int currentWaveIndex = 0;             // 현재 진행 중인 웨이브 인덱스
    private int totalEnemiesSpawnedSoFar = 0; // 지금까지 스폰된 모든 적의 총 수
    private bool isSpawningWave = false;      // 현재 웨이브가 스폰 중인지 여부
    private bool wavesStarted = false;        // 웨이브 시퀀스가 시작되었는지 여부
    private StageManager stageManager;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        stageManager = FindAnyObjectByType<StageManager>();

        // WaveProgress에 저장된 진행도에 따라 시작 웨이브 결정
        if (WaveProgress.lastClearedWaveIndex < 0)
        {
            currentWaveIndex = 0;
        }
        else
        {
            currentWaveIndex = WaveProgress.lastClearedWaveIndex + 1;
        }

        if (currentWaveIndex >= waves.Length)
        {
            currentWaveIndex = 0;
        }

        if (stageManager != null)
        {
            stageManager.curWaveNum = currentWaveIndex + 1;
        }
    }

    void Update()
    {
        if (player == null) return;

        if (wavesStarted && !isSpawningWave)
        {
            int currentKills = StageManager.Instance.GetKilledEnemyCount();

            if (currentKills == totalEnemiesSpawnedSoFar && totalEnemiesSpawnedSoFar > 0)
            {
                int clearedWaveIndex = currentWaveIndex - 1;
                bool isLastWave = (clearedWaveIndex >= waves.Length - 1);

                Debug.Log("웨이브 하나 종료! 상점으로 이동 준비");

                wavesStarted = false;

                if (StageManager.Instance != null)
                {
                    StageManager.Instance.OnWaveCleared(clearedWaveIndex, isLastWave);
                }
            }
        }
        else if (!wavesStarted)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= activationRange)
            {
                wavesStarted = true;
                StartCoroutine(SpawnNextWave());
            }
        }
    }

    IEnumerator SpawnNextWave()
    {
        isSpawningWave = true;
        Wave wave = waves[currentWaveIndex];

        GameObject chase = wave.enemyPrefab;
        GameObject staticEnemy = wave.staticEnemyPrefab;

        // UI 업데이트
        if (stageManager != null)
        {
            stageManager.curWaveNum = currentWaveIndex + 1;
        }

        Debug.Log((currentWaveIndex + 1) + "번째 웨이브 시작!");

        // ================= [장애물 스폰 로직 추가됨] =================
        // 맵 장애물 스포너를 찾아 현재 웨이브 정보를 넘겨줍니다.
        // (매번 찾는게 무거우면 Start에서 미리 찾아 변수에 담아두는 것을 권장합니다)
        MapObstacleSpawner obstacleSpawner = FindAnyObjectByType<MapObstacleSpawner>();
        if (obstacleSpawner != null)
        {
            obstacleSpawner.CheckAndSpawnObstacle(currentWaveIndex);
        }
        // ==========================================================

        int totalCount = wave.count;

        int staticCount = totalCount / 2;
        int chaseCount = totalCount - staticCount;

        List<GameObject> enemiesToSpawn = new List<GameObject>();

        for (int i = 0; i < staticCount; i++)
        {
            enemiesToSpawn.Add(staticEnemy);
        }

        for (int i = 0; i < chaseCount; i++)
        {
            enemiesToSpawn.Add(chase);
        }

        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            GameObject temp = enemiesToSpawn[i];
            int randomIndex = Random.Range(i, enemiesToSpawn.Count);
            enemiesToSpawn[i] = enemiesToSpawn[randomIndex];
            enemiesToSpawn[randomIndex] = temp;
        }

        // 첫 번째 웨이브가 아니라면 웨이브 사이 대기 시간 적용
        if (currentWaveIndex > 0)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        // 현재 웨이브의 적 스폰
        for (int i = 0; i < wave.count; i++)
        {
            GameObject enemyToSpawn = enemiesToSpawn[i];

            // 스폰 위치 계산
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0f,
                Random.Range(-spawnRange, spawnRange)
            );

            // 적 생성
            Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
            // 스테이지의 총 스폰 수 카운트 증가
            totalEnemiesSpawnedSoFar++;

            // ================= [끊어서 스폰하기 로직] =================
            // 설정한 마리 수(spawnBatchCount)만큼 스폰했다면 잠시 대기
            // 단, 마지막 몬스터 스폰 후에는 대기하지 않음
            if ((i + 1) % spawnBatchCount == 0 && (i + 1) < wave.count)
            {
                yield return new WaitForSeconds(spawnBatchInterval);
            }
            // =======================================================
        }

        // 방금 웨이브 스폰이 끝났으므로, 다음에 스폰할 웨이브로 인덱스 증가
        currentWaveIndex++;

        isSpawningWave = false;
        Debug.Log("웨이브 스폰 완료. 적 처치 대기 중...");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange * 2, 0.1f, spawnRange * 2));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }

    private void OpenAugment()
    {

    }
}