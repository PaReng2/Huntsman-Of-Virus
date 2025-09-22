using UnityEngine;

public class CameraSet : MonoBehaviour
{
    public Transform target; // ī�޶� ����ٴ� ��� (�÷��̾�)
    public Vector3 offset;   // �÷��̾�κ��� ������ �Ÿ��� ����

    [Range(1, 10)]
    public float smoothSpeed = 5f; // ī�޶� �������� �ε巯�� ���� (1�� ���� ������, 10�� ���� ����)

    // LateUpdate()�� ����ϸ� ��� ������Ʈ�� ������Ʈ�� ���� �ڿ� ī�޶� ��������
    // ī�޶� �����ų� ����� ������ ����
    private void LateUpdate()
    {
        
        Vector3 desiredPosition = target.position + offset;

        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        
        transform.LookAt(target);
    }
}