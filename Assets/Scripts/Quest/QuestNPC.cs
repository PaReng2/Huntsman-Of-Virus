using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPC : MonoBehaviour
{
    public bool isQuestNPC;
    public QuestDataSO currentQuest;
    
    //해당 스크립트는 퀘스트 수행 시 상호작용이 필요한 NPC를 위해 제작되었음

    
    public void QuestStart()
    {
        QuestManager manager = FindObjectOfType<QuestManager>();
        
        manager.QuestStart(currentQuest);
    }

    public void QuestComplete()
    {
        QuestManager manager = FindObjectOfType<QuestManager>();
        
        manager.QuestCompleted();
    }
}
