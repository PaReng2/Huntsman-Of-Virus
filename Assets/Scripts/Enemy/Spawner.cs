using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 인스펙터에서 설정할 웨이브 목록
    public Wave[] waves;

    public float timeBetweenWaves; // 웨이브 사이의 대기 시간
    public float activationRange ; // 스포너 활성화 범위
    public float spawnRange;       // 스폰 범위

    private Transform player;
    private int currentWaveIndex = 0;         // 현재 진행 중인 웨이브 인덱스
    private int totalEnemiesSpawnedSoFar = 0; // 지금까지 스폰된 모든 적의 총 수
    private bool isSpawningWave = false;      // 현재 웨이브가 스폰 중인지 여부
    private bool wavesStarted = false;        // 웨이브 시퀀스가 시작되었는지 여부
    private StageManager stageManager;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        stageManager = FindAnyObjectByType<StageManager>();

        // StageManager에 모든 웨이브의 총 적 수를 계산하여 등록
        int totalEnemiesInStage = 0;
        foreach (Wave wave in waves)
        {
            totalEnemiesInStage += wave.count;
        }

        if (StageManager.Instance != null)
        {
            StageManager.Instance.SetTotalEnemyCount(totalEnemiesInStage);
        }
        stageManager.curWaveNum = currentWaveIndex;
    }

    void Update()
    {
        if (player == null) return;
        if (wavesStarted && !isSpawningWave) // 웨이브가 시작되었고, 현재 스폰 중이 아닐 때
        {
            // 현재까지 스폰된 모든 적이 처치되었는지 확인
            int currentKills = StageManager.Instance.GetKilledEnemyCount();

            if (currentKills == totalEnemiesSpawnedSoFar)
            {
                // 아직 다음 웨이브가 남아있다면
                if (currentWaveIndex < waves.Length)
                {
                    // 다음 웨이브 시작
                    StartCoroutine(SpawnNextWave());
                }
                // (모든 웨이브가 끝났다면 Spawner는 할 일을 마칩니다)
            }
        }
        else if (!wavesStarted) // 웨이브가 아직 시작되지 않았다면
        {
            // 플레이어가 활성화 범위 안에 들어왔는지 확인
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= activationRange)
            {
                // 첫 번째 웨이브 시작
                wavesStarted = true;
                StartCoroutine(SpawnNextWave());
            }
        }
    }

    IEnumerator SpawnNextWave()
    {
        isSpawningWave = true;
        Wave wave = waves[currentWaveIndex];

        Debug.Log((currentWaveIndex + 1) + "번째 웨이브 시작!");

        // 첫 번째 웨이브가 아니라면 웨이브 사이 대기 시간 적용
        if (currentWaveIndex > 0)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
        }

        // 현재 웨이브의 적 스폰
        for (int i = 0; i < wave.count; i++)
        {
            // 스폰 위치 계산
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0f,
                Random.Range(-spawnRange, spawnRange)
            );

            // 적 생성
            Instantiate(wave.enemyPrefab, spawnPos, Quaternion.identity);

            // 스테이지의 총 스폰 수 카운트 증가
            totalEnemiesSpawnedSoFar++;

            // 다음 적 스폰까지 대기
            //yield return new WaitForSeconds(wave.rate);
        }

        currentWaveIndex++; // 다음 웨이브 인덱스로
        stageManager.curWaveNum = currentWaveIndex;
        isSpawningWave = false;
        Debug.Log("웨이브 스폰 완료. 적 처치 대기 중...");
    }


    // (기존 OnDrawGizmosSelected 코드는 디버깅에 유용하므로 그대로 둡니다)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange * 2, 0.1f, spawnRange * 2));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}