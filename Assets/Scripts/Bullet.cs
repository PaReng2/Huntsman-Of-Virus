using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float DestroyDelay = 5f;

    private Vector3 moveDir;

    void Start()
    {
        Destroy(gameObject, DestroyDelay);
    }

    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 dir)
    {
        moveDir = dir;
        transform.rotation = Quaternion.LookRotation(dir);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);

        }
    }
}
