using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class BearFollower2D : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 2.4f;

    [Header("Posición relativa")]
    public float followOffsetX = -1.2f; // distancia fija respecto al player
    public float followOffsetY = 0f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private Transform target;

    private bool initialized = false;

    private Vector3 lastPlayerPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void InitAtPlayer(Transform player)
    {
        target = player;

        // Posición correcta desde el inicio (offset)
        Vector3 startPos = new Vector3(
            player.position.x + followOffsetX,
            player.position.y + followOffsetY,
            transform.position.z
        );

        rb.position = startPos;
        lastPlayerPos = player.position;

        initialized = true;
    }

    private void FixedUpdate()
    {
        if (!initialized)
            return;
        // Comprobaciones de estado
        if (GameManager.Check("Act3_End"))
        {
            Destroy(gameObject);
            return;
        }

        if (!GameManager.Check("Act3_BearActive"))
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("walk", false);
            return;
        }

        // Encontrar al jugador
        if (target == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            target = go != null ? go.transform : null;
            if (target == null) return;

            lastPlayerPos = target.position;
        }

        // Dirección de movimiento del jugador
        Vector3 playerMove = target.position - lastPlayerPos;
        lastPlayerPos = target.position;

        // Posición deseada del oso (offset relativo)
        Vector2 desiredPos = new Vector2(
            target.position.x + followOffsetX,
            target.position.y + followOffsetY
        );

        // Calculamos el vector de movimiento hacia la posición deseada
        Vector2 moveDir = desiredPos - rb.position;

        // Si nos estamos moviendo, normalizamos y aplicamos velocidad
        if (moveDir.magnitude > 0.001f)
        {
            Vector2 moveStep = moveDir.normalized * speed * Time.fixedDeltaTime;
            if (moveStep.magnitude > moveDir.magnitude)
                moveStep = moveDir; // no pasarse de la posición deseada
            rb.MovePosition(rb.position + moveStep);
        }

        // Determinar si el jugador se acerca al oso
        bool playerApproachingBear =
            Mathf.Sign(playerMove.x) == Mathf.Sign(rb.position.x - target.position.x) &&
            Mathf.Abs(playerMove.x) > 0.001f;

        // Animación instantánea según movimiento real
        anim.SetBool("walk", moveDir.magnitude > 0.001f);

        // Flip del oso
        if (playerApproachingBear)
        {
            // Huye: mira en dirección contraria al player
            sr.flipX = moveDir.x < 0f;
        }
        else
        {
            // Normal: mira al player
            sr.flipX = target.position.x < rb.position.x;
        }
    }
}
