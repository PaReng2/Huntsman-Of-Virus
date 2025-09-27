using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI")] 
    public GameObject DialoguePanel;          // 대화창 전체 패널 (활성/비활성 제어)
    public Image CharacterImage;              // 캐릭터 초상화 이미지
    public TextMeshProUGUI characterNameText; // 캐릭터 이름 표시
    public TextMeshProUGUI dialogueText;      // 대사 표시
    public Button nextButton;                 // 다음 대사로 넘기는 버튼

    [Header("default setting")] 
    public Sprite defaultCharacterImage;      // 캐릭터 이미지가 없을 때 기본 이미지

    [Header("typing")] 
    public float typingSpeed = 0.05f;         // 글자 타이핑 속도 (초 단위)
    public bool skipTypingOnClick = true;     // 타이핑 중 클릭 시 전체 문장 즉시 출력 여부

    // 내부 상태값
    private DialogueDataSO currentDialogue;   // 현재 진행 중인 대화 데이터
    private int currentLineIndex = 0;         // 현재 몇 번째 대사인지
    private bool isDialogueActive = false;    // 대화창이 열려 있는지 여부
    private bool isTyping = false;            // 현재 글자를 하나씩 찍는 중인지 여부
    private Coroutine typingCoroutine;        // 타이핑 효과를 돌리고 있는 코루틴 참조
    private bool isTalking = false;           // 대화중인지 확인
    private PlayerAttackRangeDealer player;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    private void Start()
    {
        DialoguePanel.SetActive(false);       // 시작할 때 대화창은 꺼둠
        nextButton.onClick.AddListener(HandleNextInput); // 버튼에 "다음 대사 처리" 함수 연결
    }

    private void Update()
    {
        // 대화 진행 중일 때 스페이스바 입력하면 다음 대사 처리
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            HandleNextInput();
        }
        StopMoveAtTalking();
    }

    // 코루틴: 글자 하나씩 찍는 효과
    IEnumerator TypeText(string textToType)
    {
        isTyping = true;            // 타이핑 중 상태로 전환
        dialogueText.text = "";     // 출력창 초기화

        // 글자 하나씩 출력
        for (int i = 0; i < textToType.Length; i++)
        {
            dialogueText.text += textToType[i];           // 글자 추가
            yield return new WaitForSeconds(typingSpeed); // 글자마다 지연 시간
        }

        isTyping = false;           // 타이핑 완료
    }

    // 타이핑 즉시 완료 (전체 문장 한 번에 출력)
    void CompleteTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);   // 진행 중인 타이핑 코루틴 중단
        }
        isTyping = false;

        // 현재 대사 전체 문장 출력
        if (currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
        {
            dialogueText.text = currentDialogue.dialogueLines[currentLineIndex];
        }
    }

    // 현재 대사 출력 시작
    void ShowCurrentLine()
    {
        if (currentDialogue != null && currentLineIndex < currentDialogue.dialogueLines.Count)
        {
            // 기존에 실행 중이던 타이핑 코루틴 있으면 중단
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
        }

        // 현재 대사 가져와서 타이핑 시작
        string currentText = currentDialogue.dialogueLines[currentLineIndex];
        typingCoroutine = StartCoroutine(TypeText(currentText));
    }

    // 대화 종료 처리
    void EndDialogue()
    {
        player = FindObjectOfType<PlayerAttackRangeDealer>();

        // 코루틴 정리
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isDialogueActive = false;
        isTyping = false;
        DialoguePanel.SetActive(false);   // 대화창 닫기
        isTalking = false;
        currentLineIndex = 0;             // 인덱스 초기화
        gameManager.isInteracting = false;
    }

    // 다음 대사로 넘기기
    public void ShowNextLine()
    {
        currentLineIndex++; // 다음 대사 인덱스로 이동

        // 모든 대사를 다 출력했으면 대화 종료
        if (currentLineIndex >= currentDialogue.dialogueLines.Count)
        {
            EndDialogue();
        }
        else
        {
            ShowCurrentLine(); // 다음 대사 출력
        }
    }

    // 입력(버튼 클릭 or 스페이스바) 처리
    public void HandleNextInput()
    {
        if (isTyping && skipTypingOnClick) // 타이핑 중인데 클릭/입력하면
        {
            CompleteTyping();              // 즉시 출력 완료
        }
        else if (!isTyping)                // 이미 타이핑 끝났으면
        {
            ShowNextLine();                // 다음 대사로 이동
        }
    }

    // 강제로 대화 스킵
    public void SkipDialogue()
    {
        EndDialogue();
    }

    // 외부에서 "현재 대화창 켜져있나?" 확인용
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    // 새로운 대화 시작
    public void StartDialogue(DialogueDataSO dialogue)
    {
        player = FindObjectOfType<PlayerAttackRangeDealer>();
        
        // 대화 데이터가 없거나 라인이 없으면 무시
        if (dialogue == null || dialogue.dialogueLines.Count == 0) return;

        currentDialogue = dialogue; // 대화 데이터 저장
        currentLineIndex = 0;       // 첫 줄부터 시작
        isDialogueActive = true;    // 대화 시작 상태로 전환

        DialoguePanel.SetActive(true); // 대화창 열기
        isTalking = true;
        characterNameText.text = dialogue.characterName; // 캐릭터 이름 표시

        // 캐릭터 이미지 세팅
        if (CharacterImage != null)
        {
            if (dialogue.characterImage != null)
            {
                CharacterImage.sprite = dialogue.characterImage;
            }
            else
            {
                CharacterImage.sprite = defaultCharacterImage;
            }
        }

        // 첫 대사 출력
        ShowCurrentLine();
        

        player.isInteracting = true;
    }

    void StopMoveAtTalking()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (isTalking)
        {
            player.playerMoveSpeed = 0;
            player.playerJumpForce = 0;
        }
        else
        {
            player.playerMoveSpeed = 5;
            player.playerJumpForce = 5;
        }
    }
}
