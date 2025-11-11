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
    }

    private void Start()
    {
        UpdateGoldUI();
    }

    void Update()
    {
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
            Instantiate(hitEffectPlayer, transform.position, Quaternion.identity);
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
}