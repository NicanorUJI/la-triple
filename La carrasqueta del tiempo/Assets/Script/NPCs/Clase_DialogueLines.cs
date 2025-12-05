using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea] public string lineText;
    public Sprite expression;
    public bool isPlayerSpeaking;


    [Header("Para dialogos con elecciones")]
    public bool isChoice;
    public NPCDialogue choiceA;
    public NPCDialogue choiceB;
    public NPCDialogue choiceC;
    public NPCDialogue choiceD;

    [Header("Dialogos con recompensas")]
    public bool hasReward;
    public string reward;

    [Header("Dialogo para minijuegos")]
    public bool startsMinigame;
    public string minigame;
}

[System.Serializable]
public class MisionLine
{
    public string speakerName;
    [TextArea] public string lineText;
    public Sprite expression;
    public bool isPlayerSpeaking;

    [Header("Para dialogos con elecciones")]
    public bool isChoice;
    public NPCDialogue choiceA;
    public NPCDialogue choiceB;
    public NPCDialogue choiceC;
    public NPCDialogue choiceD;
}

