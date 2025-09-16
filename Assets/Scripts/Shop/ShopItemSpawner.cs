using UnityEngine;

// 상점 내에서 아이템들이 스폰될 위치

public class ShopItemSpawner : MonoBehaviour
{
    [HideInInspector] public ShopItem assignedItem; 
    [HideInInspector] public GameObject spawnedModel; 
    [HideInInspector] public bool isPurchased = false;


    public Transform modelParent; // 모델을 자식으로 붙일 부모 (옵션)


    // UI 텍스트 메시지 (예: "F to buy")는 UIManager를 통해 제어
    private void Awake()
    {
        // 스폰 포인트는 자신(transform)
    }


    // 모델을 실제로 씬에 생성
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