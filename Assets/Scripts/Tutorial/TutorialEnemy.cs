using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        DialogueNPC npcDialogue = FindObjectOfType<DialogueNPC>();
        if (collision.gameObject.tag == "Bullet" && npcDialogue.dialogueNum > 3)
        {
            Destroy(gameObject);
        }
    }
}
