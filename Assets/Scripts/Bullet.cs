using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("�Ѿ� ���󰡴� �ӵ� ����")]
    public float speed = 20f;                       //�Ѿ� �ӵ�
    private float destroyDelay = 5f;                //�Ѿ��� �ڵ����� �������� �ð�

    private Vector3 moveDir;                        //�Ѿ� ����

    void Start()
    {
       //�����ð��� ������ �ı�
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
        //���� �浹�ϸ� �ı�
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);

        }
    }
}
