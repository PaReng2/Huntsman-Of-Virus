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

    [Header("Inventory UI (left-bottom)")]
    public RectTransform leftBottomContainer;   
    public GameObject uiItemPrefab;
    public float uiIconSize = 64f;
    public float spacing = 8f;

    private List<GameObject> uiItems = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        player = FindAnyObjectByType<PlayerController>();
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
        if (item == null) return;
        if (item.type == ShopItemType.HealthPotion || item.uiSprite == null) return;

        if (leftBottomContainer == null || uiItemPrefab == null)
        {
            Debug.LogWarning("UIManager: Inventory UI not fully assigned. Cannot add item icon.");
            return;
        }

        GameObject go = Instantiate(uiItemPrefab, leftBottomContainer);
        Image img = go.GetComponent<Image>();
        if (img != null) img.sprite = item.uiSprite;

        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt != null)
            rt.sizeDelta = new Vector2(uiIconSize, uiIconSize);

        LayoutElement le = go.GetComponent<LayoutElement>();
        if (le != null)
        {
            le.preferredWidth = uiIconSize;
            le.preferredHeight = uiIconSize;
        }

        uiItems.Add(go);
    }

    public void ClearInventoryUI()
    {
        foreach (var go in uiItems)
        {
            if (go != null)
                Destroy(go);
        }
        uiItems.Clear();
    }
}