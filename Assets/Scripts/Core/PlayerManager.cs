using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public GameObject pauseMenuScreen;
    public GameObject gameOverPanel;
    public GameObject mobileControlPanel;
    public GameObject animalsPanel;
    public GameObject lifeContainer;
    public GameObject clinicPanel;

    [Header("Player Animator")]
    public Animator playerAnimator;

    public void PauseGame()
    {
        Time.timeScale = 0f;

        pauseMenuScreen.SetActive(true);
        mobileControlPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        animalsPanel.SetActive(false);
        lifeContainer.SetActive(false);
    }

    public void ResumeGame()
    {
        // quitar animación de captura
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("isCatching", false);
        }

        Time.timeScale = 1f;

        pauseMenuScreen.SetActive(false);
        mobileControlPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        animalsPanel.SetActive(false);
        lifeContainer.SetActive(true);
        clinicPanel.SetActive(false);

        // volver a activar movimiento del mono
        if (CatchPlayer.capturedMonkey != null)
        {
            AnimalBrain brain = CatchPlayer.capturedMonkey.GetComponent<AnimalBrain>();

            if (brain != null)
            {
                brain.enabled = true;
                brain.SetCaptured(false);
            }
        }
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
        HealthManager.health = 3;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );

        HealthManager.health = 3;
    }
}