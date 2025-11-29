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
    public float shieldCooldown = 2f;         // 쉴드 쿨타임 (2초)
    private float curShieldTime = 0f;         // 쉴드 남은 시간
    private bool isShieldActive = false;
    private PlayerController playerController;

    [Header("Unlocked Skills")]
    public List<Skills> unlockedSkills = new List<Skills>();

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        // 쉴드는 아이템으로 해제되어 있지 않으면 생성하지 않도록 변경
        if (IsSkillUnlocked(Skills.shield))
        {
            ActivateShield();
        }
        else
        {
            isShieldActive = false;
            curShieldTime = 0f;
        }
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

        // 스킬 사용은 해당 스킬이 잠금 해제된 경우에만 허용
        tornado();
        fire();
    }

    public bool IsSkillUnlocked(Skills skill)
    {
        return unlockedSkills != null && unlockedSkills.Contains(skill);
    }

    public void UnlockSkill(Skills skill)
    {
        if (unlockedSkills == null) unlockedSkills = new List<Skills>();
        if (!unlockedSkills.Contains(skill))
        {
            unlockedSkills.Add(skill);
            Debug.Log($"[PlayerSkills] 스킬 잠금 해제: {skill}");

            // 스킬이 즉시 활성화를 요구하는 경우 처리
            if (skill == Skills.shield)
            {
                // 쉴드가 꺼져있고 쿨이 끝나있다면 즉시 활성화
                if (!isShieldActive && curShieldTime <= 0f)
                    ActivateShield();
            }
        }
    }

    public void tornado()
    {
        // 잠금 해제되어 있지 않으면 동작하지 않음
        if (!IsSkillUnlocked(Skills.tornado)) return;

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
        // 잠금 해제되어 있지 않으면 동작하지 않음
        if (!IsSkillUnlocked(Skills.fire)) return;

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
        // 쉴드 스킬이 잠금해제되어 있지 않으면 만들지 않음
        if (!IsSkillUnlocked(Skills.shield))
        {
            Debug.Log("[PlayerSkills] 쉴드 스킬이 잠금 상태라 활성화하지 않습니다.");
            return;
        }

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
        curShieldTime = shieldCooldown; // 재충전 시작
        Debug.Log($"쉴드 파괴!  {shieldCooldown}초 후 재활성화됩니다.");
        if (playerController != null)
        {
            playerController.GrantInvincibilityOnShieldBreak();
        }
    }
}