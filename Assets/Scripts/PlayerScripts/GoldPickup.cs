using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPickup : MonoBehaviour
{
    public int amount;
    PlayerController playerController;

    [SerializeField]
    private int goldMoveSpeed;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        AutoTakeGold();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (playerController != null)
            {
                playerController.AddGold(amount);
            }

            Destroy(gameObject);
        }
    }

    void AutoTakeGold()
    {
        int playerLayer = LayerMask.GetMask("Player");

        Collider[] collider = Physics.OverlapSphere(transform.position, 3f, playerLayer);

        bool hasPlayer= collider.Length > 0;

        Vector3 playerTranform = playerController.transform.position;

        if (hasPlayer)
        {
            if (Vector3.Distance(playerTranform, transform.position) > 0.1f)
            {
                transform.position += (playerTranform - transform.position).normalized * goldMoveSpeed * Time.deltaTime;
            }
        }
    }
}
