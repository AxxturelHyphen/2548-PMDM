using PMDM.Core;
using UnityEngine;

namespace PMDM.PowerUps
{
    /// <summary>
    /// Power-Up: Deshace el último movimiento.
    /// Restaura el estado del tablero al turno anterior.
    /// </summary>
    [CreateAssetMenu(fileName = "UndoPowerUp", menuName = "2548/PowerUps/Undo")]
    public class UndoPowerUp : PowerUpBase
    {
        protected override bool Execute(BoardManager board)
        {
            if (!board.CanUndo()) return false;
            board.Undo();
            return true;
        }
    }
}
