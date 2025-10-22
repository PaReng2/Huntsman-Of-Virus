using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class StaticEnemy : MonoBehaviour
{
    public enum Enemystate { Idle, Attack}
    public Enemystate state = Enemystate.Idle;

    public float attackRange = 6f;          //���� ���� �Ÿ�
    public float attackCooldown = 1.5f;     //���� ��Ÿ��
    public GameObject projectilePrefab;     //����ü ������
    public Transform firepoint;             //�߻� ��ġ
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
            // ���� ��ٿ�� �߻�
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
            // �߻� ���� ��� (XZ ��� ����, Y�� �߻� ��ġ ����)
            Vector3 targetPos = new Vector3(player.position.x, firepoint.position.y, player.position.z);
            Vector3 dir = (targetPos - firepoint.position).normalized;

            // ����ü ����
            GameObject proj = Instantiate(projectilePrefab, firepoint.position, Quaternion.LookRotation(dir));

            // EnemyProjectile�� �ִٸ� ���� ����
            EnemyProjectile ep = proj.GetComponent<EnemyProjectile>();
            if (ep != null)
            {
                ep.SetDirection(dir);
            }

            // �� Y�� ȸ���� �÷��̾� ��������
            Vector3 lookDir = dir;
            lookDir.y = 0; // Y�ุ ����
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
