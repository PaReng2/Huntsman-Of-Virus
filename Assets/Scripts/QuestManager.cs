using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("퀘스트 정보")]
    public int currentQuestId;
    public TextMeshProUGUI currentQuestName;
    public TextMeshProUGUI currentQuestDesc;
    public GameObject QuestPanel;

    private QuestDataSO questData;
    private bool isCurrentQuestClear;
    
    // Start is called before the first frame update
    void Start()
    {
        QuestPanel.SetActive(false);
        currentQuestId = questData.questID;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartQuest()
    {
        QuestPanel.SetActive(true);
    }

    void QuestTracker()
    {
        
    }

    void SaveQuest()
    {
        
    }
}
