using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelExitTrigger : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Renderer visualRenderer;
    [SerializeField] private Collider2D portalCollider;

    [Header("Level Progression")]
    [Tooltip("Nombre de la escena del siguiente nivel. Vacío si es el nivel final.")]
    [SerializeField] private string nextSceneName;
    [Tooltip("Índice del nivel actual (0=Tutorial, 1=Level1, 2=Level2, 3=Level3, 4=Final). Se usa para marcar como completado.")]
    [SerializeField] private int levelIndex;
    [Tooltip("Si es el nivel final, muestra el panel de cierre en vez de cargar otra escena.")]
    [SerializeField] private bool isFinalLevel;

    [Header("Help Points Requeridos")]
    [Tooltip("Puntos de Help Point necesarios para que el portal sea visible. 0 = visible desde el inicio (útil para testing).")]
    [SerializeField] private int helpPointsRequired = 100;

    [Header("Interaction")]
    [Tooltip("Botón/prompt visual que aparece al colisionar con el portal. Se recomienda un botón UI para soporte táctil.")]
    [SerializeField] private GameObject interactPrompt;

    private bool isPlayerNearby;
    private bool isActivated;

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
            btn.onClick.AddListener(ExitLevel);
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
            ExitLevel();
    }

    private void ActivatePortal()
    {
        isActivated = true;
        SetPortalState(true);
    }

    private void SetPortalState(bool active)
    {
        if (visualRenderer != null)
            visualRenderer.enabled = active;
        if (portalCollider != null)
            portalCollider.enabled = active;
    }

    public void ExitLevel()
    {
        if (isFinalLevel)
        {
            GameEndingManager ending = FindObjectOfType<GameEndingManager>();
            if (ending != null)
            {
                LevelManager.Instance?.CompleteLevel(levelIndex);
                ending.ShowEnding();
            }
        }
        else
        {
            LevelManager.Instance?.CompleteLevel(levelIndex);
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActivated && collision.CompareTag("Doctor_Mori"))
        {
            isPlayerNearby = true;
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Doctor_Mori"))
        {
            isPlayerNearby = false;
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }
}
