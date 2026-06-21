using UnityEngine;

public class LevelDifficultyConfig : MonoBehaviour
{
    [Header("Decaimiento de Salud de Monos")]
    [Tooltip("Multiplicador de velocidad de pérdida de salud. 1.0 = normal (5 min en morir). 2.0 = 2x más rápido.")]
    public float healthDecayMultiplier = 1f;

    [Header("Spawn de Monos")]
    [Tooltip("Intervalo en segundos entre cada aparición de mono.")]
    public float spawnInterval = 10f;
    [Tooltip("Máximo de monos simultáneos en el nivel.")]
    public int maxMonkeys = 5;

    [Header("Confianza")]
    [Tooltip("Multiplicador de pérdida de confianza por mono muerto. 1.0 = 2 puntos por muerte.")]
    public float trustLossMultiplier = 1f;

    [Header("Estrés")]
    [Tooltip("Multiplicador de velocidad de incremento de estrés en monos. 1.0 = normal.")]
    public float stressMultiplier = 1f;

    [Header("Bonus por Estrés Bajo (Help Points y Confianza)")]
    [Tooltip("Umbral de estrés para considerar \"estrés bajo\" y aplicar bonificaciones.")]
    public float lowStressThreshold = 30f;
    [Tooltip("Multiplicador de Help Points cuando el animal tiene estrés bajo. 1.5 = 50%% más.")]
    public float lowStressHelpPointsMultiplier = 1.5f;
    [Tooltip("Multiplicador de Puntos de Confianza cuando el animal tiene estrés bajo. 1.5 = 50%% más.")]
    public float lowStressTrustPointsMultiplier = 1.5f;
    [Tooltip("Probabilidad (0-1) de reducir tratamientos a 1 cuando el estrés es bajo.")]
    [Range(0f, 1f)]
    public float lowStressTreatmentsChance = 0.7f;

    [Header("Estrés por Emoción del Jugador")]
    [Tooltip("Cambio de estrés por segundo cuando el jugador está en SILENCE. Positivo = aumenta el estrés.")]
    public float stressRateOnSilence = 3f;
    [Tooltip("Cambio de estrés por segundo cuando el jugador está en NORMAL. Valor negativo = reduce el estrés.")]
    public float stressRateOnNormal = -6f;
    [Tooltip("Cambio de estrés por segundo cuando el jugador está en NERVOUS.")]
    public float stressRateOnNervous = 8f;
    [Tooltip("Cambio de estrés por segundo cuando el jugador está en PANIC.")]
    public float stressRateOnPanic = 10f;

    public static LevelDifficultyConfig Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
