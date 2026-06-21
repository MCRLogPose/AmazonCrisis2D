using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject mobileControlPanel;

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        mobileControlPanel.SetActive(false);

        // Pausar juego
        Time.timeScale = 0f;
    }

}
