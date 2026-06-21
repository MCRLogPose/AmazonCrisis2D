using UnityEngine;
using UnityEngine.UI;

public class MicrophoneButtonUI : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("El componente de Imagen del botón de micrófono.")]
    [SerializeField] private Image buttonImage;

    [Header("Sprites")]
    [Tooltip("Imagen que se muestra cuando el micrófono está encendido (activo).")]
    [SerializeField] private Sprite micOnSprite;

    [Tooltip("Imagen que se muestra cuando el micrófono está apagado (silenciado).")]
    [SerializeField] private Sprite micOffSprite;

    [Header("Initial State")]
    [SerializeField] private bool startActive = true;

    // Estado interno del micrófono
    private bool isMicOn;

    // Propiedad pública para leer el estado desde otros scripts
    public bool IsMicOn => isMicOn;

    private void Start()
    {
        // Si no se asignó la imagen manualmente, intenta obtenerla en el mismo GameObject
        if (buttonImage == null)
        {
            buttonImage = GetComponent<Image>();
        }

        // Establecer el estado inicial
        isMicOn = startActive;
        UpdateMicrophoneUI();
    }

    /// <summary>
    /// Alterna el estado del micrófono entre encendido y apagado.
    /// Asigna esta función al evento OnClick del botón en Unity.
    /// </summary>
    public void ToggleMicrophone()
    {
        isMicOn = !isMicOn;
        UpdateMicrophoneUI();

        // Aquí puedes conectar lógica adicional, por ejemplo:
        // - Silenciar o activar transmisiones de chat de voz (ej. Photon Voice, Vivox).
        // - Iniciar o detener la grabación/reconocimiento de voz de Unity.
        // - Comunicarse con el AudioManager del juego.
        
        Debug.Log("Micrófono " + (isMicOn ? "ENCENDIDO" : "APAGADO"));
    }

    /// <summary>
    /// Actualiza el Sprite de la imagen del botón según el estado.
    /// </summary>
    private void UpdateMicrophoneUI()
    {
        if (buttonImage == null)
        {
            Debug.LogError("Falta asignar la referencia de ButtonImage en " + name);
            return;
        }

        if (isMicOn)
        {
            if (micOnSprite != null)
            {
                buttonImage.sprite = micOnSprite;
            }
            else
            {
                Debug.LogWarning("No se ha asignado el Sprite de Micrófono Encendido.");
            }
        }
        else
        {
            if (micOffSprite != null)
            {
                buttonImage.sprite = micOffSprite;
            }
            else
            {
                Debug.LogWarning("No se ha asignado el Sprite de Micrófono Apagado.");
            }
        }
    }
}
