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

    [Header("Retratos")]
    public Image portraitImage;
    public GameObject joaquinPortrait_Object;
    public Image joaquinPortrait;

    [Header("Botones")]
    public GameObject button_choice1;
    public GameObject button_choice2;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private NPCDialogue originalDialogue;

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        Debug.Log("Interactuando");

        //Si no hay dialogo, no puede hablar
        if (dialogueData.lines.Length == 0)
        { return; }

        //En caso de que haya dialogo empezado, ir a la siguiente linea
        if (isDialogueActive)
        {
            NextLine();
        }
        //Si no hay dialogo empezado, empezar la conversacion
        else
        {
            originalDialogue = dialogueData;
            StartDialogue();
        }


    }

    void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;
        /*
        nameText.SetText(dialogueData.npcName);
        portraitImage.sprite = dialogueData.expresiones[dialogueIndex];*/

        DialogueLine line = dialogueData.lines[dialogueIndex];
        nameText.text = line.speakerName;
        portraitImage.sprite = line.expression;

        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine(line));

    }

    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            //dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            dialogueText.text = dialogueData.lines[dialogueIndex].lineText;
            isTyping = false;
        }

        //Si hay mas lineas de texto
        else if(++dialogueIndex < dialogueData.lines.Length)
        {
            DialogueLine line = dialogueData.lines[dialogueIndex];

            if(line.isChoice)
            {
                Debug.Log(dialogueIndex);
                joaquinPortrait_Object.SetActive(false);
                button_choice1.SetActive(true);
                button_choice2.SetActive(true);

                // get the Button components
                Button b1 = button_choice1.GetComponent<Button>();
                Button b2 = button_choice2.GetComponent<Button>();

                // clear previous listeners
                b1.onClick.RemoveAllListeners();
                b2.onClick.RemoveAllListeners();

                // capture the current line into a local variable for the closures
                DialogueLine current = line;

                // add listeners that call a single handler with the chosen dialogue
                b1.onClick.AddListener(() => OnChoiceSelected(current.choiceA));
                b2.onClick.AddListener(() => OnChoiceSelected(current.choiceB));

            }

            if(line.isPlayerSpeaking) joaquinPortrait.sprite = line.expression;
            else portraitImage.sprite = line.expression;
            StartCoroutine(TypeLine(line));
        }

        else
        {
            EndDialogue();
        }

    }

    IEnumerator TypeLine(DialogueLine line)
    {
        isTyping = true;
        dialogueText.SetText("");
        nameText.text = line.speakerName;


        foreach (char letter in line.lineText)
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(.05f);
            }

        isTyping = false;
    }

    public void LoadDialogue(NPCDialogue newDialogue)
    {
        dialogueData = newDialogue;
        StartDialogue();
    }

    public void EndDialogue()
    {
        dialogueData = originalDialogue;
        StopAllCoroutines();
        isDialogueActive =false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
    }


    private void OnChoiceSelected(NPCDialogue nextDialogue)
    {
        Debug.Log($"OnChoiceSelected called. current index = {dialogueIndex} on instance {GetInstanceID()}");
        // hide the choice UI
        button_choice1.SetActive(false);
        button_choice2.SetActive(false);
        joaquinPortrait_Object.SetActive(true);

        if (nextDialogue == null)
        {
            Debug.LogError("Choice has no dialogue assigned!");
            return;
        }

        LoadDialogue(nextDialogue);
    }


}



