using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 컴포넌트를 사용하기 위해 추가

// 인벤토리 아이템 데이터를 저장할 클래스
[System.Serializable]
public class Item
{
    public string itemName; // 아이템 이름
    public Sprite itemIcon; // 아이템 아이콘 이미지
    public int quantity;    // 아이템 수량
}

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryPanel; // 인벤토리 패널 (부모)
    public Transform inventorySlotParent; // 인벤토리 슬롯들이 배치될 부모 오브젝트 (예: GridLayoutGroup)
    public GameObject inventorySlotPrefab; // 인벤토리 슬롯 프리팹

    // 테스트를 위한 아이템 목록 (인스펙터에서 직접 설정 가능)
    public List<Item> items = new List<Item>();

    // 인벤토리 창이 가로 5칸 * 세로 3칸으로 총 15개의 슬롯을 가집니다.
    private const int INVENTORY_SIZE = 15;
    private GameObject[] currentSlots = new GameObject[INVENTORY_SIZE]; // 현재 생성된 슬롯들을 저장할 배열

    void Start()
    {
        // 처음에는 인벤토리 패널을 닫아둡니다.
        InventoryPanel.SetActive(false);
        // 게임 시작 시, 미리 15개의 빈 슬롯을 생성합니다.
        InstantiateInitialSlots();
    }

    // 인벤토리 열기 버튼에 연결할 함수
    public void OpenInventory()
    {
        InventoryPanel.SetActive(true);
        UpdateInventoryUI(); // UI를 업데이트하여 아이템을 표시합니다.
    }

    // 인벤토리 닫기 버튼에 연결할 함수
    public void CloseInventory()
    {
        InventoryPanel.SetActive(false);
    }

    // 게임 시작 시, 인벤토리 크기(15개)에 맞게 미리 슬롯을 생성하는 함수
    private void InstantiateInitialSlots()
    {
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            GameObject slotObject = Instantiate(inventorySlotPrefab, inventorySlotParent);
            currentSlots[i] = slotObject;

            // 초기에는 모든 슬롯의 아이콘과 수량을 비활성화합니다.
            // GetComponentInChildren을 사용하여 자식 오브젝트 이름에 의존하지 않습니다.
            Image itemIconImage = slotObject.GetComponentInChildren<Image>();
            Text quantityText = slotObject.GetComponentInChildren<Text>();

            if (itemIconImage != null)
            {
                itemIconImage.enabled = false;
            }
            if (quantityText != null)
            {
                quantityText.enabled = false;
            }
        }
    }

    // 인벤토리 UI를 업데이트하는 핵심 로직
    private void UpdateInventoryUI()
    {
        // 모든 슬롯의 아이콘과 수량을 초기화합니다.
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            Image itemIconImage = currentSlots[i].GetComponentInChildren<Image>();
            Text quantityText = currentSlots[i].GetComponentInChildren<Text>();

            if (itemIconImage != null)
            {
                itemIconImage.enabled = false;
            }
            if (quantityText != null)
            {
                quantityText.enabled = false;
            }
        }

        // 아이템 목록을 순회하며 슬롯에 아이템 정보를 표시합니다.
        for (int i = 0; i < items.Count && i < INVENTORY_SIZE; i++)
        {
            GameObject slotObject = currentSlots[i];

            Image itemIconImage = slotObject.GetComponentInChildren<Image>();
            Text quantityText = slotObject.GetComponentInChildren<Text>();

            // 아이콘 설정
            if (itemIconImage != null)
            {
                itemIconImage.sprite = items[i].itemIcon;
                itemIconImage.enabled = true;
            }

            // 수량 설정
            if (quantityText != null)
            {
                if (items[i].quantity > 1)
                {
                    quantityText.text = items[i].quantity.ToString();
                    quantityText.enabled = true;
                }
                else
                {
                    quantityText.enabled = false;
                }
            }
        }
    }
}
