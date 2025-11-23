using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WaveProgress
{
    // 마지막으로 클리어한 웨이브 인덱스 (0부터 시작)
    // 아무 웨이브도 클리어하지 않았다면 -1
    public static int lastClearedWaveIndex = -1;

    // 웨이브 정보를 초기화 (1웨이브부터 시작하게)
    public static void Reset()
    {
        lastClearedWaveIndex = -1;
    }
}
