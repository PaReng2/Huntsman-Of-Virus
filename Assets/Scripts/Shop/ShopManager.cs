using System.Collections.Generic;
using UnityEngine;

// ���� ���� ������ �����ʸ� �浹���� �ʰ� (�ߺ����� �ʰ�) �����ϴ� ��ũ��Ʈ
public class ShopManager : MonoBehaviour
{
    public List<ShopItem> allItems; // �������� ����ϴ� ��� ������ ��� (Inspector���� ����)
    public List<ShopItemSpawner> spawners;  // �������� ������ ����/��ġ�ϴ� ������ ������Ʈ���� ���


    private void Start()
    {
        // spawners ����Ʈ�� ����ְų� null�� ���, ������ ��� ShopItemSpawner ������Ʈ�� ã�Ƽ� �Ҵ�
        if (spawners == null || spawners.Count == 0)
            spawners = new List<ShopItemSpawner>(FindObjectsOfType<ShopItemSpawner>());


        // �����ʿ� �ߺ����� �ʴ� �������� �Ҵ��ϴ� �Լ� ȣ��
        AssignUniqueItemsToSpawners();
    }


    // ��� �����ʿ� �ߺ����� �ʴ� �������� �Ҵ��ϰ� ������Ű�� �޼���
    void AssignUniqueItemsToSpawners()
    {
        // ������ ����� ���ų� ��������� �Ҵ� �۾��� �ߴ��ϰ� ����
        if (allItems == null || allItems.Count == 0) return;


        // 1. ������ �ε��� ��� ���� �� ���� (����)

        List<int> indices = new List<int>();                    // ������ �ε����� ���� ����Ʈ
        for (int i = 0; i < allItems.Count; i++) indices.Add(i); // 0���� allItems.Count-1 ������ �ε��� �߰� (ex: 0, 1, 2, ...)

        // Fisher-Yates ���� �˰����� ����Ͽ� �ε��� ����Ʈ�� �������� ����
        for (int i = 0; i < indices.Count; i++)
        {
            int r = Random.Range(i, indices.Count);             // ���� �ε���(i)���� ����Ʈ �� ������ ���� �ε��� r�� ����
            int tmp = indices[i];                               // ���� �ε��� i�� ���� �ӽ� ����
            indices[i] = indices[r];                            // i ��ġ�� ���� ��ġ r�� ���� �Ҵ� (��ȯ)
            indices[r] = tmp;                                   // r ��ġ�� ���� i ��ġ�� �� �Ҵ� (��ȯ �Ϸ�)
        }
        // �̷ν� indices ����Ʈ���� ������ ����� �ε������� ������ ������ ���� ��


        // 2. ������ �Ҵ� �� ����

        // ������ �������� �Ҵ��� ������/�������� ������ ���� (������ ������ ������ ���� �� ���� ��)
        int assignCount = Mathf.Min(spawners.Count, allItems.Count);

        // ������ ����(assignCount)��ŭ�� �����ʿ� ���õ� �ε��� ������� ������ �Ҵ�
        for (int i = 0; i < assignCount; i++)
        {
            // i��° �����ʿ� ���õ� indices[i]�� �ش��ϴ� ������ �Ҵ� (�ߺ� ����)
            spawners[i].assignedItem = allItems[indices[i]];
            spawners[i].isPurchased = false; // ���ŵ��� ���� ���·� ����
            spawners[i].SpawnModel();        // �Ҵ�� ������ ���� ���� (���� ���� ������Ʈ ����)
        }


        // 3. ���� ������ ó�� (������ �Ҵ���� ���� ������)

        // �������� �Ҵ���� ���� ������ �����ʵ� ó��
        for (int j = assignCount; j < spawners.Count; j++)
        {
            spawners[j].assignedItem = null;    // �Ҵ�� �������� ������ ���
            spawners[j].isPurchased = true;     // �������� �����Ƿ� '���ŵ�' ��ó��(��Ȱ��ȭ) ó��
            // �̹� ������ ���� �ִٸ� ��Ȱ��ȭ ó�� (������ ������ �ܿ� ���� ���� ��츦 ���)
            if (spawners[j].spawnedModel != null) spawners[j].spawnedModel.SetActive(false);
        }
    }
}