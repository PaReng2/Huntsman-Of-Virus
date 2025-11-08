using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    
    private int totalEnemyCount = 0;
    private int killedEnemyCount = 0;
    private bool portalSpawned = false;
    private Spawner spawner;
    public TextMeshProUGUI curWaveText;
    public int curWaveNum;

    //기존 Spawner 스크립트의 Start()에서 이 변수를 사용하므로 남겨둠
    // WaveSpawner에서는 사용X
    public int TotalEnemyCount;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        spawner = FindAnyObjectByType<Spawner>();
        
    }

    private void Update()
    {
        curWaveText.text = $"Wave {curWaveNum}";
    }

    public void SetTotalEnemyCount(int count)
    {
        totalEnemyCount = count;
        killedEnemyCount = 0;
        portalSpawned = false;
        Debug.Log("이 스테이지의 총 적 수: " + totalEnemyCount);
    }

    public void OnEnemySpawned()
    {
        // (필요시 사용)
    }

    public void OnEnemyKilled()
    {
        killedEnemyCount++;
        Debug.Log("적 처치: " + killedEnemyCount + " / " + totalEnemyCount);

        // 모든 웨이브의 모든 적을 다 처치했는지 확인
        if (!portalSpawned && killedEnemyCount >= totalEnemyCount)
        {
            
        }
    }

    // WaveSpawner가 현재 처치된 적 수를 확인할 수 있도록 Getter를 추가합니다.
    public int GetKilledEnemyCount()
    {
        return killedEnemyCount;
    }

    
}