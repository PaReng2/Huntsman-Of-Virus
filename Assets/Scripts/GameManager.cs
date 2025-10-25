using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isInteracting;
    public TextMeshProUGUI hpText;
    private PlayerController player;

    private void Awake()
    {
        isInteracting = false;
        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        hpText.text =  $"{player.curPlayerHp} / {player.playerMaxHP}";
    }

}
