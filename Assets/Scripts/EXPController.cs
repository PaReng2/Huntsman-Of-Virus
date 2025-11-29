using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXPController : MonoBehaviour
{
    private static EXPController instance;

    private PlayerController pc;
    public int curLevel;
    public int curEXP;

    private void Awake()
    {
        instance = this;
        if (instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        pc.currentLevel = curLevel;
        pc.currentExp = curEXP;
    }

    public void SaveLevel(int level)
    {
        curLevel = level;

    }

    public void SaveEXP(int EXP)
    {
        curEXP = EXP;

    }
}
