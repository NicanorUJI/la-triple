using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        Debug.Log("Interactuando");
        
        //Si no hay dialogo, no puede hablar
        if (dialogueData == null)
        { return; }

        //En caso de que haya dialogo empezado, ir a la siguiente linea
        if (isDialogueActive)
        {
            NextLine();
        }
        //Si no hay dialogo empezado, empezar la conversacion
        else
        {
            StartDialogue();
        }


    }

    void StartDialogue()
    {
        
        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.npcPortrait;

        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine());

    }

    void NextLine()
    {
        if(isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }

        //Si hay mas lineas de texto
        else if(++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }

        else
        {
            EndDialogue();
        }

    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex]) 
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(.05f);
        }

        isTyping = false;
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive =false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
    }
}


