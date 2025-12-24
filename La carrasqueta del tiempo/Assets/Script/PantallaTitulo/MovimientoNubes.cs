using UnityEngine;

public class MovimientoNubes : MonoBehaviour
{
    public float timePerStep = 2f;   // tiempo entre movimientos
    public float pixelsPerUnit = 16f;

    float screenRight;
    float screenLeft;
    float cloudWidth;

    float realX;
    float timer;

    void Start()
    {
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;

        screenRight = Camera.main.transform.position.x + screenWidth;
        screenLeft = Camera.main.transform.position.x - screenWidth;

        cloudWidth = GetComponent<SpriteRenderer>().bounds.size.x;

        realX = transform.position.x;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timePerStep)
        {
            timer = 0f;

            //mover 1 píxel
            realX -= 1f / pixelsPerUnit;

            // wrap infinito
            if (realX < screenLeft - cloudWidth / 2)
            {
                realX = screenRight + cloudWidth / 2;
            }

            // ajuste a píxel (aquí ya es exacto, pero por seguridad)
            float pixelX = Mathf.Round(realX * pixelsPerUnit) / pixelsPerUnit;

            transform.position = new Vector3(
                pixelX,
                transform.position.y,
                transform.position.z
            );
        }
    }
}


