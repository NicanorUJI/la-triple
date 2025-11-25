using UnityEngine;

[CreateAssetMenu(fileName = "NPC_MisionPrincipalSelector", menuName = "MisionPrincipalSelector")]
public class NPC_MisionPrincipalSelector : ScriptableObject
{
    public Sprite defaultSprite;
    public MisionLine[] lines;
}
