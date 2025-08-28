using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public GameObject QuestPanel;       //퀘스트 패널
    public TextMeshProUGUI QuestName;   //퀘스트 이름
    public TextMeshProUGUI QuestDesc;   //퀘스트 설명
    public bool isCompletedQuest;       //퀘스트 완료 여부
    public TextMeshProUGUI isCompleted; //퀘스트 완료 표시용 글
    public QuestDataSO currentQuestData;
    void Start()
    {
        QuestPanel.SetActive(false);
    }

    public void QuestStart(QuestDataSO newQuest)
    {
        currentQuestData = newQuest;
        if (currentQuestData.questId == 10)
        {
            QuestPanel.SetActive(true);
            QuestName.text = currentQuestData.QuestName;
            QuestDesc.text = currentQuestData.desc;
            isCompleted.color = Color.red;
            isCompleted.text = "미완료";
            
            
        }
    }

    public void QuestCompleted()
    {
        isCompletedQuest = true;
        currentQuestData.isCompleted = true;
        isCompleted.color = Color.green;
        isCompleted.text = "완료";
        
    }
}
