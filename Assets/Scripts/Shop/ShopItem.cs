using UnityEngine;

// �������� �Ǹ��� ������ (Generic : �Ϲ����� ������, HealthPotion : ȹ�� �� ȸ�� ȿ���� ��� ����� �� ������� ������)

public enum ShopItemType { Generic, HealthPotion }


[CreateAssetMenu(menuName = "Shop/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public ShopItemType type = ShopItemType.Generic;
    public int itemPrice = 10;


    // 3D �� ������
    public GameObject modelPrefab;


    // UI�� ��������Ʈ 
    public Sprite uiSprite;


    // ȿ����
    public int healAmount = 25;
    public int attackBonus = 10;


    public void ApplyEffect(PlayerStats player)
    {
        if (player == null) return;


        switch (type)
        {
            case ShopItemType.HealthPotion:
                player.Heal(healAmount);
                break;
            case ShopItemType.Generic:
                player.AddAttack(attackBonus);
                break;
        }
    }
}