using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // ← 추가

// 상점에서의 UI 관리
public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance;

    [Header("Interact UI")]
    public CanvasGroup interactCanvasGroup;
    public TextMeshProUGUI interactText;    // ← TMP로 변경

    [Header("Temporary Message")]
    public TextMeshProUGUI tempMessageText; // ← TMP로 변경

    [Header("Inventory UI (left-top)")]
    public RectTransform leftTopContainer;
    public GameObject uiItemPrefab;
    public float uiIconSize = 64f;
    public float spacing = 8f;

    private List<GameObject> uiItems = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (interactCanvasGroup != null) interactCanvasGroup.alpha = 0f;
        if (tempMessageText != null) tempMessageText.gameObject.SetActive(false);

        HorizontalLayoutGroup hl = leftTopContainer.GetComponent<HorizontalLayoutGroup>();
        if (hl != null) hl.spacing = spacing;
    }

    public void ShowInteract(string message)
    {
        if (interactCanvasGroup == null || interactText == null) return;
        interactText.text = message;
        interactCanvasGroup.alpha = 1f;
        interactCanvasGroup.blocksRaycasts = true;
    }

    public void HideInteract()
    {
        if (interactCanvasGroup == null) return;
        interactCanvasGroup.alpha = 0f;
        interactCanvasGroup.blocksRaycasts = false;
    }

    public void ShowTempMessage(string msg, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ShowTempCoroutine(msg, duration));
    }

    System.Collections.IEnumerator ShowTempCoroutine(string msg, float duration)
    {
        if (tempMessageText == null) yield break;
        tempMessageText.gameObject.SetActive(true);
        tempMessageText.text = msg;
        yield return new WaitForSeconds(duration);
        tempMessageText.gameObject.SetActive(false);
    }

    public void AddItemToLeftTop(ShopItem item)
    {
        if (item == null) return;
        if (item.type == ShopItemType.HealthPotion || item.uiSprite == null) return;

        GameObject go = Instantiate(uiItemPrefab, leftTopContainer);
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
}