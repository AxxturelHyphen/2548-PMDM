using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Botones")]
    [SerializeField] private Button botonJugar;
    [SerializeField] private Button botonSalir;

    private SceneController sceneController => SceneController.Instance;

    void Start()
    {
        // Verificar referencias
        if (botonJugar == null)
        {
            Debug.LogError("Botón Jugar no asignado en el Inspector!");
            return;
        }

        if (botonSalir == null)
        {
            Debug.LogError("Botón Salir no asignado en el Inspector!");
            return;
        }

        // Asignar eventos a los botones
        botonJugar.onClick.AddListener(IniciarJuego);
        botonSalir.onClick.AddListener(SalirDelJuego);
    }

    private void IniciarJuego()
    {
        if (sceneController != null)
        {
            sceneController.LoadGame();
        }
        else
        {
            Debug.LogError("SceneController no encontrado! Asegúrate de que existe en la escena.");
        }
    }

    private void SalirDelJuego()
    {
        if (sceneController != null)
        {
            sceneController.QuitGame();
        }
        else
        {
            Debug.LogError("SceneController no encontrado!");
        }
    }

    void OnDestroy()
    {
        // Limpiar eventos
        if (botonJugar != null)
        {
            botonJugar.onClick.RemoveListener(IniciarJuego);
        }

        if (botonSalir != null)
        {
            botonSalir.onClick.RemoveListener(SalirDelJuego);
        }
    }
}
