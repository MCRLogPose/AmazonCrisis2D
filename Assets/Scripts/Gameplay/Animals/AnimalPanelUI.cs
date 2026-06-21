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

    private string GetProbabilityText(float stressLevel)
    {
        if (stressLevel < 0f)
        {
            return "Probabilidad: --";
        }

        float lowThreshold = 30f;
        float treatmentsChance = 0.7f;
        float helpPointsMultiplier = 1.5f;
        float trustPointsMultiplier = 1.5f;

        if (LevelDifficultyConfig.Instance != null)
        {
            lowThreshold = LevelDifficultyConfig.Instance.lowStressThreshold;
            treatmentsChance = LevelDifficultyConfig.Instance.lowStressTreatmentsChance;
            helpPointsMultiplier = LevelDifficultyConfig.Instance.lowStressHelpPointsMultiplier;
            trustPointsMultiplier = LevelDifficultyConfig.Instance.lowStressTrustPointsMultiplier;
        }

        if (stressLevel < lowThreshold)
        {
            int hpBonusPercent = Mathf.RoundToInt((helpPointsMultiplier - 1f) * 100f);
            int tpBonusPercent = Mathf.RoundToInt((trustPointsMultiplier - 1f) * 100f);
            return $"Probabilidad: {treatmentsChance * 100f:F0}% (1 tratamiento) +{hpBonusPercent}% Help pts +{tpBonusPercent}% Confianza";
        }
        else if (stressLevel >= 70f)
        {
            return "Probabilidad: 0% (Muy estresado)";
        }
        else
        {
            return "Probabilidad: Normal";
        }
    }
}
