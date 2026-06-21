using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchPlayer : MonoBehaviour
{
    [Header("UI")]
    public GameObject catchButton;
    public GameObject monkeyPanel;
    public GameObject pauseMenuScreen;
    public GameObject gameOverPanel;
    public GameObject mobileControlPanel;
    public GameObject lifeContainer;

    private GameObject nearbyMonkey;
    private bool canCatch;
    public static GameObject capturedMonkey;
    public AnimalPanelUI panelUI;

    private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioClip catchSound;
    [SerializeField] private AudioClip catchFailSound;

    [Header("Capture Settings")]
    [Tooltip("Probabilidad de FALLO de captura con estres alto (70-100). 0.4 = 40%.")]
    [Range(0f, 1f)]
    [SerializeField] private float highStressFailChance = 0.4f;

    [Tooltip("Cuanto aumenta el estres del mono si la captura falla.")]
    [SerializeField] private float stressOnFailCapture = 15f;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (catchButton != null) catchButton.SetActive(false);
        if (monkeyPanel != null) monkeyPanel.SetActive(false);
    }

    void Update()
    {
        if (canCatch && Input.GetKeyDown(KeyCode.R))
        {
            CatchMonkey();
        }
    }

    public void CatchMonkey()
    {
        if (nearbyMonkey == null) return;

        AnimalStress stressComponent = nearbyMonkey.GetComponent<AnimalStress>();
        if (stressComponent != null && stressComponent.currentStress >= 70f)
        {
            if (Random.value < highStressFailChance)
            {
                FailCaptureHighStress(stressComponent);
                return;
            }
        }

        ExecuteSuccessfulCatch();
    }

    private void FailCaptureHighStress(AnimalStress stressComponent)
    {
        Debug.Log("[CatchPlayer] Captura fallida: el mono estaba muy estresado y escapo.");

        stressComponent.IncreaseStress(stressOnFailCapture);

        AnimalBrain brain = nearbyMonkey.GetComponent<AnimalBrain>();
        if (brain != null)
        {
            brain.movement.Jump();
            brain.ChangeState(AnimalStateType.RunAway);
        }

        if (catchFailSound != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(catchFailSound);

        if (animator != null)
            animator.SetBool("isCatching", false);
    }

    private void ExecuteSuccessfulCatch()
    {
        if (animator != null)
            animator.SetBool("isCatching", true);

        AnimalBrain brain = nearbyMonkey.GetComponent<AnimalBrain>();
        if (brain != null)
            brain.SetCaptured(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(catchSound);

        capturedMonkey = nearbyMonkey;

        AnimalData animalData = nearbyMonkey.GetComponent<AnimalData>();
        if (animalData != null)
            animalData.enabled = false;

        Time.timeScale = 0f;

        if (monkeyPanel != null)       monkeyPanel.SetActive(true);
        if (pauseMenuScreen != null)   pauseMenuScreen.SetActive(false);
        if (gameOverPanel != null)     gameOverPanel.SetActive(false);
        if (mobileControlPanel != null) mobileControlPanel.SetActive(false);
        if (lifeContainer != null)     lifeContainer.SetActive(false);

        if (animalData != null && panelUI != null)
        {
            float currentStress = -1f;
            AnimalStress stressComp = nearbyMonkey.GetComponent<AnimalStress>();
            if (stressComp != null)
                currentStress = stressComp.currentStress;

            panelUI.ShowAnimalData(animalData.currentDisease, currentStress);
        }
    }

    public void HealAnimal()
    {
        if (nearbyMonkey == null) return;

        AnimalData animal = nearbyMonkey.GetComponent<AnimalData>();
        if (animal == null) return;

        bool added = ClinicManager.instance.AddAnimal(animal);

        if (added)
        {
            AnimalsSpawner spawner = FindObjectOfType<AnimalsSpawner>();
            if (spawner != null) spawner.RemoveMonkey();

            PlayerManager playerManager = FindObjectOfType<PlayerManager>();
            if (playerManager != null) playerManager.ResumeGame();

            Time.timeScale = 1f;

            // Capturar referencia antes de limpiar nearbyMonkey
            GameObject toDestroy = nearbyMonkey;
            nearbyMonkey = null;
            capturedMonkey = null;

            // Desactivar primero para liberar referencias del editor (evita errores del inspector)
            // luego destruir en el siguiente frame
            StartCoroutine(SafeDestroyMonkey(toDestroy));
        }
    }

    private IEnumerator SafeDestroyMonkey(GameObject monkey)
    {
        if (monkey == null) yield break;
        monkey.SetActive(false);
        yield return null;
        Destroy(monkey);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monkey"))
        {
            nearbyMonkey = collision.gameObject;
            canCatch = true;
            if (catchButton != null) catchButton.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (animator != null)
            animator.SetBool("isCatching", false);

        if (collision.CompareTag("Monkey"))
        {
            nearbyMonkey = null;
            canCatch = false;
            if (catchButton != null) catchButton.SetActive(false);
        }
    }
}
