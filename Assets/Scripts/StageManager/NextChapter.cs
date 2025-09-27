using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextChapter : MonoBehaviour
{
    [Header("�Ѿ é�� �ۼ� // 1 - Ʃ�丮��, 2 - é��1, 3 - ����")]
    public int chapterNum;

    public GameObject isInsteraction;     // "��ȣ�ۿ� ����" UI ������Ʈ
    

    private void Update()
    {
        GoNext();

    }

    void GoNext()
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

        if(hasPlayer && Input.GetKeyDown(KeyCode.F))
            SceneManager.LoadScene(chapterNum);

    }


}
