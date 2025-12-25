using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "NPC")]
public class NPCDialogue : ScriptableObject
{
    public NPCDialogue originalDialogue;
    public Sprite defaultSprite;
    public Sprite joaquinDefault;
    public DialogueLine[] lines;

    [Header("Condiciones")]
    public string[] conditions;
}
