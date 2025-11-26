using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueNPC : MonoBehaviour
{
    [Header("다이얼로그 순서")]
    public DialogueDataSO stDialogue;     // 이 NPC가 가지고 있는 대화 데이터 (ScriptableObject)
    public DialogueDataSO ndDialogue;
    public DialogueDataSO rdDialogue;
    public DialogueDataSO fourthDialogue;

    public GameObject isInsteraction;     // "상호작용 가능" UI 오브젝트
    private DialogueManager dialogueManager; // 씬에 존재하는 DialogueManager 참조
    public int dialogueNum;

    public bool CantAttack;

    private GameManager gameManager;
    void Awake()
    {
        // 씬에서 DialogueManager를 찾음
        dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager is Null"); // 없으면 에러 출력
        }
        dialogueNum = 1;
        CantAttack = true;
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        isDialogue();
        
        if (dialogueNum >= 3)
        {
            CantAttack = false;
        }
    }
    
    
    // 대화 시작 조건 체크
    void isDialogue()
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
        if (gameManager.isInteracting)
        {
            isInsteraction.SetActive(false);
        }


        // 플레이어가 근처에 있을 때만 대화 가능
        if (hasPlayer)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                gameManager.isInteracting = true;

                if (dialogueNum == 1)
                {
                    dialogueManager.StartDialogue(stDialogue);
                }
                else if (dialogueNum == 2)
                {
                    dialogueManager.StartDialogue(ndDialogue);
                }
                else if (dialogueNum == 3)
                {
                    dialogueManager.StartDialogue(rdDialogue);
                    
                }
                else if (dialogueNum == 4)
                {
                    dialogueManager.StartDialogue(fourthDialogue);
                    
                }
                else
                {
                    dialogueManager.StartDialogue(fourthDialogue);

                }
                dialogueNum++;
                Debug.Log(dialogueNum);

            }
           
        }
        
        
    }

    // 에디터에서 NPC 선택 시, 감지 범위(구체)를 시각적으로 보여줌
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 2f); // 반경 2짜리 초록 구체
    }
}
