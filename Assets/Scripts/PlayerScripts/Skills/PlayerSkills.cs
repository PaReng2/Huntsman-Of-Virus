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
        shield,
        heal,
        bomb,
        slowField
    }

    [Header("EffectSpawnPosition")]
    public Transform effectSpawn;

    [Header("tornado")]
    public GameObject tornadoEffect;
    public float tornadoCooldown = 3f;     
    private float curTornadoTime = 0f;     

    [Header("fire")]
    public GameObject fireEffect;
    public float fireCooldown = 5f;        
    private float curFireTime = 0f;        

    [Header("shield")]
    public GameObject shieldEffect;
    public float shieldCooldown = 2f;      
    private float curShieldTime = 0f;      
    private bool isShieldActive = false;
    private PlayerController playerController;

    [Header("heal")]
    public GameObject healEffect;
    public float healCooldown = 10f;      
    private float curHealTime = 0f;      

    [Header("bomb")]
    public GameObject bombEffect;
    public float bombCooldown = 15f;      
    private float curBombTime = 0f;    
    
    [Header("slowField")]
    public GameObject slowFieldEffect;
    public float slowFieldCooldown = 10f;      
    private float curSlowFieldTime = 0f;      

    [Header("Unlocked Skills")]
    public List<Skills> unlockedSkills = new List<Skills>();

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

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
        HandleCooldowns(); // 쿨타임 감소 로직 분리
        HandleSkillInput(); // 입력 처리 로직 (순서대로 키 할당)
    }

    // 쿨타임 관리 함수 (Update 가독성을 위해 분리)
    private void HandleCooldowns()
    {
        if (curTornadoTime > 0) curTornadoTime = Mathf.Max(0, curTornadoTime - Time.deltaTime);
        if (curFireTime > 0) curFireTime = Mathf.Max(0, curFireTime - Time.deltaTime);
        if (curHealTime > 0) curHealTime = Mathf.Max(0, curHealTime - Time.deltaTime);
        if (curBombTime > 0) curBombTime = Mathf.Max(0, curBombTime - Time.deltaTime);
        if (curSlowFieldTime > 0) curSlowFieldTime = Mathf.Max(0, curSlowFieldTime - Time.deltaTime);

        if (!isShieldActive && curShieldTime > 0)
        {
            curShieldTime -= Time.deltaTime;
            if (curShieldTime <= 0)
            {
                ActivateShield();
            }
        }
    }

    // 핵심 로직: 리스트 순서대로 키 입력 처리
    private void HandleSkillInput()
    {
        // 1번부터 9번까지만 처리 (보통 숫자키가 9개이므로)
        for (int i = 0; i < unlockedSkills.Count; i++)
        {
            if (i > 8) break; // 키보드 숫자키 범위를 넘어가면 무시

            // KeyCode.Alpha1은 '1'번 키입니다. i가 0일 때 Alpha1, 1일 때 Alpha2가 됩니다.
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                UseSkill(unlockedSkills[i]);
            }
        }
    }

    // 스킬 타입에 따라 실제 발동 함수 호출
    private void UseSkill(Skills skill)
    {
        switch (skill)
        {
            case Skills.tornado:
                CastTornado();
                break;
            case Skills.fire:
                CastFire();
                break;
            case Skills.heal:
                CastHeal();
                break;
            case Skills.bomb:
                CastBomb();
                break;
            case Skills.slowField:
                CastSlowField();
                break;
            case Skills.shield:
                // 쉴드는 보통 패시브(자동)이므로 키를 눌러도 반응하지 않거나, 
                // 수동 발동이 필요하다면 여기에 로직 추가
                Debug.Log("쉴드는 자동 발동 스킬입니다.");
                break;
        }
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
            Debug.Log($"[PlayerSkills] 스킬 잠금 해제: {skill}. (단축키: {unlockedSkills.Count}번)");

            if (skill == Skills.shield)
            {
                if (!isShieldActive && curShieldTime <= 0f)
                    ActivateShield();
            }
        }
    }

    // --- 아래부터는 각 스킬의 실제 발동 로직 (Input 체크 제거됨) ---

    public void CastTornado()
    {
        if (curTornadoTime <= 0)
        {
            Instantiate(tornadoEffect, effectSpawn);
            curTornadoTime = tornadoCooldown;
        }
        else
        {
            Debug.Log($"토네이도 쿨타임! 남은 시간: {curTornadoTime:F1}초");
        }
    }

    public void CastHeal()
    {
        if (curHealTime <= 0)
        {
            Instantiate(healEffect, effectSpawn);
            curHealTime = healCooldown;
        }
        else
        {
            Debug.Log($"힐 쿨타임! 남은 시간: {curHealTime:F1}초");
        }
    }

    public void CastFire()
    {
        if (curFireTime <= 0)
        {
            Instantiate(fireEffect, effectSpawn);
            curFireTime = fireCooldown;
        }
        else
        {
            Debug.Log($"파이어 쿨타임! 남은 시간: {curFireTime:F1}초");
        }
    }

    public void CastBomb()
    {
        if (curBombTime <= 0)
        {
            Instantiate(bombEffect, effectSpawn);
            curBombTime = bombCooldown;
        }
        else
        {
            Debug.Log($"자폭 쿨타임! 남은 시간: {curBombTime:F1}초");
        }
    }

    public void CastSlowField()
    {
        if (curSlowFieldTime <= 0)
        {
            Instantiate(slowFieldEffect, effectSpawn);
            curSlowFieldTime = slowFieldCooldown;
        }
        else
        {
            Debug.Log($"슬로우 장판 쿨타임! 남은 시간: {curSlowFieldTime:F1}초");
        }
    }

    // --- 쉴드 관련 로직 ---

    private void ActivateShield()
    {
        if (!IsSkillUnlocked(Skills.shield)) return;

        GameObject shieldInstance = Instantiate(shieldEffect, effectSpawn);
        Shield shieldScript = shieldInstance.GetComponent<Shield>();

        if (shieldScript != null)
        {
            shieldScript.SetPlayerSkills(this);
        }

        isShieldActive = true;
        curShieldTime = 0f;
        Debug.Log("쉴드 활성화!");
    }

    public void ShieldDestroyed()
    {
        isShieldActive = false;
        curShieldTime = shieldCooldown;
        Debug.Log($"쉴드 파괴!  {shieldCooldown}초 후 재활성화됩니다.");
        if (playerController != null)
        {
            playerController.GrantInvincibilityOnShieldBreak();
        }
    }

    // --- 유틸리티 ---

    public float GetRemainingCooldown(Skills skill)
    {
        switch (skill)
        {
            case Skills.tornado: return Mathf.Max(0f, curTornadoTime);
            case Skills.fire: return Mathf.Max(0f, curFireTime);
            case Skills.shield:
                if (isShieldActive) return 0f;
                return Mathf.Max(0f, curShieldTime);
            case Skills.bomb: return Mathf.Max(0f, curBombTime);
            case Skills.slowField: return Mathf.Max(0f, curSlowFieldTime);
            case Skills.heal: return Mathf.Max(0f, curHealTime); // heal 추가
            default: return 0f;
        }
    }

    public float GetCooldownDuration(Skills skill)
    {
        switch (skill)
        {
            case Skills.tornado: return tornadoCooldown;        
            case Skills.fire: return fireCooldown;
            case Skills.shield: return shieldCooldown;          
            case Skills.bomb: return bombCooldown;
            case Skills.slowField: return slowFieldCooldown;        
            case Skills.heal: return healCooldown; // heal 추가
            default: return 0f;
        }
    }

    public bool IsSkillOnCooldown(Skills skill)
    {
        return GetRemainingCooldown(skill) > 0f;
    }
}