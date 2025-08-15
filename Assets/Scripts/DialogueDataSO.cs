using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueDataSO : ScriptableObject
{
    [Header("캐릭터 정보")] 
    public string characterName = "character";
    public Sprite characterImage;

    [Header("대화 내용")] 
    [TextArea(3, 10)] 
    public List<string> dialogueLines = new List<string>();
}
