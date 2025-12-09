using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float lifeTime;
    public int Damage;

    public float explosionDelay = 1.0f;
    public float explosionRadius = 5f;

    private bool hasExploded = false;

    void Start()
    {
        Invoke("ExplodeAndDestroy", lifeTime);
    }

    void Update()
    {
        if (transform.parent != null)
        {
            gameObject.transform.rotation = transform.parent.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;

        if (other.CompareTag("Enemy"))
        {
            ChaseEnemy chaseEnemyComponent = other.GetComponent<ChaseEnemy>();
            StaticEnemy staticEnemy = other.GetComponent<StaticEnemy>();   
            if (chaseEnemyComponent != null)
            {
                chaseEnemyComponent.TakeDamage(Damage);
                
                ExplodeAndDestroy();
            }
            if (staticEnemy != null)
            {
                staticEnemy.TakeDamage(Damage);
                ExplodeAndDestroy();

            }
        }
    }

    void ExplodeAndDestroy()
    {
        if (hasExploded) return;
        hasExploded = true;

        PerformExplosionDamage();

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        CancelInvoke("ExplodeAndDestroy");

        Destroy(gameObject, explosionDelay);
    }

    private void PerformExplosionDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                ChaseEnemy enemy = hitCollider.GetComponent<ChaseEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(Damage);
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(transform.position, explosionRadius);

    }
}
