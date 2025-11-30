using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObstacleSpawner : MonoBehaviour
{
    public static MapObstacleSpawner Instance;

    [Header("Obstacle Prefabs")]
    public GameObject infectedCPU;
    public GameObject infectedGPU;
    public GameObject infectedMemory;
    public bool isActive;

    private Spawner spawnerCS;
    private PlayerController pc;
    private static int curLevel;
    Vector3 origin;

    public Vector3 spawnArea = new Vector3(5f, 0f, 5f);
    private void Awake()
    {
        Instance = this;
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        spawnerCS = FindAnyObjectByType<Spawner>();
        pc = FindAnyObjectByType<PlayerController>();
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        curLevel = pc.currentLevel;
        isActive = true;
    }
    // Spawner 스크립트에서 웨이브가 시작될 때 이 함수를 호출해줄 것입니다.
    public void CheckAndSpawnObstacle(int currentPlayerLevel)
    {
        // 웨이브 인덱스는 0부터 시작하므로 +1을 해줍니다 (0 -> 1웨이브)
        int currentLevel = currentPlayerLevel;

        if (curLevel < currentLevel)
        {
            SpawnRandomObstacle();
            curLevel++;
            
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
        if (spawnerCS != null)
        {
            origin = spawnerCS.transform.position;

        }


        // 범위 내 랜덤 오프셋 계산 (-범위/2 ~ +범위/2)
        float randomX = Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
        float randomY = Random.Range(-spawnArea.y / 2, spawnArea.y / 2);
        float randomZ = Random.Range(-spawnArea.z / 2, spawnArea.z / 2);

        Vector3 randomOffset = new Vector3(randomX, randomY, randomZ);

        // 최종 스폰 위치
        Vector3 spawnPos = origin + randomOffset;

        // 3. 생성
        Instantiate(targetPrefab, spawnPos, Quaternion.identity);
        Debug.Log($"레벨업! {targetPrefab.name} 생성됨. 위치: {spawnPos}");
    }
    private void OnDrawGizmosSelected()
    {
        if (spawnerCS != null)
        {
            Gizmos.color = Color.green;
            // spawnerCS 위치를 중심으로 spawnArea 크기의 박스를 그립니다.
            Gizmos.DrawWireCube(spawnerCS.transform.position, spawnArea);
        }
    }
}