using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName ="PlayerStat", menuName ="Player/PlayerStat")]
public class PlayerStatusSO : ScriptableObject
{
    public float playerHP;
    public float playerAttackPower;
    public float playerAttackRate;
    public float playerMoveSpeed;
    public float playerAttackRange;
}
