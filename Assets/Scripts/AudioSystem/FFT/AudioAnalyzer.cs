using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FrequencyAnalyzer))]
public class AudioAnalyzer : MonoBehaviour
{
    [Header("Microphone Settings")]
    [SerializeField] private string microphoneDevice;
    [SerializeField] private int sampleWindow = 512;

    [Header("Detection Thresholds")]
    [SerializeField] private float silenceThreshold = 0.003f;
    [SerializeField] private float normalThreshold = 0.012f;
    [SerializeField] private float nervousThreshold = 0.006f;
    [SerializeField] private float panicThreshold = 0.018f;

    [Header("FFT Emotion Analysis")]
    [SerializeField] private float analysisInterval = 0.1f;
    [SerializeField] private float historyDuration = 2f;
    [Range(0f, 1f)]
    [SerializeField] private float minimumDominance = 0.25f;
    [Range(0f, 1f)]
    [SerializeField] private float nervousAggressionThreshold = 0.06f;
    [Range(0f, 1f)]
    [SerializeField] private float panicAggressionThreshold = 0.08f;
    [Range(0f, 1f)]
    [SerializeField] private float normalAggressionMax = 0.7f;
    [Range(0f, 1f)]
    [SerializeField] private float nervousHighEnergyThreshold = 0.1f;
    [Range(0f, 1f)]
    [SerializeField] private float panicHighEnergyThreshold = 0.12f;
    [SerializeField] private float nervousCentroidThreshold = 700f;
    [SerializeField] private float panicCentroidThreshold = 900f;
    [SerializeField] private float panicRequiredDuration = 0.3f;
    [SerializeField] private float nervousRequiredDuration = 0.2f;
    [SerializeField] private float normalRequiredDuration = 0.3f;
    [SerializeField] private float silenceRequiredDuration = 0.2f;

    [Header("Emotion Hold")]
    [SerializeField] private float silenceHoldDuration = 0.5f;
    [SerializeField] private float normalHoldDuration = 1.2f;
    [SerializeField] private float nervousHoldDuration = 2.5f;
    [SerializeField] private float panicHoldDuration = 3.5f;

    [Header("References")]
    [SerializeField] private MicrophoneButtonUI microphoneButton;
    [SerializeField] private FrequencyAnalyzer frequencyAnalyzer;

    public event Action<PlayerEmotionState, float> OnEmotionStateChanged;

    private AudioClip microphoneClip;
    private float[] samples;

    private bool isRecording;
    private PlayerEmotionState currentEmotionState = PlayerEmotionState.SILENCE;
    private float currentRmsLevel;
    private float nextAnalysisTime;
    private float emotionHoldUntil;
    private FrequencyAnalysisData currentFrequencyData;
    private readonly List<EmotionFrame> emotionHistory = new List<EmotionFrame>();

    public PlayerEmotionState CurrentEmotionState => currentEmotionState;
    public float CurrentRmsLevel => currentRmsLevel;
    public FrequencyAnalysisData CurrentFrequencyData => currentFrequencyData;
    public bool IsRecording => isRecording;

    private void Reset()
    {
        ApplyUltraSensitiveEmotionPreset();
    }

    [ContextMenu("Apply Ultra Sensitive Emotion Preset")]
    private void ApplyUltraSensitiveEmotionPreset()
    {
        sampleWindow = 512;
        silenceThreshold = 0.003f;
        nervousThreshold = 0.006f;
        normalThreshold = 0.012f;
        panicThreshold = 0.018f;
        analysisInterval = 0.1f;
        historyDuration = 2f;
        minimumDominance = 0.25f;
        nervousAggressionThreshold = 0.06f;
        panicAggressionThreshold = 0.08f;
        normalAggressionMax = 0.7f;
        nervousHighEnergyThreshold = 0.1f;
        panicHighEnergyThreshold = 0.12f;
        nervousCentroidThreshold = 700f;
        panicCentroidThreshold = 900f;
        panicRequiredDuration = 0.3f;
        nervousRequiredDuration = 0.2f;
        normalRequiredDuration = 0.3f;
        silenceRequiredDuration = 0.2f;
        silenceHoldDuration = 0.5f;
        normalHoldDuration = 1.2f;
        nervousHoldDuration = 2.5f;
        panicHoldDuration = 3.5f;
    }

    private void Start()
    {
        sampleWindow = Mathf.Max(1, sampleWindow);
        samples = new float[sampleWindow];

        if (frequencyAnalyzer == null)
            frequencyAnalyzer = GetComponent<FrequencyAnalyzer>();

        if (frequencyAnalyzer == null)
            frequencyAnalyzer = gameObject.AddComponent<FrequencyAnalyzer>();

        if (Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("No se detecto ningun microfono.");
        }
    }

    private void Update()
    {
        if (microphoneButton == null)
            return;

        if (microphoneButton.IsMicOn && !isRecording)
        {
            StartMicrophone();
        }

        if (!microphoneButton.IsMicOn && isRecording)
        {
            StopMicrophone();
        }

        if (isRecording)
        {
            AnalyzeAudio();
        }
    }

    private void StartMicrophone()
    {
        if (string.IsNullOrEmpty(microphoneDevice))
        {
            Debug.LogWarning("[AudioAnalyzer] No hay microfono disponible para analizar.");
            SetEmotionState(PlayerEmotionState.SILENCE, 0f);
            return;
        }

        microphoneClip = Microphone.Start(
            microphoneDevice,
            true,
            10,
            AudioSettings.outputSampleRate
        );

        isRecording = true;

        Debug.Log("Microfono ACTIVADO");
    }

    private void StopMicrophone()
    {
        if (string.IsNullOrEmpty(microphoneDevice))
            return;

        Microphone.End(microphoneDevice);

        isRecording = false;
        emotionHistory.Clear();
        SetEmotionState(PlayerEmotionState.SILENCE, 0f);

        Debug.Log("Microfono DESACTIVADO");
    }

    // Analiza el audio del micrófono para detectar el nivel de RMS y las características de frecuencia, y actualiza el estado emocional del jugador en consecuencia.
    private void AnalyzeAudio()
    {
        int micPosition = Microphone.GetPosition(microphoneDevice) - sampleWindow;

        if (micPosition < 0)
            return;

        microphoneClip.GetData(samples, micPosition);

        float level = GetRMSLevel(samples);
        currentRmsLevel = level;

        if (Time.time < emotionHoldUntil)
            return;

        if (Time.time < nextAnalysisTime)
            return;

        nextAnalysisTime = Time.time + analysisInterval;

        if (frequencyAnalyzer != null)
            currentFrequencyData = frequencyAnalyzer.Analyze(samples, AudioSettings.outputSampleRate);

        PlayerEmotionState instantState = DetectInstantEmotion(level, currentFrequencyData);
        AddEmotionFrame(instantState, level, currentFrequencyData);
        SetEmotionState(GetStableEmotionState(), level);
    }

    // Calcula el nivel de RMS (Root Mean Square) del audio a partir de los datos de muestra, lo que proporciona una medida de la energía del sonido.
    private float GetRMSLevel(float[] samples)
    {
        float sum = 0;

        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }

        return Mathf.Sqrt(sum / samples.Length);
    }

    // Detecta el estado emocional instantáneo basado en el nivel de RMS y los datos de análisis de frecuencia, utilizando umbrales específicos para cada estado.
    private PlayerEmotionState DetectInstantEmotion(float level, FrequencyAnalysisData frequencyData)
    {
        if (level < silenceThreshold)
        {
            return PlayerEmotionState.SILENCE;
        }

        bool hasAggressiveSpectrum = frequencyData.Aggression >= panicAggressionThreshold
            || frequencyData.HighEnergyRatio >= panicHighEnergyThreshold
            || frequencyData.SpectralCentroid >= panicCentroidThreshold;

        if (level >= panicThreshold && hasAggressiveSpectrum)
        {
            return PlayerEmotionState.PANIC;
        }

        bool hasStableVoicePattern = frequencyData.VocalEnergyRatio >= 0.2f
            && frequencyData.Consistency >= 0.45f;

        if (level < nervousThreshold)
        {
            return PlayerEmotionState.NORMAL;
        }

        bool hasNervousSpectrum = frequencyData.Aggression >= nervousAggressionThreshold
            || frequencyData.HighEnergyRatio >= nervousHighEnergyThreshold
            || frequencyData.SpectralCentroid >= nervousCentroidThreshold;

        if (level >= nervousThreshold && hasNervousSpectrum)
        {
            return PlayerEmotionState.NERVOUS;
        }

        if (level < normalThreshold && frequencyData.Aggression <= normalAggressionMax && hasStableVoicePattern)
        {
            return PlayerEmotionState.NORMAL;
        }

        return PlayerEmotionState.NERVOUS;
    }

    // Agrega un nuevo marco de emoción al historial, incluyendo el estado emocional detectado, el nivel de RMS y los datos de agresión del análisis de frecuencia, y luego poda el historial para mantener solo los marcos dentro de la duración especificada.
    private void AddEmotionFrame(PlayerEmotionState emotionState, float rmsLevel, FrequencyAnalysisData frequencyData)
    {
        emotionHistory.Add(new EmotionFrame(Time.time, emotionState, rmsLevel, frequencyData.Aggression));
        PruneEmotionHistory();
    }

    // Elimina los marcos de emoción del historial que son más antiguos que la duración especificada, asegurando que solo se mantengan los marcos relevantes para el análisis actual.
    private void PruneEmotionHistory()
    {
        float oldestAllowedTime = Time.time - historyDuration;

        for (int i = emotionHistory.Count - 1; i >= 0; i--)
        {
            if (emotionHistory[i].Time >= oldestAllowedTime)
                continue;

            emotionHistory.RemoveAt(i);
        }
    }

    // Determina el estado emocional estable actual basado en el historial de emociones, considerando la dominancia de los estados, la duración sostenida de cada estado y los umbrales de agresión para evitar cambios bruscos.
    private PlayerEmotionState GetStableEmotionState()
    {
        if (emotionHistory.Count == 0)
            return PlayerEmotionState.SILENCE;

        PlayerEmotionState dominantState = GetDominantHistoryState(out float dominance);

        if (dominantState == currentEmotionState)
            return currentEmotionState;

        if (dominance < minimumDominance)
            return currentEmotionState;

        float requiredDuration = GetRequiredDuration(dominantState);

        if (dominantState == PlayerEmotionState.PANIC && GetSustainedDuration(dominantState) < requiredDuration)
            return currentEmotionState;

        if (dominantState != PlayerEmotionState.PANIC && GetHistoryCoverage() < requiredDuration)
            return currentEmotionState;

        return dominantState;
    }

    // Analiza el historial de emociones para determinar cuál es el estado emocional dominante, calculando la proporción de cada estado en el historial y devolviendo el estado con la mayor dominancia junto con su proporción.
    private PlayerEmotionState GetDominantHistoryState(out float dominance)
    {
        int silenceCount = 0;
        int normalCount = 0;
        int nervousCount = 0;
        int panicCount = 0;
        
        // Cuenta la cantidad de ocurrencias de cada estado emocional en el historial.
        for (int i = 0; i < emotionHistory.Count; i++)
        {
            switch (emotionHistory[i].State)
            {
                case PlayerEmotionState.SILENCE:
                    silenceCount++;
                    break;
                case PlayerEmotionState.NORMAL:
                    normalCount++;
                    break;
                case PlayerEmotionState.NERVOUS:
                    nervousCount++;
                    break;
                case PlayerEmotionState.PANIC:
                    panicCount++;
                    break;
            }
        }

        PlayerEmotionState dominantState = PlayerEmotionState.SILENCE;
        int dominantCount = silenceCount;

        if (normalCount > dominantCount)
        {
            dominantState = PlayerEmotionState.NORMAL;
            dominantCount = normalCount;
        }

        if (nervousCount > dominantCount)
        {
            dominantState = PlayerEmotionState.NERVOUS;
            dominantCount = nervousCount;
        }

        if (panicCount > dominantCount)
        {
            dominantState = PlayerEmotionState.PANIC;
            dominantCount = panicCount;
        }

        dominance = dominantCount / (float)emotionHistory.Count;
        return dominantState;
    }

    // Calcula la duración sostenida del estado emocional dominante en el historial, determinando cuánto tiempo ha estado presente ese estado de manera continua desde su última aparición.
    private float GetSustainedDuration(PlayerEmotionState emotionState)
    {
        if (emotionHistory.Count == 0)
            return 0f;

        float newestTime = emotionHistory[emotionHistory.Count - 1].Time;
        float oldestSustainedTime = newestTime;

        for (int i = emotionHistory.Count - 1; i >= 0; i--)
        {
            if (emotionHistory[i].State != emotionState)
                break;

            oldestSustainedTime = emotionHistory[i].Time;
        }

        return newestTime - oldestSustainedTime + analysisInterval;
    }

    // Calcula la cobertura total del historial de emociones, es decir, el tiempo transcurrido entre el primer y el último marco de emoción en el historial, lo que ayuda a determinar si se ha acumulado suficiente información para cambiar el estado emocional.
    private float GetHistoryCoverage()
    {
        if (emotionHistory.Count == 0)
            return 0f;

        return emotionHistory[emotionHistory.Count - 1].Time - emotionHistory[0].Time + analysisInterval;
    }

    // Obtiene la duración requerida para que un estado emocional sea considerado estable, basada en el estado emocional específico.
    private float GetRequiredDuration(PlayerEmotionState emotionState)
    {
        switch (emotionState)
        {
            case PlayerEmotionState.SILENCE:
                return silenceRequiredDuration;
            case PlayerEmotionState.NORMAL:
                return normalRequiredDuration;
            case PlayerEmotionState.NERVOUS:
                return nervousRequiredDuration;
            case PlayerEmotionState.PANIC:
                return panicRequiredDuration;
            default:
                return normalRequiredDuration;
        }
    }

    // Actualiza el estado emocional actual del jugador, estableciendo el nuevo estado y el nivel de RMS, y emitiendo un evento si el estado ha cambiado, además de iniciar un período de retención para evitar cambios bruscos.
    private void SetEmotionState(PlayerEmotionState newState, float rmsLevel)
    {
        currentRmsLevel = rmsLevel;

        if (currentEmotionState == newState)
            return;

        currentEmotionState = newState;
        emotionHoldUntil = Time.time + GetHoldDuration(currentEmotionState);
        emotionHistory.Clear();
        OnEmotionStateChanged?.Invoke(currentEmotionState, currentRmsLevel);

        Debug.Log("Estado emocional: " + currentEmotionState + " | RMS: " + currentRmsLevel.ToString("F4"));
    }

    // Obtiene la duración de retención para el estado emocional actual, lo que determina cuánto tiempo se mantendrá ese estado antes de permitir un cambio a otro estado, basado en el estado emocional específico.
    private float GetHoldDuration(PlayerEmotionState emotionState)
    {
        switch (emotionState)
        {
            case PlayerEmotionState.SILENCE:
                return silenceHoldDuration;
            case PlayerEmotionState.NORMAL:
                return normalHoldDuration;
            case PlayerEmotionState.NERVOUS:
                return nervousHoldDuration;
            case PlayerEmotionState.PANIC:
                return panicHoldDuration;
            default:
                return normalHoldDuration;
        }
    }

    // Estructura para almacenar un marco de emoción en el historial, que incluye el tiempo del marco, el estado emocional detectado, el nivel de RMS y el nivel de agresión del análisis de frecuencia.
    private struct EmotionFrame
    {
        public readonly float Time;
        public readonly PlayerEmotionState State;
        public readonly float RmsLevel;
        public readonly float Aggression;

        public EmotionFrame(float time, PlayerEmotionState state, float rmsLevel, float aggression)
        {
            Time = time;
            State = state;
            RmsLevel = rmsLevel;
            Aggression = aggression;
        }
    }
}
