using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClinicInteraction : MonoBehaviour
{
    public GameObject clinicButton;
    public GameObject clinicPanel;
    public GameObject monkeyPanel;
    public GameObject pauseMenuScreen;
    public GameObject gameOverPanel;
    public GameObject mobileControlPanel;
    public GameObject lifeContainer;

    private bool playerNearby;

    void Start()
    {
        clinicButton.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            OpenClinicPanel();
        }
    }

    public void OpenClinicPanel()
    {
        clinicPanel.SetActive(true);
        monkeyPanel.SetActive(false);
        pauseMenuScreen.SetActive(false);
        gameOverPanel.SetActive(false);
        mobileControlPanel.SetActive(false);
        lifeContainer.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Doctor_Mori"))
        {
            playerNearby = true;

            clinicButton.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Doctor_Mori"))
        {
            playerNearby = false;

            clinicButton.SetActive(false);
        }
    }
}
