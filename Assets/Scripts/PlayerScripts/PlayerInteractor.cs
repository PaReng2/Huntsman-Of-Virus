using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.F;

    private ShopItemSpawner nearbySpawner = null;
    private DropItem nearbyDropItem = null;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // 상점 아이템 구매
        if (nearbySpawner != null && nearbySpawner.assignedItem != null && !nearbySpawner.isPurchased)
        {
            if (Input.GetKeyDown(interactKey))
                TryBuy(nearbySpawner);
        }

        // 드랍 아이템 획득
        if (nearbyDropItem != null)
        {
            if (Input.GetKeyDown(interactKey))
            {
                nearbyDropItem.Collect(playerController);
                nearbyDropItem = null;
            }
        }
    }

    private void TryBuy(ShopItemSpawner spawner)
    {
        ShopItem item = spawner.assignedItem;
        if (item == null) return;

        if (playerController.SpendGold(item.itemPrice))
        {
            item.ApplyEffect(playerController);
            if (item.type != ShopItemType.HealthPotion)
                ShopUIManager.Instance.AddItemToLeftTop(item);

            spawner.DeactivateModel();
            ShopUIManager.Instance.HideInteract();
        }
        else
        {
            ShopUIManager.Instance.ShowTempMessage("골드가 부족합니다.", 1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 상점 아이템
        ShopItemSpawner sp = other.GetComponent<ShopItemSpawner>();
        if (sp != null && sp.assignedItem != null && !sp.isPurchased)
        {
            nearbySpawner = sp;
            ShopUIManager.Instance.ShowInteract($"F - 구매 {sp.assignedItem.itemName} ({sp.assignedItem.itemPrice}g)");
            return;
        }

        // 드랍 아이템
        DropItem di = other.GetComponent<DropItem>();
        if (di != null)
        {
            nearbyDropItem = di;
            if (di.GetAssignedItem() != null)
                ShopUIManager.Instance.ShowInteract($"F - 아이템 획득 : {di.GetAssignedItem().itemName}");
            else
                ShopUIManager.Instance.ShowInteract("F - 아이템 획득");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ShopItemSpawner sp = other.GetComponent<ShopItemSpawner>();
        if (sp != null && sp == nearbySpawner)
        {
            nearbySpawner = null;
            ShopUIManager.Instance.HideInteract();
            return;
        }

        DropItem di = other.GetComponent<DropItem>();
        if (di != null && di == nearbyDropItem)
        {
            nearbyDropItem = null;
            ShopUIManager.Instance.HideInteract();
        }
    }
}