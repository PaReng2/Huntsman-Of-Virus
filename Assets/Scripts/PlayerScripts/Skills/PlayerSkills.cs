using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkills : MonoBehaviour
{
    public enum Skills
    {
        tornado,
        fire
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
}
