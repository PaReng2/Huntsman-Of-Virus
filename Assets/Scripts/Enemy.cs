using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //�Ѿ˰� �浹�ϸ� �ı�
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);

        }
    }
}
