using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeNPC : MonoBehaviour
{

    [Header("��ȣ�ۿ� �ؽ�Ʈ")]
    public GameObject isInsteraction;     // "��ȣ�ۿ� ����" UI ������Ʈ

    public GameObject UpgradePanel;
    private PlayerController player;
    private PlayerAttackRangeDealer attackRangeDealer;
    

    void Start()
    {
        UpgradePanel.SetActive(false);
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        
    }

    void IsUpgrading()
    {
        PlayerAttackRangeDealer rangeDealer = FindObjectOfType<PlayerAttackRangeDealer>();
        // "Player"��� ���̾ ������ ����ũ ��������
        int playerLayer = LayerMask.GetMask("Player");

        // �÷��̾ NPC �ֺ� 2f �ݰ� �ȿ� �ִ��� Ȯ�� (��ü ������ �浹ü Ž��)
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, playerLayer);

        // Ž���� �÷��̾ �ϳ��� ������ true
        bool hasPlayer = colliders.Length > 0;

        // �÷��̾� ��ó�� ������ ��ȣ�ۿ� UI Ȱ��ȭ, �ƴϸ� ��Ȱ��ȭ
        isInsteraction.SetActive(hasPlayer);

        if (hasPlayer)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                UpgradePanel.SetActive(true);
            }
        }
    }

    void HPUpgragde()
    {
        player.playerHP += 20;
    }

    void AttackRateUpgrade()
    {
        attackRangeDealer.AttackRate -= 0.2f;
    }
}
