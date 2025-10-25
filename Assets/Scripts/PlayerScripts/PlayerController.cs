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
        playerMoveSpeed = playerStatus.playerMoveSpeed;
        attackDelay = playerStatus.playerAttackRate;
        attackRange = playerStatus.playerAttackRange;
        attackPower = playerStatus.playerAttackPower;
        curPlayerHp = playerStatus.playerHP;
        playerMaxHP = playerStatus.playerHP;
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
            isGrounded = true;
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
        curPlayerHp -= damage;
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

}