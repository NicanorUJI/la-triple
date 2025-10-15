using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;

    [Header("Fondo")]
    public SpriteRenderer background;
    public float padding = 0.1f; // espacio desde el borde del fondo

    [Header("Límite máximo en Y")]
    public float maxY = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private float minX, maxX, minY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Calcular límites según el fondo y el padding
        Vector3 bgPos = background.transform.position;
        Vector2 bgSize = background.bounds.size;

        minX = bgPos.x - bgSize.x / 2f + padding;
        maxX = bgPos.x + bgSize.x / 2f - padding;
        minY = bgPos.y - bgSize.y / 2f + padding;
    }

    void Update()
    {
        var keyboard = Keyboard.current;

        float moveX = 0f;
        float moveY = 0f;

        if (keyboard.rightArrowKey.isPressed) moveX = 1f;
        if (keyboard.leftArrowKey.isPressed) moveX = -1f;
        if (keyboard.upArrowKey.isPressed) moveY = 1f;
        if (keyboard.downArrowKey.isPressed) moveY = -1f;

        moveInput = new Vector2(moveX, moveY);

        // Predecir la próxima posición
        float nextX = rb.position.x + moveInput.x * speed * Time.deltaTime;
        float nextY = rb.position.y + moveInput.y * speed * Time.deltaTime;

        // Limitar movimiento en X
        if (nextX-0.2 > maxX && moveInput.x > 0) moveInput.x = 0;
        if (nextX+0.2 < minX && moveInput.x < 0) moveInput.x = 0;

        // Limitar movimiento en Y
        if (nextY+0.12 > maxY && moveInput.y > 0) moveInput.y = 0; // límite superior asignable
        if (nextY+0.1 < minY && moveInput.y < 0) moveInput.y = 0; // límite inferior según fondo

        // Normalizar solo si es necesario
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * speed;
    }
}
