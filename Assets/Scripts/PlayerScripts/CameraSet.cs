using UnityEngine;

public class CameraSet : MonoBehaviour
{
    public Transform target; // 카메라가 따라다닐 대상 (플레이어)
    public Vector3 offset;   // 플레이어로부터 떨어진 거리와 방향

    [Range(1, 10)]
    public float smoothSpeed = 5f; // 카메라 움직임의 부드러움 정도 (1이 가장 느리고, 10이 가장 빠름)

    // LateUpdate()를 사용하면 모든 오브젝트의 업데이트가 끝난 뒤에 카메라가 움직여서
    // 카메라가 떨리거나 끊기는 현상을 방지
    private void LateUpdate()
    {
        
        Vector3 desiredPosition = target.position + offset;

        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        
        transform.LookAt(target);
    }
}