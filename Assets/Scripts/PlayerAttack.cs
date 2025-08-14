using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject Bullet;
    public Transform FirePos;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Instantiate(Bullet, FirePos.transform.position, FirePos.transform.rotation);
        }
    }
}
