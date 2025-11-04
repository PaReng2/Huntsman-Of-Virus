using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName ="Stage/Wave")]
public class Wave : ScriptableObject
{
    public GameObject enemyPrefab;  // 이 웨이브에서 스폰할 적 프리팹
    public int count;               // 스폰할 적의 수
    public float rate;              // 적 스폰 간격 
}
