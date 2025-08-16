using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("player stat")]
    public int playerMoveSpeed = 5;
    public int playerJumpForce = 5;

    private Rigidbody rb;
    private bool isGrounded;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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
