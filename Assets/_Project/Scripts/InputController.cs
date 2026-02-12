using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private BoardModel model;
    private BoardView view;
    private PowerupManager powerupManager;
    private ScoreUI scoreUI;
    private SceneController sceneController => SceneController.Instance;
    private GameInputs inputs;
    private bool canMove = true;

    void Awake()
    {
        inputs = new GameInputs();
    }

    void OnEnable()
    {
        inputs.Enable();
        inputs.Gameplay.Move.performed += OnMove;
        inputs.Gameplay.Reset.performed += OnReset;
    }

    void OnDisable()
    {
        inputs.Gameplay.Move.performed -= OnMove;
        inputs.Gameplay.Reset.performed -= OnReset;
        inputs.Disable();
    }

    void Start()
    {
        model = new BoardModel();
        view = GetComponent<BoardView>();
        powerupManager = GetComponent<PowerupManager>();
        scoreUI = GetComponent<ScoreUI>();

        if (view == null)
        {
            Debug.LogError("BoardView not found!");
            enabled = false;
            return;
        }
        // Inicializar el PowerupManager si existe
        if (powerupManager != null)
        {
            powerupManager.Initialize(model, view);
        }
        else
        {
            Debug.LogWarning("PowerupManager not found!");
        }

        // Inicializar el ScoreUI si existe
        if (scoreUI != null)
        {
            scoreUI.Initialize(model);
        }
        else
        {
            Debug.LogWarning("ScoreUI not found!");
        }

        view.Render(model);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        
        Vector2 direction = context.ReadValue<Vector2>();
        
        Direction dir;
        
        if (direction.y > 0.5f)
            dir = Direction.Up;
        else if (direction.y < -0.5f)
            dir = Direction.Down;
        else if (direction.x < -0.5f)
            dir = Direction.Left;
        else if (direction.x > 0.5f)
            dir = Direction.Right;
        else
            return;

        ProcessMove(dir);
    }

    private void OnReset(InputAction.CallbackContext context)
    {
        model.Reset();
        view.Render(model);
    }

    private void ProcessMove(Direction dir)
    {
        // Guardar el estado antes del movimiento para el undo
        if (powerupManager != null)
        {
            powerupManager.SaveState(model.GetGrid());
        }

        if (model.Move(dir))
        {
            canMove = false;
            view.Render(model);

            Invoke(nameof(EnableMove), view.animationDuration);

            // Verificar Game Over después de renderizar
            if (model.IsGameOver())
            {
                // Deshabilitar input permanentemente
                inputs.Disable();

                // Verificar que SceneController existe antes de cargar escena
                if (sceneController != null)
                {
                    // Esperar a que termine la animación antes de cambiar escena
                    StartCoroutine(LoadGameOverAfterDelay(view.animationDuration + 0.5f));
                }
                else
                {
                    Debug.LogError("SceneController no encontrado! No se puede cargar GameOver.");
                }
            }
        }
    }

    private System.Collections.IEnumerator LoadGameOverAfterDelay(float delay)
    {
        // Esperar el tiempo especificado
        yield return new WaitForSeconds(delay); 

        // Cargar la escena GameOver con la puntuación final
        sceneController.LoadGameOver(model.Score);
    }

    private void EnableMove()
    {
        canMove = true;
    }
}