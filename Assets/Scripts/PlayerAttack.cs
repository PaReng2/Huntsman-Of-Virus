using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject BulletPrefab;   // ÃÑ¾Ë ÇÁ¸®ÆÕ
    public Transform FirePos;         // ¹ß»ç À§Ä¡

    void Update()
    {
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

        Vector3 dir = (targetPoint - FirePos.position).normalized;

        GameObject bullet = Instantiate(BulletPrefab, FirePos.position, Quaternion.identity);

        bullet.GetComponent<Bullet>().SetDirection(dir);
    }
}