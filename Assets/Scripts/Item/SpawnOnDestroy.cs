using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [Header("�ı� �� ������ ������")]
    public GameObject prefabToSpawn;

    [Header("������ ���� Ȯ�� (0~1 ����)")]
    [Range(0f, 1f)]
    public float spawnChance = 0.1f;  

    [Header("ȸ�� ��� ����")]
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