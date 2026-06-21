using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalsSpawner : MonoBehaviour
{
    public GameObject monkeyPrefab;
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    [Tooltip("Intervalo base entre spawns. LevelDifficultyConfig puede sobrescribirlo.")]
    public float spawnInterval = 10f;
    [Tooltip("Máximo base de monos simultáneos. LevelDifficultyConfig puede sobrescribirlo.")]
    public int maxMonkeys = 5;

    private float actualSpawnInterval;
    private int actualMaxMonkeys;
    private float timer;
    private int currentMonkeys;

    [Header("Disease Settings")]
    public DiseaseData[] allDiseases;

    void Start()
    {
        if (LevelDifficultyConfig.Instance != null)
        {
            actualSpawnInterval = LevelDifficultyConfig.Instance.spawnInterval;
            actualMaxMonkeys = LevelDifficultyConfig.Instance.maxMonkeys;
        }
        else
        {
            actualSpawnInterval = spawnInterval;
            actualMaxMonkeys = maxMonkeys;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= actualSpawnInterval && currentMonkeys < actualMaxMonkeys)
        {
            SpawnMonkey();
            timer = 0f;
        }
    }

    void SpawnMonkey()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        GameObject monkey = Instantiate(
            monkeyPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        // Obtener script del animal
        AnimalData animal =
            monkey.GetComponent<AnimalData>();

        // Lista temporal de enfermedades v�lidas
        List<DiseaseData> validDiseases =
            new List<DiseaseData>();

        // Buscar enfermedades compatibles
        foreach (DiseaseData disease in allDiseases)
        {
            if (
                disease.animalType == animal.animalType ||
                disease.animalType == "All"
            )
            {
                validDiseases.Add(disease);
            }
        }

        // Asignar enfermedad random
        if (validDiseases.Count > 0)
        {
            animal.currentDisease =
                validDiseases[
                    Random.Range(0, validDiseases.Count)
                ];
        }

        currentMonkeys++;

        // cuando muere, restar contador
        monkey.GetComponent<HealthAnimals>().OnDeath += HandleMonkeyDeath;
    }

    void HandleMonkeyDeath()
    {
        currentMonkeys--;
    }

    public void RemoveMonkey()
    {
        currentMonkeys--;

        if (currentMonkeys < 0)
        {
            currentMonkeys = 0;
        }
    }
}
