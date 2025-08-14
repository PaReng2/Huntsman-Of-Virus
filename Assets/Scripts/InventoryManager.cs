using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���� ������Ʈ�� ����ϱ� ���� �߰�

// �κ��丮 ������ �����͸� ������ Ŭ����
[System.Serializable]
public class Item
{
    public string itemName; // ������ �̸�
    public Sprite itemIcon; // ������ ������ �̹���
    public int quantity;    // ������ ����
}

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryPanel; // �κ��丮 �г� (�θ�)
    public Transform inventorySlotParent; // �κ��丮 ���Ե��� ��ġ�� �θ� ������Ʈ (��: GridLayoutGroup)
    public GameObject inventorySlotPrefab; // �κ��丮 ���� ������

    // �׽�Ʈ�� ���� ������ ��� (�ν����Ϳ��� ���� ���� ����)
    public List<Item> items = new List<Item>();

    // �κ��丮 â�� ���� 5ĭ * ���� 3ĭ���� �� 15���� ������ �����ϴ�.
    private const int INVENTORY_SIZE = 15;
    private GameObject[] currentSlots = new GameObject[INVENTORY_SIZE]; // ���� ������ ���Ե��� ������ �迭

    void Start()
    {
        // ó������ �κ��丮 �г��� �ݾƵӴϴ�.
        InventoryPanel.SetActive(false);
        // ���� ���� ��, �̸� 15���� �� ������ �����մϴ�.
        InstantiateInitialSlots();
    }

    // �κ��丮 ���� ��ư�� ������ �Լ�
    public void OpenInventory()
    {
        InventoryPanel.SetActive(true);
        UpdateInventoryUI(); // UI�� ������Ʈ�Ͽ� �������� ǥ���մϴ�.
    }

    // �κ��丮 �ݱ� ��ư�� ������ �Լ�
    public void CloseInventory()
    {
        InventoryPanel.SetActive(false);
    }

    // ���� ���� ��, �κ��丮 ũ��(15��)�� �°� �̸� ������ �����ϴ� �Լ�
    private void InstantiateInitialSlots()
    {
        for (int i = 0; i < INVENTORY_SIZE; i++)
        {
            GameObject slotObject = Instantiate(inventorySlotPrefab, inventorySlotParent);
            currentSlots[i] = slotObject;

            // �ʱ⿡�� ��� ������ �����ܰ� ������ ��Ȱ��ȭ�մϴ�.
            // GetComponentInChildren�� ����Ͽ� �ڽ� ������Ʈ �̸��� �������� �ʽ��ϴ�.
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

    // �κ��丮 UI�� ������Ʈ�ϴ� �ٽ� ����
    private void UpdateInventoryUI()
    {
        // ��� ������ �����ܰ� ������ �ʱ�ȭ�մϴ�.
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

        // ������ ����� ��ȸ�ϸ� ���Կ� ������ ������ ǥ���մϴ�.
        for (int i = 0; i < items.Count && i < INVENTORY_SIZE; i++)
        {
            GameObject slotObject = currentSlots[i];

            Image itemIconImage = slotObject.GetComponentInChildren<Image>();
            Text quantityText = slotObject.GetComponentInChildren<Text>();

            // ������ ����
            if (itemIconImage != null)
            {
                itemIconImage.sprite = items[i].itemIcon;
                itemIconImage.enabled = true;
            }

            // ���� ����
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
