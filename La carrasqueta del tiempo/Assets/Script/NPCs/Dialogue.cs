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

    [Header("Skip diálogo")]
    public bool skipDialogueWhenFlagSet = false;
    public string skipDialogueFlag;
    public string skipDialogueScene;


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
            if (opcion == null)
                continue;

            bool correct = true;
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
            {
                if (opcion.name == "Act2_Quest_Espardenyes_Return"
                    && GameManager.Check("Act2_Q_ESP_Done"))
                {
                    continue;
                }
                if (opcion.name == "Act2_Quest_Llanca_Return"
                    && GameManager.Check("Act2_Q_LLANCE_Done"))
                {
                    continue;
                }
                if (opcion.name == "Act2_Quest_Menjar_ReturnHoney"
                    && (GameManager.Check("Act2_Q_MENJAR_Done") || GameManager.Check("Act2_Q_MENJAR_NeedsMeat")))
                {
                    continue;
                }
                if (opcion.name == "Act2_Quest_Menjar_ReturnMeat"
                    && GameManager.Check("Act2_Q_MENJAR_Done"))
                {
                    continue;
                }
                if (opcion.name == "Act2_Quest_Esquelles_Intro"
                    && GameManager.Check("Act2_Q_ESQUELLES_Done"))
                {
                    continue;
                }
                
                if (opcion.name == "Act3_Start_MaripiliPasado"
                    && GameManager.Check("Act3_Started"))
                {
                    continue;
                }
                if (opcion.name == "Act3_BarranquetPasado_Briefing"
                    && GameManager.Check("Act3_PastBriefingDone"))
                {
                    continue;
                }
                if (opcion.name == "Act3_BarranquetPasado_Celebration"
                    && GameManager.Check("Act3_PastCelebrationDone"))
                {
                    continue;
                }
                if (opcion.name == "Act3_CarrasquetaPresent_Warn"
                    && GameManager.Check("Act3_CarrasquetaPresentWarned"))
                {
                    continue;
                }
                if (opcion.name == "Act3_BarranquetPresent_Final"
                    && GameManager.Check("Act3_End"))
                {
                    continue;
                }
                return opcion;
            }
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

        // 1) Si ya hay un diálogo activo, solo avanzamos líneas
        if (isDialogueActive)
        {
            NextLine();
            return;
        }

        // 2) Si está configurado para saltar diálogo cuando un flag está activo
        if (skipDialogueWhenFlagSet
            && !string.IsNullOrEmpty(skipDialogueFlag)
            && GameManager.Check(skipDialogueFlag))
        {
            Debug.Log($"NPC {name}: skip de diálogo per flag {skipDialogueFlag}");

            if (!string.IsNullOrEmpty(skipDialogueScene))
            {
                SceneManager.LoadScene(skipDialogueScene);
            }

            return;
        }

        // 3) Comportamiento normal: buscamos qué diálogo tocaría ahora
        dialogueData = getDialogue();

        if (dialogueData == null)
        {
            Debug.LogWarning($"NPC {name}: getDialogue() va tornar null, no hi ha diàleg que complisca condicions.");
            return;
        }

        if (dialogueData.lines == null || dialogueData.lines.Length == 0)
        {
            Debug.LogWarning($"NPC {name}: el diàleg '{dialogueData.name}' no té línies.");
            return;
        }

        originalDialogue = dialogueData;
        StartDialogue();
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

        // CASO 1: aún se está escribiendo la línea -> la terminamos de golpe
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = line.lineText;
            isTyping = false;

            if (line.isChoice)
            {
                ShowChoices(line);
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

            return;
        }

        // CASO 2: ya ha terminado de escribir
        // Si hay opciones en pantalla, no avanzamos con E
        if (isChoiceActive)
            return;

        // Pasamos a la siguiente línea
        dialogueIndex++;

        if (dialogueIndex < dialogueData.lines.Length)
        {
            line = dialogueData.lines[dialogueIndex];
            SetPortraitFromLine(line);
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
        isChoiceActive = false;

        dialogueText.SetText("");
        nameText.text = line.speakerName;

        foreach (char letter in line.lineText)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(.05f);
        }

        isTyping = false;

        // Una vez escrita la línea, vemos qué toca
        if (line.isChoice)
        {
            ShowChoices(line);
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
        if (button_choice1 != null) button_choice1.SetActive(false);
        if (button_choice2 != null) button_choice2.SetActive(false);
        if (button_choice3 != null) button_choice3.SetActive(false);
        if (button_choice4 != null) button_choice4.SetActive(false);

        if (joaquinPortrait_Object != null)
            joaquinPortrait_Object.SetActive(true);

        isChoiceActive = false;

        if (nextDialogue == null)
        {
            EndDialogue();
            return;
        }

        LoadDialogue(nextDialogue);
    }

    private void ActivateMinigame(string minigame)
    {
        Debug.Log("Se va a activar minijuego");
        SceneManager.LoadScene(minigame);
    }

    private void ShowChoices(DialogueLine line)
    {
        isChoiceActive = true;
        joaquinPortrait_Object.SetActive(false);

        // Apagamos todos los botones al principio
        if (button_choice1 != null) button_choice1.SetActive(false);
        if (button_choice2 != null) button_choice2.SetActive(false);
        if (button_choice3 != null) button_choice3.SetActive(false);
        if (button_choice4 != null) button_choice4.SetActive(false);

        // Configuramos cada botón solo si hay diálogo asignado
        SetupChoiceButton(button_choice1, line.choiceA);
        SetupChoiceButton(button_choice2, line.choiceB);
        SetupChoiceButton(button_choice3, line.choiceC);
        SetupChoiceButton(button_choice4, line.choiceD);
    }

    private void SetupChoiceButton(GameObject buttonObj, NPCDialogue nextDialogue)
    {
        if (buttonObj == null)
            return;

        var button = buttonObj.GetComponent<Button>();
        if (button == null)
            return;

        // Si no hay diálogo, ocultamos el botón
        if (nextDialogue == null)
        {
            buttonObj.SetActive(false);
            button.onClick.RemoveAllListeners();
            return;
        }

        // si la opción es de una quest ya completada, ocultar el botón
        if (!IsQuestChoiceAvailable(nextDialogue))
        {
            buttonObj.SetActive(false);
            button.onClick.RemoveAllListeners();
            return;
        }

        // Si está disponible, activamos el botón y registramos el click
        buttonObj.SetActive(true);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnChoiceSelected(nextDialogue));
    }

    private bool IsQuestChoiceAvailable(NPCDialogue dlg)
    {
        if (dlg == null)
            return false;

        // Usar nombres exactos de los NPCDialogues en la carpeta Dialogues/Misiones!!

        // Espardenyes
        if (dlg.name == "Act2_Quest_Espardenyes_Intro" && GameManager.Check("Act2_Q_ESP_Done"))
            return false;

        // Llança
        if (dlg.name == "Act2_Quest_Llanca_Intro" && GameManager.Check("Act2_Q_LLANCE_Done"))
            return false;

        // Menjar
        if (dlg.name == "Act2_Quest_Menjar_Intro" && GameManager.Check("Act2_Q_MENJAR_Done"))
            return false;

        return true;
    }
}