using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public List<LevelData> levels =
        new List<LevelData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeLevels();
    }

    private void InitializeLevels()
    {
        if (levels.Count > 0)
            return;

        for (int i = 0; i < 5; i++)
        {
            LevelData level =
                new LevelData();

            level.levelNumber = i + 1;

            level.unlocked = true; // Desbloqueado por defecto para acceso directo temporal

            level.completed = false;

            levels.Add(level);
        }
    }

    public void CompleteLevel(int levelIndex)
    {
        if (levelIndex < 0 ||
            levelIndex >= levels.Count)
            return;

        levels[levelIndex].completed = true;

        if (levelIndex + 1 < levels.Count)
        {
            levels[levelIndex + 1].unlocked = true;
        }
    }
}