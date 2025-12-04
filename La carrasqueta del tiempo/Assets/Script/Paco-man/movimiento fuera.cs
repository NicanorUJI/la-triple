using UnityEngine;
using System.Collections.Generic;

public class GhostyMovement : MonoBehaviour
{
    [Header("General Movement")]
    public float speed = 4f;
    public LayerMask wallLayer;
    public string turnTag = "Turn";
    public string doorTag = "Door";

    private enum GhostState { Waiting, Normal }
    private GhostState state = GhostState.Waiting;

    private Vector2 moveDirection = Vector2.zero;
    private Vector3 startPosition;

    private Rigidbody2D rb;
    private Collider2D col;

    private bool canChooseDirection = true;
    private bool wasTouchingTurn = false;
    private float turnOverlapMargin = 0.02f;

    private bool hasDamaged = false;

    // Detectar patrones repetidos
    private Queue<Vector2> lastMoves = new Queue<Vector2>();
    private int patternRepeatLimit = 4;

    // Primeros movimientos obligatorios
    private Queue<Vector2> initialPattern = new Queue<Vector2>();
    private bool executingInitialPattern = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;
        startPosition = transform.position;
    }

    private void Start()
    {
        bool chooseFirstPattern = Random.value < 0.5f;

        initialPattern.Clear();
        if (chooseFirstPattern)
        {
            initialPattern.Enqueue(Vector2.right);
            initialPattern.Enqueue(Vector2.up);
            initialPattern.Enqueue(Vector2.right);
            initialPattern.Enqueue(Vector2.down);
            initialPattern.Enqueue(Vector2.right);
        }
        else
        {
            initialPattern.Enqueue(Vector2.left);
            initialPattern.Enqueue(Vector2.up);
            initialPattern.Enqueue(Vector2.left);
            initialPattern.Enqueue(Vector2.down);
            initialPattern.Enqueue(Vector2.left);
        }

        executingInitialPattern = true;
        state = GhostState.Normal;
    }

    private void FixedUpdate()
    {
        if (state == GhostState.Normal)
            NormalMovementLogic();
        else
            rb.linearVelocity = Vector2.zero;
    }

    private void NormalMovementLogic()
    {
        bool touchingTurnCollider = IsTouchingTurnCollider();

        // Primeros movimientos obligatorios
        if (executingInitialPattern && initialPattern.Count > 0)
        {
            moveDirection = initialPattern.Dequeue();
            rb.linearVelocity = CanMoveFully(moveDirection) ? moveDirection * speed : Vector2.zero;

            if (initialPattern.Count == 0)
                executingInitialPattern = false;

            wasTouchingTurn = touchingTurnCollider;
            return;
        }

        if (touchingTurnCollider && !wasTouchingTurn)
        {
            SnapToTurnCollider();
            TryChooseNewDirection();
        }

        if (touchingTurnCollider && wasTouchingTurn)
        {
            if (!CanMoveFully(moveDirection) && canChooseDirection)
            {
                canChooseDirection = false;
                Invoke(nameof(ForceChangeDirection), 0.1f);
            }
        }

        wasTouchingTurn = touchingTurnCollider;
        rb.linearVelocity = CanMoveFully(moveDirection) ? moveDirection * speed : Vector2.zero;
    }

    private void ForceChangeDirection()
    {
        if (!CanMoveFully(moveDirection))
        {
            moveDirection = GetRandomDirectionExceptCurrent(moveDirection, allowPatternOverride: true);
            AddMoveToHistory(moveDirection);
        }
        canChooseDirection = true;
    }

    private void SnapToTurnCollider()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(col.bounds.center, col.bounds.size * 0.9f, 0f);
        foreach (var h in hits)
        {
            if (h.CompareTag(turnTag))
            {
                transform.position = new Vector3(h.bounds.center.x, h.bounds.center.y, transform.position.z);
                break;
            }
        }
    }

    private void TryChooseNewDirection()
    {
        moveDirection = GetRandomDirectionExceptCurrent(moveDirection);
        canChooseDirection = false;
        Invoke(nameof(ResetChooseFlag), 0.2f);

        AddMoveToHistory(moveDirection);

        if (IsRepeatingPattern())
        {
            moveDirection = GetRandomDirectionExceptCurrent(moveDirection, allowPatternOverride: true);
            AddMoveToHistory(moveDirection);
        }
    }

    private void AddMoveToHistory(Vector2 dir)
    {
        lastMoves.Enqueue(dir);
        if (lastMoves.Count > patternRepeatLimit) lastMoves.Dequeue();
    }

    private void ResetChooseFlag() => canChooseDirection = true;

    private bool IsRepeatingPattern()
    {
        if (lastMoves.Count < patternRepeatLimit) return false;
        Vector2[] arr = lastMoves.ToArray();
        for (int i = 0; i <= arr.Length - 4; i++)
            if (arr[i] == arr[i + 2] && arr[i + 1] == arr[i + 3])
                return true;
        return false;
    }

    private bool CanMoveFully(Vector2 direction)
    {
        if (direction == Vector2.zero) return false;

        Vector2 position = col.bounds.center;
        Vector2 size = col.bounds.size;
        float distance = Mathf.Max(size.x, size.y) / 2f + 0.05f;

        RaycastHit2D hit = Physics2D.BoxCast(position, size * 0.9f, 0f, direction, distance, wallLayer);
        if (hit.collider == null) return true;

        if (hit.collider.CompareTag(doorTag))
            return direction == Vector2.up; // solo atraviesan hacia arriba

        return false;
    }

    private bool IsTouchingTurnCollider()
    {
        Vector2 center = col.bounds.center;
        Vector2 size = col.bounds.size;
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size * 0.9f + Vector2.one * turnOverlapMargin, 0f);
        foreach (var h in hits)
            if (h != col && h.CompareTag(turnTag)) return true;
        return false;
    }

    private Vector2 GetRandomDirectionExceptCurrent(Vector2 current, bool allowPatternOverride = false)
    {
        Vector2[] dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        List<Vector2> valid = new List<Vector2>();
        Vector2 opposite = current != Vector2.zero ? -current : Vector2.zero;

        foreach (var d in dirs)
            if (d != current && d != opposite && CanMoveFully(d))
                valid.Add(d);

        if (valid.Count == 0)
            return allowPatternOverride ? dirs[Random.Range(0, dirs.Length)] : current;

        return valid[Random.Range(0, valid.Count)];
    }

    public void RespawnGhost()
    {
        transform.position = startPosition;
        moveDirection = Vector2.zero;
        wasTouchingTurn = false;
        canChooseDirection = true;
        lastMoves.Clear();
        initialPattern.Clear();
        executingInitialPattern = true;
        state = GhostState.Waiting;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasDamaged)
        {
            hasDamaged = true;
            collision.GetComponent<PacoManMovement>()?.ReceiveDamage();
            Invoke(nameof(ResetDamageFlag), 0.5f);
        }
    }

    private void ResetDamageFlag() => hasDamaged = false;
}
