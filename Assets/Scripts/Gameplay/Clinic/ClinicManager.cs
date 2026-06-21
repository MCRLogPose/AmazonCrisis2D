using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClinicManager : MonoBehaviour
{
    public static ClinicManager instance;

    public List<CapturedAnimalData> animalsInClinic = new List<CapturedAnimalData>();

    public int maxAnimals = 3;

    private void Awake()
    {
        instance = this;
    }

    public bool AddAnimal(AnimalData animal)
    {
        if (animalsInClinic.Count >= maxAnimals)
        {
            return false;
        }

        CapturedAnimalData data =
            new CapturedAnimalData();

        data.animalName = animal.animalType;

        data.disease = animal.currentDisease;

        data.remainingTreatments =
            animal.currentDisease.treatmentCount;

        data.currentTimer =
            animal.currentDisease.minutesBetweenTreatments * 60f;

        data.isRecovered = false;

        // Copiar las recompensas por defecto de la enfermedad
        data.helpPointsReward = animal.currentDisease.helpPointsReward;
        data.trustPoints = animal.currentDisease.trustPoints;

        // Modificar según estrés si está por debajo del umbral configurado
        AnimalStress stressComponent = animal.GetComponent<AnimalStress>();
        if (stressComponent != null)
        {
            float threshold = 30f;
            float treatmentsChance = 0.7f;
            float helpPointsMultiplier = 1.5f;
            float trustPointsMultiplier = 1.5f;

            if (LevelDifficultyConfig.Instance != null)
            {
                threshold = LevelDifficultyConfig.Instance.lowStressThreshold;
                treatmentsChance = LevelDifficultyConfig.Instance.lowStressTreatmentsChance;
                helpPointsMultiplier = LevelDifficultyConfig.Instance.lowStressHelpPointsMultiplier;
                trustPointsMultiplier = LevelDifficultyConfig.Instance.lowStressTrustPointsMultiplier;
            }

            if (stressComponent.currentStress < threshold)
            {
                // Probabilidad configurable de que requiera solo 1 tratamiento
                if (Random.value < treatmentsChance)
                {
                    data.remainingTreatments = 1;
                    Debug.Log("[ClinicManager] ¡Estrés bajo! Tratamientos reducidos de " + 
                              animal.currentDisease.treatmentCount + " a 1.");
                }

                // Multiplicador configurable de Help Points por ser dócil
                data.helpPointsReward = Mathf.RoundToInt(data.helpPointsReward * helpPointsMultiplier);
                Debug.Log("[ClinicManager] ¡Estrés bajo! Help Points incrementados a " + data.helpPointsReward);

                // Multiplicador configurable de Puntos de Confianza por ser dócil
                data.trustPoints = Mathf.RoundToInt(data.trustPoints * trustPointsMultiplier);
                Debug.Log("[ClinicManager] ¡Estrés bajo! Puntos de Confianza incrementados a " + data.trustPoints);
            }
        }

        animalsInClinic.Add(data);

        return true;
    }

    private void Update()
    {
        foreach (CapturedAnimalData animal in animalsInClinic)
        {
            if (animal.isRecovered)
                continue;

            if (animal.currentTimer > 0)
            {
                animal.currentTimer -= Time.deltaTime;
                if (animal.currentTimer < 0)
                {
                    animal.currentTimer = 0;
                }
            }
        }
    }
}
