using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("총알 날라가는 속도 조정")]
    public float speed = 20f;                       //총알 속도
    private float destroyDelay = 5f;                //총알이 자동으로 없어지는 시간

    private Vector3 moveDir;                        //총알 방향

    void Start()
    {
       //일정시간이 지나면 파괴
        Destroy(gameObject, destroyDelay);
    }

    void Update()
    {
        transform.position += moveDir * speed * Time.deltaTime;
    }

    public void SetDirection(Vector3 dir)
    {
        moveDir = dir;
        transform.rotation = Quaternion.LookRotation(dir);
    }
    private void OnCollisionEnter(Collision collision)
    {
        //적과 충돌하면 파괴
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);

        }
    }
}
