using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName ="PlayerStat", menuName ="Player/PlayerStat")]
public class PlayerStatusSO : ScriptableObject
{
    public static PlayerStatusSO instance;

    private void OnEnable()
    {
        instance = this;
    }

    public int playerHP;
    public float playerAttackPower;
    public float playerAttackRate;
    public float playerMoveSpeed;
    public float playerAttackRange;
    public int playerCurLevel;
    public int playerCurEXP;
}
