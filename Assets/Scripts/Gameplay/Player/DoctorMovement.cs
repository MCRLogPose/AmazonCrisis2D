using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoctorMovement : MonoBehaviour
{
    public float jumpForce;
    public float speed;
    PlayerControls controls;

    [Header("Double Jump Settings")]
    [SerializeField] private int maxJumps = 2;
    private int remainingJumps;

    [Header("Audio Settings")]
    [Tooltip("Si está activo, los pasos sonarán automáticamente basados en tiempo. Desactívalo si prefieres usar Animation Events.")]
    [SerializeField] private bool useTimerBasedFootsteps = true;
    [Tooltip("Lista de clips de audio para los pasos. ¡Arrastra aquí los 50 archivos de sonido de paso de hierba!")]
    [SerializeField] private AudioClip[] footstepClips;
    [Tooltip("Intervalo en segundos entre cada paso al caminar.")]
    [SerializeField] private float footstepInterval = 0.35f;
    [Range(0f, 1f)] [SerializeField] private float footstepVolume = 0.8f;
    [Range(0.5f, 1.5f)] [SerializeField] private float pitchMin = 0.85f;
    [Range(0.5f, 1.5f)] [SerializeField] private float pitchMax = 1.15f;

    private float footstepTimer;
    private AudioSource audioSource;

    private new Rigidbody2D rigidbody2D;
    private float horizontalInput;
    private bool isGrounded;
    private Animator animator;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Enable();

        controls.Land.Walk.performed += ctx => horizontalInput = ctx.ReadValue<float>();
        controls.Land.Walk.canceled += ctx => horizontalInput = 0.0f;

        // Saltará al presionar el botón de salto (admite doble salto en el aire)
        controls.Land.Jump.started += ctx =>
        {
            if (ctx.ReadValue<float>() > 0)
                TryJump();
        };
        controls.Land.Jump.canceled += ctx => { };
    }

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Obtener o añadir el componente AudioSource local para el sonido 3D/2D
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0.0f; // Audio 2D por defecto, cambiar a 1.0f en el inspector si es 3D
        }

        // Inicializar el timer de pasos para que suene el primer paso inmediatamente al caminar
        footstepTimer = footstepInterval;
    }
    void Update()
    {
        //horizontalInput = Input.GetAxisRaw("Horizontal");

        // Animaciones
        animator.SetBool("isJumping", !isGrounded);
        animator.SetBool("isWalk", isGrounded && horizontalInput != 0.0f);

        // Girar personaje
        if (horizontalInput < 0.0f)
            transform.localScale = new Vector3(-4, 4, 1);
        else if (horizontalInput > 0.0f)
            transform.localScale = new Vector3(4, 4, 1);

        // Raycast correcto
        Debug.DrawRay(transform.position, Vector3.down * 1.6f, Color.red);
        isGrounded = Physics2D.Raycast(transform.position, Vector3.down, 1.6f);
        if (isGrounded && rigidbody2D.velocity.y <= 0.01f)
        {
            remainingJumps = maxJumps;
        }

        // Lógica de reproducción de pasos basada en tiempo
        if (useTimerBasedFootsteps)
        {
            if (isGrounded && horizontalInput != 0.0f)
            {
                footstepTimer += Time.deltaTime;
                if (footstepTimer >= footstepInterval)
                {
                    PlayRandomFootstep();
                    footstepTimer = 0f;
                }
            }
            else
            {
                // Al detenerse, reiniciamos el temporizador al intervalo para que al volver a caminar
                // el primer paso suene de forma instantánea y reactiva.
                footstepTimer = footstepInterval;
            }
        }
    }
    private void TryJump()
    {
        if (!isGrounded && remainingJumps == maxJumps)
        {
            remainingJumps = maxJumps - 1; // Pierde el primer salto si se cayó de un borde sin saltar
        }

        if (remainingJumps > 0)
        {
            // Cancelar velocidad vertical previa para que el segundo salto se sienta limpio y con fuerza constante
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);
            rigidbody2D.AddForce(Vector2.up * jumpForce);
            remainingJumps--;
        }
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = new Vector2(horizontalInput * speed, rigidbody2D.velocity.y);
    }

    /// <summary>
    /// Reproduce un sonido de paso aleatorio con variación de volumen y tono (pitch).
    /// Puede llamarse mediante el temporizador interno o a través de Animation Events en la animación de caminar.
    /// </summary>
    public void PlayRandomFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0)
            return;

        if (audioSource == null)
            return;

        // Seleccionar un clip aleatorio de la lista
        int randomIndex = Random.Range(0, footstepClips.Length);
        AudioClip clip = footstepClips[randomIndex];

        if (clip != null)
        {
            // Variación aleatoria de pitch (tono) para evitar la monotonía mecánica
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            
            // Variación sutil de volumen alrededor del valor base
            float randomVolume = Random.Range(footstepVolume * 0.9f, footstepVolume * 1.1f);
            audioSource.PlayOneShot(clip, Mathf.Clamp01(randomVolume));
        }
    }
}
