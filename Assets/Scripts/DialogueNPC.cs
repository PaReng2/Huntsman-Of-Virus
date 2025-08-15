using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueNPC : MonoBehaviour
{
    public DialogueDataSO myDialogue;
    public Image interactionImage;
    public LayerMask playerLayer;
    public GameObject isInsteraction;
    
    private DialogueManager dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager is Null");
        }

    }

    private void Update()
    {
        int playerLayer = LayerMask.GetMask("Player");
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f, playerLayer);
        
        bool hasPlayer = colliders.Length > 0;

        isInsteraction.SetActive(hasPlayer);

        if (hasPlayer)
        {
            isDialogue();
        }
       
    }

    void isDialogue()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (dialogueManager == null) return;
            if (dialogueManager.IsDialogueActive()) return;
            if (myDialogue == null) return;
            
            dialogueManager.StartDialogue(myDialogue);
        }
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
