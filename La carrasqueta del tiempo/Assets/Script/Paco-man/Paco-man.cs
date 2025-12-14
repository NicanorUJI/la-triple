using UnityEngine;

public class PacoManMovement : MonoBehaviour
{
    public float speed = 5f;
    public LayerMask wallLayer;
    public string turnTag = "Turn";
    public string doorTag = "Door";

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 nextDirection = Vector2.zero;

    private Rigidbody2D rb;
    private Collider2D col;

    private float turnOverlapMargin = 0.01f;
    private Vector3 startPosition;

    public bool isImmortal = false;
    public float immortalTime = 0.5f;

    // NUEVO
    private bool wasTouchingTurn = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.gravityScale = 0;
        startPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) nextDirection = Vector2.up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) nextDirection = Vector2.down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) nextDirection = Vector2.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) nextDirection = Vector2.right;

        UpdateRotation();
    }

    private void FixedUpdate()
    {
        bool touchingTurnCollider = IsTouchingTurnCollider();

        // ===== CENTRAR AL ENTRAR EN UN GIRO =====
        if (touchingTurnCollider && !wasTouchingTurn)
        {
            SnapToTurnCollider();
        }

        if (nextDirection != Vector2.zero)
        {
            if (moveDirection == Vector2.zero)
            {
                if (CanMoveFully(nextDirection))
                {
                    moveDirection = nextDirection;
                    nextDirection = Vector2.zero;
                }
            }
            else
            {
                bool isOpposite =
                    Vector2.Dot(moveDirection.normalized, nextDirection.normalized) < -0.9f;

                bool isPerpendicular =
                    Mathf.Abs(Vector2.Dot(moveDirection.normalized, nextDirection.normalized)) < 0.2f;

                if (isOpposite && CanMoveFully(nextDirection))
                {
                    moveDirection = nextDirection;
                    nextDirection = Vector2.zero;
                }
                else if (isPerpendicular && touchingTurnCollider && CanMoveFully(nextDirection))
                {
                    moveDirection = nextDirection;
                    nextDirection = Vector2.zero;
                }
            }
        }

        wasTouchingTurn = touchingTurnCollider;

        rb.linearVelocity = CanMoveFully(moveDirection)
            ? moveDirection * speed
            : Vector2.zero;
    }

    private void UpdateRotation()
    {
        if (moveDirection == Vector2.zero) return;

        if (moveDirection == Vector2.right)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else if (moveDirection == Vector2.left)
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        else if (moveDirection == Vector2.up)
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        else if (moveDirection == Vector2.down)
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
    }

    // ===== CENTRADO EN GIRO =====
    private void SnapToTurnCollider()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            col.bounds.center,
            col.bounds.size * 0.9f,
            0f
        );

        foreach (Collider2D h in hits)
        {
            if (h.CompareTag(turnTag))
            {
                transform.position = new Vector3(
                    h.bounds.center.x,
                    h.bounds.center.y,
                    transform.position.z
                );
                break;
            }
        }
    }

    private bool CanMoveFully(Vector2 direction)
    {
        if (direction == Vector2.zero) return false;

        Vector2 position = col.bounds.center;
        Vector2 size = col.bounds.size;
        float distance = Mathf.Max(size.x, size.y) / 2f + 0.05f;

        RaycastHit2D hit = Physics2D.BoxCast(
            position,
            size * 0.9f,
            0f,
            direction,
            distance,
            wallLayer
        );

        if (hit.collider == null) return true;
        if (hit.collider.CompareTag(doorTag)) return true;
        return false;
    }

    private bool IsTouchingTurnCollider()
    {
        Vector2 center = col.bounds.center;
        Vector2 size = col.bounds.size;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            center,
            size * 0.9f + Vector2.one * turnOverlapMargin,
            0f
        );

        foreach (Collider2D h in hits)
        {
            if (h == col) continue;
            if (h.CompareTag(turnTag)) return true;
        }

        return false;
    }

    public void RespawnPacman()
    {
        transform.position = startPosition;
        moveDirection = Vector2.zero;
        nextDirection = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        isImmortal = false;
    }

    public void ReceiveDamage(GhostyMovement ghost = null)
    {
        if (isImmortal) return;
        if (ghost != null && (ghost.IsEaten() || ghost.IsScared())) return;

        isImmortal = true;
        Manager.Instance.DamagePlayer();
        Invoke(nameof(ResetImmortal), immortalTime);
    }

    private void ResetImmortal() => isImmortal = false;
}
