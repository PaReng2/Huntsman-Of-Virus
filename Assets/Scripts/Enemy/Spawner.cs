using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemy;                // ������ �� ������
    public Transform spawnPoint;            // ���� ��ġ
    public float spawnInterval = 3f;        // ���� �ֱ� (�� ����)
    public int maxSpawnCount = 5;           // �ִ� ��ȯ ����

    private int currentSpawnCount = 0;      // ���ݱ��� ��ȯ�� ����
    private bool interacted = false;        // �̹� ��ȣ�ۿ� �ߴ��� üũ

    void Start()
    {
        StageManager.Instance.SetTotalEnemyCount(maxSpawnCount);
    }
    void Update()
    {
        // �����̽� Ű�� ������ �� (�� ���� ����ǵ��� ���� �߰�)
        if (!interacted && Input.GetKeyDown(KeyCode.Space))
        {
            OnInteract();
            interacted = true; // �ߺ� ���� ����
        }
    }
    public void OnInteract()
    {

        // ���� �������� �ݺ� ��ȯ ����
        InvokeRepeating(nameof(SpawnItem), 0f, spawnInterval);
    }

    void SpawnItem()
    {
        if (currentSpawnCount < maxSpawnCount)
        {
            GameObject newEnemy = Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            StageManager.Instance.OnEnemySpawned(); // ���� ���� ������ ī��Ʈ ����
            currentSpawnCount++;
        }
        else
        {
            CancelInvoke(nameof(SpawnItem));
        }
    }
}
