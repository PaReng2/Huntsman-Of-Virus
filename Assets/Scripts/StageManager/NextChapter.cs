using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextChapter : MonoBehaviour
{
    [Header("넘어갈 챕터 작성 // 1 - 튜토리얼, 2 - 챕터1, 3 - 상점")]
    public int chapterNum;

    public GameObject isInsteraction;     // "상호작용 가능" UI 오브젝트
    

    private void Update()
    {
        GoNext();

    }

    void GoNext()
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

        if(hasPlayer && Input.GetKeyDown(KeyCode.F))
            SceneManager.LoadScene(chapterNum);

    }

}
