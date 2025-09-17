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

        // 플레이어가 NPC 주변 2f 반경 안에 있는지 확인 (구체 범위로 충돌체 탐지)
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, playerLayer);

        // 탐지된 플레이어가 하나라도 있으면 true
        bool hasPlayer = colliders.Length > 0;

        // 플레이어 근처에 있으면 상호작용 UI 활성화, 아니면 비활성화
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
        Gizmos.DrawWireSphere(transform.position, 2f); // 반경 2짜리 초록 구체
    }
}
