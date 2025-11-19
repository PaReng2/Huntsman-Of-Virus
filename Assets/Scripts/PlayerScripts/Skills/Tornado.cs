using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    float dist = 7f;            //이동거리
    float speed = 5f;           //이동 속도
    float frequency = 20f;      //파동 빈도
    float waveHeight = 0.5f;    //파동 높이
    int lifeTime = 5;

    public int curTornadoDamage;

    Vector3 pos, localScale;
    bool dirRight = true;

    ChaseEnemy chaseEnemy;

    private void Awake()
    {
        chaseEnemy = FindAnyObjectByType<ChaseEnemy>();    
    }

    void Start()
    {
        curTornadoDamage = 30;
        pos = transform.position;
        localScale = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.x > dist)
        {
            dirRight = false;
        }
        else if (transform.position.x < -dist)
        {
            dirRight = true;
        }

        if (dirRight)
        {
            GoRight();
        }
        else
        {
            GoLeft();
        }
    }

    private void Update()
    {
        Destroy(gameObject, lifeTime);
    }
    void GoRight()
    {
        localScale.x = 1;
        transform.transform.localScale = localScale;
        pos -= transform.right * Time.deltaTime * speed;
        transform.position = pos + transform.up * Mathf.Sin(Time.time * frequency) * waveHeight;
    }
    void GoLeft()
    {
        localScale.x = -1;
        transform.transform.localScale = localScale;
        pos -= transform.right * Time.deltaTime * speed;
        transform.position = pos + transform.up * Mathf.Sin(Time.time * frequency) * waveHeight;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            chaseEnemy.TakeDamage(curTornadoDamage);
        }
    }
}
