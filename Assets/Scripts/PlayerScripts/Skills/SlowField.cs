using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowField : MonoBehaviour
{
    [Header("Field Settings")]
    public float duration = 5f;
    public float slowAmount = 0.5f;


    private const string EnemyTag = "Enemy";

    private List<GameObject> affectedEnemies = new List<GameObject>();

    void Start()
    {
        Destroy(gameObject, duration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(EnemyTag))
        {
            if (!affectedEnemies.Contains(other.gameObject))
            {
                affectedEnemies.Add(other.gameObject);
                ApplySlowEffect(other.gameObject, true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(EnemyTag))
        {
            if (affectedEnemies.Contains(other.gameObject))
            {
                affectedEnemies.Remove(other.gameObject);
                ApplySlowEffect(other.gameObject, false);
            }
        }
    }

    private void ApplySlowEffect(GameObject enemy, bool apply)
    {
        ISlowable slowableComponent = enemy.GetComponent<ISlowable>();

        if (slowableComponent != null)
        {

            float resultingSpeedRatio = 1f - slowAmount;

            if (apply)
            {
                slowableComponent.ApplySlow(resultingSpeedRatio);
                Debug.Log($"[SlowField] {enemy.name}에게 {slowAmount * 100}% 슬로우 적용. (남은 속도 비율: {resultingSpeedRatio})");
            }
            else
            {
                slowableComponent.RemoveSlow();
                Debug.Log($"[SlowField] {enemy.name}의 슬로우 효과 해제.");
            }
        }
        else
        {
            Debug.LogWarning($"[SlowField] {enemy.name} 오브젝트가 ISlowable 인터페이스를 구현하지 않았거나 스크립트를 찾을 수 없습니다.");
        }
    }
}
