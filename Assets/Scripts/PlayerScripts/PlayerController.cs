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
    public float playerDashForce = 3f;
    public float attackDelay;
    public float attackRange;
    public float attackPower;
    private float runSpeed;
    public GameObject hitEffectPlayer;
    public GameObject deathEffect;
    public GameObject gameOverPanel;
    private bool isInvincible = false;
    public float invincibleDuration = 1f;   // 무적 시간 (1초)

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponentInChildren<Animator>();

        ApplyStatusFromSO();

        curPlayerHp = playerMaxHP;
        runSpeed = playerMoveSpeed * 1.5f;

        if (augmentSystem == null)
            augmentSystem = FindObjectOfType<AugmentSystem>();

        currentLevel = 1;
        currentExp = 0;
        expToNextLevel = GetRequiredExpForLevel(currentLevel);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetLevelAndExp();

        if (augmentSystem != null)
            augmentSystem.ResetAllLevelsForNewStage();

        ApplyStatusFromSO();
    }

    private void ResetLevelAndExp()
    {
        currentLevel = 1;
        currentExp = 0;
        expToNextLevel = GetRequiredExpForLevel(currentLevel);
        Debug.Log($"[Player] Scene changed. Level/EXP reset → Level {currentLevel}, EXP {currentExp}/{expToNextLevel}");
    }

    private int GetRequiredExpForLevel(int level)
    {
        return baseExpRequirement + (level - 1) * expIncreasePerLevel;
    }

    private void Start()
    {
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
        Dash();
        Turn();
        anime.SetBool("isGrounded", isGrounded);
    }

    void Move()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        if (hAxis != 0 || vAxis != 0)
            anime.SetBool("walk", true);
        else
            anime.SetBool("walk", false);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerMoveSpeed = runSpeed;
            anime.SetFloat("RunSpeed", runSpeed);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            playerMoveSpeed = playerStatus.playerMoveSpeed; 
        }

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        transform.position += moveVec * playerMoveSpeed * Time.deltaTime;

        anime.SetBool("walk", moveVec != Vector3.zero);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerMoveSpeed = runSpeed;
            Debug.Log("달리기");
            anime.SetBool("walk", false);
            anime.SetBool("run", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            anime.SetBool("run", false);
            playerMoveSpeed = playerStatus.playerMoveSpeed; 
        }
    }

    void Dash()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(moveVec * playerDashForce, ForceMode.Impulse);
            anime.SetTrigger("jump");
        }
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
            anime.ResetTrigger("jump");
            anime.SetBool("isGrounded", true);
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
        UpdateGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (playerGold >= amount)
        {
            playerGold -= amount;
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
        attackPower += amount;
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
            Color[] colors = new Color[rend.materials.Length];
            for (int i = 0; i < rend.materials.Length; i++)
                colors[i] = rend.materials[i].color;
            originalColors.Add(colors);
        }

        while (elapsed < invincibleDuration)
        {
            foreach (Renderer rend in renderers)
            {
                foreach (Material mat in rend.materials)
                    mat.color = Color.red;
            }

            yield return new WaitForSeconds(blinkInterval);

            for (int r = 0; r < renderers.Length; r++)
            {
                Renderer rend = renderers[r];
                for (int i = 0; i < rend.materials.Length; i++)
                    rend.materials[i].color = originalColors[r][i];
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval * 2;
        }

        for (int r = 0; r < renderers.Length; r++)
        {
            Renderer rend = renderers[r];
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
    }

    private void LevelUp()
    {
        GameObject levelUp = Instantiate(levelUpEffect, effectTransform);
        levelUp.transform.localScale = new Vector3(3, 3, 3);

        currentLevel++;
        expToNextLevel = GetRequiredExpForLevel(currentLevel);

        Debug.Log($"[Player] Level Up! → Level {currentLevel}, Next EXP : {expToNextLevel}, Current EXP : {currentExp}");

        if (augmentSystem != null)
        {
            bool opened = augmentSystem.TryOpenPanel();
            if (!opened)
            {
                Debug.Log("[Player] 레벨업 했지만 더 이상 강화 가능한 증강이 없음.");
            }
        }
    }
}