using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("player stat")]
    public int playerHP;
    public float playerMoveSpeed;
    public float playerJumpForce = 3f;
    public float attackDelay;
    public float attackRange;
    public float attackPower;
    private float runSpeed;
    

    [Header("player Data")]
    public PlayerStatusSO playerStatus;

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
        anime = GetComponentInChildren<Animator>();
        playerMoveSpeed = playerStatus.playerMoveSpeed;
        attackDelay = playerStatus.playerAttackRate;
        attackRange = playerStatus.playerAttackRange;
        attackPower = playerStatus.playerAttackPower;
        playerHP = playerStatus.playerHP;
        runSpeed = playerMoveSpeed * 1.5f;
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

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        transform.position += moveVec * playerMoveSpeed * Time.deltaTime;

        anime.SetBool("walk", moveVec != Vector3.zero);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
            playerMoveSpeed = runSpeed;
            Debug.Log("´Þ¸®±â");
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
        {
            isGrounded = false;
        }
    }

}
