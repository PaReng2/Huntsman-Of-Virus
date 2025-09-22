using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{

    public GameObject isInsteraction;
    public ItemSO itemData;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetItem();

    }
    void GetItem()
    {
        InventoryManager manager = FindObjectOfType<InventoryManager>();
        int playerLayer = LayerMask.GetMask("Player");

        // �÷��̾ NPC �ֺ� 2f �ݰ� �ȿ� �ִ��� Ȯ�� (��ü ������ �浹ü Ž��)
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, playerLayer);

        // Ž���� �÷��̾ �ϳ��� ������ true
        bool hasPlayer = colliders.Length > 0;

        // �÷��̾� ��ó�� ������ ��ȣ�ۿ� UI Ȱ��ȭ, �ƴϸ� ��Ȱ��ȭ
        isInsteraction.SetActive(hasPlayer);
        if (hasPlayer)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Destroy(gameObject);
                isInsteraction.SetActive(false);
                manager.AddItem(itemData);

            }
        }
        
    }                                                                               

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 2f); // �ݰ� 2¥�� �ʷ� ��ü
    }
}
