using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider HPBar;
    private int curHP;
    private PlayerController player;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();
    }
    private void Start()
    {
        HPBar.maxValue = player.playerHP;
        HPBar.minValue = 0f;
        curHP = player.playerHP;
        HPBar.value = HPBar.maxValue;
        
    }

    private void Update()
    {
        HPBar.value = curHP; 
    }



}
