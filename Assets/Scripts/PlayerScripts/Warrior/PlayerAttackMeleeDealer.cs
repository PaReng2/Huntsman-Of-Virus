using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackMeleeDealer : MonoBehaviour
{
    [Header("공격 관련")]
    public float attackRange = 2f;         
    public float attackPower = 10f;        
    public float attackRate;          
    private float curLeftAttackTime = 0f;
    private float lastClickedTime = 0f;
    public static int noOfClicks = 0;

    private int comboStep = 0;          // 현재 콤보 단계 (0: 처음, 1: 1타, 2: 2타)
    private float comboTimer = 0f;      // 콤보 유지 시간 체크용
    private float comboWindow = 5f;     // 콤보 허용 시간 (5초)

    [Header("Effect")]
    public GameObject Slash;
    public float effectForwardOffset = 0.5f;

    [Header("기타")]
    public LayerMask enemyLayer;           
    public Transform attackPoint;          
    public PlayerStatusSO playerData;      
    public bool isInteracting;

    private GameManager gameManager;
    private DialogueNPC tutorial;
    private Animator anime;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        tutorial = FindAnyObjectByType<DialogueNPC>();
        anime = GetComponent<Animator>();
    }

    private void Start()
    {
        attackRate = playerData.playerAttackRate;
        if (attackRate <= 0)
        {
            attackRate = 1f;
        }
        attackRange = playerData.playerAttackRange;
        attackPower = playerData.playerAttackPower;
    }

    private void Update()
    {
        isInteracting = gameManager.isInteracting;

        // 기본 공격 쿨타임(공속) 감소
        if (curLeftAttackTime > 0)
            curLeftAttackTime -= Time.deltaTime;

        // ★ 콤보 타이머 로직: 5초가 지나면 콤보 단계를 0으로 리셋
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0)
            {
                comboStep = 0;
                // Debug.Log("콤보 시간 초과! 다시 1타부터 시작합니다.");
            }
        }

        // 공격 입력
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack();
        }

        // 중요: 기존에 있던 else { anime.SetBool(...) } 구문은 완전히 지우셔야 합니다.
    }

    void TryAttack()
    {
        // 쿨타임 중이거나 상호작용 중이면 공격 불가
        if (curLeftAttackTime > 0) return;
        if (isInteracting) return;

        comboStep++; // 클릭할 때마다 단계 상승

        if (comboStep == 1)
        {
            // 1타 실행
            anime.SetTrigger("attack1"); 
            Debug.Log("공격 1타!");
        }
        else if (comboStep == 2)
        {
            // 2타 실행 (1타 후 5초 안에 눌렀을 경우 여기로 옴)
            anime.SetTrigger("attack2");
            Debug.Log("공격 2타! (콤보)");

            comboStep = 0; // 2타까지 때렸으니 다시 처음으로 초기화
        }
        else
        {
            // 예외 처리: 혹시 단계가 꼬이면 1타로 강제 설정
            comboStep = 1;
            anime.SetTrigger("attack1");
        }

        comboTimer = comboWindow;       // 타이머를 다시 5초로 충전
        Attack();                       // 데미지 및 이펙트 처리 (기존 함수 그대로 사용)
        curLeftAttackTime = attackRate; // 공격 쿨타임 적용
    }


    void Attack()
    {
        Debug.Log("근거리 공격!");

        Collider[] hitEnemies = Physics.OverlapCapsule(attackPoint.position, attackPoint.position, attackRange, enemyLayer);

        if (hitEnemies.Length == 0)
        {
            Debug.Log("적에게 맞지 않았습니다.");
        }

        foreach (Collider enemy in hitEnemies)
        {
            ChaseEnemy enemyComponent = enemy.GetComponent<ChaseEnemy>();
            StaticEnemy enemyComponent2 = enemy.GetComponent<StaticEnemy>();
            TutorialEnemy enemyComponentT = enemy.GetComponent<TutorialEnemy>();
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

            if (enemyComponentT != null)
            {
                enemyComponentT.TakeDamage(attackPower);
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