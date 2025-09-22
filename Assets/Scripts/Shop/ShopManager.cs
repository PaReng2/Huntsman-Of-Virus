using System.Collections.Generic;
using UnityEngine;

// 여러 개의 아이템 스포너를 충돌하지 않게 관리

public class ShopManager : MonoBehaviour
{
    public List<ShopItem> allItems; 
    public List<ShopItemSpawner> spawners;


    private void Start()
    {
        if (spawners == null || spawners.Count == 0)
            spawners = new List<ShopItemSpawner>(FindObjectsOfType<ShopItemSpawner>());


        AssignUniqueItemsToSpawners();
    }


    void AssignUniqueItemsToSpawners()
    {
        if (allItems == null || allItems.Count == 0) return;


        List<int> indices = new List<int>();
        for (int i = 0; i < allItems.Count; i++) indices.Add(i);
        for (int i = 0; i < indices.Count; i++)
        {
            int r = Random.Range(i, indices.Count);
            int tmp = indices[i]; indices[i] = indices[r]; indices[r] = tmp;
        }


        int assignCount = Mathf.Min(spawners.Count, allItems.Count);
        for (int i = 0; i < assignCount; i++)
        {
            spawners[i].assignedItem = allItems[indices[i]];
            spawners[i].isPurchased = false;
            spawners[i].SpawnModel();
        }


        for (int j = assignCount; j < spawners.Count; j++)
        {
            spawners[j].assignedItem = null;
            spawners[j].isPurchased = true; 
            if (spawners[j].spawnedModel != null) spawners[j].spawnedModel.SetActive(false);
        }
    }
}