using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HP UI")]
    public Slider HPBar;

    [Header("Stat Panel Texts")]
    public TMP_Text maxHpText;
    public TMP_Text atkText;
    public TMP_Text spdText;
    public TMP_Text rangeText;
    public TMP_Text atkSpdText;

    private PlayerController player;
    private PlayerSkills playerSkills;

    [Header("Inventory UI (left-bottom)")]
    public RectTransform leftBottomContainer;
    public GameObject uiItemPrefab;
    public float uiIconSize = 64f;
    public float spacing = 8f;

    private class UIItemEntry
    {
        public GameObject go;
        public ShopItem item;
        public TMP_Text cooldownText;
        public bool isSkillItem;
        public List<PlayerSkills.Skills> associatedSkills = new List<PlayerSkills.Skills>();
    }

    private List<UIItemEntry> uiItems = new List<UIItemEntry>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        player = FindAnyObjectByType<PlayerController>();
        if (player != null)
        {
            playerSkills = player.GetComponent<PlayerSkills>();
        }
    }

    private void Start()
    {
        if (player != null && HPBar != null)
        {
            HPBar.minValue = 0f;
            HPBar.maxValue = player.playerMaxHP;
            HPBar.value = player.curPlayerHp;
        }

        if (leftBottomContainer != null)
        {
            HorizontalLayoutGroup hl = leftBottomContainer.GetComponent<HorizontalLayoutGroup>();
            if (hl != null)
            {
                hl.spacing = spacing;
            }
        }

        if (leftBottomContainer == null || uiItemPrefab == null)
        {
            Debug.LogWarning("UIManager: Inventory UI references are not set. Skipping inventory init.");
            return;
        }

        LoadInventoryFromProgress();
    }

    private void Update()
    {
        if (player == null) return;

        UpdateHPBar();
        UpdateStatTexts();
        UpdateItemCooldownDisplays();
    }

    private void UpdateHPBar()
    {
        if (HPBar == null) return;

        HPBar.maxValue = player.playerMaxHP;
        HPBar.value = player.curPlayerHp;
    }

    private void UpdateStatTexts()
    {
        if (maxHpText != null)
            maxHpText.text = $"MAXHP : {player.playerMaxHP}";

        if (atkText != null)
            atkText.text = $"ATK : {player.attackPower:F1}";

        if (spdText != null)
            spdText.text = $"SPD : {player.playerMoveSpeed:F1}";

        if (rangeText != null)
            rangeText.text = $"RANGE : {player.attackRange:F1}";

        float atkSpdValue = (player.attackDelay > 0.0001f) ? (1f / player.attackDelay) : 0;
        if (atkSpdText != null)
            atkSpdText.text = $"ATKSPD : {atkSpdValue:F1}";
    }

    private void LoadInventoryFromProgress()
    {
        ClearInventoryUI();

        if (PlayerProgress.purchasedItems == null) return;

        foreach (var item in PlayerProgress.purchasedItems)
        {
            if (item != null)
                AddItemToLeftBottom(item);
        }
    }

    public void AddItemToLeftBottom(ShopItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("[UIManager] AddItemToLeftBottom 호출되었으나 item이 null 입니다. 무시합니다.");
            return;
        }

        if (item.type == ShopItemType.HealthPotion)
        {
            return;
        }

        if (uiItemPrefab == null)
        {
            Debug.LogError("[UIManager] uiItemPrefab이 할당되어 있지 않습니다. Inspector에서 할당해주세요.");
            return;
        }

        if (leftBottomContainer == null)
        {
            Debug.LogError("[UIManager] leftBottomContainer(RectTransform)가 할당되어 있지 않습니다. Inspector에서 할당해주세요.");
            return;
        }

        GameObject go = null;
        try
        {
            go = Instantiate(uiItemPrefab, leftBottomContainer);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[UIManager] uiItemPrefab 인스턴스화 실패: {ex.Message}");
            return;
        }

        if (go == null)
        {
            Debug.LogError("[UIManager] Instantiate(uiItemPrefab) 결과가 null 입니다.");
            return;
        }

        Image img = go.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogWarning("[UIManager] 생성된 UI 아이템에 Image 컴포넌트가 없습니다. 프리팹을 확인하세요.");
        }
        else
        {
            if (item.uiSprite != null)
                img.sprite = item.uiSprite;
            else
                img.sprite = null;
        }

        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt != null)
            rt.sizeDelta = new Vector2(uiIconSize, uiIconSize);
        else
            Debug.LogWarning("[UIManager] 생성된 uiItemPrefab에 RectTransform이 없습니다. UI 프리팹이 UI용인지 확인하세요.");

        LayoutElement le = go.GetComponent<LayoutElement>();
        if (le != null)
        {
            le.preferredWidth = uiIconSize;
            le.preferredHeight = uiIconSize;
        }

        UIItemEntry entry = new UIItemEntry
        {
            go = go,
            item = item,
            isSkillItem = false,
            associatedSkills = new List<PlayerSkills.Skills>(),
            cooldownText = null
        };

        if (item.skillUnlocks != null)
        {
            foreach (var st in item.skillUnlocks)
            {
                entry.associatedSkills.Add(st.skill);
            }
        }

        if (entry.associatedSkills != null && entry.associatedSkills.Count > 0)
        {
            entry.isSkillItem = true;

            GameObject textGO = new GameObject("CooldownText", typeof(RectTransform));
            textGO.transform.SetParent(go.transform, false);

            RectTransform textRT = textGO.GetComponent<RectTransform>();
            if (textRT != null)
            {
                textRT.anchorMin = new Vector2(0.5f, 1f);
                textRT.anchorMax = new Vector2(0.5f, 1f);
                textRT.pivot = new Vector2(0.5f, 0f); 
                textRT.anchoredPosition = new Vector2(0f, 6f);
                textRT.sizeDelta = new Vector2(uiIconSize, 28f);
            }

            int computedFontSize = Mathf.Clamp(Mathf.RoundToInt(uiIconSize * 0.35f), 14, 40);

            TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = "";
                tmp.fontSize = computedFontSize;
                tmp.fontStyle = FontStyles.Bold;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.raycastTarget = false;
                tmp.enableAutoSizing = false;
                tmp.margin = new Vector4(0, 0, 0, 0);
                entry.cooldownText = tmp;
            }
            else
            {
                Debug.LogWarning("[UIManager] TextMeshProUGUI 컴포넌트 생성 실패. Text 표시가 되지 않습니다.");
            }
        }

        uiItems.Add(entry);
    }

    public void ClearInventoryUI()
    {
        foreach (var entry in uiItems)
        {
            if (entry != null && entry.go != null)
                Destroy(entry.go);
        }
        uiItems.Clear();
    }

    private void UpdateItemCooldownDisplays()
    {
        if (playerSkills == null)
        {
            if (player != null)
                playerSkills = player.GetComponent<PlayerSkills>();
            if (playerSkills == null) return;
        }

        foreach (var entry in uiItems)
        {
            if (entry == null) continue;

            if (!entry.isSkillItem)
            {
                if (entry.cooldownText != null)
                    entry.cooldownText.text = "";
                continue;
            }

            float maxRemaining = 0f;
            foreach (var s in entry.associatedSkills)
            {
                float rem = playerSkills.GetRemainingCooldown(s);
                if (rem > maxRemaining) maxRemaining = rem;
            }

            if (entry.cooldownText != null)
            {
                if (maxRemaining > 0.049f)
                {
                    entry.cooldownText.text = $"{maxRemaining:F1}s";
                }
                else
                {
                    entry.cooldownText.text = "";
                }
            }
        }
    }
}