using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimalPanelUI : MonoBehaviour
{
    public TMP_Text diseaseNameTXT;
    public TMP_Text treatmentCountTXT;
    public TMP_Text timesMinuteTXT;
    public TMP_Text helpPointsTXT;
    public TMP_Text trustPointsTXT;
    public TMP_Text animalTypeTXT;

    [Tooltip("Texto que muestra la probabilidad de reduccion de tratamientos segun el nivel de estres.")]
    public TMP_Text probabilityTXT;

    /// <summary>
    /// Muestra los datos de enfermedad del animal sin informacion de estres.
    /// </summary>
    public void ShowAnimalData(DiseaseData disease)
    {
        ShowAnimalData(disease, -1f);
    }

    /// <summary>
    /// Muestra los datos de enfermedad del animal e indica la probabilidad de mejora segun su estres.
    /// </summary>
    public void ShowAnimalData(DiseaseData disease, float stressLevel)
    {
        if (disease == null) return;

        diseaseNameTXT.text = disease.diseaseName;
        treatmentCountTXT.text = disease.treatmentCount.ToString();
        timesMinuteTXT.text = disease.minutesBetweenTreatments + " min";
        helpPointsTXT.text = disease.helpPointsReward.ToString();

        if (trustPointsTXT != null)
            trustPointsTXT.text = disease.trustPoints.ToString();

        animalTypeTXT.text = disease.animalType;

        // Mostrar probabilidad de mejora si el campo existe
        if (probabilityTXT != null)
        {
            probabilityTXT.text = GetProbabilityText(stressLevel);
        }
    }

    /// <summary>
    /// Devuelve el texto descriptivo de la probabilidad de reduccion de tratamientos segun el estres.
    /// </summary>
    private string GetProbabilityText(float stressLevel)
    {
        if (stressLevel < 0f)
        {
            // No se pudo obtener el nivel de estres
            return "Probabilidad: --";
        }
        else if (stressLevel < 30f)
        {
            // Estres bajo: alta probabilidad de reduccion y bonus de puntos
            return "Probabilidad: 70% (1 tratamiento) +50% Help pts";
        }
        else if (stressLevel >= 70f)
        {
            // Estres alto: comportamiento agresivo, sin bonos
            return "Probabilidad: 0% (Muy estresado)";
        }
        else
        {
            // Rango medio: sin modificaciones
            return "Probabilidad: Normal";
        }
    }
}
