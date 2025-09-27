using UnityEngine;

// ���� ������ �����۵��� ������ ��ġ

public class ShopItemSpawner : MonoBehaviour
{
    [HideInInspector] public ShopItem assignedItem; 
    [HideInInspector] public GameObject spawnedModel; 
    [HideInInspector] public bool isPurchased = false;


    public Transform modelParent; // ���� �ڽ����� ���� �θ� (�ɼ�)


    // UI �ؽ�Ʈ �޽��� (��: "F to buy")�� UIManager�� ���� ����
    private void Awake()
    {
        // ���� ����Ʈ�� �ڽ�(transform)
    }


    // ���� ������ ���� ����
    public void SpawnModel()
    {
        if (assignedItem == null || assignedItem.modelPrefab == null) return;


        if (spawnedModel != null) Destroy(spawnedModel);


        spawnedModel = Instantiate(assignedItem.modelPrefab, transform.position, Quaternion.identity);
        if (modelParent != null)
            spawnedModel.transform.SetParent(modelParent, true);
    }


    public void DeactivateModel()
    {
        if (spawnedModel != null)
        {
            spawnedModel.SetActive(false);
        }
        isPurchased = true;
    }
}