using Cinemachine;
using UnityEngine;

public class CameraSet : MonoBehaviour
{
    // �ó׸ӽ� ���� ī�޶� �Ҵ��� ����
    public CinemachineVirtualCamera virtualCamera;

    // ī�޶� ����ٴ� �÷��̾� ���� ������Ʈ
    public Transform playerTarget;

    void Start()
    {
        // virtualCamera ������ �Ҵ���� �ʾҴٸ�, ������ CinemachineVirtualCamera ������Ʈ�� ã�Ƽ� �Ҵ�
        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }

        // ī�޶� ���� ����� �Ҵ�
        if (virtualCamera != null && playerTarget != null)
        {
            virtualCamera.Follow = playerTarget;

            //�߰������� �ٵ�(Body)�� ����(Aim) ������ ��ũ��Ʈ�� �����
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            if (transposer != null)
            {
                transposer.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetOnAssign;
                transposer.m_FollowOffset = new Vector3(8f, 10f, -8.5f);
            }
        }
    }
}