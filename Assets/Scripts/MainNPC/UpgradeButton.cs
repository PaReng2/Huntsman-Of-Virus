using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeButton : MonoBehaviour
{
    private PlayerAttackRangeDealer attackRangeDealer;
    private PlayerController player;
    private UIManager uiManager;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        uiManager = FindObjectOfType<UIManager>();
        attackRangeDealer = FindObjectOfType<PlayerAttackRangeDealer>();
    }

    public void HPUpgragde()
    {
        player.playerHP += 20;
        uiManager.HPBar.maxValue += 20;
        uiManager.curHP = player.playerHP;
    }
    public void AttackRateUpgrade()
    {
        attackRangeDealer.AttackRate -= 0.2f;
    }
}
