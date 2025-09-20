using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("player stat")]
    public float playerMoveSpeed;
    public float playerJumpForce = 3;
    public float attackDelay;
    public float attackRange;
    public float attackPower;

    [Header("player Data")]
    public PlayerStatusSO playerStatus;

    private Rigidbody rb;
    private bool isGrounded;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerMoveSpeed = playerStatus.playerMoveSpeed;
        attackDelay = playerStatus.playerAttackDelay;
        attackRange = playerStatus.playerAttackRange;
        attackPower = playerStatus.playerAttackPower;
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float MoveY = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerMoveSpeed *= 1.5f;
        }
        

        Vector3 moveDir = new Vector3(moveX, 0, MoveY);
        transform.Translate(moveDir * playerMoveSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if(isGrounded && Input.GetKeyDown(KeyCode.Space))
            rb.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
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
