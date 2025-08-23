using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        //ÃÑ¾Ë°ú Ãæµ¹ÇÏ¸é ÆÄ±«
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);

        }
    }
}
