using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSlot : MonoBehaviour
{
    [Header("UI References")]
    public Image iconimage;
    public Text nameText;
    public Text descriptionText;
    public Text progressText;
    public Slider progressSlider;

    public void SetAchievement(AchievementData achievement , float progress)
    {
        if(nameText != null)
            nameText.text = achievement.achievementName;

        if(descriptionText != null)
            descriptionText.text = achievement.description;

        if(iconimage != null)
            iconimage.sprite = achievement.icon;

        if (progressSlider != null)
            progressSlider.value = achievement.isUnlocked ? 1f : progress;

        if (progressText != null)
        {
            if(achievement.isUnlocked)
            {
                progressText.text = "¿Ï·á";
            }
            else
            {
                int current = Mathf.FloorToInt(progress * achievement.requiredAmount);
                progressText.text = current + "/" + achievement.requiredAmount;
            }
        }
    }
}
