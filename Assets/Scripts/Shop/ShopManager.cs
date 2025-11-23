using System.Collections.Generic;
using UnityEngine;

// 여러 개의 아이템 스포너를 중복 없이 관리
public class ShopManager : MonoBehaviour
{
    public List<ShopItem> allItems;              // 상점에서 취급하는 전체 아이템
    public List<ShopItemSpawner> spawners;       // 씬에 배치된 스포너들 (4개)

    private void Start()
    {
        // 스포너 리스트를 안 채워놨으면 씬에서 자동으로 찾아서 채움
        if (spawners == null || spawners.Count == 0)
            spawners = new List<ShopItemSpawner>(FindObjectsOfType<ShopItemSpawner>());

        AssignUniqueItemsToSpawners();
    }

    /// <summary>
    /// - 동일한 아이템은 한 번만 등장
    /// - PlayerProgress에서 이미 구매한 아이템은 후보에서 제외
    /// - 남은 아이템 수가 스포너 수보다 적으면 나머지 스포너는 비워둠
    /// </summary>
    private void AssignUniqueItemsToSpawners()
    {
        if (allItems == null || allItems.Count == 0) return;

        // PlayerProgress.purchasedItems 방어코드
        if (PlayerProgress.purchasedItems == null)
            PlayerProgress.purchasedItems = new List<ShopItem>();

        // 아직 구매하지 않은 아이템만 후보로 모으기
        List<ShopItem> notPurchased = new List<ShopItem>();
        foreach (var item in allItems)
        {
            if (item == null) continue;
            if (!PlayerProgress.purchasedItems.Contains(item))
                notPurchased.Add(item);
        }

        // 구매하지 않은 아이템이 하나도 없으면 모든 스포너를 비워둔다
        if (notPurchased.Count == 0)
        {
            foreach (var spawn in spawners)
            {
                if (spawn == null) continue;
                spawn.assignedItem = null;
                spawn.isPurchased = true;
                if (spawn.spawnedModel != null)
                    spawn.spawnedModel.SetActive(false);
            }
            return;
        }

        // 후보 아이템 섞기 (Fisher-Yates)
        for (int i = 0; i < notPurchased.Count; i++)
        {
            int r = Random.Range(i, notPurchased.Count);
            ShopItem tmp = notPurchased[i];
            notPurchased[i] = notPurchased[r];
            notPurchased[r] = tmp;
        }

        // 실제로 배치할 개수 = min(스포너 수, 남은 아이템 수)
        int assignCount = Mathf.Min(spawners.Count, notPurchased.Count);

        // 스포너에 아이템 할당
        for (int i = 0; i < spawners.Count; i++)
        {
            ShopItemSpawner spawner = spawners[i];
            if (spawner == null) continue;

            if (i < assignCount)
            {
                // 남은 아이템들 중에서 하나씩만 사용 (동일 아이템 재등장 X)
                spawner.assignedItem = notPurchased[i];
                spawner.isPurchased = false;
                spawner.SpawnModel();
            }
            else
            {
                // 남는 스포너는 비워두기
                spawner.assignedItem = null;
                spawner.isPurchased = true;
                if (spawner.spawnedModel != null)
                    spawner.spawnedModel.SetActive(false);
            }
        }
    }
}