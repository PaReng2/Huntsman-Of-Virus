using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseEnemy : MonoBehaviour
{
    public Transform target;
    public EnemySO enemyData;

    private PlayerController player;
    private float curEnemyHP;
    private bool isDead = false;

    private NavMeshAgent agent;

    private void Awake()
    {
        enemyData = Resources.Load<EnemySO>("Enemy");
        player = FindObjectOfType<PlayerController>();
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player").transform;

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        curEnemyHP = enemyData.EnemyHP;

        // 스폰 시 StageManager에 등록
        StageManager.Instance.OnEnemySpawned();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(1f); // 총알 한 발당 데미지 1
        }
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);

            Vector3 direction = (target.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        curEnemyHP -= damage;

        if (curEnemyHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        StageManager.Instance.OnEnemyKilled();
        Destroy(gameObject);
    }
}
