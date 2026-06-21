using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthAnimals : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Action OnDeath;

    [Header("Desgaste de vida")]
    public float timeToDie = 300f; // segundos (300 = 5 minutos)

    [Header("Audio Settings")]
    [Tooltip("Clip de audio que se reproducira unicamente cuando el animal muera.")]
    [SerializeField] private AudioClip deathClip;

    private float healthDecayPerSecond;
    private bool isDead = false;

    public HealthBarUI healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        float difficultyMultiplier = 1f;
        if (LevelDifficultyConfig.Instance != null)
            difficultyMultiplier = LevelDifficultyConfig.Instance.healthDecayMultiplier;

        healthDecayPerSecond = (maxHealth / timeToDie) * difficultyMultiplier;

        if (healthBar != null)
            healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (!isDead)
            DecreaseHealthOverTime();
    }

    void DecreaseHealthOverTime()
    {
        if (currentHealth <= 0) return;

        float decayMultiplier = 1f;
        AnimalStress stressComponent = GetComponent<AnimalStress>();
        if (stressComponent != null)
        {
            if (stressComponent.currentStress >= 70f)
                decayMultiplier = 1.8f;
            else if (stressComponent.currentStress < 30f)
                decayMultiplier = 0.3f;
        }

        currentHealth -= healthDecayPerSecond * decayMultiplier * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("[HealthAnimals] El mono murio.");

        if (deathClip != null)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(deathClip);
            else
                AudioSource.PlayClipAtPoint(deathClip, transform.position);
        }

        TrustManager trustManager = FindObjectOfType<TrustManager>();
        if (trustManager != null)
            trustManager.LoseTrust();
        else
            Debug.LogWarning("[HealthAnimals] TrustManager no encontrado en la escena.");

        // Disparar evento ANTES de desactivar
        OnDeath?.Invoke();

        // Desactivar el GameObject primero para que el Editor libere sus referencias
        // antes de que Destroy lo elimine del motor (evita errores del Inspector)
        StartCoroutine(SafeDestroy());
    }

    private IEnumerator SafeDestroy()
    {
        // Desactivar inmediatamente oculta el objeto y libera referencias del Editor
        gameObject.SetActive(false);
        // Esperar un frame para que el Editor actualice su seleccion
        yield return null;
        Destroy(gameObject);
    }
}
