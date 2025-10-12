using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackMeleeDealer : MonoBehaviour
{
    [Header("공격 쿨타임 관리")]
    public float AttackRate = 1f;           
    public float curLeftAttackTime = 0f;    

    [Header("기타")]
    public LayerMask enemyLayer;            
    public Transform attackPoint;           
    public PlayerStatusSO playerData;       
    public bool isInteracting;              

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        isInteracting = gameManager.isInteracting;

        if (curLeftAttackTime > 0)
            curLeftAttackTime -= Time.deltaTime;

        // 공격 입력
        if (Input.GetMouseButtonDown(0))
        {
            if (curLeftAttackTime <= 0)
            {
                if (isInteracting)
                {
                    Debug.Log("상호작용 중에는 공격할 수 없습니다.");
                    return;
                }

                Attack();
                curLeftAttackTime = AttackRate; // 공격 후 쿨타임 초기화
            }
            else
            {
                Debug.Log("공격 재정비 중...");
            }
        }
    }

    void Attack()
    {
        Debug.Log("근거리 공격!");

        // ScriptableObject에서 공격력, 공격 범위 가져오기
        float attackPower = playerData.playerAttackPower;
        float attackRange = playerData.playerAttackRange;

        // 공격 범위 내 적 탐색
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(attackPower);
            }
        }

        // 공격 애니메이션 트리거는 여기에
    }

    // 공격 범위 시각화
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || playerData == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, playerData.playerAttackRange);
    }
}