using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI scoreText;

    private BoardModel model;

    /// <summary>w
    /// Inicializar la UI del score con el modelo del juego
    /// </summary>
    public void Initialize(BoardModel boardModel)
    {
        if (boardModel == null)
        {
            Debug.LogError("BoardModel es null en ScoreUI.Initialize()");
            return;
        }

        model = boardModel;

        // Suscribirse al evento de cambio de score
        model.OnScoreChanged += UpdateScoreDisplay;

        // Actualizar el display inicial con el score actual
        UpdateScoreDisplay(model.Score);
    }

    private void OnDestroy()
    {
        // Desuscribirse del evento al destruir el objeto
        if (model != null)
        {
            model.OnScoreChanged -= UpdateScoreDisplay;
        }
    }

    /// <summary>
    /// Actualizar el texto del score en la UI
    /// </summary>
    private void UpdateScoreDisplay(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = newScore.ToString();
        }
        else
        {
            Debug.LogWarning("scoreText no está asignado en ScoreUI");
        }
    }
}
