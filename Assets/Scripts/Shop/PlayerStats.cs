using UnityEngine;

// �÷��̾��� �⺻���� ���� (�߰� �� ���� ����)

public class PlayerStats : MonoBehaviour
{
    public int gold = 100;
    public int hp = 80;
    public int maxHp = 100;
    public int attack = 10;


    public void AddGold(int amount) => gold += amount;
    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }


    public void Heal(int amount)
    {
        hp = Mathf.Min(maxHp, hp + amount);
    }


    public void AddAttack(int amount)
    {
        attack += amount;
    }
}