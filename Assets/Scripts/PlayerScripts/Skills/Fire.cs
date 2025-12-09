using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public int lifeTime;
    public int Damage;
    ChaseEnemy chaseEnemy;

    void Start()
    {
        Destroy(gameObject, lifeTime); 
    }

    void Update()
    {
        gameObject.transform.rotation = transform.parent.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            ChaseEnemy chaseEnemyComponent = other.GetComponent<ChaseEnemy>();
            StaticEnemy staticEnemy = other.GetComponent<StaticEnemy>();
            if (chaseEnemyComponent != null)
            {
                chaseEnemyComponent.TakeDamage(Damage);
            }
            if (staticEnemy != null)
            {
                staticEnemy.TakeDamage(Damage);

            }
        }
    }
}
