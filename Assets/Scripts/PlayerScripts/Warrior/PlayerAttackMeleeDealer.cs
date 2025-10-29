using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackMeleeDealer : MonoBehaviour
{
    [Header("공격 관련")]
    public float attackRange = 2f;         
    public float attackPower = 10f;        
    public float attackRate = 1f;          
    private float curLeftAttackTime = 0f;  

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

    private void Start()
    {
        attackRate = playerData.playerAttackRate;
        attackRange = playerData.playerAttackRange;
        attackPower = playerData.playerAttackPower;
    }

    private void Update()
    {
        isInteracting = gameManager.isInteracting;

        if (curLeftAttackTime > 0)
            curLeftAttackTime -= Time.deltaTime;

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
                curLeftAttackTime = attackRate;
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

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        if (hitEnemies.Length == 0)
        {
            Debug.Log("적에게 맞지 않았습니다.");
        }

        foreach (Collider enemy in hitEnemies)
        {
            ChaseEnemy enemyComponent = enemy.GetComponent<ChaseEnemy>();
            StaticEnemy enemyComponent2 = enemy.GetComponent<StaticEnemy>();

            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(attackPower);
                Debug.Log($"적 {enemy.name}에게 {attackPower} 피해를 줌!");
            }
            else
            {
                enemyComponent2.TakeDamage(attackPower);
                Debug.Log($"적 {enemy.name}에게 {attackPower} 피해를 줌!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange * attackPoint.lossyScale.x);
    }
}