using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class StaticEnemy : MonoBehaviour
{
    public enum Enemystate { Idle, Attack}
    public Enemystate state = Enemystate.Idle;

    public float attackRange = 6f;          //공격 시작 거리
    public float attackCooldown = 1.5f;     //공격 쿨타임
    public GameObject projectilePrefab;     //투사체 프리팹
    public Transform firepoint;             //발사 위치
    public EnemySO enemyData;
    private float curEnemyHP;
    private bool isDead = false;

    private Transform player;
    private float lastAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lastAttackTime = -attackCooldown;

        if (enemyData != null)
            curEnemyHP = enemyData.EnemyHP;

    }
    void Update()
    {
        if (player == null) return;

        Vector3 playerTargetPos = new Vector3(player.position.x, transform.position.y, player.position.z);

        float dist = Vector3.Distance(player.position, transform.position);


        switch (state)
        {
            case Enemystate.Idle:
                if (dist < attackRange)
                    state = Enemystate.Attack;
                break;
                case Enemystate.Attack:
                if (dist < attackRange)
                    AttackPlayer();
                else if (dist > attackRange)
                    state = Enemystate.Idle;
                break;
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

        Destroy(gameObject);
    }
}
