using UnityEngine;
using TMPro;
using PMDM.Helpers;

namespace PMDM.Core
{
    /// <summary>
    /// Representa una ficha individual en el tablero.
    /// Almacena su valor Fibonacci y gestiona su representación visual.
    /// </summary>
    public class Tile : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshPro _valueText;
        [SerializeField] private SpriteRenderer _background;

        [Header("Animation")]
        [SerializeField] private float _animationSpeed = 10f;
        [SerializeField] private float _popScale = 1.2f;

        private long _value;
        private Vector2Int _gridPosition;
        private Vector3 _targetPosition;
        private Vector3 _targetScale;
        private bool _isMoving;
        private bool _merged;

        public long Value => _value;
        public Vector2Int GridPosition => _gridPosition;
        public bool IsMoving => _isMoving;
        public bool Merged { get => _merged; set => _merged = value; }

        // Colores asociados a cada nivel Fibonacci (índice en la secuencia)
        private static readonly Color[] TileColors = new Color[]
        {
            new Color(0.93f, 0.89f, 0.85f), // 1
            new Color(0.93f, 0.87f, 0.78f), // 1 (segundo)
            new Color(0.95f, 0.69f, 0.47f), // 2
            new Color(0.96f, 0.58f, 0.39f), // 3
            new Color(0.96f, 0.49f, 0.37f), // 5
            new Color(0.96f, 0.37f, 0.23f), // 8
            new Color(0.93f, 0.81f, 0.45f), // 13
            new Color(0.93f, 0.80f, 0.38f), // 21
            new Color(0.93f, 0.78f, 0.31f), // 34
            new Color(0.93f, 0.77f, 0.25f), // 55
            new Color(0.93f, 0.75f, 0.18f), // 89
            new Color(0.93f, 0.73f, 0.11f), // 144
            new Color(0.48f, 0.31f, 0.63f), // 233
            new Color(0.40f, 0.23f, 0.60f), // 377
            new Color(0.33f, 0.18f, 0.55f), // 610
            new Color(0.25f, 0.13f, 0.50f), // 987
            new Color(0.18f, 0.08f, 0.45f), // 1597
            new Color(0.10f, 0.05f, 0.40f), // 2584
        };

        private void Update()
        {
            if (_isMoving)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    _targetPosition,
                    Time.deltaTime * _animationSpeed
                );

                if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
                {
                    transform.position = _targetPosition;
                    _isMoving = false;
                }
            }

            // Animación de escala (pop al fusionarse)
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                _targetScale,
                Time.deltaTime * _animationSpeed
            );
        }

        public void Initialize(long value, Vector2Int gridPos, Vector3 worldPos)
        {
            _value = value;
            _gridPosition = gridPos;
            transform.position = worldPos;
            _targetPosition = worldPos;
            _targetScale = Vector3.one;
            _merged = false;

            // Efecto pop al aparecer
            transform.localScale = Vector3.zero;
            _targetScale = Vector3.one;

            UpdateVisual();
        }

        public void SetValue(long newValue)
        {
            _value = newValue;
            _merged = true;

            // Pop al fusionarse
            _targetScale = Vector3.one * _popScale;
            Invoke(nameof(ResetScale), 0.1f);

            UpdateVisual();
        }

        private void ResetScale()
        {
            _targetScale = Vector3.one;
        }

        public void MoveTo(Vector2Int newGridPos, Vector3 worldPos)
        {
            _gridPosition = newGridPos;
            _targetPosition = worldPos;
            _isMoving = true;
        }

        public void SetGridPosition(Vector2Int pos)
        {
            _gridPosition = pos;
        }

        private void UpdateVisual()
        {
            if (_valueText != null)
                _valueText.text = _value.ToString();

            if (_background != null)
            {
                int fibIndex = FibonacciHelper.GetFibonacciIndex(_value);
                int colorIndex = Mathf.Clamp(fibIndex - 1, 0, TileColors.Length - 1);
                _background.color = TileColors[colorIndex];
            }

            // Texto oscuro para valores bajos, claro para altos
            if (_valueText != null)
            {
                _valueText.color = _value <= 5 ? Color.black : Color.white;
            }
        }

        public bool CanMergeWith(Tile other)
        {
            if (other == null) return false;
            return FibonacciHelper.AreConsecutiveFibonacci(_value, other.Value);
        }

        public void DestroyTile()
        {
            Destroy(gameObject);
        }
    }
}
