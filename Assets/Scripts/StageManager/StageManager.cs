using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject[] stagePrefabs;   // 스테이지 프리팹 배열
    private int currentStageIndex = 0;  // 현재 스테이지 번호 (씬에 Stage_1 있다고 가정)
    private GameObject currentStageObj; // 현재 스테이지 오브젝트

    void Start()
    {
        //  씬에 배치된 Stage_1 찾아서 등록
        currentStageObj = GameObject.Find("Stage_1");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))                    //우클릭 누를시 스테이지 변경
        {
            NextStage();
        }
    }

    void NextStage()
    {
        //  현재 스테이지 제거
        if (currentStageObj != null)
        {
            Destroy(currentStageObj);
        }

        // 다음 스테이지 인덱스
        currentStageIndex++;
        if (currentStageIndex >= stagePrefabs.Length)
        {
            currentStageIndex = 0; // 끝까지 가면 다시 처음으로
        }

        //  새 스테이지 생성
        currentStageObj = Instantiate(stagePrefabs[currentStageIndex], Vector3.zero, Quaternion.identity);

        // 스테이지 이름 1부터 시작할수 있도록
        currentStageObj.name = "Stage_" + (currentStageIndex + 1);
    }
}
