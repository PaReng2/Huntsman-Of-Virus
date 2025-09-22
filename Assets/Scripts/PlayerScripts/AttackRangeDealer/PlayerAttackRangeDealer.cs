using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackRangeDealer : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;
    public Transform firePoint;
    public PlayerStatusSO playerData;
    public bool isInteracting;
    

    private void Start()
    {
        isInteracting = false;
    }

    private void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            if (isInteracting)
            {
                return;
            }
            else if(!isInteracting)
            {
                Attack();

            }

        }
    }

    void Attack()
    {
        GameObject intantBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = firePoint.forward * 50;

        
    }
}
