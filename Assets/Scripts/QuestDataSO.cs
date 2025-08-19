using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Quest/New Quest")]
public class QuestDataSO : ScriptableObject
{
    public int questID;
    public string questName;
    public bool isClear;

    [TextArea(2,10)]
    public List<string> questDesc = new List<string>();
}
