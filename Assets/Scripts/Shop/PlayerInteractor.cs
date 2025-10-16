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
            // ���� ����
            item.ApplyEffect(playerController);

            // UI �߰� (���� ����)
            if (item.type != ShopItemType.HealthPotion)
                ShopUIManager.Instance.AddItemToLeftTop(item);

            // �� ��Ȱ��ȭ �� �籸�� ����
            spawner.DeactivateModel();
            ShopUIManager.Instance.HideInteract();
        }
        else
        {
            // ��� ���� �޽���
            ShopUIManager.Instance.ShowTempMessage("��尡 �����մϴ�.", 1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ShopItemSpawner sp = other.GetComponent<ShopItemSpawner>();
        if (sp != null && sp.assignedItem != null && !sp.isPurchased)
        {
            nearbySpawner = sp;
            ShopUIManager.Instance.ShowInteract($"F - ���� {sp.assignedItem.itemName} ({sp.assignedItem.itemPrice}g)");
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