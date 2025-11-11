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

    [Header("Effect")]
    public GameObject Slash;
    public float effectForwardOffset = 0.5f;

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
            TutorialEnemy tutorial  = enemy.GetComponent<TutorialEnemy>();

            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(attackPower);
                Debug.Log($"적 {enemy.name}에게 {attackPower} 피해를 줌!");
            }
            else if(enemyComponent2 != null) 
            {
                enemyComponent2.TakeDamage(attackPower);
                Debug.Log($"적 {enemy.name}에게 {attackPower} 피해를 줌!");
            }
            else
            {
                tutorial.TakeDamage(attackPower);
                Debug.Log($"적 {enemy.name}에게 {attackPower} 피해를 줌!");
            }
        }
        if (Slash != null)
        {
            // 1. 최종 위치 계산
            Vector3 spawnPosition = attackPoint.position;

            // 캐릭터의 정면 방향(transform.forward)으로 'effectForwardOffset'만큼 이동
            spawnPosition += transform.forward * effectForwardOffset;


            // 2. 최종 회전 계산
            // 캐릭터의 현재 회전 값
            Quaternion playerRotation = transform.rotation;

            // Y축 90도 추가 회전
            Quaternion offsetRotation = Quaternion.Euler(0, 0, 0);

            // 캐릭터 회전에 Y축 90도 회전을 더함 (순서 중요: 플레이어 회전 * 오프셋 회전)
            Quaternion finalRotation = playerRotation * offsetRotation; // ⬅️ 추가/수정된 부분


            // 3. 계산된 위치와 회전으로 인스턴스화
            GameObject effectInstance = Instantiate(
                Slash,
                spawnPosition,
                finalRotation // ⬅️ 수정: 계산된 최종 회전 사용
            );

            // 0.5초 후에 생성된 인스턴스를 파괴
            Destroy(effectInstance, 0.5f);
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}