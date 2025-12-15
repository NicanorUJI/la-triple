using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BearFollower2D : MonoBehaviour
{
    public float speed = 2.4f;
    public float stopDistanceX = 0.8f;

    [Header("Anti-atasco")]
    public float stuckSeconds = 2.0f;
    public float minDeltaDistanceX = 0.05f;

    private Rigidbody2D rb;
    private Transform target;

    private float lastAbsDx = Mathf.Infinity;
    private float stuckTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Top-down: sin gravedad, sin rotación, sin movimiento en Y por física
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    private void FixedUpdate()
    {
        // Si terminó el acto 3, fuera
        if (GameManager.Check("Act3_End"))
        {
            Destroy(gameObject);
            return;
        }

        // Solo persigue cuando está activo
        if (!GameManager.Check("Act3_BearActive"))
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (target == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            target = go != null ? go.transform : null;
            if (target == null) return;
        }

        // Solo perseguir en X
        float dx = target.position.x - transform.position.x;
        float absDx = Mathf.Abs(dx);

        if (absDx <= stopDistanceX)
        {
            rb.linearVelocity = Vector2.zero;
            stuckTimer = 0f;
            lastAbsDx = absDx;
            return;
        }

        float dirX = Mathf.Sign(dx);
        rb.linearVelocity = new Vector2(dirX * speed, 0f);

        // Anti-atasco: si no progresa en X, reposiciona cerca del jugador en X (manteniendo Y)
        float progress = lastAbsDx - absDx;
        if (progress < minDeltaDistanceX) stuckTimer += Time.fixedDeltaTime;
        else stuckTimer = 0f;

        lastAbsDx = absDx;

        if (stuckTimer >= stuckSeconds)
        {
            float offsetX = Random.value < 0.5f ? -1.5f : 1.5f;
            Vector3 pos = transform.position;
            pos.x = target.position.x + offsetX;

            transform.position = pos;
            rb.linearVelocity = Vector2.zero;
            lastAbsDx = Mathf.Infinity;

            stuckTimer = 0f;
        }
    }
}
