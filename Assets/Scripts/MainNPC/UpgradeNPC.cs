using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeNPC : MonoBehaviour
{

    [Header("상호작용 텍스트")]
    public GameObject isInsteraction;     // "상호작용 가능" UI 오브젝트

    public GameObject UpgradePanel;
    private PlayerController player;
    private PlayerAttackRangeDealer attackRangeDealer;
    

    void Start()
    {
        UpgradePanel.SetActive(false);
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        
    }

    void IsUpgrading()
    {
        PlayerAttackRangeDealer rangeDealer = FindObjectOfType<PlayerAttackRangeDealer>();
        // "Player"라는 레이어를 감지할 마스크 가져오기
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
                UpgradePanel.SetActive(true);
            }
        }
    }

    void HPUpgragde()
    {
        player.playerHP += 20;
    }

    void AttackRateUpgrade()
    {
        attackRangeDealer.AttackRate -= 0.2f;
    }
}
