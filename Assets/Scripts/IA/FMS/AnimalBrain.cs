using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class AnimalBrain : MonoBehaviour
{
    private AnimalState currentState;

    public Dictionary<AnimalStateType, AnimalState> states = new Dictionary<AnimalStateType, AnimalState>();

    [HideInInspector] public AnimalMovement movement;
    [HideInInspector] public AnimalSensor sensor;
    [HideInInspector] public Animator animator;

    private AudioSource audioSource;

    [Header("Animal Audio")]
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private float soundInterval = 8f;
    private float soundTimer;

    [Header("Audio State")]
    [SerializeField] private bool isCaptured = false;

    // sirve para inicializar las referencias a los componentes necesarios en el Awake, asegurando que el script tenga acceso a ellos antes de cualquier lógica de estado o sonido
    private void Awake()
    {
        movement = GetComponent<AnimalMovement>();
        sensor = GetComponent<AnimalSensor>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // En el Start, se configuran los estados del animal y se establece el estado inicial. Esto permite que el animal comience a comportarse de acuerdo con su lógica de estado desde el momento en que el juego comienza.
    private void Start()
    {
        states.Add(AnimalStateType.Idle, new IdleState(this));
        states.Add(AnimalStateType.Walk, new WalkState(this));
        states.Add(AnimalStateType.RunAway, new RunAwayState(this));

        ChangeState(AnimalStateType.Walk);
    }

    // El método Update se encarga de actualizar el estado actual del animal y manejar la reproducción de sonidos. Si el animal no está capturado, se reproduce un sonido de fondo a intervalos regulares. Esto añade una capa de realismo al comportamiento del animal, haciendo que el entorno sea más inmersivo.
    private void Update()
    {
        currentState?.Update();
        if (!isCaptured)
        {
            HandleAnimalSound();
        }
    }

    // Este método permite cambiar el estado de captura del animal. Si el animal es capturado, se detiene cualquier sonido que esté reproduciéndose. Esto es importante para reflejar el cambio en el comportamiento del animal y evitar que siga emitiendo sonidos mientras está capturado, lo que podría romper la inmersión o causar confusión al jugador.
    public void SetCaptured(bool value)
    {
        isCaptured = value;

        if (isCaptured)
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    // Este método maneja la lógica de reproducción de sonidos para el animal. Utiliza un temporizador para determinar cuándo reproducir el siguiente sonido, asegurando que los sonidos se reproduzcan a intervalos regulares. Si el audioSource y el idleSound están configurados correctamente, se reproducirá el sonido de fondo del animal, lo que contribuye a la atmósfera del juego.
    private void HandleAnimalSound()
    {
        soundTimer += Time.deltaTime;

        if (soundTimer >= soundInterval)
        {
            if (audioSource != null && idleSound != null)
            {
                audioSource.clip = idleSound;
                audioSource.Play();
            }

            soundTimer = 0f;
        }
    }

    // Este método es fundamental para la gestión de estados del animal. Permite cambiar el estado actual del animal a un nuevo estado especificado por el parámetro newState. Antes de cambiar al nuevo estado, se llama al método Exit() del estado actual para realizar cualquier limpieza necesaria. Luego, se asigna el nuevo estado desde el diccionario de estados y se llama al método Enter() del nuevo estado para inicializarlo. Esto asegura una transición suave entre estados y permite que cada estado maneje su propia lógica de entrada y salida.
    public void ChangeState(AnimalStateType newState)
    {
        currentState?.Exit();
        currentState = states[newState];
        currentState.Enter();
    }
}
