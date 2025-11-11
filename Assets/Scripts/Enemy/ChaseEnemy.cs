using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ChaseEnemy : MonoBehaviour
{
    public EnemySO enemyData;
    public Transform target;
    private PlayerController player;
    private NavMeshAgent agent;
    public GameObject hitEffectEnemy;

    private float curEnemyHP;
    private bool isDead = false;

    public int damage = 20;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;
    private Rigidbody rb;


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
        rb = GetComponent<Rigidbody>();

    }

    private void Update()
    {
        if (isDead || target == null || !agent.isOnNavMesh) return;

        if (agent != null && agent.enabled)
        {
            agent.SetDestination(target.position);

            Vector3 dir = (target.position - transform.position).normalized;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
        }
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

        if (hitEffectEnemy != null)
        {
            Instantiate(hitEffectEnemy, transform.position, Quaternion.identity);
        }

        Vector3 hitDir = (transform.position - player.transform.position).normalized;
        ApplyKnockback(hitDir, 8f);

        if (curEnemyHP <= 0)
            Die();
    }


    private void Die()
    {
        if (isDead) return;
        isDead = true;

        StageManager.Instance.OnEnemyKilled();
        Destroy(gameObject);
    }
    public void ApplyKnockback(Vector3 knockbackDir, float force)
    {
        if (agent != null)
            agent.isStopped = true; 

        rb.isKinematic = false;

        knockbackDir.y = 0f; 
        rb.AddForce(knockbackDir.normalized * force, ForceMode.Impulse);

        StartCoroutine(ResumeAfterKnockback(0.4f)); 
    }

    IEnumerator ResumeAfterKnockback(float delay)
    {
        yield return new WaitForSeconds(delay);

    
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

      
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 3f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position); 
            }
        }

        agent.isStopped = false; 
    }

}
