using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClinicPanelUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] public TMP_Text IDAnimalTXT;
    public TMP_Text diseaseNameTXT;
    public TMP_Text helpPointsTXT;
    public TMP_Text trustPointsTXT;
    public TMP_Text timesMinuteTXT;
    public TMP_Text treatmentCountTXT;
    public TMP_Text typeAnimalTXT;

    private int currentIndex = 0;

    private void Update()
    {
        ShowAnimal();
    }

    public void ShowAnimal()
    {
        if (ClinicManager.instance == null || 
            ClinicManager.instance.animalsInClinic == null || 
            ClinicManager.instance.animalsInClinic.Count == 0)
        {
            currentIndex = 0;
            ClearClinicPanel();
            return;
        }

        if (currentIndex < 0 || currentIndex >= ClinicManager.instance.animalsInClinic.Count)
        {
            currentIndex = 0;
        }

        CapturedAnimalData animal =
            ClinicManager.instance.animalsInClinic[currentIndex];
        // Aquí actualizamos los textos con la información del animal

        //colocar el ID o indice de la lista de animalws en la clínica
        IDAnimalTXT.text = $"ID: {currentIndex + 1}";

        diseaseNameTXT.text = animal.disease.diseaseName;

        helpPointsTXT.text = animal.helpPointsReward.ToString();

        if (trustPointsTXT != null)
        {
            trustPointsTXT.text = animal.trustPoints.ToString();
        }

        int minutes = Mathf.FloorToInt(animal.currentTimer / 60);
        int seconds = Mathf.FloorToInt(animal.currentTimer % 60);

        timesMinuteTXT.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        treatmentCountTXT.text = animal.remainingTreatments.ToString();

        typeAnimalTXT.text = animal.animalName;
    }

    public void NextAnimal()
    {
        if (ClinicManager.instance == null || 
            ClinicManager.instance.animalsInClinic == null || 
            ClinicManager.instance.animalsInClinic.Count == 0)
        {
            currentIndex = 0;
            ClearClinicPanel();
            return;
        }

        currentIndex++;

        if (currentIndex >= ClinicManager.instance.animalsInClinic.Count)
        {
            currentIndex = 0;
        }

        ShowAnimal();
    }

    public void PreviousAnimal()
    {
        if (ClinicManager.instance == null || 
            ClinicManager.instance.animalsInClinic == null || 
            ClinicManager.instance.animalsInClinic.Count == 0)
        {
            currentIndex = 0;
            ClearClinicPanel();
            return;
        }

        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex =
                ClinicManager.instance.animalsInClinic.Count - 1;
        }

        ShowAnimal();
    }

    public void HealCurrentAnimal()
    {
        if (ClinicManager.instance == null || 
            ClinicManager.instance.animalsInClinic == null || 
            ClinicManager.instance.animalsInClinic.Count == 0)
        {
            currentIndex = 0;
            ClearClinicPanel();
            return;
        }

        if (currentIndex < 0 || currentIndex >= ClinicManager.instance.animalsInClinic.Count)
        {
            currentIndex = 0;
        }

        CapturedAnimalData animal =
            ClinicManager.instance.animalsInClinic[currentIndex];

        if (animal.currentTimer > 0)
            return;

        animal.remainingTreatments--;

        if (animal.remainingTreatments <= 0)
        {
            animal.isRecovered = true;

            // aqu luego dars help points
            HelpPointsManager.instance.AddPoints(animal.helpPointsReward);

            // Sumar puntos de confianza
            if (TrustManager.Instance != null)
            {
                TrustManager.Instance.AddTrust(animal.trustPoints);
            }

            // Eliminar el animal de la clínica
            ClinicManager.instance.animalsInClinic.Remove(animal);

            ClearClinicPanel();
        }
        else
        {
            animal.currentTimer =
                animal.disease.minutesBetweenTreatments * 60f;
        }

        if (ClinicManager.instance.animalsInClinic.Count == 0)
        {
            currentIndex = 0;
            ClearClinicPanel();
        }
        else
        {
            if (currentIndex >= ClinicManager.instance.animalsInClinic.Count)
            {
                currentIndex = 0;
            }
            ShowAnimal();
        }
    }

    // fuction for clear the clinic panel when the player exit the clinic
    public void ClearClinicPanel()
    {
        IDAnimalTXT.text = "ID: -";
        diseaseNameTXT.text = "";
        helpPointsTXT.text = "";
        if (trustPointsTXT != null) trustPointsTXT.text = "";
        timesMinuteTXT.text = "";
        treatmentCountTXT.text = "";
        typeAnimalTXT.text = "";
    }
}
