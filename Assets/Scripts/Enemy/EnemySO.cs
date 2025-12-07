using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Enemy", menuName ="Enemy/NewEnemy")]
public class EnemySO : ScriptableObject
{
    public float EnemyHP;
    public float EnemyAttackPower;
    public float EnemyMoveSpeed = 3.5f;
}
