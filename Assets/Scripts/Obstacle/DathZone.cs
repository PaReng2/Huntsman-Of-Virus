using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DathZone : MonoBehaviour
{
    PlayerController controller;

    private void Awake()
    {
        controller = FindAnyObjectByType<PlayerController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            controller.Die();
        }
    }
}
