using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public EnemySO enemyData;

    private PlayerController player;
    private float curEnemyHP;

    NavMeshAgent agent;

    private bool isDead = false; 

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
        StageManager.Instance.OnEnemySpawned(); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            if (StageManager.Instance != null)
            {
                StageManager.Instance.OnEnemyKilled();
            }

            Destroy(gameObject);
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
        Debug.Log($"{gameObject.name}이(가) {damage}의 피해를 입음! 남은 체력: {curEnemyHP}");

        if (curEnemyHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} 사망!");
        StageManager.Instance.OnEnemyKilled(); 
        Destroy(gameObject);
    }
}