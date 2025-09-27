using System.Collections.Generic;
using UnityEngine;

// 여러 개의 아이템 스포너를 충돌하지 않게 (중복되지 않게) 관리하는 스크립트
public class ShopManager : MonoBehaviour
{
    public List<ShopItem> allItems; // 상점에서 취급하는 모든 아이템 목록 (Inspector에서 설정)
    public List<ShopItemSpawner> spawners;  // 아이템을 실제로 생성/배치하는 스포너 오브젝트들의 목록


    private void Start()
    {
        // spawners 리스트가 비어있거나 null인 경우, 씬에서 모든 ShopItemSpawner 컴포넌트를 찾아서 할당
        if (spawners == null || spawners.Count == 0)
            spawners = new List<ShopItemSpawner>(FindObjectsOfType<ShopItemSpawner>());


        // 스포너에 중복되지 않는 아이템을 할당하는 함수 호출
        AssignUniqueItemsToSpawners();
    }


    // 모든 스포너에 중복되지 않는 아이템을 할당하고 스폰시키는 메서드
    void AssignUniqueItemsToSpawners()
    {
        // 아이템 목록이 없거나 비어있으면 할당 작업을 중단하고 리턴
        if (allItems == null || allItems.Count == 0) return;


        // 1. 아이템 인덱스 목록 생성 및 섞기 (셔플)

        List<int> indices = new List<int>();                    // 아이템 인덱스를 담을 리스트
        for (int i = 0; i < allItems.Count; i++) indices.Add(i); // 0부터 allItems.Count-1 까지의 인덱스 추가 (ex: 0, 1, 2, ...)

        // Fisher-Yates 셔플 알고리즘을 사용하여 인덱스 리스트를 무작위로 섞음
        for (int i = 0; i < indices.Count; i++)
        {
            int r = Random.Range(i, indices.Count);             // 현재 인덱스(i)부터 리스트 끝 사이의 랜덤 인덱스 r을 선택
            int tmp = indices[i];                               // 현재 인덱스 i의 값을 임시 저장
            indices[i] = indices[r];                            // i 위치에 랜덤 위치 r의 값을 할당 (교환)
            indices[r] = tmp;                                   // r 위치에 원래 i 위치의 값 할당 (교환 완료)
        }
        // 이로써 indices 리스트에는 아이템 목록의 인덱스들이 무작위 순서로 담기게 됨


        // 2. 아이템 할당 및 스폰

        // 실제로 아이템을 할당할 스포너/아이템의 개수를 결정 (스포너 개수와 아이템 개수 중 작은 값)
        int assignCount = Mathf.Min(spawners.Count, allItems.Count);

        // 결정된 개수(assignCount)만큼의 스포너에 셔플된 인덱스 순서대로 아이템 할당
        for (int i = 0; i < assignCount; i++)
        {
            // i번째 스포너에 셔플된 indices[i]에 해당하는 아이템 할당 (중복 방지)
            spawners[i].assignedItem = allItems[indices[i]];
            spawners[i].isPurchased = false; // 구매되지 않은 상태로 설정
            spawners[i].SpawnModel();        // 할당된 아이템 모델을 스폰 (실제 게임 오브젝트 생성)
        }


        // 3. 남은 스포너 처리 (아이템 할당되지 않은 스포너)

        // 아이템이 할당되지 않은 나머지 스포너들 처리
        for (int j = assignCount; j < spawners.Count; j++)
        {
            spawners[j].assignedItem = null;    // 할당된 아이템이 없음을 명시
            spawners[j].isPurchased = true;     // 아이템이 없으므로 '구매된' 것처럼(비활성화) 처리
            // 이미 스폰된 모델이 있다면 비활성화 처리 (이전에 스폰된 잔여 모델이 있을 경우를 대비)
            if (spawners[j].spawnedModel != null) spawners[j].spawnedModel.SetActive(false);
        }
    }
}