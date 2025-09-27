using UnityEngine;

// 플레이어와 상점 아이템 간의 상호작용

[RequireComponent(typeof(PlayerShopStats))]
public class PlayerInteractor : MonoBehaviour
{
    public KeyCode buyKey = KeyCode.F;


    private ShopItemSpawner nearbySpawner = null;
    private PlayerStats playerStats;


    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }


    private void Update()
    {
        if (nearbySpawner != null && nearbySpawner.assignedItem != null && !nearbySpawner.isPurchased)
        {
            if (Input.GetKeyDown(buyKey))
            {
                TryBuy(nearbySpawner);
            }
        }
    }


    private void TryBuy(ShopItemSpawner spawner)
    {
        ShopItem item = spawner.assignedItem;
        if (item == null) return;


        if (playerStats.SpendGold(item.itemPrice))
        {
            // 구매 성공
            item.ApplyEffect(playerStats);


            // UI에 아이템 추가 (health potion은 UI에 추가 안함)
            if (item.type != ShopItemType.HealthPotion)
                ShopUIManager.Instance.AddItemToLeftTop(item);


            // 모델 비활성화 및 재구매 불가 처리
            spawner.DeactivateModel();


            // 더이상 인터랙트 텍스트를 보이지 않게 함
            ShopUIManager.Instance.HideInteract();
        }
        else
        {
            // 골드 부족 메시지 1초
            ShopUIManager.Instance.ShowTempMessage("Not enough gold", 1f);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        ShopItemSpawner sp = other.GetComponent<ShopItemSpawner>();
        if (sp != null && sp.assignedItem != null && !sp.isPurchased)
        {
            nearbySpawner = sp;
            ShopUIManager.Instance.ShowInteract($"F - Buy {sp.assignedItem.itemName} ({sp.assignedItem.itemPrice}g)");
        }
    }


    private void OnTriggerExit(Collider other)
    {
        ShopItemSpawner sp = other.GetComponent<ShopItemSpawner>();
        if (sp != null && sp == nearbySpawner)
        {
            nearbySpawner = null;
            ShopUIManager.Instance.HideInteract();
        }
    }
}