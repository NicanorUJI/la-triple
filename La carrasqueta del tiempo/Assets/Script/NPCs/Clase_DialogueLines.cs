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
    public NPCDialogue choiceD  ;
}

