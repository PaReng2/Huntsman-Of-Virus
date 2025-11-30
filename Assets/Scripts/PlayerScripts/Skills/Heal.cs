using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    // 힐량
    public int healAmount = 10;

    public float effectDuration = 1.0f;

    private void Start()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        if (playerController != null)
        {
            // PlayerController의 Heal 메서드를 호출하여 체력을 회복
            playerController.Heal(healAmount);
            Debug.Log($"[Heal] 플레이어를 {healAmount}만큼 치료했습니다. 현재 HP: {playerController.curPlayerHp}/{playerController.playerMaxHP}");
        }

        Destroy(gameObject, effectDuration);
    }
}
