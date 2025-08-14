using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float DestroyDelay = 5.0f;
    void Start()
    {
        Destroy(gameObject, DestroyDelay);
    }
    void Update()
    {
        transform.Translate(Vector3.forward * 1f);
    }

}
