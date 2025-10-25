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
    public float AttackRate;
    public float curLeftAttackTime;

    private GameManager gameManager;
    public Animator animator;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        //if (animator == null )
        //    animator = gameManager.GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        AttackRate = playerData.playerAttackRate;
    }

    private void Update()
    {
        isInteracting = gameManager.isInteracting;

        // curLeftAttackTime이 0보다 클 때만 감소시켜서 음수가 되는 것을 방지합니다.
        if (curLeftAttackTime > 0)
        {
            curLeftAttackTime -= Time.deltaTime;
        }

        // 이 아래는 원래 코드와 동일합니다.
        if (Input.GetMouseButtonDown(0))
        {
            if (curLeftAttackTime <= 0)
            {
                if (isInteracting)
                {
                    Debug.Log("상호작용중 공격불가");
                    return;
                }
                else if (!isInteracting)
                {
                    Attack();   
                    // 공격 후 쿨다운 시간을 AttackRate로 재설정합니다.
                    curLeftAttackTime = AttackRate;
                    
                }
            }
            else
            {
                Debug.Log("재정비중");
                return;
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
