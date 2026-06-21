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

        // Modificar según estrés si es menor a 30
        AnimalStress stressComponent = animal.GetComponent<AnimalStress>();
        if (stressComponent != null)
        {
            if (stressComponent.currentStress < 30f)
            {
                // 70% de probabilidad de que requiera solo 1 tratamiento
                if (Random.value < 0.7f)
                {
                    data.remainingTreatments = 1;
                    Debug.Log("[ClinicManager] ¡Estrés bajo! Tratamientos reducidos de " + 
                              animal.currentDisease.treatmentCount + " a 1.");
                }

                // Incrementar un 50% los Help Points por ser dócil
                data.helpPointsReward = Mathf.RoundToInt(data.helpPointsReward * 1.5f);
                Debug.Log("[ClinicManager] ¡Estrés bajo! Help Points incrementados a " + data.helpPointsReward);
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
