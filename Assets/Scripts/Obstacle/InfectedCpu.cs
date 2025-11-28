using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectedCpu : MonoBehaviour
{
    [Header("데미지 설정")]
    public int damage = 5;        // 데미지 5
    public float tickInterval = 0.5f; // 0.5초 간격

    private PlayerController pc;
    private Coroutine dotDamageCoroutine; // 실행 중인 코루틴을 저장할 변수

    private void Awake()
    {
        pc = FindAnyObjectByType<PlayerController>();
    }

    // 플레이어가 닿기 시작했을 때
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 혹시라도 이미 실행 중이면 멈추고 다시 시작 (안전장치)
            if (dotDamageCoroutine != null) StopCoroutine(dotDamageCoroutine);

            // 반복 데미지 코루틴 시작
            dotDamageCoroutine = StartCoroutine(DealDotDamage());
        }
    }

    // 플레이어가 떨어졌을 때
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 닿는게 끝났으므로 데미지 주는 코루틴 중단
            if (dotDamageCoroutine != null)
            {
                StopCoroutine(dotDamageCoroutine);
                dotDamageCoroutine = null;
            }
        }
    }

    IEnumerator DealDotDamage()
    {
        // OnCollisionExit에서 StopCoroutine을 할 때까지 무한 반복
        while (true)
        {
            if (pc != null)
            {
                pc.TakeDamage(damage);
                Debug.Log($"플레이어에게 {damage} 데미지 줌!");
            }

            // 0.5초 대기
            yield return new WaitForSeconds(tickInterval);
        }
    }
}