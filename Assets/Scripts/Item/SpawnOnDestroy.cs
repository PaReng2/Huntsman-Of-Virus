using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [Header("파괴 시 생성할 프리팹")]
    public GameObject prefabToSpawn;

    [Header("프리팹 생성 확률 (0~1 사이)")]
    [Range(0f, 1f)]
    public float spawnChance = 0.1f;  

    [Header("회전 상속 여부")]
    public bool inheritRotation = false;

    private void OnDestroy()
    {
        if (!Application.isPlaying) return;
       
        float rand = Random.value;
        if (rand > spawnChance) return;
        
        if (prefabToSpawn != null)
        {
            Vector3 spawnPos = transform.position;
            Quaternion spawnRot = inheritRotation ? transform.rotation : Quaternion.identity;

            Instantiate(prefabToSpawn, spawnPos, spawnRot);
        }
    }
}