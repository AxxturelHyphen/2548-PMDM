using UnityEngine;
using UnityEngine.InputSystem;
using PMDM.Core;

namespace PMDM.Input
{
    /// <summary>
    /// Maneja la entrada del jugador (teclado WASD/flechas y swipe táctil).
    /// Traduce la entrada a direcciones de movimiento para el GameManager.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("Swipe Settings")]
        [SerializeField] private float _swipeThreshold = 50f;
        [SerializeField] private float _swipeCooldown = 0.15f;

        private Vector2 _touchStartPos;
        private bool _isSwiping;
        private float _lastMoveTime;

        private void Update()
        {
            if (GameManager.Instance == null) return;
            if (GameManager.Instance.CurrentState != GameState.Playing) return;

            HandleKeyboardInput();
            HandleTouchInput();
        }

        private void HandleKeyboardInput()
        {
            if (Time.time - _lastMoveTime < _swipeCooldown) return;

            MoveDirection? direction = null;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.wasPressedThisFrame ||
                    Keyboard.current.upArrowKey.wasPressedThisFrame)
                    direction = MoveDirection.Up;
                else if (Keyboard.current.sKey.wasPressedThisFrame ||
                         Keyboard.current.downArrowKey.wasPressedThisFrame)
                    direction = MoveDirection.Down;
                else if (Keyboard.current.aKey.wasPressedThisFrame ||
                         Keyboard.current.leftArrowKey.wasPressedThisFrame)
                    direction = MoveDirection.Left;
                else if (Keyboard.current.dKey.wasPressedThisFrame ||
                         Keyboard.current.rightArrowKey.wasPressedThisFrame)
                    direction = MoveDirection.Right;

                // Undo con Z
                if (Keyboard.current.zKey.wasPressedThisFrame)
                    GameManager.Instance.TryUndo();

                // Restart con R
                if (Keyboard.current.rKey.wasPressedThisFrame)
                    GameManager.Instance.StartNewGame();
            }

            if (direction.HasValue)
            {
                GameManager.Instance.TryMove(direction.Value);
                _lastMoveTime = Time.time;
            }
        }

        private void HandleTouchInput()
        {
            if (Touchscreen.current == null) return;

            var primaryTouch = Touchscreen.current.primaryTouch;

            if (primaryTouch.press.wasPressedThisFrame)
            {
                _touchStartPos = primaryTouch.position.ReadValue();
                _isSwiping = true;
            }

            if (primaryTouch.press.wasReleasedThisFrame && _isSwiping)
            {
                _isSwiping = false;
                Vector2 endPos = primaryTouch.position.ReadValue();
                ProcessSwipe(_touchStartPos, endPos);
            }
        }

        private void ProcessSwipe(Vector2 start, Vector2 end)
        {
            if (Time.time - _lastMoveTime < _swipeCooldown) return;

            Vector2 delta = end - start;

            if (delta.magnitude < _swipeThreshold) return;

            MoveDirection direction;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                direction = delta.x > 0 ? MoveDirection.Right : MoveDirection.Left;
            else
                direction = delta.y > 0 ? MoveDirection.Up : MoveDirection.Down;

            GameManager.Instance.TryMove(direction);
            _lastMoveTime = Time.time;
        }
    }
}
