using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour, IInteractable
{
    //public NPCDialogue dialogueData;
    public NPCDialogue[] opciones;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;

    [Header("Retratos")]
    public Image portraitImage;
    public GameObject joaquinPortrait_Object;
    public Image joaquinPortrait;

    [Header("Botones")]
    public GameObject button_choice1;
    public GameObject button_choice2;
    public GameObject button_choice3;
    public GameObject button_choice4;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive, isChoiceActive;
    private NPCDialogue originalDialogue;
    private PlayerMovement movement;
    private RewardManager rewardManager;

    private NPCDialogue dialogueData;

    public NPCDialogue getDialogue()
    {
        foreach (NPCDialogue opcion in opciones)
        {
            // Si el elemento está vacío en el inspector, lo saltamos
            if (opcion == null)
                continue;

            bool correct = true;

            // Si las conditions son null, lo tratamos como "sin condiciones"
            var conds = opcion.conditions;

            if (conds != null)
            {
                foreach (string condicion in conds)
                {
                    if (!GameManager.Check(condicion))
                    {
                        correct = false;
                        break;
                    }
                }
            }

            if (correct)
                return opcion;
        }

        Debug.LogWarning("NPC.getDialogue: no se encontró ningún diálogo válido en 'opciones'.");
        return null;
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        Debug.Log("Interactuando");
        movement = FindObjectOfType<PlayerMovement>();

        dialogueData = getDialogue();

        // 🔹 Si no hay diálogo válido, salimos sin romper nada
        if (dialogueData == null)
        {
            Debug.LogWarning($"NPC {name}: getDialogue() devolvió null, no hay diálogo que cumpla las condiciones.");
            return;
        }

        // 🔹 Si el diálogo no tiene líneas, también salimos
        if (dialogueData.lines == null || dialogueData.lines.Length == 0)
        {
            Debug.LogWarning($"NPC {name}: el diálogo '{dialogueData.name}' no tiene líneas.");
            return;
        }

        // En caso de que haya diálogo empezado, ir a la siguiente línea
        if (isDialogueActive)
        {
            NextLine();
        }
        // Si no hay diálogo empezado, empezar la conversación
        else
        {
            originalDialogue = dialogueData;
            StartDialogue();
        }
    }

    void StartDialogue()
    {

        isDialogueActive = true;
        isChoiceActive = false;
        dialogueIndex = 0;
        
        movement.canPlayerMove = false;

        Dialogu﻿eLine line = dialogueData.lines[dialogueIndex];
        nameText.text = line.speakerName;

        SetPortraitFromLine(line);

        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine(line));

    }

    private void SetPortraitFromLine(DialogueLine line)
    {
        if (line.isPlayerSpeaking)
        {
            // Habla Joaquín
            joaquinPortrait.sprite = line.expression;
        }
        else
        {
            // Habla el NPC
            portraitImage.sprite = line.expression;
        }
    }

    void NextLine()
    {
        if (dialogueData == null || dialogueData.lines == null || dialogueData.lines.Length == 0)
        {
            EndDialogue();
            return;
        }

        if (dialogueIndex < 0)
            dialogueIndex = 0;

        if (dialogueIndex >= dialogueData.lines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueData.lines[dialogueIndex];

        // 🟡 CASO 1: estaba escribiendo y el jugador pulsa E -> terminar de escribir esta línea
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = line.lineText;
            isTyping = false;

            if (line.isChoice)
            {
                if (line.choiceC == null)
                {
                    isChoiceActive = true;
                    Debug.Log("Eligiendo 2");
                    joaquinPortrait_Object.SetActive(false);
                    button_choice1.SetActive(true);
                    button_choice2.SetActive(true);

                    Button b1 = button_choice1.GetComponent<Button>();
                    Button b2 = button_choice2.GetComponent<Button>();

                    b1.onClick.RemoveAllListeners();
                    b2.onClick.RemoveAllListeners();

                    DialogueLine current = line;

                    b1.onClick.AddListener(() => OnChoiceSelected(current.choiceA));
                    b2.onClick.AddListener(() => OnChoiceSelected(current.choiceB));
                }
                else
                {
                    isChoiceActive = true;
                    Debug.Log("Eligiendo 4");
                    joaquinPortrait_Object.SetActive(false);
                    button_choice1.SetActive(true);
                    button_choice2.SetActive(true);
                    button_choice3.SetActive(true);
                    button_choice4.SetActive(true);

                    Button b1 = button_choice1.GetComponent<Button>();
                    Button b2 = button_choice2.GetComponent<Button>();

                    b1.onClick.RemoveAllListeners();
                    b2.onClick.RemoveAllListeners();

                    DialogueLine current = line;

                    b1.onClick.AddListener(() => OnChoiceSelected(current.choiceA));
                    b2.onClick.AddListener(() => OnChoiceSelected(current.choiceB));
                }
            }
            else if (line.hasReward)
            {
                if (rewardManager == null)
                    rewardManager = FindObjectOfType<RewardManager>();

                if (rewardManager != null)
                    rewardManager.giveReward(line.reward);
            }

            else if (line.startsMinigame)
            {
                ActivateMinigame(line.minigame);
            }

            return; // importante: no seguir a la parte de abajo
        }

        // 🟡 CASO 2: ya se terminó de escribir la línea, pasar a la siguiente
        if (!isChoiceActive && ++dialogueIndex < dialogueData.lines.Length)
        {
            Debug.Log(dialogueIndex);

            line = dialogueData.lines[dialogueIndex];

            SetPortraitFromLine(line);

            StartCoroutine(TypeLine(line));
        }
        else
        {
            // No hay más líneas -> cerrar diálogo
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

        //Si hay opciones de dialogo, salen una vez se acaba el dialogo
        if (line.isChoice)
        {
            isChoiceActive = true;
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

        else if (line.hasReward)
        {
            if (rewardManager == null)
                rewardManager = FindObjectOfType<RewardManager>();

            if (rewardManager != null)
                rewardManager.giveReward(line.reward);
        }

        else if (line.startsMinigame)
        {
            ActivateMinigame(line.minigame);
        }
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
        movement.canPlayerMove = true;
    }


    private void OnChoiceSelected(NPCDialogue nextDialogue)
    {
        button_choice1.SetActive(false);
        button_choice2.SetActive(false);
        joaquinPortrait_Object.SetActive(true);
        isChoiceActive = false;

        if (nextDialogue == null)
        {
            return;
        }

        LoadDialogue(nextDialogue);
    }

    private void ActivateMinigame(string minigame)
    {
        Debug.Log("Se va a activar minijuego");
        SceneManager.LoadScene(minigame);
    }


}



