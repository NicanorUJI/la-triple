using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSorter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    // Factor para escalar el valor Y a un entero útil
    [SerializeField] private int sortingOffset = 100;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        // Cuanto más abajo (menor Y), mayor sortingOrder → aparece delante
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * sortingOffset);
    }
}
