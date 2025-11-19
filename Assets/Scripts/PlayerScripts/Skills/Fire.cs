using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public int lifeTime;
    public int Damage;
    ChaseEnemy chaseEnemy;

    private void Awake()
    {
        chaseEnemy = FindAnyObjectByType<ChaseEnemy>();
    }

    void Start()
    {
        gameObject.transform.rotation = transform.parent.rotation;
    }

    void Update()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            chaseEnemy.TakeDamage(Damage);
        }
    }
}
