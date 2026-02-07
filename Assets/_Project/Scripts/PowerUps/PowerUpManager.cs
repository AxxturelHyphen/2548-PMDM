using System.Collections.Generic;
using UnityEngine;
using PMDM.Core;

namespace PMDM.PowerUps
{
    /// <summary>
    /// Gestiona la colección de power-ups disponibles para el jugador.
    /// Coordina la activación y el estado de cada power-up.
    /// </summary>
    public class PowerUpManager : MonoBehaviour
    {
        [SerializeField] private BoardManager _boardManager;
        [SerializeField] private List<PowerUpBase> _availablePowerUps = new List<PowerUpBase>();

        public IReadOnlyList<PowerUpBase> AvailablePowerUps => _availablePowerUps;

        public System.Action<PowerUpBase> OnPowerUpActivated;
        public System.Action<PowerUpBase> OnPowerUpFailed;

        public void ResetPowerUps()
        {
            foreach (var powerUp in _availablePowerUps)
                powerUp.Initialize();
        }

        /// <summary>
        /// Activa un power-up por índice.
        /// </summary>
        public bool ActivatePowerUp(int index)
        {
            if (index < 0 || index >= _availablePowerUps.Count)
                return false;

            return ActivatePowerUp(_availablePowerUps[index]);
        }

        /// <summary>
        /// Activa un power-up concreto.
        /// </summary>
        public bool ActivatePowerUp(PowerUpBase powerUp)
        {
            if (_boardManager == null)
            {
                Debug.LogError("PowerUpManager: BoardManager no asignado!");
                return false;
            }

            if (GameManager.Instance.CurrentState != GameState.Playing)
                return false;

            bool success = powerUp.TryActivate(_boardManager);

            if (success)
                OnPowerUpActivated?.Invoke(powerUp);
            else
                OnPowerUpFailed?.Invoke(powerUp);

            return success;
        }

        /// <summary>
        /// Busca un power-up por nombre.
        /// </summary>
        public PowerUpBase GetPowerUpByName(string displayName)
        {
            return FindPowerUpRecursive(displayName, 0);
        }

        private PowerUpBase FindPowerUpRecursive(string name, int index)
        {
            if (index >= _availablePowerUps.Count) return null;

            if (_availablePowerUps[index].DisplayName == name)
                return _availablePowerUps[index];

            return FindPowerUpRecursive(name, index + 1);
        }
    }
}
