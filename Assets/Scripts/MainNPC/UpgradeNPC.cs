using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeNPC : MonoBehaviour
{

    [Header("��ȣ�ۿ� �ؽ�Ʈ")]
    public GameObject isInsteraction;     // "��ȣ�ۿ� ����" UI ������Ʈ
    private PlayerController player;
    public GameObject UpgradePanel;
    private GameManager gameManager;
    

    void Start()
    {
        UpgradePanel.SetActive(false);
        player = FindObjectOfType<PlayerController>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        IsUpgrading();
    }

    void IsUpgrading()
    {
        PlayerAttackRangeDealer rangeDealer = FindObjectOfType<PlayerAttackRangeDealer>();
        // "Player"��� ���̾ ������ ����ũ ��������
        int playerLayer = LayerMask.GetMask("Player");

        // �÷��̾ NPC �ֺ� 2f �ݰ� �ȿ� �ִ��� Ȯ�� (��ü ������ �浹ü Ž��)
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3f, playerLayer);

        // Ž���� �÷��̾ �ϳ��� ������ true
        bool hasPlayer = colliders.Length > 0;

        // �÷��̾� ��ó�� ������ ��ȣ�ۿ� UI Ȱ��ȭ, �ƴϸ� ��Ȱ��ȭ
        if(!gameManager.isInteracting)
        {
            isInsteraction.SetActive(hasPlayer);

        }

        if (hasPlayer)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isInsteraction.SetActive(false);
                UpgradePanel.SetActive(true);
                gameManager.isInteracting = true;
                Debug.Log($"���׷��̵� â ǥ�� {gameManager.isInteracting}");

            }
            if (gameManager.isInteracting && Input.GetKeyDown(KeyCode.Escape))
            {
                UpgradePanel.SetActive(false);
                gameManager.isInteracting = false;

                Debug.Log("���׷��̵� â ����");
            }
        }
    }

    

    
}
