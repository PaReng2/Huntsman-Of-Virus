using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public int playerMoveSpeed = 5;

    void Update()
    {
        Move();
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float MoveY = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(moveX, 0, MoveY).normalized;
        transform.Translate(moveDir * playerMoveSpeed * Time.deltaTime);
    }
}
