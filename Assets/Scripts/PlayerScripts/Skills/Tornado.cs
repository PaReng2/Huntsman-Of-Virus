using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    
    int lifeTime = 5;

    public int curTornadoDamage;

    ChaseEnemy chaseEnemy;

    [Header("이동 설정")]
    [Tooltip("토네이도의 전진 속도 (z축 기준)")]
    public float forwardSpeed = 5.0f;

    [Tooltip("구불구불한 움직임의 강도 (좌우 이동 폭)")]
    public float swayMagnitude = 2.0f;

    [Tooltip("구불구불한 움직임의 속도 (파동의 빈도)")]
    public float swaySpeed = 2.0f;

    // 토네이도가 소환된 시점을 기준으로 시간의 흐름을 측정
    private float startTime;

    void Start()
    {
        startTime = Time.time;
        chaseEnemy = FindAnyObjectByType<ChaseEnemy>();
    }

    void Update()
    {
        Destroy(gameObject, lifeTime);
        HandleWavyMovement();
    }

    void HandleWavyMovement()
    {
        // 1. 전진 이동
        // 토네이도의 Transform을 기준으로 앞(Forward, Local Z)으로 계속 이동
        Vector3 forwardMovement = transform.forward * forwardSpeed * Time.deltaTime;

        // 2. 구불구불한 좌우 이동 계산
        // Mathf.Sin 함수를 사용하여 시간에 따라 -1.0f ~ 1.0f 사이의 값으로 변동하는 값을 얻음
        float timeSinceStart = Time.time - startTime;
        float swayValue = Mathf.Sin(timeSinceStart * swaySpeed) * swayMagnitude;

        // 토네이도의 Transform을 기준으로 오른쪽(Right, Local X) 방향으로 좌우 이동 적용
        Vector3 sideMovement = transform.right * swayValue * Time.deltaTime;

        // 3. 최종 이동 적용
        // 전진 이동과 좌우 이동을 합하여 Transform을 이동시킴
        transform.position += forwardMovement + sideMovement;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            chaseEnemy.TakeDamage(curTornadoDamage);
        }
    }
}
