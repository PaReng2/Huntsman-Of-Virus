using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerSkills : MonoBehaviour
{
    public enum Skills
    {
        tornado,
        fire,
        shield
    }

    [Header("EffectSpawnPosition")]
    public Transform effectSpawn;

    [Header("tornado")]
    public GameObject tornadoEffect;
    public float tornadoCooldown = 3f;      // 토네이도 쿨타임 (3초)
    private float curTornadoTime = 0f;      // 토네이도 남은 시간

    [Header("fire")]
    public GameObject fireEffect;
    public float fireCooldown = 5f;         // 파이어 쿨타임 (5초)
    private float curFireTime = 0f;         // 파이어 남은 시간

    [Header("shield")]
    public GameObject shieldEffect;
    public float shieldCooldown = 2f;         // 쉴드 쿨타임 (5초)
    private float curShieldTime = 0f;         // 쉴드 남은 시간
    private bool isShieldActive = false;
    private PlayerController playerController;

    private void Start()
    {
        ActivateShield();
        playerController = FindObjectOfType<PlayerController>();
    }
    private void Update()
    {
        if (curTornadoTime > 0)
        {
            curTornadoTime -= Time.deltaTime;
        }

        if (curFireTime > 0)
        {
            curFireTime -= Time.deltaTime;
        }
        if (!isShieldActive && curShieldTime > 0)
        {
            curShieldTime -= Time.deltaTime;

            // 쿨타임이 끝나면 쉴드를 재활성화
            if (curShieldTime <= 0)
            {
                ActivateShield();
            }
        }
        tornado();
        fire();
    }
    public void tornado()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (curTornadoTime <= 0)
            {
                Instantiate(tornadoEffect, effectSpawn);

                curTornadoTime = tornadoCooldown; // 쿨타임 재설정
            }
            else
            {
                Debug.Log($"토네이도 쿨타임! 남은 시간: {curTornadoTime:F1}초");
            }
        }
    }

    public void fire()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (curFireTime <= 0)
            {
                Instantiate(fireEffect, effectSpawn);
                curFireTime = fireCooldown; // 쿨타임 재설정
            }
            else
            {
                Debug.Log($"파이어 쿨타임! 남은 시간: {curFireTime:F1}초");
            }
        }
    }
    private void ActivateShield()
    {
        // 쉴드 이펙트를 생성하고, Shield 스크립트에 이 PlayerSkills 스크립트를 전달
        GameObject shieldInstance = Instantiate(shieldEffect, effectSpawn);
        Shield shieldScript = shieldInstance.GetComponent<Shield>();

        if (shieldScript != null)
        {
            // Shield 스크립트가 PlayerSkills를 참조할 수 있도록 설정
            shieldScript.SetPlayerSkills(this);
        }

        isShieldActive = true;
        curShieldTime = 0f; // 쿨타임 초기화
        Debug.Log("쉴드 활성화!");
    }

    public void ShieldDestroyed()
    {
        isShieldActive = false;
        curShieldTime = shieldCooldown; // 5초 재충전 시작
        Debug.Log($"쉴드 파괴!  {shieldCooldown}초 후 재활성화됩니다.");
        if (playerController != null)
        {
            playerController.GrantInvincibilityOnShieldBreak();
        }
    }
}
