using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AugmentSystem : MonoBehaviour
{
    public enum AugmentType
    {
        MAXHP,
        ATK,
        SPD,
        ATKSPD,
        RANGE
    }

    [Serializable]
    public class Augment
    {
        public string displayName;
        public AugmentType type;
        [NonSerialized] public int level;
    }

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

    [SerializeField] private List<Augment> augments = new List<Augment>();

    private readonly List<int> currentOptions = new List<int>(3);

    [SerializeField] private GameManager gameManager;

    private void Reset()
    {
        EnsureDefaultAugments();
    }

    private void Awake()
    {
        EnsureDefaultAugments();
        WireButtons();
        HidePanelImmediate();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void EnsureDefaultAugments()
    {
        if (augments == null) augments = new List<Augment>();

        if (augments.Count != 5)
        {
            augments.Clear();
            augments.Add(new Augment { displayName = "MAXHP", type = AugmentType.MAXHP, level = 0 });
            augments.Add(new Augment { displayName = "ATK", type = AugmentType.ATK, level = 0 });
            augments.Add(new Augment { displayName = "SPD", type = AugmentType.SPD, level = 0 });
            augments.Add(new Augment { displayName = "ATKSPD", type = AugmentType.ATKSPD, level = 0 });
            augments.Add(new Augment { displayName = "RANGE", type = AugmentType.RANGE, level = 0 });
        }
        else
        {
            foreach (var a in augments)
                a.level = 0;
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
                optionLabels[i].text = BuildLabel(augments[idx]);

                optionButtons[i].interactable = false;
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

        StartCoroutine(UnlockButtonsAfterDelay());

        if (pauseGameWhilePanelOpen)
            PauseGame(true);

        return true;
    }

    private IEnumerator UnlockButtonsAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        foreach (var btn in optionButtons)
        {
            if (btn.gameObject.activeSelf)
                btn.interactable = true;
        }
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
        gameManager.isInteracting = false;
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

    public int GetAugmentLevel(AugmentType type)
    {
        for (int i = 0; i < augments.Count; i++)
        {
            if (augments[i].type == type)
                return augments[i].level;
        }
        return 0;
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