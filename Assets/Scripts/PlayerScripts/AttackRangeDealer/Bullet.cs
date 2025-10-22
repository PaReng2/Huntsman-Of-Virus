using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;

    private void Start()
    {
        Destroy(gameObject, 3f); // 생존시간 기본 3초
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * 20f * Time.deltaTime); // 총알 이동
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 적 충돌 처리
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ChaseEnemy chaseEnemy = collision.gameObject.GetComponent<ChaseEnemy>();
            if (chaseEnemy != null) chaseEnemy.TakeDamage(damage);

            StaticEnemy staticEnemy = collision.gameObject.GetComponent<StaticEnemy>();
            if (staticEnemy != null) staticEnemy.TakeDamage(damage);
        }


        Destroy(gameObject);
    }
}
