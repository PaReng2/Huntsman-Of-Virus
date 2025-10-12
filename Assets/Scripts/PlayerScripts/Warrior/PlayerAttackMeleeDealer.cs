using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackMeleeDealer : MonoBehaviour
{
    [Header("���� ��Ÿ�� ����")]
    public float AttackRate = 1f;           
    public float curLeftAttackTime = 0f;    

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

    private void Update()
    {
        isInteracting = gameManager.isInteracting;

        if (curLeftAttackTime > 0)
            curLeftAttackTime -= Time.deltaTime;

        // ���� �Է�
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
                curLeftAttackTime = AttackRate; // ���� �� ��Ÿ�� �ʱ�ȭ
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

        // ScriptableObject���� ���ݷ�, ���� ���� ��������
        float attackPower = playerData.playerAttackPower;
        float attackRange = playerData.playerAttackRange;

        // ���� ���� �� �� Ž��
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(attackPower);
            }
        }

        // ���� �ִϸ��̼� Ʈ���Ŵ� ���⿡
    }

    // ���� ���� �ð�ȭ
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || playerData == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, playerData.playerAttackRange);
    }
}