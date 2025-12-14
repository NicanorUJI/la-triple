using UnityEngine;

[CreateAssetMenu(fileName = "carta", menuName = "Scriptable Objects/carta")]
public class carta : ScriptableObject
{
    public Sprite carta_imagen;
    public int tipo; //El 1 es el AS y el 2 es el Joker
    public bool activo;
}
