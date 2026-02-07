using UnityEngine;
using PMDM.Core;

namespace PMDM.PowerUps
{
    /// <summary>
    /// Clase base abstracta para todos los power-ups del juego 2548.
    /// Define la interfaz común: cooldown, activación y efecto.
    /// </summary>
    public abstract class PowerUpBase : ScriptableObject
    {
        [Header("Power-Up Settings")]
        [SerializeField] private string _displayName;
        [SerializeField] [TextArea] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private int _maxUses = 3;
        [SerializeField] private float _cooldownSeconds = 5f;

        private int _remainingUses;
        private float _lastUsedTime = float.NegativeInfinity;

        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Icon => _icon;
        public int RemainingUses => _remainingUses;
        public float CooldownSeconds => _cooldownSeconds;
        public bool IsOnCooldown => Time.time - _lastUsedTime < _cooldownSeconds;
        public bool CanUse => _remainingUses > 0 && !IsOnCooldown;

        public float CooldownRemaining =>
            Mathf.Max(0f, _cooldownSeconds - (Time.time - _lastUsedTime));

        public System.Action<PowerUpBase> OnUsed;
        public System.Action<PowerUpBase> OnCooldownFinished;

        public void Initialize()
        {
            _remainingUses = _maxUses;
            _lastUsedTime = float.NegativeInfinity;
        }

        public bool TryActivate(BoardManager board)
        {
            if (!CanUse)
            {
                Debug.LogWarning($"PowerUp '{_displayName}' no disponible. " +
                    $"Usos: {_remainingUses}, Cooldown: {IsOnCooldown}");
                return false;
            }

            bool success = Execute(board);

            if (success)
            {
                _remainingUses--;
                _lastUsedTime = Time.time;
                OnUsed?.Invoke(this);
            }

            return success;
        }

        /// <summary>
        /// Implementa el efecto concreto del power-up.
        /// Devuelve true si se ejecutó correctamente.
        /// </summary>
        protected abstract bool Execute(BoardManager board);
    }
}
