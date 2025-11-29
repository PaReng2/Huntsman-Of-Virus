using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private PlayerSkills playerSkills;

    public void SetPlayerSkills(PlayerSkills skills)
    {
        playerSkills = skills;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (playerSkills != null)
            {
                playerSkills.ShieldDestroyed();
            }

            Destroy(gameObject);
        }
    }
}
