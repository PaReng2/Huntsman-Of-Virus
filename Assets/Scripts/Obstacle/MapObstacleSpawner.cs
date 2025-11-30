using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapObstacleSpawner : MonoBehaviour
{
    public static MapObstacleSpawner Instance;

    [Header("Obstacle Prefabs (index: 0=CPU,1=GPU,2=Memory)")]
    public GameObject infectedCPU;
    public GameObject infectedGPU;
    public GameObject infectedMemory;

    [Header("Spawn area (centered at spawner or referenced spawner if exists)")]
    public Vector3 spawnArea = new Vector3(5f, 0f, 5f);

    // 외부에서 제어할 활성화 플래그 (Spawner가 사용)
    [Tooltip("외부에서 웨이브 시작/종료 시 활성화 상태를 제어할 수 있습니다.")]
    public bool isActive = true;

    // 내부참조
    private Spawner spawnerCS;
    private PlayerController pc;

    // 현재 씬에서 인스턴스화된 장애물들(씬 떠날 때 파괴하기 위함)
    private List<GameObject> currentSceneObstacles = new List<GameObject>();

    // 현재 활성 씬 이름 캐시
    private string currentSceneName;

    // 현재 저장된 플레이어 레벨(체크용)
    private int curLevel;

    private void Awake()
    {
        // 싱글톤 안정화
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 초기 참조 (씬이 로드되면 Start/OnSceneLoaded에서 갱신)
        spawnerCS = FindObjectOfType<Spawner>();
        pc = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        // PlayerProgress에 저장된 레벨로 초기화
        curLevel = PlayerProgress.savedLevel;
        currentSceneName = SceneManager.GetActiveScene().name;

        // 씬에 저장된 장애물이 있으면 복원
        RestoreObstaclesForScene(currentSceneName);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void CheckAndSpawnObstacle(int currentPlayerLevel)
    {
        if (!isActive) return; // isActive가 false면 동작하지 않음

        if (currentPlayerLevel <= curLevel) return;

        // 레벨이 증가한 만큼 반복(만약 멀티레벨 업 한 번에 여러 레벨 올렸다면 그 차이만큼 생성)
        while (curLevel < currentPlayerLevel)
        {
            SpawnRandomObstacle();
            curLevel++;
            PlayerProgress.savedLevel = curLevel; // 동기화
        }
    }

    private void SpawnRandomObstacle()
    {
        int prefabIndex = UnityEngine.Random.Range(0, 3); 
        GameObject prefab = GetPrefabByIndex(prefabIndex);
        if (prefab == null)
        {
            Debug.LogWarning("[MapObstacleSpawner] prefab null for index " + prefabIndex);
            return;
        }

        if (spawnerCS == null) spawnerCS = FindObjectOfType<Spawner>();
        Vector3 origin = (spawnerCS != null) ? spawnerCS.transform.position : transform.position;

        float rx = UnityEngine.Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f);
        float ry = UnityEngine.Random.Range(-spawnArea.y / 2f, spawnArea.y / 2f);
        float rz = UnityEngine.Random.Range(-spawnArea.z / 2f, spawnArea.z / 2f);
        Vector3 spawnPos = origin + new Vector3(rx, ry, rz);

        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
        currentSceneObstacles.Add(go);

        string sceneName = SceneManager.GetActiveScene().name;
        PlayerProgress.AddObstacleRecord(sceneName, new PlayerProgress.ObstacleRecord(prefabIndex, spawnPos, Quaternion.identity));

        Debug.Log($"[MapObstacleSpawner] Spawned obstacle {prefab.name} at {spawnPos} in scene {sceneName}");
    }

    private GameObject GetPrefabByIndex(int idx)
    {
        switch (idx)
        {
            case 0: return infectedCPU;
            case 1: return infectedGPU;
            case 2: return infectedMemory;
            default: return null;
        }
    }

    // 씬 로드 콜백
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 이전 씬에서 만든 객체들 정리
        ClearCurrentSceneObstacles();

        currentSceneName = scene.name;

        // 씬에 저장된 레코드로 복원
        RestoreObstaclesForScene(currentSceneName);

        // 참조 갱신
        spawnerCS = FindObjectOfType<Spawner>();
        pc = FindObjectOfType<PlayerController>();
    }

    // PlayerProgress에 저장된 레코드로 해당 씬의 장애물 복원
    private void RestoreObstaclesForScene(string sceneName)
    {
        var list = PlayerProgress.GetObstacleRecords(sceneName);
        if (list == null || list.Count == 0) return;

        for (int i = 0; i < list.Count; i++)
        {
            var rec = list[i];
            GameObject prefab = GetPrefabByIndex(rec.prefabIndex);
            if (prefab == null) continue;

            GameObject go = Instantiate(prefab, rec.position, rec.rotation);
            currentSceneObstacles.Add(go);
        }

        Debug.Log($"[MapObstacleSpawner] Restored {list.Count} obstacles for scene {sceneName}");
    }

    // 현재 씬에서 인스턴스화한 장애물들 모두 파괴
    private void ClearCurrentSceneObstacles()
    {
        for (int i = 0; i < currentSceneObstacles.Count; i++)
        {
            if (currentSceneObstacles[i] != null)
                Destroy(currentSceneObstacles[i]);
        }
        currentSceneObstacles.Clear();
    }

    public void ClearStoredObstaclesForScene(string sceneName)
    {
        PlayerProgress.ClearObstacleRecordsForScene(sceneName);
    }

    public void ClearAllStoredObstacles()
    {
        PlayerProgress.ClearAllObstacleRecords();
        ClearCurrentSceneObstacles();
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnerCS != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnerCS.transform.position, spawnArea);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, spawnArea);
        }
    }
}