using System.Collections.Generic;
using UnityEngine;

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

    // 전체 진행(골드, 아이템, 증강, 레벨/EXP)을 초기화
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
    }
}