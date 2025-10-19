using UnityEngine;
using System.Collections.Generic;

public class DropItem : MonoBehaviour
{
    [Header("������ ������")]
    public List<ShopItem> possibleItems;
    private ShopItem assignedItem;      
    private GameObject spawnedModel;    

    private bool isCollected = false;

    [Header("�� ���� ��ġ")]
    public Transform modelParent; 

    private void Start()
    {
        AssignRandomItem();
        SpawnModel();
    }

    void AssignRandomItem()
    {
        if (possibleItems == null || possibleItems.Count == 0)
        {
            Debug.LogWarning("DropItem: possibleItems�� ��� �ֽ��ϴ�!");
            return;
        }

        int randIndex = Random.Range(0, possibleItems.Count);
        assignedItem = possibleItems[randIndex];
    }

    void SpawnModel()
    {
        if (assignedItem == null || assignedItem.modelPrefab == null)
        {
            Debug.LogWarning("DropItem: �� �������� �����ϴ�!");
            return;
        }

        if (spawnedModel != null) Destroy(spawnedModel);

        spawnedModel = Instantiate(assignedItem.modelPrefab, transform.position, Quaternion.identity);
        if (modelParent != null)
            spawnedModel.transform.SetParent(modelParent, true);
        else
            spawnedModel.transform.SetParent(transform, true);
    }

    public void Collect(PlayerController player)
    {
        if (isCollected || assignedItem == null) return;

        isCollected = true;

        assignedItem.ApplyEffect(player);

        if (assignedItem.type != ShopItemType.HealthPotion)
            ShopUIManager.Instance.AddItemToLeftTop(assignedItem);

        if (spawnedModel != null)
            spawnedModel.SetActive(false);

        ShopUIManager.Instance.HideInteract();

        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }

    public ShopItem GetAssignedItem() => assignedItem;
}