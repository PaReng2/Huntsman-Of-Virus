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

    [System.Serializable]
    public struct SkillToggle
    {
        public PlayerSkills.Skills skill;
        public bool unlock;
    }

    // 인스펙터에서 체크하여 어떤 스킬을 활성화시킬지 설정할 수 있음.
    public List<SkillToggle> skillUnlocks = new List<SkillToggle>();

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

        // 비체력형 아이템(영구효과)은 구매한 목록에 저장
        if (type != ShopItemType.HealthPotion)
        {
            if (PlayerProgress.purchasedItems == null)
                PlayerProgress.purchasedItems = new List<ShopItem>();

            if (!PlayerProgress.purchasedItems.Contains(this))
                PlayerProgress.purchasedItems.Add(this);
        }

        // --- 스킬 해제 적용 (PlayerSkills에 알리기) ---
        PlayerSkills ps = player.GetComponent<PlayerSkills>();
        if (ps != null && skillUnlocks != null)
        {
            foreach (var st in skillUnlocks)
            {
                if (st.unlock)
                {
                    ps.UnlockSkill(st.skill);
                    Debug.Log($"[ShopItem] {player.name} 에게 스킬 {st.skill} 잠금 해제 적용됨 ({itemName}).");
                }
            }
        }
    }
}