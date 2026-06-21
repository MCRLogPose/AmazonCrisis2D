using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDisease", menuName = "Diseases/Disease")]
public class DiseaseData : ScriptableObject
{
    public string diseaseName;

    public string animalType;

    public int treatmentCount;

    public float minutesBetweenTreatments;

    public int helpPointsReward;

    public int trustPoints;
}