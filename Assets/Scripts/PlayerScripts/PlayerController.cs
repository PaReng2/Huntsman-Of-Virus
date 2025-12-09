using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("player stat")]
    public int curPlayerHp;
    public int playerMaxHP;
    public float playerMoveSpeed;
    public float playerJumpforce = 5f;
    public float attackDelay;
    public float attackRange;
    public float attackPower;
    private float runSpeed;
    public GameObject hitEffectPlayer;
    public GameObject deathEffect;
    public GameObject gameOverPanel;
    public TextMeshProUGUI playerPointWave;
    private bool isInvincible = false;
    public float invincibleDuration = 1f;   // 무적 시간 (1초)
    private int attackStep = 0; // 0: Idle, 1: Attack1, 2: Attack2

    [Header("Roll Dash")]
    public float rollSpeed = 15f; // 구르기 이동 속도
    public float rollDuration = 0.3f; // 구르기 지속 시간 (애니메이션 길이에 맞춰 조정)
    private bool isRolling = false; // 구르기 상태 플래그
    public GameObject dashEffect;

    [Header("player Data (ScriptableObject)")]
    public PlayerStatusSO playerStatus;

    [Header("Gold / Attack UI")]
    public int playerGold = 0;
    public TMP_Text goldText;

    [Header("Level / EXP")]
    public GameObject levelUpEffect;
    public Transform effectTransform;
    public int currentLevel = 1;
    public int currentExp = 0;
    public int expToNextLevel = 50;
    [SerializeField] private int baseExpRequirement = 50;
    [SerializeField] private int expIncreasePerLevel = 20;
    public Slider expSlider;
    public TMP_Text expText;
    public GameObject level_5_Effet;
    public GameObject level_7_Effet;
    public GameObject level_10_Effet;
    public GameObject level_15_Effet;

    [Header("Augment System")]
    [SerializeField] private AugmentSystem augmentSystem;

    [Header("Augment Base Stats")]
    [SerializeField] private float baseMaxHP;
    [SerializeField] private float baseAttackPower;
    [SerializeField] private float baseMoveSpeed;
    [SerializeField] private float baseAttackDelay;
    [SerializeField] private float baseAttackRange;

    private Rigidbody rb;
    private bool isGrounded;
    private Animator anime;

    [Header("Camera")]
    float hAxis;
    float vAxis;
    Vector3 moveVec;
    public Camera followCamera;

    [Header("GoldErea")]
    public float goldErea;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
        // SO 기본 스탯
        ApplyStatusFromSO();

        // 저장된 레벨/경험치 불러오기
        currentLevel = Mathf.Max(1, PlayerProgress.savedLevel);
        currentExp = Mathf.Max(0, PlayerProgress.savedExp);

        expToNextLevel = GetRequiredExpForLevel(currentLevel);

        // 기존 동작 유지
        curPlayerHp = playerMaxHP;
        runSpeed = playerMoveSpeed * 1.5f;

        if (augmentSystem == null)
            augmentSystem = FindObjectOfType<AugmentSystem>();

        playerGold = PlayerProgress.savedGold;

        ReapplyPurchasedItems();
    }

    private int GetRequiredExpForLevel(int level)
    {
        return baseExpRequirement + (level - 1) * expIncreasePerLevel;
    }

    private void Start()
    {
        UpdateExpUI();
        UpdateGoldUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            playerGold += 40;
        }

        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            if (anime != null)
            {
                anime.SetBool("walk", false);
                anime.SetBool("run", false);
            }
            return;
        }

        if (augmentSystem != null)
        {
            ApplyAugments();
        }

        Move();
        Turn();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryRollDash();
        }
    }

    void Move()
    {
        if (isRolling) return;

        // raw 입력
        hAxis = Input.GetAxisRaw("Horizontal"); 
        vAxis = Input.GetAxisRaw("Vertical");   

        if (followCamera != null)
        {
            Vector3 camForward = followCamera.transform.forward;
            Vector3 camRight = followCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 desired = camForward * vAxis + camRight * hAxis;

            moveVec = desired.sqrMagnitude > 1e-6f ? desired.normalized : Vector3.zero;
        }
        else
        {
            moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        }

        // 걷기 애니메이션
        bool isWalking = moveVec != Vector3.zero;
        if (anime != null)
            anime.SetBool("walk", isWalking);

        // 달리기 (Shift)
        if (Input.GetKey(KeyCode.LeftShift) && isWalking)
        {
            playerMoveSpeed = runSpeed;
            if (anime != null) anime.SetBool("run", true);
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                if (playerStatus != null)
                    playerMoveSpeed = playerStatus.playerMoveSpeed;
            }
            if (anime != null) anime.SetBool("run", false);
        }

        // 이동 적용 (transform 이동 유지)
        transform.position += moveVec * playerMoveSpeed * Time.deltaTime;
    }

    void TryRollDash()
    {
        GameManager manager = FindAnyObjectByType<GameManager>();
         if (manager.isInteracting) return;

        // 대시 방향 결정 (moveVec를 기반으로 대시. moveVec이 0이면 플레이어 전방)
        Vector3 dashDirection = moveVec.normalized;
        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward;
        }

        if (dashEffect != null)
            Instantiate(dashEffect, transform.position, gameObject.transform.rotation);

        StartCoroutine(RollDashCoroutine(dashDirection));
    }

    IEnumerator RollDashCoroutine(Vector3 dashDirection)
    {
        isRolling = true;
        if (anime != null) anime.SetBool("roll", true);
        if (rb != null) rb.velocity = Vector3.zero;

        float startTime = Time.time;

        while (Time.time < startTime + rollDuration)
        {
            if (rb != null)
                rb.MovePosition(rb.position + dashDirection * rollSpeed * Time.deltaTime);
            else
                transform.position += dashDirection * rollSpeed * Time.deltaTime;

            yield return null;
        }

        isRolling = false;
        if (anime != null) anime.SetBool("roll", false);
        if (rb != null) rb.velocity = Vector3.zero;
    }

    void Turn()
    {
        if (moveVec != Vector3.zero)
            transform.LookAt(transform.position + moveVec);

        if (followCamera == null) return;

        Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 100))
        {
            Vector3 nextVec = rayHit.point;
            nextVec.y = transform.position.y;
            transform.LookAt(nextVec);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    public void AddGold(int amount)
    {
        playerGold += amount;
        PlayerProgress.savedGold = playerGold;
        UpdateGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (playerGold >= amount)
        {
            playerGold -= amount;
            PlayerProgress.savedGold = playerGold;
            UpdateGoldUI();
            return true;
        }
        return false;
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = "Gold : " + playerGold;
    }

    public void Heal(int amount)
    {
        curPlayerHp += amount;
        if (curPlayerHp > playerMaxHP)
            curPlayerHp = playerMaxHP;
    }

    public void AddAttack(int amount)
    {
        baseAttackPower += amount;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        curPlayerHp -= damage;

        if (hitEffectPlayer != null)
        {
            GameObject effect = Instantiate(hitEffectPlayer, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        StartCoroutine(InvincibleCoroutine());

        if (curPlayerHp <= 0)
        {
            curPlayerHp = 0;
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("플레이어 사망");

        AchievementManager.instance?.UpdateProgress(AchievementType.totalDeaths, 1);
        StageManager.Instance.StopTimer();
        

        PlayerProgress.ResetAllProgress();

        StageManager sm = FindAnyObjectByType<StageManager>();
        sm.OnPlayerDied();

        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.AngleAxis(180f, Vector3.up));
            Destroy(effect, 5f);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            playerPointWave.text = $"Player Point \n Wave : {sm.curWaveNum} \n Level : {currentLevel}";
        }
        Destroy(gameObject);
    }

    public void ApplyStatusFromSO()
    {
        if (playerStatus == null) return;

        baseMoveSpeed = playerStatus.playerMoveSpeed;
        baseAttackDelay = playerStatus.playerAttackRate;
        baseAttackRange = playerStatus.playerAttackRange;
        baseAttackPower = playerStatus.playerAttackPower;
        baseMaxHP = playerStatus.playerHP;

        playerMoveSpeed = baseMoveSpeed;
        attackDelay = baseAttackDelay;
        attackRange = baseAttackRange;
        attackPower = baseAttackPower;
        playerMaxHP = Mathf.RoundToInt(baseMaxHP);

        if (curPlayerHp > playerMaxHP || curPlayerHp <= 0)
            curPlayerHp = playerMaxHP;

        runSpeed = playerMoveSpeed * 1.5f;
    }

    private void ApplyAugments()
    {
        int maxHpLevel = augmentSystem.GetAugmentLevel(AugmentSystem.AugmentType.MAXHP);
        int atkLevel = augmentSystem.GetAugmentLevel(AugmentSystem.AugmentType.ATK);
        int spdLevel = augmentSystem.GetAugmentLevel(AugmentSystem.AugmentType.SPD);
        int atkSpdLevel = augmentSystem.GetAugmentLevel(AugmentSystem.AugmentType.ATKSPD);
        int rangeLevel = augmentSystem.GetAugmentLevel(AugmentSystem.AugmentType.RANGE);

        float maxHpMult = 1f + 0.10f * maxHpLevel;

        int prevMaxHp = playerMaxHP > 0 ? playerMaxHP : Mathf.RoundToInt(baseMaxHP);

        int newMaxHp = Mathf.RoundToInt(baseMaxHP * maxHpMult);
        if (newMaxHp < 1) newMaxHp = 1;

        int diff = newMaxHp - prevMaxHp;
        if (diff > 0)
        {
            curPlayerHp += diff;
        }

        playerMaxHP = newMaxHp;
        if (curPlayerHp > playerMaxHP)
            curPlayerHp = playerMaxHP;
        if (curPlayerHp < 1)
            curPlayerHp = 1;

        float atkMult = 1f + 0.10f * atkLevel;
        attackPower = baseAttackPower * atkMult;

        float spdMult = 1f + 0.10f * spdLevel;
        playerMoveSpeed = baseMoveSpeed * spdMult;
        runSpeed = playerMoveSpeed * 1.5f;

        float rangeMult = 1f + 0.05f * rangeLevel;
        attackRange = baseAttackRange * rangeMult;

        float delayMult = 1f - 0.05f * atkSpdLevel;
        delayMult = Mathf.Clamp(delayMult, 0.1f, 10f);
        attackDelay = baseAttackDelay * delayMult;
    }

    public void ApplyKnockback(Vector3 hitDirection, float force)
    {
        if (rb != null)
        {
            hitDirection.y = 0.4f;
            rb.velocity = Vector3.zero;
            rb.AddForce(hitDirection.normalized * force, ForceMode.Impulse);
        }
    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float blinkInterval = 0.1f;
        float elapsed = 0f;

        List<Color[]> originalColors = new List<Color[]>();
        foreach (Renderer rend in renderers)
        {
            if (rend == null) continue;

            Color[] colors = new Color[rend.materials.Length];
            for (int i = 0; i < rend.materials.Length; i++)
                colors[i] = rend.materials[i].color;
            originalColors.Add(colors);
        }

        while (elapsed < invincibleDuration)
        {
            foreach (Renderer rend in renderers)
            {
                if (rend == null) continue;

                foreach (Material mat in rend.materials)
                    mat.color = Color.red;
            }

            yield return new WaitForSeconds(blinkInterval);

            for (int r = 0; r < renderers.Length; r++)
            {
                Renderer rend = renderers[r];
                if (rend == null) continue;
                for (int i = 0; i < rend.materials.Length; i++)
                    rend.materials[i].color = originalColors[r][i];
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval * 2;
        }

        for (int r = 0; r < renderers.Length; r++)
        {
            Renderer rend = renderers[r];
            if (rend == null) continue;
            for (int i = 0; i < rend.materials.Length; i++)
                rend.materials[i].color = originalColors[r][i];
        }

        isInvincible = false;
    }

    public void AddExperience(int amount)
    {
        currentExp += amount;
        PlayerProgress.savedExp = currentExp;

        Debug.Log($"[Player] Get EXP {amount}. ({currentExp}/{expToNextLevel})");

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }

        PlayerProgress.savedExp = currentExp;
        UpdateExpUI();
    }

    private void LevelUp()
    {
        // 레벨업 이펙트
        if (levelUpEffect != null && effectTransform != null)
        {
            GameObject levelUp = Instantiate(levelUpEffect, effectTransform);
            levelUp.transform.localScale = new Vector3(3, 3, 3);
        }

        // 레벨 증가 & 저장 동기화
        currentLevel++;
        PlayerProgress.savedLevel = currentLevel;

        // 다음 레벨 필요 EXP 재계산
        expToNextLevel = GetRequiredExpForLevel(currentLevel);

        // 현재 EXP 저장
        PlayerProgress.savedExp = currentExp;

        // 레벨별 이펙트
        if (currentLevel >= 15)
        {
            if (level_15_Effet != null && effectTransform != null) Instantiate(level_15_Effet, effectTransform);
        }
        else if (currentLevel >= 10)
        {
            if (level_10_Effet != null && effectTransform != null) Instantiate(level_10_Effet, effectTransform);
        }
        else if (currentLevel >= 7)
        {
            if (level_7_Effet != null && effectTransform != null) Instantiate(level_7_Effet, effectTransform);
        }
        else if (currentLevel >= 5)
        {
            if (level_5_Effet != null && effectTransform != null) Instantiate(level_5_Effet, effectTransform);
        }

        Debug.Log($"[Player] Level Up! → Level {currentLevel}, Next EXP : {expToNextLevel}, Current EXP : {currentExp}");

        if (augmentSystem != null)
        {
            bool opened = augmentSystem.TryOpenPanel();
            if (!opened)
            {
                Debug.Log("[Player] 레벨업 했지만 더 이상 강화 가능한 증강이 없음.");
            }
        }

        MapObstacleSpawner.Instance?.CheckAndSpawnObstacle(currentLevel);

        UpdateExpUI();
    }

    public void UpdateExpUI()
    {
        if (expSlider != null)
        {
            expSlider.maxValue = expToNextLevel;
            expSlider.value = currentExp;
        }

        if (expText != null)
        {
            expText.text = $"{currentExp} / {expToNextLevel}";
        }
    }

    private void ReapplyPurchasedItems()
    {
        if (PlayerProgress.purchasedItems == null)
        {
            PlayerProgress.purchasedItems = new List<ShopItem>();
            return;
        }

        foreach (var item in PlayerProgress.purchasedItems)
        {
            if (item != null)
            {
                item.ApplyEffect(this);
            }
        }
    }

    public void GrantInvincibilityOnShieldBreak()
    {
        if (isInvincible) return;

        StartCoroutine(InvincibleCoroutine());
    }
}