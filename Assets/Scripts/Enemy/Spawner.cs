using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemy;                // 스폰할 적 프리팹
    public Transform spawnPoint;            // 스폰 위치
    public float spawnInterval = 3f;        // 스폰 주기 (초 단위)
    public int maxSpawnCount = 5;           // 최대 소환 개수

    private int currentSpawnCount = 0;      // 지금까지 소환된 개수
    private bool interacted = false;        // 이미 상호작용 했는지 체크

    void Update()
    {
        // 스페이스 키를 눌렀을 때 (한 번만 실행되도록 조건 추가)
        if (!interacted && Input.GetKeyDown(KeyCode.Space))
        {
            OnInteract();
            interacted = true; // 중복 실행 방지
        }
    }
    public void OnInteract()
    {

        // 일정 간격으로 반복 소환 시작
        InvokeRepeating(nameof(SpawnItem), 0f, spawnInterval);
    }

    void SpawnItem()
    {
        if (currentSpawnCount < maxSpawnCount)
        {
            Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            currentSpawnCount++;
        }
        else
        {
            // 5마리 이상이면 소환 중지
            CancelInvoke(nameof(SpawnItem));
        }
    }
}