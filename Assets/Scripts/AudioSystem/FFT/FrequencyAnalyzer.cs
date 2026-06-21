using UnityEngine;

public struct FrequencyAnalysisData
{
    public float DominantFrequency;
    public float SpectralCentroid;
    public float LowEnergyRatio;
    public float MidEnergyRatio;
    public float HighEnergyRatio;
    public float VocalEnergyRatio;
    public float SpectralFlux;
    public float Consistency;
    public float Aggression;
}

public class FrequencyAnalyzer : MonoBehaviour
{
    [Header("Frequency Bands")]
    [SerializeField] private float lowBandMaxHz = 250f;
    [SerializeField] private float midBandMaxHz = 1000f;
    [SerializeField] private float vocalBandMinHz = 85f;
    [SerializeField] private float vocalBandMaxHz = 4000f;

    private float[] real;
    private float[] imaginary;
    private float[] magnitudes;
    private float[] previousMagnitudes;

    // realiza un analisis de frecuencia en los samples de audio y devuelve un objeto con los datos analizados
    public FrequencyAnalysisData Analyze(float[] samples, int sampleRate)
    {
        FrequencyAnalysisData data = new FrequencyAnalysisData();

        if (samples == null || samples.Length < 2 || sampleRate <= 0)
            return data;

        int fftSize = Mathf.ClosestPowerOfTwo(samples.Length);

        if (fftSize > samples.Length)
            fftSize /= 2;

        if (fftSize < 2)
            return data;

        EnsureBuffers(fftSize);
        CopySamplesWithHannWindow(samples, fftSize);
        RunFft(real, imaginary, fftSize);
        BuildMagnitudeSpectrum(fftSize);

        return CalculateFrequencyData(sampleRate, fftSize);
    }

    // Asegura que los buffers de FFT estén inicializados y tengan el tamaño correcto
    private void EnsureBuffers(int fftSize)
    {
        int spectrumSize = fftSize / 2;

        if (real != null && real.Length == fftSize)
            return;

        real = new float[fftSize];
        imaginary = new float[fftSize];
        magnitudes = new float[spectrumSize];
        previousMagnitudes = new float[spectrumSize];
    }

    // Copia los samples de audio al buffer de FFT aplicando una ventana de Hann para reducir el leakage
    private void CopySamplesWithHannWindow(float[] samples, int fftSize)
    {
        for (int i = 0; i < fftSize; i++)
        {
            float window = 0.5f * (1f - Mathf.Cos(2f * Mathf.PI * i / (fftSize - 1)));
            real[i] = samples[i] * window;
            imaginary[i] = 0f;
        }
    }

    // Implementación del algoritmo FFT de Cooley-Tukey para transformar los datos de tiempo a frecuencia
    private void RunFft(float[] realValues, float[] imaginaryValues, int fftSize)
    {
        int bitReversedIndex = 0;

        for (int i = 1; i < fftSize; i++)
        {
            int bit = fftSize >> 1;

            while ((bitReversedIndex & bit) != 0)
            {
                bitReversedIndex ^= bit;
                bit >>= 1;
            }

            bitReversedIndex ^= bit;

            if (i < bitReversedIndex)
            {
                Swap(realValues, i, bitReversedIndex);
                Swap(imaginaryValues, i, bitReversedIndex);
            }
        }

        for (int length = 2; length <= fftSize; length <<= 1)
        {
            float angle = -2f * Mathf.PI / length;
            float wLengthReal = Mathf.Cos(angle);
            float wLengthImaginary = Mathf.Sin(angle);

            for (int i = 0; i < fftSize; i += length)
            {
                float wReal = 1f;
                float wImaginary = 0f;
                int halfLength = length >> 1;

                for (int j = 0; j < halfLength; j++)
                {
                    int evenIndex = i + j;
                    int oddIndex = evenIndex + halfLength;

                    float oddReal = realValues[oddIndex] * wReal - imaginaryValues[oddIndex] * wImaginary;
                    float oddImaginary = realValues[oddIndex] * wImaginary + imaginaryValues[oddIndex] * wReal;

                    realValues[oddIndex] = realValues[evenIndex] - oddReal;
                    imaginaryValues[oddIndex] = imaginaryValues[evenIndex] - oddImaginary;
                    realValues[evenIndex] += oddReal;
                    imaginaryValues[evenIndex] += oddImaginary;

                    float nextWReal = wReal * wLengthReal - wImaginary * wLengthImaginary;
                    wImaginary = wReal * wLengthImaginary + wImaginary * wLengthReal;
                    wReal = nextWReal;
                }
            }
        }
    }

    // Construye el espectro de magnitudes a partir de los componentes reales e imaginarios del FFT
    private void BuildMagnitudeSpectrum(int fftSize)
    {
        for (int i = 0; i < magnitudes.Length; i++)
        {
            previousMagnitudes[i] = magnitudes[i];
            magnitudes[i] = Mathf.Sqrt(real[i] * real[i] + imaginary[i] * imaginary[i]);
        }
    }

    // Calcula los datos de análisis de frecuencia a partir del espectro de magnitudes, incluyendo energía en bandas, centroides y agresividad
    private FrequencyAnalysisData CalculateFrequencyData(int sampleRate, int fftSize)
    {
        FrequencyAnalysisData data = new FrequencyAnalysisData();

        float totalEnergy = 0f;
        float lowEnergy = 0f;
        float midEnergy = 0f;
        float highEnergy = 0f;
        float vocalEnergy = 0f;
        float weightedFrequencySum = 0f;
        float positiveFlux = 0f;
        float dominantMagnitude = 0f;

        for (int i = 1; i < magnitudes.Length; i++)
        {
            float frequency = i * sampleRate / (float)fftSize;
            float magnitude = magnitudes[i];
            float energy = magnitude * magnitude;

            totalEnergy += energy;
            weightedFrequencySum += frequency * magnitude;
            positiveFlux += Mathf.Max(0f, magnitude - previousMagnitudes[i]);

            if (frequency <= lowBandMaxHz)
                lowEnergy += energy;
            else if (frequency <= midBandMaxHz)
                midEnergy += energy;
            else
                highEnergy += energy;

            if (frequency >= vocalBandMinHz && frequency <= vocalBandMaxHz)
                vocalEnergy += energy;

            if (magnitude > dominantMagnitude)
            {
                dominantMagnitude = magnitude;
                data.DominantFrequency = frequency;
            }
        }

        if (totalEnergy <= 0f)
            return data;

        float magnitudeSum = 0f;

        for (int i = 1; i < magnitudes.Length; i++)
            magnitudeSum += magnitudes[i];

        data.SpectralCentroid = magnitudeSum > 0f ? weightedFrequencySum / magnitudeSum : 0f;
        data.LowEnergyRatio = lowEnergy / totalEnergy;
        data.MidEnergyRatio = midEnergy / totalEnergy;
        data.HighEnergyRatio = highEnergy / totalEnergy;
        data.VocalEnergyRatio = vocalEnergy / totalEnergy;
        data.SpectralFlux = magnitudeSum > 0f ? positiveFlux / magnitudeSum : 0f;
        data.Consistency = 1f - Mathf.Clamp01(data.SpectralFlux * 4f);
        data.Aggression = CalculateAggression(data);

        return data;
    }

    // Calcula un valor de agresividad basado en el centroid, el flujo espectral y la energía en altas frecuencias, con pesos personalizados para cada componente
    private float CalculateAggression(FrequencyAnalysisData data)
    {
        float centroidPressure = Mathf.InverseLerp(midBandMaxHz, vocalBandMaxHz, data.SpectralCentroid);
        float fluxPressure = Mathf.Clamp01(data.SpectralFlux * 4f);
        float highPressure = Mathf.Clamp01(data.HighEnergyRatio);

        return Mathf.Clamp01(highPressure * 0.45f + fluxPressure * 0.35f + centroidPressure * 0.2f);
    }
    
    // Intercambia dos elementos en un array de valores flotantes
    private void Swap(float[] values, int a, int b)
    {
        float temp = values[a];
        values[a] = values[b];
        values[b] = temp;
    }
}
