using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ChaseEnemy : MonoBehaviour
{
    public EnemySO enemyData;
    public Transform target;
    private PlayerController player;
    private NavMeshAgent agent;

    private float curEnemyHP;
    private bool isDead = false;

    public int damage = 20;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    private void Awake()
    {
        if (enemyData == null)
            enemyData = Resources.Load<EnemySO>("Enemy");

        player = FindObjectOfType<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        curEnemyHP = enemyData.EnemyHP;
        StageManager.Instance.OnEnemySpawned();
    }

    private void Update()
    {
        if (isDead || target == null) return;

        agent.SetDestination(target.position);
        Vector3 dir = (target.position - transform.position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
                Destroy(other.gameObject);
            }
        }
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    lastAttackTime = Time.time;

                    pc.TakeDamage(damage);

                    Vector3 knockbackDir = (pc.transform.position - transform.position).normalized;
                    pc.ApplyKnockback(knockbackDir, 5f);
                }
            }
        }
    }


    public void TakeDamage(float damage)
    {
        if (isDead) return;
        curEnemyHP -= damage;

        Vector3 hitDir = (transform.position - player.transform.position).normalized;
        ApplyKnockback(hitDir, 5f);

        if (curEnemyHP <= 0)
            Die();
    }


    private void Die()
    {
        if (isDead) return;
        isDead = true;

        StageManager.Instance.OnEnemyKilled();
        Destroy(gameObject, 1.5f);
    }
    public void ApplyKnockback(Vector3 hitDirection, float force)
    {
        NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        Rigidbody rb = GetComponent<Rigidbody>();

        if (agent != null)
            agent.isStopped = true;

        if (rb != null)
        {
            hitDirection.y = 2f;
            rb.velocity = Vector3.zero;
            rb.AddForce(hitDirection.normalized * force, ForceMode.Impulse);
        }

        StartCoroutine(ResumeAfterKnockback(agent, 0.3f));
    }

    private IEnumerator ResumeAfterKnockback(UnityEngine.AI.NavMeshAgent agent, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (agent != null)
            agent.isStopped = false;
    }

}
