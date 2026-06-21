using UnityEngine;

public class AnimalData : MonoBehaviour
{
    [Header("Animal Info")]
    public string animalType = "Monkey";

    [HideInInspector]
    public DiseaseData currentDisease;
}