using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private BoardModel model;
    private BoardView view;
    private PowerupManager powerupManager;
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

            if (model.IsGameOver())
            {
                Debug.Log("GAME OVER! Score: " + model.Score);
            }
        }
    }

    private void EnableMove()
    {
        canMove = true;
    }
}