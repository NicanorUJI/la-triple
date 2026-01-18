using UnityEngine;

public class RotarSegunMovimiento : MonoBehaviour
{
    Rigidbody2D rb;
    CircleCollider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

    void FixedUpdate()
    {
        float velocidad = rb.linearVelocity.x;

        // radio real del collider
        float radio = col.radius * transform.lossyScale.x;

        // ω = v / r  (física real)
        float angularVel = -(velocidad / radio) * Mathf.Rad2Deg;

        rb.angularVelocity = angularVel;

        // cortar micro vibraciones
        if (Mathf.Abs(velocidad) < 0.05f)
        {
            rb.angularVelocity = 0f;
        }
    }
}
