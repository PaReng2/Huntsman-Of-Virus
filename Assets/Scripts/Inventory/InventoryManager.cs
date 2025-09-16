using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<int> inventory = new List<int>();
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
            GameObject newItemSlot = Instantiate(inventoryItemPrefab, inventoryPanel);

            Image itemImage = newItemSlot.GetComponent<Image>();

            if (itemImage != null && itemData.ItemIMG != null)
            {
                itemImage.sprite = itemData.ItemIMG;
            }
            inventory.Add(1);
        }
    }

}
