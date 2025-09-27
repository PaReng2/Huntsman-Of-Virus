using UnityEngine;

// 상점에서 판매할 아이템 (Generic : 일반적인 아이템, HealthPotion : 획득 시 회복 효과만 즉시 적용된 뒤 사라지는 아이템)

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