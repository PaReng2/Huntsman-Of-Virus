using UnityEngine;

public class Tornado : MonoBehaviour
{
    int lifeTime = 5;
    public int curTornadoDamage;


    [Header("이동 설정")]
    public float forwardSpeed = 5.0f;

    [Header("지형 설정")]
    [Tooltip("토네이도가 이동할 바닥의 Y축 높이.")]
    public float groundYLevel = 0.0f;

    void Start()
    {
        // 충돌 로직용 (주의: 맵에 적이 여러 명이면 로직 수정 필요할 수 있음)

        // 5초 후 소멸
        Destroy(gameObject, lifeTime);

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // 주의: 여기서 'chaseEnemy' 변수는 Start에서 찾은 '특정 한 놈'입니다.
            // 부딪힌 '그 적(other)'에게 데미지를 주려면 아래처럼 수정하는 것이 좋습니다.
            ChaseEnemy enemyComponent = other.GetComponent<ChaseEnemy>();
            StaticEnemy staticEnemy = other.GetComponent<StaticEnemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(curTornadoDamage);
            }
            if (staticEnemy != null)
            {
                staticEnemy.TakeDamage(curTornadoDamage);

            }

        }
    }
}