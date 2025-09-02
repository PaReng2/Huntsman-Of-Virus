using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform target;

    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player").transform;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        //ÃÑ¾Ë°ú Ãæµ¹ÇÏ¸é ÆÄ±«
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);

        }
    }
    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(target.position);

            Vector3 direction = (target.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
    }
}
