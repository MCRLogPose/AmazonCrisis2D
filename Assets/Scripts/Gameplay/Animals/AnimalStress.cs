using UnityEngine;
using UnityEngine.UI;

public class AnimalStress : MonoBehaviour
{
    [Header("Stress Settings")]
    public float maxStress = 100f;
    public float currentStress = 50f; // Comienza en nivel medio (50)
    public float stressIncreaseRate = 3f; // Puntos de estrés por segundo de incremento

    [Header("UI Reference")]
    [Tooltip("Control deslizante (Slider) opcional para mostrar la barra de estrés sobre el animal.")]
    [SerializeField] private Slider stressSlider;

    private AnimalBrain brain;
    private HealthAnimals health;

    // Valores originales del animal para actuar como base
    private float originalWalkSpeed = 2f;
    private float originalRunSpeed = 4f;
    private float originalJumpForce = 5f;
    private float originalDetectionDistance = 3f;

    private void Awake()
    {
        brain = GetComponent<AnimalBrain>();
        health = GetComponent<HealthAnimals>();
    }

    private void Start()
    {
        // Guardar valores originales de movimiento y sensor si están disponibles
        if (brain != null)
        {
            if (brain.movement != null)
            {
                originalWalkSpeed = brain.movement.walkSpeed;
                originalRunSpeed = brain.movement.runSpeed;
                originalJumpForce = brain.movement.jumpForce;
            }

            if (brain.sensor != null)
            {
                originalDetectionDistance = brain.sensor.detectionDistance;
            }
        }

        if (stressSlider != null)
        {
            stressSlider.maxValue = maxStress;
            stressSlider.value = currentStress;
        }
    }

    private AudioAnalyzer audioAnalyzer;

    private void Update()
    {
        float difficultyMultiplier = 1f;
        if (LevelDifficultyConfig.Instance != null)
            difficultyMultiplier = LevelDifficultyConfig.Instance.stressMultiplier;

        float stressChange = stressIncreaseRate * difficultyMultiplier;

        // Si el jugador está cerca, el micrófono influye en el estrés
        if (brain != null && brain.sensor != null && brain.sensor.IsPlayerNear())
        {
            if (audioAnalyzer == null)
            {
                audioAnalyzer = FindObjectOfType<AudioAnalyzer>();
            }

            if (audioAnalyzer != null && audioAnalyzer.IsRecording)
            {
                float rateNormal = -6f;
                float rateSilence = stressIncreaseRate;
                float rateNervous = 8f;
                float ratePanic = 10f;

                if (LevelDifficultyConfig.Instance != null)
                {
                    rateSilence = LevelDifficultyConfig.Instance.stressRateOnSilence;
                    rateNormal = LevelDifficultyConfig.Instance.stressRateOnNormal;
                    rateNervous = LevelDifficultyConfig.Instance.stressRateOnNervous;
                    ratePanic = LevelDifficultyConfig.Instance.stressRateOnPanic;
                }

                switch (audioAnalyzer.CurrentEmotionState)
                {
                    case PlayerEmotionState.NORMAL:
                        stressChange = rateNormal;
                        break;
                    case PlayerEmotionState.SILENCE:
                        stressChange = rateSilence;
                        break;
                    case PlayerEmotionState.NERVOUS:
                        stressChange = rateNervous;
                        break;
                    case PlayerEmotionState.PANIC:
                        stressChange = ratePanic;
                        break;
                }
            }
        }

        // Aplicar el cambio de estrés correspondiente
        if (stressChange > 0)
        {
            IncreaseStress(stressChange * Time.deltaTime);
        }
        else if (stressChange < 0)
        {
            DecreaseStress(Mathf.Abs(stressChange) * Time.deltaTime);
        }

        // Aplicar los efectos según el nivel de estrés
        ApplyStressEffects();
    }


    /// <summary>
    /// Incrementa el estrés en un monto específico.
    /// </summary>
    public void IncreaseStress(float amount)
    {
        currentStress = Mathf.Clamp(currentStress + amount, 0f, maxStress);
        if (stressSlider != null)
        {
            stressSlider.value = currentStress;
        }
    }

    /// <summary>
    /// Disminuye el estrés en un monto específico.
    /// </summary>
    public void DecreaseStress(float amount)
    {
        currentStress = Mathf.Clamp(currentStress - amount, 0f, maxStress);
        if (stressSlider != null)
        {
            stressSlider.value = currentStress;
        }
    }

    /// <summary>
    /// Aplica multiplicadores y efectos de rango dinámicos basados en el nivel de estrés.
    /// </summary>
    private void ApplyStressEffects()
    {
        if (currentStress >= 70f)
        {
            // ESTRÉS ALTO (70 a 100):
            // - Corre más rápido (1.6x)
            // - Salta más alto (1.4x)
            // - Rango de detección más amplio (1.8x) -> Más alerta, huye mucho antes (difícil de capturar)
            if (brain != null)
            {
                if (brain.movement != null)
                {
                    brain.movement.walkSpeed = originalWalkSpeed * 1.5f;
                    brain.movement.runSpeed = originalRunSpeed * 1.6f;
                    brain.movement.jumpForce = originalJumpForce * 1.4f;
                }

                if (brain.sensor != null)
                {
                    brain.sensor.detectionDistance = originalDetectionDistance * 1.8f;
                }
            }
        }
        else if (currentStress < 30f)
        {
            // ESTRÉS BAJO (< 30):
            // - Más calmado, camina y corre lento (0.7x)
            // - Menos alerta (0.5x de distancia de detección) -> Muy fácil de acercarse
            // - Regeneración lenta de salud
            if (brain != null)
            {
                if (brain.movement != null)
                {
                    brain.movement.walkSpeed = originalWalkSpeed * 0.7f;
                    brain.movement.runSpeed = originalRunSpeed * 0.7f;
                    brain.movement.jumpForce = originalJumpForce * 0.8f;
                }

                if (brain.sensor != null)
                {
                    brain.sensor.detectionDistance = originalDetectionDistance * 0.5f;
                }
            }

            // Regeneración lenta de salud
            if (health != null && health.currentHealth < health.maxHealth && health.currentHealth > 0f)
            {
                // Regenerar 1.5 puntos de salud por segundo
                health.currentHealth = Mathf.Min(health.currentHealth + 1.5f * Time.deltaTime, health.maxHealth);
                if (health.healthBar != null)
                {
                    health.healthBar.SetHealth(health.currentHealth);
                }
            }
        }
        else
        {
            // RANGO MEDIO (30 a 70):
            // Comportamiento normal del animal
            if (brain != null)
            {
                if (brain.movement != null)
                {
                    brain.movement.walkSpeed = originalWalkSpeed;
                    brain.movement.runSpeed = originalRunSpeed;
                    brain.movement.jumpForce = originalJumpForce;
                }

                if (brain.sensor != null)
                {
                    brain.sensor.detectionDistance = originalDetectionDistance;
                }
            }
        }
    }
}
