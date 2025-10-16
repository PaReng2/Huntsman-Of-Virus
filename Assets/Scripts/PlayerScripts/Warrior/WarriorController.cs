using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MonoBehaviour
{
    [Header("Player Stat")]
    public int playerHP;
    public float playerMoveSpeed;
    public float playerJumpForce = 3f;
    public float attackDelay;
    public float attackRange;
    public float attackPower;
    private float runSpeed;

    [Header("Player Data")]
    public PlayerStatusSO playerStatus;

    private Rigidbody rb;
    private bool isGrounded;

    [Header("Camera")]
    public Camera followCamera;
    public float cameraFollowSpeed = 5f;
    public Vector3 cameraOffset = new Vector3(0, 10, -8);

    float hAxis;
    float vAxis;
    Vector3 moveVec;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        CameraFollow();
    }

    void Move()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        Vector3 camForward = followCamera.transform.forward;
        Vector3 camRight = followCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        moveVec = (camForward * vAxis + camRight * hAxis).normalized;

        float curSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : playerMoveSpeed;

        rb.MovePosition(rb.position + moveVec * curSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            rb.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
    }

    void Turn()
    {
        Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 100))
        {
            Vector3 nextVec = rayHit.point;
            nextVec.y = transform.position.y;
            transform.LookAt(nextVec);
        }
        else if (moveVec != Vector3.zero)
        {
            transform.LookAt(transform.position + moveVec);
        }
    }

    void CameraFollow()
    {
        if (followCamera == null)
            return;

        Vector3 targetPos = transform.position + cameraOffset;
        followCamera.transform.position = Vector3.Lerp(followCamera.transform.position, targetPos, Time.deltaTime * cameraFollowSpeed);
        followCamera.transform.LookAt(transform.position + Vector3.up * 2f);
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
}