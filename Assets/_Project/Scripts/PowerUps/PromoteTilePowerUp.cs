using System.Collections.Generic;
using UnityEngine;
using PMDM.Core;
using PMDM.Helpers;

namespace PMDM.PowerUps
{
    /// <summary>
    /// Power-Up: Promueve la ficha de mayor valor al siguiente número Fibonacci.
    /// Ej: 8 -> 13, 13 -> 21, etc.
    /// </summary>
    [CreateAssetMenu(fileName = "PromoteTilePowerUp", menuName = "2548/PowerUps/PromoteTile")]
    public class PromoteTilePowerUp : PowerUpBase
    {
        protected override bool Execute(BoardManager board)
        {
            List<Tile> tiles = board.GetAllTiles();
            if (tiles.Count == 0) return false;

            // Encontrar la ficha con mayor valor recursivamente
            Tile maxTile = FindMaxTileRecursive(tiles, 0, null);

            if (maxTile == null) return false;

            long nextFib = FibonacciHelper.GetNextFibonacci(maxTile.Value);
            if (nextFib <= 0) return false;

            maxTile.SetValue(nextFib);
            return true;
        }

        private Tile FindMaxTileRecursive(List<Tile> tiles, int index, Tile currentMax)
        {
            if (index >= tiles.Count) return currentMax;

            Tile candidate = tiles[index];
            if (currentMax == null || candidate.Value > currentMax.Value)
                currentMax = candidate;

            return FindMaxTileRecursive(tiles, index + 1, currentMax);
        }
    }
}
