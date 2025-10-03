using Cinemachine;
using UnityEngine;

public class CameraSet : MonoBehaviour
{
    // 시네머신 가상 카메라를 할당할 변수
    public CinemachineVirtualCamera virtualCamera;

    // 카메라가 따라다닐 플레이어 게임 오브젝트
    public Transform playerTarget;

    void Start()
    {
        // virtualCamera 변수가 할당되지 않았다면, 씬에서 CinemachineVirtualCamera 컴포넌트를 찾아서 할당
        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }

        // 카메라가 따라갈 대상을 할당
        if (virtualCamera != null && playerTarget != null)
        {
            virtualCamera.Follow = playerTarget;

            //추가적으로 바디(Body)와 조준(Aim) 설정도 스크립트로 제어가능
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;
                transposer.m_FollowOffset = new Vector3(8f, 10f, -8.5f);
            }
        }
    }
}