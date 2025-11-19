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

    [Header("fire")]
    public GameObject fireEffect;

    private void Update()
    {
        tornado();
        fire();
    }
    public void tornado()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) 
            Instantiate(tornadoEffect, effectSpawn);
    }

    public void fire()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
            Instantiate(fireEffect, effectSpawn);
    }
}
