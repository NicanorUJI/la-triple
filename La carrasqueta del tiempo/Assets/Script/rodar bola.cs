using UnityEngine;

public class RotarAlColisionar : MonoBehaviour
{
    public float torque = 10f;        // Fuerza de rotación
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Aplica torque cada vez que colisiona con algo
        rb.AddTorque(torque, ForceMode2D.Impulse);
    }
}