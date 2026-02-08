using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerupsUI : MonoBehaviour
{
    [Header("Referencias")]
    public PowerupManager powerupManager;
    
    [Header("Botones")]
    public Button undoButton;
    public Button hammerButton;
    public Button shuffleButton;
    public Button audioButton;
    
    [Header("Contadores de texto")]
    public TextMeshProUGUI undoCountText;
    public TextMeshProUGUI hammerCountText;
    public TextMeshProUGUI shuffleCountText;
    
    [Header("Icono de audio")]
    public Image audioIcon;
    public Sprite audioOnSprite;
    public Sprite audioOffSprite;
    
    private bool audioMuted = false;

    private void Start()
    {
        // Suscribirse al evento de cambio de powerups
        if (powerupManager != null)
        {
            powerupManager.OnPowerupCountChanged += UpdateCounters;
        }
        
        // Conectar los eventos de los botones manualmente por código
        if (undoButton != null)
        {
            undoButton.onClick.AddListener(UseUndo);
        }
        
        if (shuffleButton != null)
        {
            shuffleButton.onClick.AddListener(UseShuffle);
        }
        
        if (audioButton != null)
        {
            audioButton.onClick.AddListener(OnAudioClicked);
        }
        
        // Cargar el estado del audio desde PlayerPrefs
        audioMuted = PlayerPrefs.GetInt("AudioMuted", 0) == 1;
        UpdateAudioIcon();
        
        // Actualizar contadores iniciales
        UpdateCounters();
    }

    private void OnDestroy()
    {
        // Desuscribirse del evento al destruir el objeto
        if (powerupManager != null)
        {
            powerupManager.OnPowerupCountChanged -= UpdateCounters;
        }
        
        // Limpiar los listeners de los botones
        if (undoButton != null)
        {
            undoButton.onClick.RemoveListener(UseUndo);
        }
        
        if (shuffleButton != null)
        {
            shuffleButton.onClick.RemoveListener(UseShuffle);
        }
        
        if (audioButton != null)
        {
            audioButton.onClick.RemoveListener(OnAudioClicked);
        }
    }

    public void UseUndo()
    {
        if (powerupManager != null && powerupManager.CanUseUndo())
        {
            powerupManager.UseUndo();
            Debug.Log("Undo usado");
        }
        else
        {
            Debug.Log("No hay undos disponibles");
        }
    }

    public void UseShuffle()
    {
        if (powerupManager != null && powerupManager.CanUseShuffle())
        {
            powerupManager.UseShuffle();
            Debug.Log("Shuffle usado");
        }
        else
        {
            Debug.Log("No hay shuffles disponibles");
        }
    }

    public void OnAudioClicked()
    {
        audioMuted = !audioMuted;
        PlayerPrefs.SetInt("AudioMuted", audioMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        UpdateAudioIcon();
        
        // Notificar al AudioManager del cambio
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMuted(audioMuted);
        }
        
        Debug.Log("Audio " + (audioMuted ? "muteado" : "activado"));
    }

    private void UpdateCounters()
    {
        if (powerupManager == null) return;
        
        // Actualizar textos de contadores
        if (undoCountText != null)
        {
            undoCountText.text = powerupManager.GetUndosLeft().ToString();
        }
        
        if (hammerCountText != null)
        {
            hammerCountText.text = powerupManager.GetHammersLeft().ToString();
        }
        
        if (shuffleCountText != null)
        {
            shuffleCountText.text = powerupManager.GetShufflesLeft().ToString();
        }
        
        // Actualizar el estado interactable de los botones
        if (undoButton != null)
        {
            undoButton.interactable = powerupManager.CanUseUndo();
        }
        
        // El hammerButton no tiene componente Button, así que no lo tocamos aquí
        
        if (shuffleButton != null)
        {
            shuffleButton.interactable = powerupManager.CanUseShuffle();
        }
    }

    private void UpdateAudioIcon()
    {
        if (audioIcon == null) return;
        
        // Cambiar el sprite según el estado
        if (audioMuted && audioOffSprite != null)
        {
            audioIcon.sprite = audioOffSprite;
            // Opcional: cambiar también el color a rojo
            audioIcon.color = new Color(1f, 0.3f, 0.3f); // Rojo
        }
        else if (!audioMuted && audioOnSprite != null)
        {
            audioIcon.sprite = audioOnSprite;
            // Opcional: cambiar el color a verde
            audioIcon.color = new Color(0.3f, 1f, 0.3f); // Verde
        }
        else
        {
            // Si no tienes sprites diferentes, solo cambia el color
            if (audioMuted)
            {
                audioIcon.color = new Color(1f, 0.3f, 0.3f); // Rojo
            }
            else
            {
                audioIcon.color = new Color(0.3f, 1f, 0.3f); // Verde
            }
        }
    }
}