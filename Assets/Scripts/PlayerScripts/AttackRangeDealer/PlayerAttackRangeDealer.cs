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
        // 1. Raycast�� ���� Ÿ�� ������ ã���ϴ�.
        // �� �κ��� �Ѿ��� ���� ������ �����ϴ� �� ������ �����մϴ�.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 targetPoint;

        // 2. Raycast�� Ư�� ���̾ �浹�ߴ��� Ȯ���մϴ�.
        // �浹�ߴٸ�, �� ������ �Ѿ��� ��ǥ�� �����մϴ�.
        if (Physics.Raycast(ray, out hit, float.MaxValue, shootableMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            // 3. �浹���� �ʾҴٸ�, ī�޶󿡼� ���� �Ÿ� ������ ������ ������ ��ǥ�� �����մϴ�.
            // �̷��� �ϸ� � ������Ʈ�͵� �浹���� �ʾƵ� �Ѿ��� �߻�˴ϴ�.
            targetPoint = ray.GetPoint(100f); // 100f�� ���ϴ� �Ÿ��� ���� �����մϴ�.
        }

        // 4. Raycast ���(���� �浹 ���� �Ǵ� ������ ����)�� ������� �Ѿ��� �߻��մϴ�.
        Vector3 direction = (targetPoint - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }
        
    }
}