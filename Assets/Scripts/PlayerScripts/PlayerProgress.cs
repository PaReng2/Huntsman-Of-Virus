using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 진행도 저장소 (씬간 유지용)
/// 이제 장애물 정보도 여기서 저장/관리합니다.
/// </summary>
public static class PlayerProgress
{
    // 씬을 오가며 유지할 골드
    public static int savedGold = 0;

    // 지금까지 구매한 상점 아이템들 
    public static List<ShopItem> purchasedItems = new List<ShopItem>();

    // 증강 레벨 저장 (인덱스: 0=MAXHP, 1=ATK, 2=SPD, 3=ATKSPD, 4=RANGE)
    public static int[] savedAugmentLevels = new int[5];

    // 씬간 유지할 플레이어 레벨/경험치 ---
    public static int savedLevel = 1;
    public static int savedExp = 0;

    // 씬 이름 -> 해당 씬에서 스폰된 장애물 레코드 리스트
    public static Dictionary<string, List<ObstacleRecord>> savedObstacles = new Dictionary<string, List<ObstacleRecord>>();

    /// 장애물 정보를 저장할 데이터 타입
    /// prefabIndex: 어떤 프리팹
    /// position, rotation: 월드 좌표
    [System.Serializable]
    public class ObstacleRecord
    {
        public int prefabIndex;
        public Vector3 position;
        public Quaternion rotation;

        public ObstacleRecord(int prefabIndex, Vector3 position, Quaternion rotation)
        {
            this.prefabIndex = prefabIndex;
            this.position = position;
            this.rotation = rotation;
        }
    }

    /// 씬 이름과 레코드를 전달하면 저장
    public static void AddObstacleRecord(string sceneName, ObstacleRecord rec)
    {
        if (!savedObstacles.ContainsKey(sceneName))
            savedObstacles[sceneName] = new List<ObstacleRecord>();

        savedObstacles[sceneName].Add(rec);
    }

    /// 특정 씬의 장애물 리스트(복사)를 반환. 씬에 기록이 없으면 빈 리스트 반환
    public static List<ObstacleRecord> GetObstacleRecords(string sceneName)
    {
        if (!savedObstacles.ContainsKey(sceneName))
            return new List<ObstacleRecord>();
        return new List<ObstacleRecord>(savedObstacles[sceneName]);
    }

    /// 특정 씬의 저장된 장애물 레코드를 모두 제거
    public static void ClearObstacleRecordsForScene(string sceneName)
    {
        if (savedObstacles.ContainsKey(sceneName))
            savedObstacles.Remove(sceneName);
    }

    public static void ClearAllObstacleRecords()
    {
        savedObstacles.Clear();
    }

    // 전체 진행(골드, 아이템, 증강, 레벨/EXP, 장애물)을 초기화
    public static void ResetAllProgress()
    {
        Debug.Log("모든 진행도 초기화됨!");

        savedGold = 0;
        if (purchasedItems == null)
            purchasedItems = new List<ShopItem>();
        else
            purchasedItems.Clear();

        if (savedAugmentLevels == null || savedAugmentLevels.Length != 5)
            savedAugmentLevels = new int[5];

        for (int i = 0; i < savedAugmentLevels.Length; i++)
            savedAugmentLevels[i] = 0;

        savedLevel = 1;
        savedExp = 0;

        // 장애물 정보도 초기화
        ClearAllObstacleRecords();
    }
}