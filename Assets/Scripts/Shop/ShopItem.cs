using UnityEngine;
using System.Collections.Generic;

public enum ShopItemType { Generic, HealthPotion }

[CreateAssetMenu(menuName = "Shop/ShopItem")]
public class ShopItem : ScriptableObject
{
    public string itemName;
    public ShopItemType type = ShopItemType.Generic;
    public int itemPrice = 10;

    // 3D 모델 프리팹
    public GameObject modelPrefab;

    // UI용 스프라이트
    public Sprite uiSprite;

    // 효과값
    public int healAmount = 25;
    public int attackBonus = 10;

    public void ApplyEffect(PlayerController player)
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

        if (type != ShopItemType.HealthPotion)
        {
            if (PlayerProgress.purchasedItems == null)
                PlayerProgress.purchasedItems = new List<ShopItem>();

            if (!PlayerProgress.purchasedItems.Contains(this))
                PlayerProgress.purchasedItems.Add(this);
        }
    }
}