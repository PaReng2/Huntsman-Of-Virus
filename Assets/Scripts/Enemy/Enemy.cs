using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform target;
    //public EnemySO enemyData;
    
    private PlayerController player;
    //private float curEnemyHP;

    NavMeshAgent agent;

    private void Awake()
    {
        //enemyData = FindObjectOfType<EnemySO>();
        player = FindObjectOfType<PlayerController>();
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.Find("Player").transform;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        //curEnemyHP = enemyData.EnemyHP;
    }
   private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Bullet"))
        {
            StageManager.Instance.OnEnemyKilled(); // 매니저에게 알림
            //curEnemyHP -= player.attackPower;
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
