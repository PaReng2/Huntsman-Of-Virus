using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;


    private int totalEnemyCount = 0;
    private int killedEnemyCount = 0;
    private bool portalSpawned = false;
    private Spawner spawner;
    public TextMeshProUGUI curWaveText;
    public int curWaveNum;

    public float totalTime = 51f; // 총 시간
    public TextMeshProUGUI timerText; // 시간 표시할 텍스트 UI
    private float currentTime;
    private Coroutine timerCoroutine;

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
    void Start()
    {
        currentTime = totalTime;
        timerCoroutine = StartCoroutine(CountdownTimer());
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
            // (현재는 사용X, 필요하면 스테이지 전체 클리어 용도로 사용)
        }
    }

    // WaveSpawner가 현재 처치된 적 수를 확인할 수 있도록 Getter를 추가합니다.
    public int GetKilledEnemyCount()
    {
        return killedEnemyCount;
    }

    public void OnWaveCleared(int clearedWaveIndex, bool isLastWave)
    {
        if (isLastWave)
        {
            // 모든 웨이브 클리어 - 웨이브 진행도 & 골드 초기화
            WaveProgress.Reset();
            PlayerProgress.ResetAllProgress();

            Debug.Log($"모든 웨이브 클리어! (마지막: Wave {clearedWaveIndex + 1})");
        }
        else
        {
            // 마지막으로 클리어한 웨이브 저장
            WaveProgress.lastClearedWaveIndex = clearedWaveIndex;
            Debug.Log($"Wave {clearedWaveIndex + 1} 클리어! " +
                      $"다음 입장 시 Wave {clearedWaveIndex + 2}부터 시작");
        }
        AchievementManager.instance.UpdateProgress(AchievementType.ClearWaves, 1);

        // 웨이브 하나가 끝날 때마다 Shop 씬으로 이동
        StartCoroutine(LoadShopAfterDelay());
    }

    public void OnPlayerDied()
    {
        // 플레이어 사망 → 웨이브 정보 초기화
        WaveProgress.Reset();
        Debug.Log("플레이어 사망! 웨이브 정보를 초기화합니다.");

        // 이 아래에서 GameOver 씬 이동 등은 원하는 대로 구현
        // 예: SceneManager.LoadScene("GameOver");
    }

    IEnumerator CountdownTimer()
    {
        while (currentTime > 0)
        {
            currentTime -= 1f; // 1초 감소
            UpdateTimerUI();
            yield return new WaitForSeconds(1f); // 1초 대기
        }

        // 시간이 다 되었을 때 실행할 내용 (예: 게임오버 처리)
        Debug.Log("타이머 종료!");
        SceneManager.LoadScene("Main");
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = currentTime.ToString("0"); // 소수점 두 자리까지 표시
        }

    }

    private IEnumerator LoadShopAfterDelay()
    {
        SceneFade fade = FindAnyObjectByType<SceneFade>();

        if (fade != null)
        {
            yield return StartCoroutine(fade.FadeOutCoroutine(3f));
        }
        else
        {
            Debug.LogWarning("SceneFade 객체를 찾을 수 없습니다. 즉시 씬 이동합니다.");
        }

        SceneManager.LoadScene("Shop");
    }
}