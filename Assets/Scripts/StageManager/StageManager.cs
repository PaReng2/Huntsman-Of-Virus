using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject[] stagePrefabs;   // �������� ������ �迭
    private int currentStageIndex = 0;  // ���� �������� ��ȣ (���� Stage_1 �ִٰ� ����)
    private GameObject currentStageObj; // ���� �������� ������Ʈ

    void Start()
    {
        //  ���� ��ġ�� Stage_1 ã�Ƽ� ���
        currentStageObj = GameObject.Find("Stage_1");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))                    //��Ŭ�� ������ �������� ����
        {
            NextStage();
        }
    }

    void NextStage()
    {
        //  ���� �������� ����
        if (currentStageObj != null)
        {
            Destroy(currentStageObj);
        }

        // ���� �������� �ε���
        currentStageIndex++;
        if (currentStageIndex >= stagePrefabs.Length)
        {
            currentStageIndex = 0; // ������ ���� �ٽ� ó������
        }

        //  �� �������� ����
        currentStageObj = Instantiate(stagePrefabs[currentStageIndex], Vector3.zero, Quaternion.identity);

        // �������� �̸� 1���� �����Ҽ� �ֵ���
        currentStageObj.name = "Stage_" + (currentStageIndex + 1);
    }
}
