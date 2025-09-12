using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName ="Item", menuName ="Item/New_Item")]
public class ItemSO : ScriptableObject
{
    public Sprite ItemIMG;
    public float potionHealStat;
    public float WeaponDemage;
    [Header("ItemType : 1 - Weapon, 2 - potion")]
    public int ItemType;
}
