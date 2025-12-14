using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class FakeTargetMover : MonoBehaviour
{
    public float speed = 5f;
    public Transform background;

    private float minX, maxX;

    void Start()
    {
        if (background != null)
        {
            // Calcula los límites en X basados en el tamaño del sprite
            SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
            float halfWidth = sr.bounds.size.x / 2f;
            minX = background.position.x - halfWidth;
            maxX = background.position.x + halfWidth;
        }
    }
    void Update()
    {
        PiRecuentoManager gm = FindAnyObjectByType<PiRecuentoManager>();
        if (gm == null || !gm.inputEnabled) return;

        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector3 newPos = transform.position + new Vector3(move, 0, 0);

        // Limita la posición dentro de los bordes
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);

        transform.position = newPos;
    }
}

