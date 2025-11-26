using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTutorialGate : MonoBehaviour
{
    DialogueNPC npc;

    private void Awake()
    {
        gameObject.SetActive(false);
        npc = FindAnyObjectByType<DialogueNPC>();
    }

    private void Update()
    {
        if (npc.dialogueNum <= 4)
        {
            Debug.Log("Æ©Åä¸®¾ó °Ç³Ê¶Ü");
        }
    }


}
