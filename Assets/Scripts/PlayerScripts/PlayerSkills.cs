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
    Vector3 force;

    public void tornado()
    {
        Instantiate(tornadoEffect,effectSpawn);
    }

    public void fire()
    {

    }
}
