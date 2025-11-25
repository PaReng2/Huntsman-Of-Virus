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

    public float knockbackDistance = 2f;    
    public float knockbackDuration = 0.2f;  
    private Coroutine knockbackRoutine;

    public GameObject goldPrefab;

    private void Awake()
    {
        if (enemyData == null)
            enemyData = Resources.Load<EnemySO>("Enemy");

        player = FindObjectOfType<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;

        curEnemyHP = enemyData.EnemyHP;
        StageManager.Instance.OnEnemySpawned();
    }

    private void Update()
    {
        if (isDead || target == null || agent == null || !agent.isOnNavMesh) return;

        if (!agent.enabled) return;

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

        if (hitEffectEnemy != null)
        {
            GameObject effect = Instantiate(hitEffectEnemy, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        if (player != null)
        {
            Vector3 hitDir = (transform.position - player.transform.position).normalized;
            ApplyKnockback(hitDir);
        }

        if (curEnemyHP <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        DropGold();

        StageManager.Instance.OnEnemyKilled();
        AchievementManager.instance.UpdateProgress(AchievementType.KillEnemies, 1);
        Destroy(gameObject);
    }

    public void ApplyKnockback(Vector3 knockbackDir)
    {
        knockbackDir.y = 0f;
        knockbackDir.Normalize();

        if (knockbackRoutine != null)
            StopCoroutine(knockbackRoutine);

        knockbackRoutine = StartCoroutine(KnockbackCoroutine(knockbackDir));
    }

    private IEnumerator KnockbackCoroutine(Vector3 dir)
    {
        if (agent != null)
            agent.isStopped = true;

        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < knockbackDuration)
        {
            float t = elapsed / knockbackDuration;
            Vector3 offset = dir * (knockbackDistance * Time.deltaTime / knockbackDuration);
            transform.position += offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (agent != null)
        {
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

        knockbackRoutine = null;
    }

    private void DropGold()
    {
        if (goldPrefab != null)
        {
            Vector3 dropPos = transform.position + Vector3.up * 0.5f;
            GameObject gold = Instantiate(goldPrefab, dropPos, Quaternion.identity);

            
        }
    }
}