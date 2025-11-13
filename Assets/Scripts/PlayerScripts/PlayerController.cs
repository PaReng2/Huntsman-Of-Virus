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
    public float playerJumpForce = 3f;
    public float attackDelay;
    public float attackRange;
    public float attackPower;
    private float runSpeed;
    public GameObject hitEffectPlayer;
    private bool isInvincible = false;
    public float invincibleDuration = 1f;   // 무적 시간 (1초)

    [Header("player Data (ScriptableObject)")]
    public PlayerStatusSO playerStatus;

    [Header("Gold / Attack UI")]
    public int playerGold = 0;
    public TMP_Text goldText;

    [Header("Level / EXP")]
    public int currentLevel = 1;           
    public int currentExp = 0;             
    public int expToNextLevel = 50;        
    [SerializeField] private int baseExpRequirement = 50;     
    [SerializeField] private int expIncreasePerLevel = 20;    

    [Header("Augment System")]
    [SerializeField] private AugmentSystem augmentSystem;     

    private Rigidbody rb;
    private bool isGrounded;
    private Animator anime;
    private bool isRunning;

    [Header("Camera")]
    float hAxis;
    float vAxis;
    Vector3 moveVec;
    public Camera followCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();

        anime = GetComponentInChildren<Animator>();
        ApplyStatusFromSO();      // 스탯 적용 함수 호출
        curPlayerHp = playerMaxHP;
        runSpeed = playerMoveSpeed * 1.5f;

        // AugmentSystem 자동 연결 (없으면 Find)
        if (augmentSystem == null)
            augmentSystem = FindObjectOfType<AugmentSystem>();

        // 레벨/경험치 초기값 설정
        currentLevel = 1;
        currentExp = 0;
        expToNextLevel = GetRequiredExpForLevel(currentLevel);
    }

    private void OnEnable()
    {
        // 씬이 바뀔 때마다 레벨/경험치 초기화
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 플레이어가 씬을 넘어갈 때마다 레벨과 경험치 초기화
        ResetLevelAndExp();

        // 스테이지 단위로 증강 레벨도 초기화
        if (augmentSystem != null)
            augmentSystem.ResetAllLevelsForNewStage();
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
        // 레벨 1 → 2 : 50
        // 레벨 2 → 3 : 70
        // 레벨 3 → 4 : 90 ...
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

        Move();
        Jump();
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
            isRunning = true;
            playerMoveSpeed = runSpeed;
            Debug.Log("달리기");
            anime.SetBool("walk", false);
            anime.SetBool("run", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
            anime.SetBool("run", false);
            playerMoveSpeed = 5;
        }
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
            anime.SetTrigger("jump");
            isGrounded = false;
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
        SceneManager.LoadScene("Main");
        Debug.Log("플레이어 사망");
    }

    public void ApplyStatusFromSO()
    {
        playerMoveSpeed = playerStatus.playerMoveSpeed;
        attackDelay = playerStatus.playerAttackRate;
        attackRange = playerStatus.playerAttackRange;
        attackPower = playerStatus.playerAttackPower;
        playerMaxHP = playerStatus.playerHP;

        if (curPlayerHp > playerMaxHP)
            curPlayerHp = playerMaxHP;
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

        // 모든 렌더러 가져오기 (자식 포함)
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float blinkInterval = 0.1f;
        float elapsed = 0f;

        // 원래 색상 저장용
        List<Color[]> originalColors = new List<Color[]>();
        foreach (Renderer rend in renderers)
        {
            Color[] colors = new Color[rend.materials.Length];
            for (int i = 0; i < rend.materials.Length; i++)
                colors[i] = rend.materials[i].color;
            originalColors.Add(colors);
        }

        // 깜빡임 루프
        while (elapsed < invincibleDuration)
        {
            // 빨갛게
            foreach (Renderer rend in renderers)
            {
                foreach (Material mat in rend.materials)
                    mat.color = Color.red;
            }

            yield return new WaitForSeconds(blinkInterval);

            // 원래 색상 복원
            for (int r = 0; r < renderers.Length; r++)
            {
                Renderer rend = renderers[r];
                for (int i = 0; i < rend.materials.Length; i++)
                    rend.materials[i].color = originalColors[r][i];
            }

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval * 2;
        }

        // 무적 끝  최종적으로 원래 색상 복원
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

        // 여러 레벨이 한 번에 오를 수도 있으니 while 사용
        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;
        expToNextLevel = GetRequiredExpForLevel(currentLevel);

        Debug.Log($"[Player] Level Up! → Level {currentLevel}, Next EXP : {expToNextLevel}, Current EXP : {currentExp}");

        // 레벨업 시 증강 패널 오픈
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