using UnityEngine;
using TMPro;

// 플레이어의 기본적인 스탯 (추가 및 수정 가능)


public class PlayerStats : MonoBehaviour
{
    public int gold = 0;
    public int hp = 100;
    public int maxHp = 100;
    public int attack = 10;

    [Header("UI")]
    public TMP_Text goldText; 

    private void Start()
    {
        UpdateGoldUI();
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateGoldUI();
            return true;
        }
        return false;
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = "Gold : " + gold;
        }
    }

    public void Heal(int amount)
    {
        hp += amount;
        if (hp > maxHp)
            hp = maxHp;
    }

    public void AddAttack(int amount)
    {
        attack += amount;
    }
}