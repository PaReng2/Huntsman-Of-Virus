using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackMeleeDealer : MonoBehaviour
{
    [Header("���� ����")]
    public float attackRange = 2f;         
    public float attackPower = 10f;        
    public float attackRate = 1f;          
    private float curLeftAttackTime = 0f;  

    [Header("��Ÿ")]
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
                    Debug.Log("��ȣ�ۿ� �߿��� ������ �� �����ϴ�.");
                    return;
                }

                Attack();
                curLeftAttackTime = attackRate;
            }
            else
            {
                Debug.Log("���� ������ ��...");
            }
        }
    }

    void Attack()
    {
        Debug.Log("�ٰŸ� ����!");

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        if (hitEnemies.Length == 0)
        {
            Debug.Log("������ ���� �ʾҽ��ϴ�.");
        }

        foreach (Collider enemy in hitEnemies)
        {
            ChaseEnemy enemyComponent = enemy.GetComponent<ChaseEnemy>();
            StaticEnemy enemyComponent2 = enemy.GetComponent<StaticEnemy>();

            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(attackPower);
                Debug.Log($"�� {enemy.name}���� {attackPower} ���ظ� ��!");
            }
            else
            {
                enemyComponent2.TakeDamage(attackPower);
                Debug.Log($"�� {enemy.name}���� {attackPower} ���ظ� ��!");
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