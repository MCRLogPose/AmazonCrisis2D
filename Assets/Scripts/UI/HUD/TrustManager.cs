using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrustManager : MonoBehaviour
{
    public static TrustManager Instance;

    [Header("Trust Settings")]
    public Slider trustSlider;

    public float maxTrust = 100f;
    public float currentTrust;

    [Header("Penalización")]
    [Tooltip("Pérdida base de confianza por mono muerto. LevelDifficultyConfig puede multiplicarlo.")]
    public float trustLossPerMonkey = 2f;

    private GameOverManager gameOverManager;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentTrust = maxTrust;

        trustSlider.maxValue = maxTrust;
        trustSlider.value = currentTrust;

        gameOverManager = FindObjectOfType<GameOverManager>();
    }

    // Llamar cuando se gana confianza
    public void AddTrust(float amount)
    {
        currentTrust += amount;
        currentTrust = Mathf.Clamp(currentTrust, 0, maxTrust);
        trustSlider.value = currentTrust;
    }

    // Llamar cuando muere un mono
    public void LoseTrust()
    {
        float multiplier = 1f;
        if (LevelDifficultyConfig.Instance != null)
            multiplier = LevelDifficultyConfig.Instance.trustLossMultiplier;

        float actualLoss = trustLossPerMonkey * multiplier;
        currentTrust -= actualLoss;

        currentTrust = Mathf.Clamp(currentTrust, 0, maxTrust);

        trustSlider.value = currentTrust;

        if (currentTrust <= 0)
        {
            LoseLife();
        }
    }

    void LoseLife()
    {
        // Restar vida
        HealthManager.health--;

        // Reiniciar confianza
        currentTrust = maxTrust;
        trustSlider.value = currentTrust;

        if (HealthManager.health <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER");
        gameOverManager.ShowGameOver();

        // Aqu� luego:
        // Time.timeScale = 0;
        // mostrar panel
    }
}
