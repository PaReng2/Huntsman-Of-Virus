using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName ="Item", menuName ="Item/New_Item")]
public class ItemSO : ScriptableObject
{
    public string ItemName;
    public Sprite ItemIMG;
    public float increaseHealth;
    public float increaseSpeed;
    public float increaseDamage;
}
