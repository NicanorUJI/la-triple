using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Tiempo : MonoBehaviour, IInteractable
{
    [Header("Datos de diálogo")]
    public NPCDialogue dialogueData;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text nameText;
    public Image portraitImage;

    [Header("Escena siguiente")]
    public string nextSceneName; // nombre de la escena a cargar al terminar

    private int dialogueIndex;
    private bool isTyping;
    private bool isDialogueActive;
    private Coroutine typingCoroutine;

    public void Interact()
    {
        // Llamado por el sistema de interacción cuando el jugador interactúa con este objeto
        if (!isDialogueActive)
        {
            StartDialogue();
        }
        else if (!isTyping)
        {
            NextLine();
        }
    }

    public bool CanInteract()
    {
        return true;
    }

    private void StartDialogue()
    {
        if (dialogueData == null || dialogueData.lines == null || dialogueData.lines.Length == 0)
        {
            Debug.LogWarning("Tiempo: dialogueData no tiene líneas, se cambia directamente de escena.");
            EndDialogue();
            return;
        }

        dialogueIndex = 0;
        isDialogueActive = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (dialogueData == null || dialogueData.lines == null)
        {
            EndDialogue();
            return;
        }

        if (dialogueIndex < 0 || dialogueIndex >= dialogueData.lines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueData.lines[dialogueIndex];

        // Nombre del hablante
        if (nameText != null)
            nameText.text = line.speakerName;

        // Retrato: usa la expresión de la línea, o el sprite por defecto del NPC
        if (portraitImage != null)
        {
            Sprite spriteToUse = line.expression != null ? line.expression : dialogueData.defaultSprite;

            if (spriteToUse != null)
            {
                portraitImage.enabled = true;
                portraitImage.sprite = spriteToUse;
            }
            else
            {
                portraitImage.enabled = false;
            }
        }

        // Texto con efecto “typewriter”
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line.lineText));
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;

        if (dialogueText != null)
            dialogueText.text = "";

        foreach (char c in text)
        {
            if (dialogueText != null)
                dialogueText.text += c;

            // Ajusta la velocidad si quieres
            yield return new WaitForSeconds(0.02f);
        }

        isTyping = false;
    }

    private void NextLine()
    {
        dialogueIndex++;

        if (dialogueData == null || dialogueData.lines == null || dialogueIndex >= dialogueData.lines.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowCurrentLine();
        }
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        isTyping = false;
        isDialogueActive = false;

        if (dialogueText != null)
            dialogueText.text = "";

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        // Cambiar de escena si se ha asignado un nombre válido
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
