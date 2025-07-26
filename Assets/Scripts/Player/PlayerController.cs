using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float moveSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        float moveH = Input.GetAxisRaw("Horizontal"); 
        float moveV = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(moveH, 0f, moveV).normalized;

        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }
}
