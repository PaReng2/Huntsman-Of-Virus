using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [Header("ÆÄ±« ½Ã »ý¼ºÇÒ ÇÁ¸®ÆÕ")]
    public GameObject prefabToSpawn; 
    public bool inheritRotation = false; 

    private void OnDestroy()
    {
        if (!Application.isPlaying) return;

        if (prefabToSpawn != null)
        {
            Vector3 spawnPos = transform.position;
            Quaternion spawnRot = inheritRotation ? transform.rotation : Quaternion.identity;

            Instantiate(prefabToSpawn, spawnPos, spawnRot);
        }
    }
}