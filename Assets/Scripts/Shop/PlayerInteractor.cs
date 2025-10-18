using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : MonoBehaviour
{
    public KeyCode buyKey = KeyCode.F;

    private ShopItemSpawner nearbySpawner = null;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
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

        if (playerController.SpendGold(item.itemPrice))
        {
            // 구매 성공
            item.ApplyEffect(playerController);

            // UI 추가 (포션 제외)
            if (item.type != ShopItemType.HealthPotion)
                ShopUIManager.Instance.AddItemToLeftTop(item);

            // 모델 비활성화 및 재구매 방지
            spawner.DeactivateModel();
            ShopUIManager.Instance.HideInteract();
        }
        else
        {
            // 골드 부족 메시지
            ShopUIManager.Instance.ShowTempMessage("골드가 부족합니다.", 1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ShopItemSpawner sp = other.GetComponent<ShopItemSpawner>();
        if (sp != null && sp.assignedItem != null && !sp.isPurchased)
        {
            nearbySpawner = sp;
            ShopUIManager.Instance.ShowInteract($"F - 구매 {sp.assignedItem.itemName} ({sp.assignedItem.itemPrice}g)");
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