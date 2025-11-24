using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementNPC : MonoBehaviour
{

    [Header("상호작용 텍스트")]
    public GameObject isInsteraction;     // "상호작용 가능" UI 오브젝트
    private PlayerController player;
    public GameObject UpgradePanel;
    private GameManager gameManager;
    

    void Start()
    {
        UpgradePanel.SetActive(false);
        player = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        OpenAchievementPannel();
    }

    void OpenAchievementPannel()
    {
        PlayerAttackRangeDealer rangeDealer = FindObjectOfType<PlayerAttackRangeDealer>();
        // "Player"라는 레이어를 감지할 마스크 가져오기
        int playerLayer = LayerMask.GetMask("Player");

        // 플레이어가 NPC 주변 2f 반경 안에 있는지 확인 (구체 범위로 충돌체 탐지)
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3f, playerLayer);

        // 탐지된 플레이어가 하나라도 있으면 true
        bool hasPlayer = colliders.Length > 0;

        // 플레이어 근처에 있으면 상호작용 UI 활성화, 아니면 비활성화
        if(!gameManager.isInteracting)
        {
            isInsteraction.SetActive(hasPlayer);

        }

        if (hasPlayer)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isInsteraction.SetActive(false);
                UpgradePanel.SetActive(true);
                gameManager.isInteracting = true;
                Debug.Log($"업적 창 표시 {gameManager.isInteracting}");

            }
            if (gameManager.isInteracting && Input.GetKeyDown(KeyCode.Escape))
            {
                UpgradePanel.SetActive(false);
                gameManager.isInteracting = false;

                Debug.Log("업적 창 꺼짐");
            }
        }
    }

    

    
}
