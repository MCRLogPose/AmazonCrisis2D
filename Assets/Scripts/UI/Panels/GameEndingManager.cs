using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameEndingManager : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private GameObject gameHUD;
    [SerializeField] private GameObject mobileControlPanel;

    [Header("Texto del mensaje")]
    [TextArea(5, 20)]
    [SerializeField] private string endingMessage = "No pude con todo.\n\nAtender a cada animal requeria tiempo, recursos y atencion que una sola persona no puede sostener. La fauna silvestre no es una mascota, y su cuidado no es trabajo de uno solo.\n\nNecesitamos politicas claras, centros de rehabilitacion con presupuesto, y que el Estado asuma su responsabilidad.\n\nEsto no termina aqui. Esto empieza cuando decidimos mirar la realidad de frente.";

    [Header("Configuracion")]
    [Tooltip("Segundos antes de que se pueda volver al menu.")]
    [SerializeField] private float minDisplayTime = 5f;

    private bool canReturnToMenu;

    private void Start()
    {
        if (endingPanel != null)
            endingPanel.SetActive(false);
    }

    public void ShowEnding()
    {
        Time.timeScale = 0f;

        if (gameHUD != null)
            gameHUD.SetActive(false);
        if (mobileControlPanel != null)
            mobileControlPanel.SetActive(false);

        if (endingPanel != null)
        {
            endingPanel.SetActive(true);

            TMPro.TextMeshProUGUI textComponent = endingPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textComponent != null)
                textComponent.text = endingMessage;
        }

        StartCoroutine(EnableMenuReturn());
    }

    private IEnumerator EnableMenuReturn()
    {
        yield return new WaitForSecondsRealtime(minDisplayTime);
        canReturnToMenu = true;
    }

    private void Update()
    {
        if (canReturnToMenu && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            ReturnToMenu();
        }
    }

    private void ReturnToMenu()
    {
        Time.timeScale = 1f;
        HealthManager.health = 3;
        SceneManager.LoadScene("Menu");
    }
}
