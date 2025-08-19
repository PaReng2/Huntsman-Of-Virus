using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueNPC : MonoBehaviour
{
    public DialogueDataSO myDialogue;     // 이 NPC가 가지고 있는 대화 데이터 (ScriptableObject)
    public GameObject isInsteraction;     // "상호작용 가능" UI 오브젝트

    [Header("퀘스트용")] 
    public DialogueDataSO beforeQuest;
    public DialogueDataSO afterQuest;
    
    private DialogueManager dialogueManager; // 씬에 존재하는 DialogueManager 참조
    
    void Start()
    {
        // 씬에서 DialogueManager를 찾음
        dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager is Null"); // 없으면 에러 출력
        }
    }

    private void Update()
    {
        // "Player"라는 레이어를 감지할 마스크 가져오기
        int playerLayer = LayerMask.GetMask("Player");
        
        // 플레이어가 NPC 주변 2f 반경 안에 있는지 확인 (구체 범위로 충돌체 탐지)
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, playerLayer);
        
        // 탐지된 플레이어가 하나라도 있으면 true
        bool hasPlayer = colliders.Length > 0;

        // 플레이어 근처에 있으면 상호작용 UI 활성화, 아니면 비활성화
        isInsteraction.SetActive(hasPlayer);

        // 플레이어가 근처에 있을 때만 대화 가능
        if (hasPlayer)
        {
            isDialogue();
        }
    }

    public void GiveQuest()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            /*if()*/
        }
    }
    
    // 대화 시작 조건 체크
    void isDialogue()
    {
        // F키를 누르면 대화 시작
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (dialogueManager == null) return;           // DialogueManager 없음 -> 리턴
            if (dialogueManager.IsDialogueActive()) return; // 이미 대화 중이면 리턴
            if (myDialogue == null) return;                // 대화 데이터 없음 -> 리턴
            
            // 모든 조건 만족하면 DialogueManager에게 대화 시작 요청
            dialogueManager.StartDialogue(myDialogue);
        }
    }

    // 에디터에서 NPC 선택 시, 감지 범위(구체)를 시각적으로 보여줌
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 2f); // 반경 2짜리 초록 구체
    }
}
