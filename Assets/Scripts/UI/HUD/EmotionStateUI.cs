using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EmotionStateUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Imagen del canvas que muestra el estado emocional actual.")]
    [SerializeField] private Image emotionStateImage;

    [Header("Audio Analyzer")]
    [Tooltip("Analizador que publica el estado emocional del jugador.")]
    [SerializeField] private AudioAnalyzer audioAnalyzer;

    [Header("Emotion Sprites")]
    [SerializeField] private Sprite silenceSprite;
    [FormerlySerializedAs("calmSprite")]
    [SerializeField] private Sprite normalSprite;
    [FormerlySerializedAs("normalSprite")]
    [SerializeField] private Sprite nervousSprite;
    [SerializeField] private Sprite panicSprite;

    private void Awake()
    {
        if (emotionStateImage == null)
            emotionStateImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (audioAnalyzer == null)
            audioAnalyzer = FindObjectOfType<AudioAnalyzer>();

        if (audioAnalyzer != null)
        {
            audioAnalyzer.OnEmotionStateChanged += HandleEmotionStateChanged;
            UpdateEmotionSprite(audioAnalyzer.CurrentEmotionState);
        }
        else
        {
            Debug.LogWarning("[EmotionStateUI] No se encontro AudioAnalyzer en la escena.");
        }
    }

    private void OnDisable()
    {
        if (audioAnalyzer != null)
            audioAnalyzer.OnEmotionStateChanged -= HandleEmotionStateChanged;
    }

    private void HandleEmotionStateChanged(PlayerEmotionState emotionState, float rmsLevel)
    {
        UpdateEmotionSprite(emotionState);
    }

    private void UpdateEmotionSprite(PlayerEmotionState emotionState)
    {
        if (emotionStateImage == null)
        {
            Debug.LogError("[EmotionStateUI] Falta asignar la imagen EmotionState en " + name);
            return;
        }

        Sprite targetSprite = GetSpriteForEmotion(emotionState);

        if (targetSprite == null)
        {
            Debug.LogWarning("[EmotionStateUI] Falta asignar sprite para el estado: " + emotionState);
            return;
        }

        emotionStateImage.sprite = targetSprite;
    }

    private Sprite GetSpriteForEmotion(PlayerEmotionState emotionState)
    {
        switch (emotionState)
        {
            case PlayerEmotionState.SILENCE:
                return silenceSprite;
            case PlayerEmotionState.NORMAL:
                return normalSprite;
            case PlayerEmotionState.NERVOUS:
                return nervousSprite;
            case PlayerEmotionState.PANIC:
                return panicSprite;
            default:
                return silenceSprite;
        }
    }
}
