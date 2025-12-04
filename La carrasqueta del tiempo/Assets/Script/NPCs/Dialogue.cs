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
            bool correct = true;
            foreach (string condicion in opcion.conditions)
            {
                if(!GameManager.Check(condicion)) { correct = false; break; }
            }
            if(correct) return opcion;

        }
        Debug.Log("Hemos  llegado aqui");
        return null; //no deberia llegar aqui
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
        isChoiceActive = false;
        dialogueIndex = 0;
        
        movement.canPlayerMove = false;

        DialogueLine line = dialogueData.lines[dialogueIndex];
        nameText.text = line.speakerName;
        portraitImage.sprite = line.expression;

        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine(line));

    }

    void NextLine()
    {
        DialogueLine line = dialogueData.lines[dialogueIndex];

        if (isTyping)
        {
            StopAllCoroutines();
            //dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            dialogueText.text = dialogueData.lines[dialogueIndex].lineText;

            isTyping = false;

            if (line.isChoice)
            {
                //por algun motivo solo hace lo de poner los 4 botones A VECES ?????
                if(line.choiceC == null)
                {
                    isChoiceActive = true;
                    Debug.Log("Eligiendo 2");
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

                else
                {
                    isChoiceActive = true;
                    Debug.Log("Eligiendo 4");
                    joaquinPortrait_Object.SetActive(false);
                    button_choice1.SetActive(true);
                    button_choice2.SetActive(true);
                    button_choice3.SetActive(true);
                    button_choice4.SetActive(true);

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
                

            }
            else if (line.hasReward)
            {
                rewardManager = FindObjectOfType<RewardManager>();
                rewardManager.giveReward(line.reward);

            }

            else if (line.startsMinigame)
            {
                ActivateMinigame(line.minigame);
            }


        }

        //Si hay mas lineas de texto
        else if(++dialogueIndex < dialogueData.lines.Length && !isChoiceActive)
        {
            Debug.Log(dialogueIndex);
            //Cambiar expresiones de los portraits
            line = dialogueData.lines[dialogueIndex];
            if (line.isPlayerSpeaking) joaquinPortrait.sprite = line.expression;
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
            rewardManager = FindObjectOfType<RewardManager>();
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



