using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Quest", menuName = "Quest/NewQuestData")]
public class QuestDataSO : ScriptableObject
{
    public string QuestName;
    public int questId;
    public bool isCompleted;
    public string desc;


}
