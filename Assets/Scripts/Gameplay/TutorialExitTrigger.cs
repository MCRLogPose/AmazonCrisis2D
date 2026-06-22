using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialExitTrigger : MonoBehaviour
{
    [Header("Visual Settings")]
    [Tooltip("El renderer visual (SpriteRenderer, MeshRenderer, etc.) que se activará al alcanzar los help points requeridos.")]
    [SerializeField] private Renderer visualRenderer;

    [Tooltip("El colisionador que se activará al alcanzar los help points requeridos.")]
    [SerializeField] private Collider2D portalCollider;

    [Header("Help Points Requeridos")]
    [Tooltip("Puntos de Help Point necesarios para que el portal sea visible. 0 = visible desde el inicio (útil para testing).")]
    [SerializeField] private int helpPointsRequired = 100;

    [Header("Interaction Settings")]
    [Tooltip("Nombre de la escena del siguiente nivel.")]
    [SerializeField] private string nextSceneName = "Level1";

    [Tooltip("Botón o prompt visual de interactuar en pantalla (opcional). Se recomienda un botón UI para soporte táctil.")]
    [SerializeField] private GameObject interactPrompt;

    private bool isPlayerNearby = false;
    private bool isActivated = false;

    private void Start()
    {
        if (visualRenderer == null)
            visualRenderer = GetComponent<Renderer>();
        if (portalCollider == null)
            portalCollider = GetComponent<Collider2D>();

        SetupInteractButton();

        interactPrompt?.SetActive(false);

        if (helpPointsRequired <= 0)
        {
            ActivatePortal();
        }
        else
        {
            SetPortalState(false);
        }
    }

    private void SetupInteractButton()
    {
        if (interactPrompt == null)
            return;

        Button btn = interactPrompt.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(LoadNextLevel);
        }
    }

    private void Update()
    {
        if (!isActivated && HelpPointsManager.instance != null &&
            HelpPointsManager.instance.currentPoints >= helpPointsRequired)
        {
            ActivatePortal();
        }

        if (isActivated && isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            LoadNextLevel();
        }
    }

    private void ActivatePortal()
    {
        isActivated = true;
        SetPortalState(true);
        Debug.Log("[TutorialExit] ¡Portal activado! El jugador puede avanzar de nivel interactuando con él.");
    }

    private void SetPortalState(bool active)
    {
        if (visualRenderer != null)
            visualRenderer.enabled = active;
        if (portalCollider != null)
            portalCollider.enabled = active;
    }

    public void LoadNextLevel()
    {
        Debug.Log("[TutorialExit] Cargando el siguiente nivel: " + nextSceneName);
        HealthManager.health = 3;
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivated && collision.CompareTag("Doctor_Mori"))
        {
            isPlayerNearby = true;
            interactPrompt?.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Doctor_Mori"))
        {
            isPlayerNearby = false;
            interactPrompt?.SetActive(false);
        }
    }
}
