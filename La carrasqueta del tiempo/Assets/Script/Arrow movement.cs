using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 5f;

    [Header("Fondo")]
    public SpriteRenderer background;
    public float padding = 0.1f;

    [Header("Límite máximo en Y")]
    public float maxY = 5f;

    [Header("Audio")]
    public AudioSource stepAudioSource;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    [Header("Animaciones")]
    public Animator animator;

    private float minX, maxX, minY;
    public bool canPlayerMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ----------------------------------------------------
        // GESTIÓN DE AUDIO DE PASOS (Centralizada en Player)
        // ----------------------------------------------------
        if (AudioManager.instance != null)
        {
            if (stepAudioSource == null)
            {
                stepAudioSource = gameObject.AddComponent<AudioSource>();
            }

            stepAudioSource.clip = AudioManager.instance.ClipPasos;

            stepAudioSource.loop = true;
            stepAudioSource.playOnAwake = false;

            // Asignamos el grupo del mixer SFX (¡El volumen se gestiona globalmente!)
            stepAudioSource.outputAudioMixerGroup = AudioManager.instance.mainMixer.FindMatchingGroups("SFX")[0];

            // Establecemos el volumen base del AudioSource a 1.0f (0 dB)
            // El volumen final lo determinará el mixer.
            stepAudioSource.volume = 1f;
        }
        else
        {
            Debug.LogError("AudioManager.instance no se ha inicializado.");
        }
        // ----------------------------------------------------

        // Calcular límites
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

        if (canPlayerMove) {
            //MOVIMIENTO HORIZONTAL
            if (keyboard.dKey.isPressed)
                moveX = 1f;
            else if (keyboard.aKey.isPressed)
                moveX = -1f;

            //MOVIMIENTO VERTICAL
            if (keyboard.wKey.isPressed)
                moveY = 1f;
            else if (keyboard.sKey.isPressed)
                moveY = -1f;
        }

        //PARA LAS ANIMACIONES
        //Si se está moviendo, en qué direccion (horizontal y vertical)
        if (moveX != 0)
            if(animator.GetBool("isGoingUp")) animator.SetBool("isGoingUp", false);
            animator.SetBool("isGoingLeft", moveX < 0);

        if (moveY != 0)
            animator.SetBool("isGoingUp", moveY > 0);

        //Comrpobar si está quieto
        bool isMoving = (moveX != 0 || moveY != 0);
        animator.SetBool("isMoving", isMoving);

        //-------------------------------------------------------------------

        moveInput = new Vector2(moveX, moveY);

        // Predecir la próxima posición
        float nextX = rb.position.x + moveInput.x * speed * Time.deltaTime;
        float nextY = rb.position.y + moveInput.y * speed * Time.deltaTime;

        // Límites X
        if (nextX - 0.2 > maxX && moveInput.x > 0) moveInput.x = 0;
        if (nextX + 0.2 < minX && moveInput.x < 0) moveInput.x = 0;

        // Límites Y
        if (nextY + 0.12 > maxY && moveInput.y > 0) moveInput.y = 0;
        if (nextY + 0.1 < minY && moveInput.y < 0) moveInput.y = 0;

        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        HandleStepAudio();
    }
    
    // Método para actualizar el volumen si se cambia desde el menú de pausa
    public void UpdateStepVolume(float newVolume)
    {
        if (stepAudioSource != null)
        {
            stepAudioSource.volume = newVolume;
        }
    }


    void HandleStepAudio()
    {
        if (stepAudioSource == null) return; 

        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            if (!stepAudioSource.isPlaying)
                stepAudioSource.Play();
        }
        else
        {
            if (stepAudioSource.isPlaying)
                stepAudioSource.Stop();
        }
    }

    void FixedUpdate()
    {
        // Usamos linearVelocity para mover el Rigidbody
        rb.linearVelocity = moveInput * speed;
    }
}