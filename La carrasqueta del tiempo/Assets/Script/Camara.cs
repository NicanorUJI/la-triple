using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow2D : MonoBehaviour
{
    public Transform target;            // El jugador a seguir
    public SpriteRenderer background;   // El fondo
    public float smoothSpeed = 0.125f;  // Suavidad del movimiento
    public Vector3 offset = new Vector3(0, 0, -10);

    private float camHalfHeight;
    private float camHalfWidth;
    private float minX, maxX;
    private float fixedY;

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        // Ajustamos la cámara para que vea toda la altura del fondo
        float bgHeight = background.bounds.size.y;
        cam.orthographicSize = bgHeight / 2f;

        // Calculamos la mitad del tamaño visible de la cámara
        camHalfHeight = cam.orthographicSize;
        camHalfWidth = cam.aspect * camHalfHeight;

        // Guardamos la Y fija (para que siempre se vea toda la altura)
        fixedY = background.transform.position.y;

        // Calculamos límites en X
        float bgWidth = background.bounds.size.x;
        Vector3 bgPos = background.transform.position;

        minX = bgPos.x - bgWidth / 2f + camHalfWidth;
        maxX = bgPos.x + bgWidth / 2f - camHalfWidth;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Posición deseada del jugador
        float clampedX = Mathf.Clamp(target.position.x, minX, maxX);

        Vector3 desiredPosition = new Vector3(clampedX, fixedY, offset.z);

        // Movimiento suave
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}
