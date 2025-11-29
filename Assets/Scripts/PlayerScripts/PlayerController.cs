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

    [Header("EXP")]
    public EXPController expController;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();
        expController = FindAnyObjectByType<EXPController>();
        // SO 기본 스탯
        ApplyStatusFromSO();

        

        curPlayerHp = playerMaxHP;
        runSpeed = playerMoveSpeed * 1.5f;

        if (augmentSystem == null)
            augmentSystem = FindObjectOfType<AugmentSystem>();

        expToNextLevel = GetRequiredExpForLevel(currentLevel);

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
        if (Mathf.Approximately(Time.timeScale, 0f))
        {
            anime.SetBool("walk", false);
            anime.SetBool("run", false);
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

        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // [변경 1] 움직임 여부에 따라 walk (bool) 설정
        bool isWalking = moveVec != Vector3.zero;
        anime.SetBool("walk", isWalking);

        // [변경 2] Shift 키 입력 시 RunSpeed(float) 대신 run(bool) 사용
        if (Input.GetKey(KeyCode.LeftShift) && isWalking)
        {
            playerMoveSpeed = runSpeed;
            anime.SetBool("run", true);
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                playerMoveSpeed = playerStatus.playerMoveSpeed;
            }

            // 키를 뗐거나 멈췄을 때 run 꺼짐
            anime.SetBool("run", false);
        }

        transform.position += moveVec * playerMoveSpeed * Time.deltaTime;
    }


    void TryRollDash()
    {
        // 공격 중이거나 무적 중이거나 이미 구르기 중이라면 대시 불가
        if (attackStep != 0 || isInvincible || isRolling) return;

        // 1. 대시 방향 결정: 움직임 입력이 있다면 그 방향, 없다면 플레이어가 바라보는 방향
        Vector3 dashDirection = moveVec.normalized;
        if (dashDirection == Vector3.zero)
        {
            dashDirection = transform.forward;
        }
        Instantiate(dashEffect, transform.position, gameObject.transform.rotation); //수정 필요
        
        // 2. 대시/구르기 코루틴 시작
        StartCoroutine(RollDashCoroutine(dashDirection));
    }

    IEnumerator RollDashCoroutine(Vector3 dashDirection)
    {
        // 1. 상태 설정
        isRolling = true;
        anime.SetBool("roll", true); 
        rb.velocity = Vector3.zero; // 구르기 시작 전 기존 속도 제거

        float startTime = Time.time;

        // 2. 구르기 이동 (지속적인 힘 적용)
        while (Time.time < startTime + rollDuration)
        {
            // 물리적인 충돌을 무시하지 않으려면 rb.MovePosition 대신 rb.velocity에 힘을 줄 수 있으나,
            // 빠른 대시를 위해 MovePosition을 사용하거나 AddForce를 사용해 원하는 속도를 유지합니다.
            rb.MovePosition(rb.position + dashDirection * rollSpeed * Time.deltaTime);
            yield return null;
        }

        // 3. 상태 초기화
        isRolling = false;
        anime.SetBool("roll", false); 
        rb.velocity = Vector3.zero; // 대시 후 속도 초기화 (미끄러짐 방지)
    }

    void Turn()
    {
        if (moveVec != Vector3.zero)
            transform.LookAt(transform.position + moveVec);

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

    private void Die()
    {
        Debug.Log("플레이어 사망");

        AchievementManager.instance?.UpdateProgress(AchievementType.totalDeaths, 1);


        StageManager.Instance.StopTimer();


        PlayerProgress.ResetAllProgress();

        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, Quaternion.AngleAxis(180f, Vector3.up));
            Destroy(effect, 5f);
        }
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Destroy(gameObject);
    }

    public void ApplyStatusFromSO()
    {
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
        Debug.Log($"[Player] Get EXP {amount}. ({currentExp}/{expToNextLevel})");

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
        UpdateExpUI();
        expController.SaveEXP(currentExp);
    }

    private void LevelUp()
    {
        GameObject levelUp = Instantiate(levelUpEffect, effectTransform);
        levelUp.transform.localScale = new Vector3(3, 3, 3);


        currentLevel++;
        expToNextLevel = GetRequiredExpForLevel(currentLevel);

        if (currentLevel >= 15)
        {
            // currentLevel이 15 이상일 때 실행 (가장 높은 레벨부터 확인)
            Instantiate(level_15_Effet, effectTransform);
        }
        else if (currentLevel >= 10)
        {
            // currentLevel이 10 이상이고 15 미만일 때 실행
            Instantiate(level_10_Effet, effectTransform);
        }
        else if (currentLevel >= 7)
        {
            // currentLevel이 7 이상이고 10 미만일 때 실행
            Instantiate(level_7_Effet, effectTransform);
        }
        else if (currentLevel >= 5)
        {
            // currentLevel이 5 이상이고 7 미만일 때 실행
            Instantiate(level_5_Effet, effectTransform);
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
        UpdateExpUI();
        expController.SaveLevel(currentLevel);

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