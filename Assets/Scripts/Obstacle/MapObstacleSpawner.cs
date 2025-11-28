using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject infectedCPU;
    public GameObject infectedGPU;
    public GameObject infectedMemory;

    private Spawner spawnerCS;

    private void Awake()
    {
        spawnerCS = FindAnyObjectByType<Spawner>();
    }

    // Spawner 스크립트에서 웨이브가 시작될 때 이 함수를 호출해줄 것입니다.
    public void CheckAndSpawnObstacle(int currentWaveIndex)
    {
        // 웨이브 인덱스는 0부터 시작하므로 +1을 해줍니다 (0 -> 1웨이브)
        int currentWaveNum = currentWaveIndex + 1;

        // 3의 배수 웨이브인지 확인 (3, 6, 9 ...)
        if (currentWaveNum % 3 == 0)
        {
            SpawnRandomObstacle();
        }
    }

    private void SpawnRandomObstacle()
    {
        // 1. 3개의 오브젝트 중 하나를 랜덤으로 선택
        GameObject targetPrefab = null;
        int randomIndex = Random.Range(0, 3); // 0, 1, 2 중 하나 반환

        switch (randomIndex)
        {
            case 0:
                targetPrefab = infectedCPU;
                break;
            case 1:
                targetPrefab = infectedGPU;
                break;
            case 2:
                targetPrefab = infectedMemory;
                break;
        }

        // 2. Spawner의 위치에 생성 (만약 Spawner 변수가 없다면 현재 위치에)
        Vector3 spawnPos = (spawnerCS != null) ? spawnerCS.transform.position : transform.position;

        if (targetPrefab != null)
        {
            Instantiate(targetPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[MapObstacle] 웨이브 돌입! {targetPrefab.name} 생성됨.");
        }
    }
}