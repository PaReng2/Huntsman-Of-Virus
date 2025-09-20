using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRangeDealer : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;
    public Transform firePoint;
    public LayerMask shootableMask;
    private Camera mainCamera;
    public PlayerStatusSO playerData;


    private void Start()
    {
        mainCamera = Camera.main;
        
    }

    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
            
        }
    }

    void Attack()
    {
        // 1. Raycast를 통해 타겟 지점을 찾습니다.
        // 이 부분은 총알이 향할 방향을 결정하는 데 여전히 유용합니다.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 targetPoint;

        // 2. Raycast가 특정 레이어에 충돌했는지 확인합니다.
        // 충돌했다면, 그 지점을 총알의 목표로 설정합니다.
        if (Physics.Raycast(ray, out hit, float.MaxValue, shootableMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            // 3. 충돌하지 않았다면, 카메라에서 일정 거리 떨어진 가상의 지점을 목표로 설정합니다.
            // 이렇게 하면 어떤 오브젝트와도 충돌하지 않아도 총알이 발사됩니다.
            targetPoint = ray.GetPoint(100f); // 100f는 원하는 거리로 조절 가능합니다.
        }

        // 4. Raycast 결과(실제 충돌 지점 또는 가상의 지점)를 기반으로 총알을 발사합니다.
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }
        
    }
}