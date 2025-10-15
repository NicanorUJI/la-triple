using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShadowSorterSafeFinal : MonoBehaviour
{
    [SerializeField] private int sortingOffset = 100;
    [SerializeField] private Transform player; // jugador
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void LateUpdate()
    {
        if (player == null) return;

        // Orden del jugador
        int playerOrder = Mathf.RoundToInt(-player.position.y * sortingOffset);

        // La sombra intenta estar justo delante del jugador
        int shadowOrder = playerOrder + 1;

        // Encuentra todos los objetos con SpriteRenderer en la escena
        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();

        foreach (var obj in allSprites)
        {
            if (obj.transform == player || obj.transform == transform) continue;

            int objOrder = Mathf.RoundToInt(-obj.transform.position.y * sortingOffset);

            // Si el objeto está delante del jugador (orden menor) y la sombra lo supera
            if (objOrder < playerOrder && shadowOrder <= objOrder)
            {
                shadowOrder = objOrder - 1; // colocar sombra detrás del objeto
            }

            // Si el objeto está detrás del jugador, no afecta

        }

        spriteRenderer.sortingOrder = shadowOrder;
    }
}
