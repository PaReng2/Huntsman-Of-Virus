using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemy : MonoBehaviour
{
    public EnemySO enemyData;
    public float curEnemyHP;
    private bool isDead;

    public GameObject DathEffect;
    private void Awake()
    {
        curEnemyHP = enemyData.EnemyHP;
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        curEnemyHP -= damage;

        if (curEnemyHP <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (DathEffect != null)
        {
            GameObject effect = Instantiate(DathEffect, transform.position, Quaternion.AngleAxis(90f,Vector3.up));
            Destroy(DathEffect, 5f);
        }

        Destroy(gameObject);
    }
}
