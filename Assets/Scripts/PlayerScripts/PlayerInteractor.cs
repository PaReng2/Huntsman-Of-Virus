using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerController))]
public class PlayerInteractor : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.F;
    public float interactRange = 2f; // ���� �Ÿ�
    public LayerMask interactLayer;  // ������ ���̾� (��: Item, Shop ��)

    private PlayerController playerController;
    private DropItem nearbyDropItem;
    private ShopItemSpawner nearbySpawner;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        DetectNearbyObjects();

        if (Input.GetKeyDown(interactKey))
        {
            if (nearbyDropItem != null)
            {
                nearbyDropItem.Collect(playerController);
                nearbyDropItem = null;
                ShopUIManager.Instance.HideInteract();
                return;
            }

            if (nearbySpawner != null && nearbySpawner.assignedItem != null && !nearbySpawner.isPurchased)
            {
                TryBuy(nearbySpawner);
                nearbySpawner = null;
                return;
            }
        }
    }

    private void DetectNearbyObjects()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactLayer);
        DropItem foundDrop = null;
        ShopItemSpawner foundShop = null;

        foreach (var hit in hits)
        {
            DropItem di = hit.GetComponent<DropItem>();
            if (di != null)
            {
                foundDrop = di;
                break;
            }

            ShopItemSpawner sp = hit.GetComponent<ShopItemSpawner>();
            if (sp != null && sp.assignedItem != null && !sp.isPurchased)
            {
                foundShop = sp;
                break;
            }
        }

        // UI ǥ�� ����
        if (foundDrop != nearbyDropItem)
        {
            Debug.Log("��ó DropItem ����");
            nearbyDropItem = foundDrop;
            if (foundDrop != null)
            {
                var item = foundDrop.GetAssignedItem();
                string msg = item != null ? $"F - ������ ȹ�� : {item.itemName}" : "F - ������ ȹ��";
                ShopUIManager.Instance.ShowInteract(msg);
            }
            else if (nearbySpawner == null)
                ShopUIManager.Instance.HideInteract();
        }

        if (foundShop != nearbySpawner)
        {
            Debug.Log("��ó ShopItem ����");
            nearbySpawner = foundShop;
            if (foundShop != null)
            {
                ShopUIManager.Instance.ShowInteract($"F - ���� {foundShop.assignedItem.itemName} ({foundShop.assignedItem.itemPrice}g)");
            }
            else if (nearbyDropItem == null)
                ShopUIManager.Instance.HideInteract();
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
            ShopUIManager.Instance.ShowTempMessage("��尡 �����մϴ�.", 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}