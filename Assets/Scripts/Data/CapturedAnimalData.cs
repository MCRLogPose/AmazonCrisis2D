using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CapturedAnimalData
{
    public string animalName;

    public DiseaseData disease;

    public int remainingTreatments;

    public float currentTimer;

    public bool isRecovered;

    [Header("Recompensas Personalizadas")]
    public int helpPointsReward;
    public int trustPoints;
}