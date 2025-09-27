using UnityEngine;

// �÷��̾�� ���� ������ ���� ��ȣ�ۿ�

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
            // ���� ����
            item.ApplyEffect(playerStats);


            // UI�� ������ �߰� (health potion�� UI�� �߰� ����)
            if (item.type != ShopItemType.HealthPotion)
                ShopUIManager.Instance.AddItemToLeftTop(item);


            // �� ��Ȱ��ȭ �� �籸�� �Ұ� ó��
            spawner.DeactivateModel();


            // ���̻� ���ͷ�Ʈ �ؽ�Ʈ�� ������ �ʰ� ��
            ShopUIManager.Instance.HideInteract();
        }
        else
        {
            // ��� ���� �޽��� 1��
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