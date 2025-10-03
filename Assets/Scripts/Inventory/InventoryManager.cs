using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // ������ ������ ��ü�� �����մϴ�.
    public List<ItemSO> inventory = new List<ItemSO>();
    public Transform inventoryPanel;
    public GameObject inventoryItemPrefab;

    private int maxinventorySize = 7;

    public void AddItem(ItemSO itemData)
    {
        if (inventory.Count >= maxinventorySize)
        {
            return;
        }
        else
        {
            // ����Ʈ�� ������ ������ �߰�
            inventory.Add(itemData);

            GameObject newItemSlot = Instantiate(inventoryItemPrefab, inventoryPanel);
            Image itemImage = newItemSlot.GetComponent<Image>();

            if (itemImage != null && itemData.ItemIMG != null)
            {
                itemImage.sprite = itemData.ItemIMG;
            }
        }
    }
}