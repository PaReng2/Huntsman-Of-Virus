using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject bulletPrefab;         // �Ѿ� ������
    public Transform firePos;               // �߻� ��ġ

    void Update()
    {
        // ���콺 ���� ��ư Ŭ�� �� �߻�
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000);
        }

        Vector3 dir = (targetPoint - firePos.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePos.position, Quaternion.identity);

        bullet.GetComponent<Bullet>().SetDirection(dir);
    }
}