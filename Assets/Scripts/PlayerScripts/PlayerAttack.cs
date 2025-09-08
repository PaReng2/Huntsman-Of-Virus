using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject bulletPrefab;         // 총알 프리팹
    public Transform firePos;               // 발사 위치

    void Update()
    {
        // 마우스 왼쪽 버튼 클릭 시 발사
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