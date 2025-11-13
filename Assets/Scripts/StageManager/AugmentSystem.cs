using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AugmentSystem : MonoBehaviour
{
    [Header("Panel & UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private TMP_Text[] optionLabels;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [Header("Config")]
    [SerializeField] private int maxLevelPerAugment = 5;

    [Header("Pause Control")]
    [SerializeField] private bool pauseGameWhilePanelOpen = true; 
    private bool isPausedByThisPanel = false;
    private float previousTimeScale = 1f;

    [Serializable]
    public class Augment
    {
        public string displayName;
        [NonSerialized] public int level;
    }

    [SerializeField] private List<Augment> augments = new List<Augment>();

    private readonly List<int> currentOptions = new List<int>(3);

    private void Reset()
    {
        EnsureDefaultAugments();
    }

    private void Awake()
    {
        EnsureDefaultAugments();
        WireButtons();
        HidePanelImmediate();
    }

    private void EnsureDefaultAugments()
    {
        if (augments == null) augments = new List<Augment>();
        if (augments.Count != 8)
        {
            augments.Clear();
            for (int i = 1; i <= 8; i++)
                augments.Add(new Augment { displayName = $"증강 {i}", level = 0 });
        }
    }

    private void WireButtons()
    {
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int captured = i;
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].onClick.AddListener(() => OnClickOptionButton(captured));
        }
    }

    public bool TryOpenPanel()
    {
        int availableCount = CountAvailableAugments();
        if (availableCount <= 0)
        {
            HidePanelImmediate();
            return false;
        }

        BuildOptionsWeightedNoRepeat(Mathf.Min(3, availableCount));

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < currentOptions.Count)
            {
                int idx = currentOptions[i];
                optionButtons[i].gameObject.SetActive(true);
                optionButtons[i].interactable = true;
                optionLabels[i].text = BuildLabel(augments[idx]);
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }

        panelRoot.SetActive(true);
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.interactable = true;
            panelCanvasGroup.blocksRaycasts = true;
            panelCanvasGroup.alpha = 1f;
        }

        if (pauseGameWhilePanelOpen)
            PauseGame(true);

        return true;
    }

    private void OnEnable()
    {
        if (panelRoot != null && panelRoot.activeInHierarchy)
        {
            if (CountAvailableAugments() <= 0)
            {
                HidePanelImmediate();
            }
        }
    }

    private int CountAvailableAugments()
    {
        int count = 0;
        foreach (var a in augments)
        {
            if (a.level < maxLevelPerAugment) count++;
        }
        return count;
    }

    private string BuildLabel(Augment a)
    {
        return $"{a.displayName}  (Lv.{a.level}/{maxLevelPerAugment})";
    }

    private void HidePanelImmediate()
    {
        if (panelCanvasGroup != null)
        {
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
            panelCanvasGroup.alpha = 0f;
        }
        if (panelRoot != null)
            panelRoot.SetActive(false);
        currentOptions.Clear();

        if (pauseGameWhilePanelOpen)
            PauseGame(false);
    }

    private void OnClickOptionButton(int optionSlot)
    {
        if (optionSlot < 0 || optionSlot >= currentOptions.Count) return;

        int augmentIndex = currentOptions[optionSlot];
        var a = augments[augmentIndex];

        if (a.level < maxLevelPerAugment)
        {
            a.level = Mathf.Min(a.level + 1, maxLevelPerAugment);
        }

        // 여기서 실제 능력 적용 로직 연결

        HidePanelImmediate();
    }

    private void BuildOptionsWeightedNoRepeat(int targetCount)
    {
        currentOptions.Clear();

        List<int> pool = new List<int>();
        for (int i = 0; i < augments.Count; i++)
        {
            if (augments[i].level < maxLevelPerAugment)
                pool.Add(i);
        }

        for (int pick = 0; pick < targetCount && pool.Count > 0; pick++)
        {
            int selected = WeightedPick(pool);
            currentOptions.Add(selected);
            pool.Remove(selected);
        }
    }

    private int WeightedPick(List<int> candidateIndices)
    {
        int totalWeight = 0;
        foreach (int idx in candidateIndices)
        {
            totalWeight += (augments[idx].level >= 1) ? 2 : 1;
        }

        int r = UnityEngine.Random.Range(0, totalWeight);
        foreach (int idx in candidateIndices)
        {
            int w = (augments[idx].level >= 1) ? 2 : 1;
            if (r < w) return idx;
            r -= w;
        }
        return candidateIndices[UnityEngine.Random.Range(0, candidateIndices.Count)];
    }

    public void ResetAllLevelsForNewStage()
    {
        foreach (var a in augments) a.level = 0;
        HidePanelImmediate();
    }

    public string GetLevelsDebugString()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < augments.Count; i++)
        {
            sb.AppendLine($"{augments[i].displayName}: Lv.{augments[i].level}/{maxLevelPerAugment}");
        }
        return sb.ToString();
    }

    private void PauseGame(bool pause)
    {
        if (pause)
        {
            if (!isPausedByThisPanel)
            {
                previousTimeScale = (Time.timeScale <= 0f) ? 1f : Time.timeScale;
                Time.timeScale = 0f;
                isPausedByThisPanel = true;
            }
        }
        else
        {
            if (isPausedByThisPanel)
            {
                Time.timeScale = previousTimeScale;
                isPausedByThisPanel = false;
            }
        }
    }
}