using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;   

public class UIManager : MonoBehaviour
{
    [Header("HP UI")]
    public Slider HPBar;

    [Header("Stat Panel Texts")]
    public TMP_Text maxHpText;   
    public TMP_Text atkText;     
    public TMP_Text spdText;     
    public TMP_Text rangeText;   
    public TMP_Text atkSpdText;  

    private PlayerController player;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    private void Start()
    {
        if (player == null || HPBar == null) return;

        HPBar.minValue = 0f;
        HPBar.maxValue = player.playerMaxHP;   
        HPBar.value = player.curPlayerHp;
    }

    private void Update()
    {
        if (player == null) return;

        UpdateHPBar();
        UpdateStatTexts();
    }

    private void UpdateHPBar()
    {
        if (HPBar == null) return;

        HPBar.maxValue = player.playerMaxHP;
        HPBar.value = player.curPlayerHp;
    }

    private void UpdateStatTexts()
    {
        if (maxHpText != null)
            maxHpText.text = $"MAXHP : {player.playerMaxHP}";

        if (atkText != null)
            atkText.text = $"ATK : {player.attackPower:F1}";

        if (spdText != null)
            spdText.text = $"SPD : {player.playerMoveSpeed:F1}";

        if (rangeText != null)
            rangeText.text = $"RANGE : {player.attackRange:F1}";

        float atkSpdValue = 0f;
        if (player.attackDelay > 0.0001f)
            atkSpdValue = 1f / player.attackDelay;  

        if (atkSpdText != null)
            atkSpdText.text = $"ATKSPD : {atkSpdValue:F1}";
    }
}