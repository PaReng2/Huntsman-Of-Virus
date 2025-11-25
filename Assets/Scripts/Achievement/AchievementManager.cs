using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AchievementManager : MonoBehaviour
{
    public static AchievementManager instance;

    [Header("Achievement Settings")]
    public List<AchievementData> allachievements = new List<AchievementData>();

    [Header("UI References")]
    public GameObject achievementPopupPrefab;
    public Transform popupParent;
    public GameObject achievementPanel;
    public Transform achievementListContent;
    public GameObject achievementSlotPrefab;

    private Dictionary<AchievementType, int> progressData = new Dictionary<AchievementType , int>();

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ResetAllAchievements();
        foreach(AchievementType type in System.Enum.GetValues(typeof(AchievementType)))
        {
            progressData[type] = 0;
        }
        LoadAchivements();
        UpdateAChievementUI();
    }

    public float GetProgress(AchievementData achievement)
    {
        if (achievement.isUnlocked) return 1f;
        int current = progressData.ContainsKey(achievement.achievementType) ? progressData[achievement.achievementType] : 0;
        return Mathf.Min((float)current / achievement.requiredAmount, 1f);
    }

    public void UpdateAChievementUI()
    {
        if(achievementListContent == null || achievementSlotPrefab == null)
            return;

        foreach(Transform child in achievementListContent)
        {
            Destroy(child.gameObject);
        }

        foreach(AchievementData achievement in allachievements)
        {
            GameObject slot = Instantiate(achievementSlotPrefab, achievementListContent);
            AchievementSlot slotScript = slot.GetComponent<AchievementSlot>();
            if(slotScript != null)
            {
                slotScript.SetAchievement(achievement, GetProgress(achievement));
            }
        }
    }

    void SetAchievement()
    {
        foreach (var kvp in progressData)
        {
            PlayerPrefs.SetInt("Achivement_" + kvp.Key, kvp.Value);
        }
        
        foreach(AchievementData achievement in allachievements)
        {
            PlayerPrefs.SetInt("Unlocked_" + achievement.name, achievement.isUnlocked ? 1 : 0);
        }

        PlayerPrefs.Save();
    }

    void LoadAchivements()
    {
        foreach(AchievementType type in System.Enum.GetValues(typeof(AchievementType)))
        {
            progressData[type] = PlayerPrefs.GetInt("Achivement_" + type, 0);
        }

        foreach(AchievementData achievement in allachievements)
        {
            achievement.isUnlocked = PlayerPrefs.GetInt("Unlocked_" + achievement.name, 0) == 1;
        }
    }

    void ResetAllAchievements()
    {
        foreach(AchievementType type in System.Enum.GetValues(typeof(AchievementType)))
        {
            progressData[type] = 0;
            PlayerPrefs.DeleteKey("Achivement_" + type);
        }

        foreach(AchievementData achievement in allachievements)
        {
            achievement.isUnlocked = false;
            PlayerPrefs.DeleteKey("Unlocked_" + achievement.name);
        }

        PlayerPrefs.Save();
        UpdateAChievementUI();
    }

    void ShowAchievementPopup(AchievementData achievement)
    {
        if(achievementPopupPrefab != null && popupParent != null)
        {
            GameObject popup = Instantiate(achievementPopupPrefab, popupParent);

            Text titleText = popup.transform.Find("Title")?.GetComponent<Text>();
            Text descText = popup.transform.Find("Description")?.GetComponent<Text>();

            if (titleText != null) titleText.text = "업적 달성";
            if(descText != null) descText.text = achievement.achievementName;

            Destroy(popup, 3.0f);
        }
    }

    public void UpdateProgress(AchievementType type, int amount = 1)
    {
        progressData[type] += amount;

        foreach(AchievementData achievement in allachievements)
        {
            if(achievement.achievementType == type && !achievement.isUnlocked)
            {
                if (progressData[type] >= achievement.requiredAmount)
                {
                    UnlockAchivement(achievement);
                }
            }
        }
    }

    void UnlockAchivement(AchievementData achievement)
    {
        achievement.isUnlocked = true;
        ShowAchievementPopup(achievement);
        UpdateAChievementUI();
        SetAchievement();
    }
}
