using UnityEngine;
using UnityEngine.UI;

public class MusicButtonUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("El componente Image del boton de musica.")]
    [SerializeField] private Image buttonImage;

    [Header("Sprites")]
    [Tooltip("Sprite que se muestra cuando la musica esta ACTIVADA.")]
    [SerializeField] private Sprite musicOnSprite;

    [Tooltip("Sprite que se muestra cuando la musica esta DESACTIVADA (mute).")]
    [SerializeField] private Sprite musicOffSprite;

    [Header("Estado Inicial")]
    [Tooltip("Si esta marcado, la musica inicia encendida.")]
    [SerializeField] private bool startMusicOn = true;

    private void Start()
    {
        // Auto-obtener Image si no fue asignado manualmente
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        // Sincronizar el sprite con el estado real del AudioManager si esta disponible
        if (AudioManager.Instance != null)
        {
            UpdateSprite(AudioManager.Instance.IsMusicOn());
        }
        else
        {
            // Fallback: usar el estado inicial configurado en el inspector
            UpdateSprite(startMusicOn);
        }
    }

    /// <summary>
    /// Alterna el estado mute de la musica.
    /// Asigna este metodo al evento OnClick del boton en Unity.
    /// </summary>
    public void ToggleMusic()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("[MusicButtonUI] AudioManager no encontrado en la escena.");
            return;
        }

        AudioManager.Instance.ToggleMusic();
        UpdateSprite(AudioManager.Instance.IsMusicOn());
    }

    /// <summary>
    /// Actualiza el sprite del boton segun si la musica esta encendida o apagada.
    /// </summary>
    private void UpdateSprite(bool isMusicOn)
    {
        if (buttonImage == null)
        {
            Debug.LogError("[MusicButtonUI] ButtonImage no asignado en: " + name);
            return;
        }

        buttonImage.sprite = isMusicOn ? musicOnSprite : musicOffSprite;

        if (isMusicOn && musicOnSprite == null)
            Debug.LogWarning("[MusicButtonUI] No se asigno el Sprite de Musica Encendida.");

        if (!isMusicOn && musicOffSprite == null)
            Debug.LogWarning("[MusicButtonUI] No se asigno el Sprite de Musica Apagada.");
    }
}
