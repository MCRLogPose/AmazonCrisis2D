using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialExitTrigger : MonoBehaviour
{
    [Header("Visual Settings")]
    [Tooltip("El renderer visual (SpriteRenderer, MeshRenderer, etc.) que se activará al llegar a 100 help points.")]
    [SerializeField] private Renderer visualRenderer;
    
    [Tooltip("El colisionador que se activará al llegar a 100 help points.")]
    [SerializeField] private Collider2D portalCollider;

    [Header("Interaction Settings")]
    [Tooltip("Nombre de la escena del siguiente nivel.")]
    [SerializeField] private string nextSceneName = "Level1";

    [Tooltip("Botón o prompt visual de interactuar en pantalla (opcional).")]
    [SerializeField] private GameObject interactPrompt;

    private bool isPlayerNearby = false;
    private bool isActivated = false;

    private void Start()
    {
        // Buscar componentes locales si no están asignados en el Inspector
        if (visualRenderer == null)
        {
            visualRenderer = GetComponent<Renderer>();
        }
        if (portalCollider == null)
        {
            portalCollider = GetComponent<Collider2D>();
        }

        // Asegurarse de que el portal esté oculto e inactivo al inicio
        SetPortalState(false);
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        // Comprobar si se ha alcanzado los 100 puntos en el HelpPointsManager
        if (!isActivated && HelpPointsManager.instance != null && HelpPointsManager.instance.currentPoints >= 100)
        {
            ActivatePortal();
        }

        // Si el portal está activo, el jugador está cerca y presiona "E", pasamos al siguiente nivel
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
        {
            visualRenderer.enabled = active;
        }
        if (portalCollider != null)
        {
            portalCollider.enabled = active;
        }
    }

    private void LoadNextLevel()
    {
        Debug.Log("[TutorialExit] Cargando el siguiente nivel: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivated && collision.CompareTag("Doctor_Mori"))
        {
            isPlayerNearby = true;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Doctor_Mori"))
        {
            isPlayerNearby = false;
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}
