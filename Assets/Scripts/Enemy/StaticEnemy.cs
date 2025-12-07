using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StaticEnemy : MonoBehaviour, ISlowable
{
    public enum Enemystate { Chase, Attack }
    public Enemystate state = Enemystate.Chase;

    public float attackRange = 6f;
    public float attackCooldown = 1.5f;
    public GameObject projectilePrefab;
    public Transform firepoint;
    public EnemySO enemyData;

    public GameObject hitEffectEnemy;

    public GameObject goldPrefab;
    private float curEnemyHP;
    private bool isDead = false;

    private NavMeshAgent agent;
    private Transform player;
    private float lastAttackTime;

    private float baseMoveSpeed;
    private float currentSlowRatio = 1f;
    private bool isSlowed = false;

    public float knockbackDistance = 2f;
    public float knockbackDuration = 0.2f;
    private Coroutine knockbackRoutine;


    public float turnSpeed = 5f;

    void Awake()
    {
        // NavMeshAgent 컴포넌트 가져오기
        agent = GetComponent<NavMeshAgent>();

        if (agent != null && enemyData != null)
        {
            agent.speed = enemyData.EnemyMoveSpeed;
            baseMoveSpeed = enemyData.EnemyMoveSpeed;
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lastAttackTime = -attackCooldown;

        if (enemyData != null)
            curEnemyHP = enemyData.EnemyHP;

        StageManager.Instance?.OnEnemySpawned();
    }

    void Update()
    {
        // isDead이거나 플레이어가 없거나 NavMeshAgent가 비활성화 상태면 중단
        if (player == null || isDead || agent == null || !agent.enabled) return;

        // 플레이어의 Y축은 무시하고 적의 Y축에 맞춰 회전 및 추적을 위한 위치 계산
        Vector3 playerTargetPos = new Vector3(player.position.x, transform.position.y, player.position.z);

        float dist = Vector3.Distance(player.position, transform.position);

        switch (state)
        {
            case Enemystate.Chase:
                agent.isStopped = false; // 이동 활성화
                agent.SetDestination(player.position); // 플레이어 위치로 이동 명령
                LookAtTarget(playerTargetPos); // 이동 중에도 플레이어 방향으로 회전

                if (dist <= attackRange)
                {
                    state = Enemystate.Attack;
                }
                break;

            case Enemystate.Attack:
                agent.isStopped = true; // 이동 정지
                LookAtTarget(playerTargetPos);

                if (dist <= attackRange)
                {
                    AttackPlayer();
                }
                else // 공격 범위를 벗어나면 다시 추적 상태로 전환
                {
                    state = Enemystate.Chase;
                }
                break;
        }
    }

    void LookAtTarget(Vector3 targetPos)
    {
        Vector3 lookDir = (targetPos - transform.position).normalized;

        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }
    void AttackPlayer()
        {
            // 일정 쿨다운마다 발사
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                ShootProjectile();
            }
        }
    void ShootProjectile()
    {
        if (projectilePrefab != null && firepoint != null)
        {
            // 발사 방향 계산 (XZ 평면 기준, Y는 발사 위치 고정)
            Vector3 targetPos = new Vector3(player.position.x, firepoint.position.y, player.position.z);
            Vector3 dir = (targetPos - firepoint.position).normalized;

            // 투사체 생성
            GameObject proj = Instantiate(projectilePrefab, firepoint.position, Quaternion.LookRotation(dir));

            // EnemyProjectile이 있다면 방향 설정
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            if (ep != null)
            {
                ep.SetDirection(dir);
            }

            // 적 Y축 회전만 플레이어 방향으로
            Vector3 lookDir = dir;
            lookDir.y = 0; // Y축만 적용
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        curEnemyHP -= damage;

        if (hitEffectEnemy != null)
        {
            // 적의 현재 위치에 이펙트 생성
            GameObject effect = Instantiate(hitEffectEnemy, transform.position, Quaternion.identity);
            // 2초 후 이펙트 파괴
            Destroy(effect, 2f);
        }

        Vector3 hitDir = (transform.position - player.position).normalized;
        ApplyKnockback(hitDir);

        if (curEnemyHP <= 0)
        {
            curEnemyHP = 0;
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (StageManager.Instance != null)
            StageManager.Instance.OnEnemyKilled();

        DropGold();

        Destroy(gameObject);
    }

    private void DropGold()
    {
        if (goldPrefab != null)
        {
            Vector3 dropPos = transform.position + Vector3.up * 0.5f;
            GameObject gold = Instantiate(goldPrefab, dropPos, Quaternion.identity);

        }
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

    public void ApplySlow(float slowRatio)
    {
        currentSlowRatio = slowRatio;
        agent.speed = baseMoveSpeed * currentSlowRatio;
        isSlowed = true;
    }

    public void RemoveSlow()
    {
        currentSlowRatio = 1f;
        agent.speed = baseMoveSpeed;
        isSlowed = false;
    }
}
