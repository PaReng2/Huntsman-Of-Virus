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

    private void Update()
    {
        tornado();
    }
    public void tornado()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)) 
            Instantiate(tornadoEffect, effectSpawn);
    }

    public void fire()
    {

    }
}
