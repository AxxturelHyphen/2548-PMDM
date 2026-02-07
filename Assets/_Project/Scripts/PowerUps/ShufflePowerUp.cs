using System.Collections.Generic;
using UnityEngine;
using PMDM.Core;

namespace PMDM.PowerUps
{
    /// <summary>
    /// Power-Up: Baraja aleatoriamente todas las fichas del tablero.
    /// Recoloca las fichas existentes en posiciones aleatorias sin cambiar sus valores.
    /// </summary>
    [CreateAssetMenu(fileName = "ShufflePowerUp", menuName = "2548/PowerUps/Shuffle")]
    public class ShufflePowerUp : PowerUpBase
    {
        protected override bool Execute(BoardManager board)
        {
            List<Tile> tiles = board.GetAllTiles();
            if (tiles.Count == 0) return false;

            // Guardar valores
            List<long> values = new List<long>();
            CollectValuesRecursive(tiles, 0, values);

            // Shuffle Fisher-Yates
            ShuffleRecursive(values, values.Count - 1);

            // Limpiar tablero
            board.ClearBoard();

            // Obtener posiciones vacías y reasignar
            List<Vector2Int> positions = board.GetEmptyCells();
            SpawnShuffledRecursive(board, values, positions, 0);

            return true;
        }

        private void CollectValuesRecursive(List<Tile> tiles, int index, List<long> values)
        {
            if (index >= tiles.Count) return;
            values.Add(tiles[index].Value);
            CollectValuesRecursive(tiles, index + 1, values);
        }

        private void ShuffleRecursive(List<long> list, int currentIndex)
        {
            if (currentIndex <= 0) return;

            int randomIndex = Random.Range(0, currentIndex + 1);
            (list[currentIndex], list[randomIndex]) = (list[randomIndex], list[currentIndex]);

            ShuffleRecursive(list, currentIndex - 1);
        }

        private void SpawnShuffledRecursive(BoardManager board, List<long> values,
            List<Vector2Int> positions, int index)
        {
            if (index >= values.Count) return;
            board.SpawnTile(positions[index], values[index]);
            SpawnShuffledRecursive(board, values, positions, index + 1);
        }
    }
}
