using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Slider HPBar;
    
    private PlayerController player;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();
    }
    private void Start()
    {
        HPBar.maxValue = player.curPlayerHp;
        HPBar.minValue = 0f;
       
        HPBar.value = HPBar.maxValue;
        
    }

    private void Update()
    {
        HPBar.value = player.curPlayerHp; 
    }



}
