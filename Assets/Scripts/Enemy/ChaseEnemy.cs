using UnityEngine;
using UnityEngine.AI;

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
            if (pc != null) pc.TakeDamage(damage);


        }
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

        StageManager.Instance.OnEnemyKilled();
        Destroy(gameObject, 1.5f);
    }
}
