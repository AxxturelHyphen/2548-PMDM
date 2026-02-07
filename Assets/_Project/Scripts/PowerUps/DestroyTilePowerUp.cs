using System.Collections.Generic;
using UnityEngine;
using PMDM.Core;

namespace PMDM.PowerUps
{
    /// <summary>
    /// Power-Up: Destruye la ficha de menor valor del tablero.
    /// Si hay empate, elimina una aleatoria entre las menores.
    /// </summary>
    [CreateAssetMenu(fileName = "DestroyTilePowerUp", menuName = "2548/PowerUps/DestroyTile")]
    public class DestroyTilePowerUp : PowerUpBase
    {
        protected override bool Execute(BoardManager board)
        {
            List<Tile> tiles = board.GetAllTiles();
            if (tiles.Count == 0) return false;

            // Encontrar el valor mínimo recursivamente
            long minValue = FindMinValueRecursive(tiles, 0, long.MaxValue);

            // Recoger todas las fichas con ese valor
            List<Tile> candidates = new List<Tile>();
            CollectCandidatesRecursive(tiles, 0, minValue, candidates);

            // Elegir una aleatoria y destruirla
            Tile target = candidates[Random.Range(0, candidates.Count)];
            board.RemoveTileAt(target.GridPosition);

            return true;
        }

        private long FindMinValueRecursive(List<Tile> tiles, int index, long currentMin)
        {
            if (index >= tiles.Count) return currentMin;

            long newMin = tiles[index].Value < currentMin ? tiles[index].Value : currentMin;
            return FindMinValueRecursive(tiles, index + 1, newMin);
        }

        private void CollectCandidatesRecursive(List<Tile> tiles, int index,
            long targetValue, List<Tile> candidates)
        {
            if (index >= tiles.Count) return;

            if (tiles[index].Value == targetValue)
                candidates.Add(tiles[index]);

            CollectCandidatesRecursive(tiles, index + 1, targetValue, candidates);
        }
    }
}
