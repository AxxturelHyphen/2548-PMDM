using System.Collections.Generic;
using UnityEngine;
using PMDM.Helpers;

namespace PMDM.Core
{
    public enum MoveDirection { Up, Down, Left, Right }

    /// <summary>
    /// Gestiona el tablero 4x4 del juego 2548.
    /// Controla el spawning de fichas, movimiento, fusión Fibonacci y detección de fin de juego.
    /// </summary>
    public class BoardManager : MonoBehaviour
    {
        public const int GridSize = 4;
        public const long WinValue = 2584; // Fibonacci objetivo

        [Header("Board Settings")]
        [SerializeField] private float _cellSize = 1.2f;
        [SerializeField] private float _cellSpacing = 0.1f;
        [SerializeField] private Transform _boardOrigin;

        [Header("Prefabs")]
        [SerializeField] private GameObject _tilePrefab;

        // Grid: [columna, fila] -> Tile (null si vacía)
        private Tile[,] _grid = new Tile[GridSize, GridSize];

        // Estado para undo
        private long[,] _previousGridValues;
        private bool _hasPreviousState;

        public System.Action<long> OnScoreChanged;
        public System.Action OnWin;
        public System.Action OnGameOver;
        public System.Action OnBoardChanged;

        private long _score;
        public long Score => _score;

        private void Start()
        {
            _previousGridValues = new long[GridSize, GridSize];
        }

        public void InitializeBoard()
        {
            ClearBoard();
            _score = 0;
            OnScoreChanged?.Invoke(_score);
            SpawnRandomTile();
            SpawnRandomTile();
        }

        public void ClearBoard()
        {
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    if (_grid[x, y] != null)
                    {
                        _grid[x, y].DestroyTile();
                        _grid[x, y] = null;
                    }
                }
            }
        }

        /// <summary>
        /// Ejecuta un movimiento en la dirección dada.
        /// Devuelve true si algo se movió o fusionó.
        /// </summary>
        public bool ExecuteMove(MoveDirection direction)
        {
            SaveState();
            ResetMergeFlags();

            bool moved = false;

            switch (direction)
            {
                case MoveDirection.Up:
                    moved = ProcessColumns(true);
                    break;
                case MoveDirection.Down:
                    moved = ProcessColumns(false);
                    break;
                case MoveDirection.Left:
                    moved = ProcessRows(true);
                    break;
                case MoveDirection.Right:
                    moved = ProcessRows(false);
                    break;
            }

            if (moved)
            {
                SpawnRandomTile();
                OnBoardChanged?.Invoke();

                if (CheckWinCondition())
                    OnWin?.Invoke();
                else if (!HasValidMoves())
                    OnGameOver?.Invoke();
            }

            return moved;
        }

        private bool ProcessColumns(bool upward)
        {
            bool moved = false;
            for (int x = 0; x < GridSize; x++)
            {
                if (upward)
                    moved |= ProcessLine(x, 0, 0, 1);  // De abajo a arriba
                else
                    moved |= ProcessLine(x, GridSize - 1, 0, -1); // De arriba a abajo
            }
            return moved;
        }

        private bool ProcessRows(bool leftward)
        {
            bool moved = false;
            for (int y = 0; y < GridSize; y++)
            {
                if (leftward)
                    moved |= ProcessLine(0, y, 1, 0);  // De derecha a izquierda
                else
                    moved |= ProcessLine(GridSize - 1, y, -1, 0); // De izquierda a derecha
            }
            return moved;
        }

        /// <summary>
        /// Procesa una línea (fila o columna) en la dirección indicada.
        /// startX/startY es el extremo hacia donde se deslizan las fichas.
        /// dx/dy indica la dirección de recorrido.
        /// </summary>
        private bool ProcessLine(int startX, int startY, int dx, int dy)
        {
            bool moved = false;
            int x = startX;
            int y = startY;

            // Recoger fichas de la línea en orden
            List<Tile> tiles = new List<Tile>();
            for (int i = 0; i < GridSize; i++)
            {
                int cx = startX + dx * i;
                int cy = startY + dy * i;
                if (_grid[cx, cy] != null)
                    tiles.Add(_grid[cx, cy]);
            }

            // Limpiar la línea
            for (int i = 0; i < GridSize; i++)
            {
                int cx = startX + dx * i;
                int cy = startY + dy * i;
                _grid[cx, cy] = null;
            }

            // Fusionar y colocar recursivamente
            List<Tile> merged = MergeTilesRecursive(tiles, 0, new List<Tile>());

            // Colocar fichas fusionadas en la línea
            for (int i = 0; i < merged.Count; i++)
            {
                int nx = startX + dx * i;
                int ny = startY + dy * i;
                Vector2Int newPos = new Vector2Int(nx, ny);

                if (merged[i].GridPosition != newPos)
                    moved = true;

                _grid[nx, ny] = merged[i];
                merged[i].MoveTo(newPos, GridToWorldPosition(newPos));
            }

            if (merged.Count != tiles.Count)
                moved = true;

            return moved;
        }

        /// <summary>
        /// Fusión recursiva de fichas en una línea.
        /// Recorre la lista, e intenta fusionar cada par consecutivo.
        /// </summary>
        private List<Tile> MergeTilesRecursive(List<Tile> tiles, int index, List<Tile> result)
        {
            if (index >= tiles.Count)
                return result;

            Tile current = tiles[index];

            // Si hay siguiente y son Fibonacci consecutivos, fusionar
            if (index + 1 < tiles.Count && !current.Merged && !tiles[index + 1].Merged)
            {
                Tile next = tiles[index + 1];
                long mergeResult = FibonacciHelper.MergeResult(current.Value, next.Value);

                if (mergeResult > 0)
                {
                    // Fusionar: el tile mayor absorbe al menor
                    Tile keeper = current.Value >= next.Value ? current : next;
                    Tile removed = current.Value >= next.Value ? next : current;

                    keeper.SetValue(mergeResult);
                    removed.DestroyTile();

                    _score += mergeResult;
                    OnScoreChanged?.Invoke(_score);

                    result.Add(keeper);
                    return MergeTilesRecursive(tiles, index + 2, result);
                }
            }

            result.Add(current);
            return MergeTilesRecursive(tiles, index + 1, result);
        }

        private void ResetMergeFlags()
        {
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    if (_grid[x, y] != null)
                        _grid[x, y].Merged = false;
        }

        /// <summary>
        /// Genera una ficha nueva en una posición vacía aleatoria.
        /// Los valores iniciales son 1 (80%) o 2 (20%), los primeros Fibonacci.
        /// </summary>
        public bool SpawnRandomTile()
        {
            List<Vector2Int> emptyCells = GetEmptyCells();
            if (emptyCells.Count == 0) return false;

            Vector2Int pos = emptyCells[Random.Range(0, emptyCells.Count)];
            long value = Random.value < 0.8f ? 1 : 2;

            SpawnTile(pos, value);
            return true;
        }

        public Tile SpawnTile(Vector2Int pos, long value)
        {
            if (_tilePrefab == null)
            {
                Debug.LogError("BoardManager: Tile prefab no asignado!");
                return null;
            }

            GameObject tileObj = Instantiate(_tilePrefab, transform);
            Tile tile = tileObj.GetComponent<Tile>();
            tile.Initialize(value, pos, GridToWorldPosition(pos));
            _grid[pos.x, pos.y] = tile;
            return tile;
        }

        /// <summary>
        /// Convierte coordenadas de grid a posición mundo.
        /// El tablero está centrado en boardOrigin.
        /// </summary>
        public Vector3 GridToWorldPosition(Vector2Int gridPos)
        {
            float totalSize = _cellSize + _cellSpacing;
            float offset = (GridSize - 1) * totalSize * 0.5f;

            Vector3 origin = _boardOrigin != null ? _boardOrigin.position : Vector3.zero;

            return new Vector3(
                origin.x + gridPos.x * totalSize - offset,
                origin.y + gridPos.y * totalSize - offset,
                0f
            );
        }

        /// <summary>
        /// Busca recursivamente todas las celdas vacías del tablero.
        /// </summary>
        public List<Vector2Int> GetEmptyCells()
        {
            var result = new List<Vector2Int>();
            FindEmptyCellsRecursive(0, 0, result);
            return result;
        }

        private void FindEmptyCellsRecursive(int x, int y, List<Vector2Int> result)
        {
            if (x >= GridSize) return;

            if (_grid[x, y] == null)
                result.Add(new Vector2Int(x, y));

            // Avanzar: siguiente fila, o siguiente columna si terminamos la fila
            int nextY = y + 1;
            int nextX = x;
            if (nextY >= GridSize)
            {
                nextY = 0;
                nextX = x + 1;
            }

            FindEmptyCellsRecursive(nextX, nextY, result);
        }

        /// <summary>
        /// Verifica recursivamente si alguna ficha alcanzó el valor objetivo (2584).
        /// </summary>
        private bool CheckWinCondition()
        {
            return CheckWinRecursive(0, 0);
        }

        private bool CheckWinRecursive(int x, int y)
        {
            if (x >= GridSize) return false;

            if (_grid[x, y] != null && _grid[x, y].Value >= WinValue)
                return true;

            int nextY = y + 1;
            int nextX = x;
            if (nextY >= GridSize)
            {
                nextY = 0;
                nextX = x + 1;
            }

            return CheckWinRecursive(nextX, nextY);
        }

        /// <summary>
        /// Comprueba recursivamente si quedan movimientos válidos.
        /// </summary>
        public bool HasValidMoves()
        {
            if (GetEmptyCells().Count > 0) return true;
            return HasAdjacentMergeRecursive(0, 0);
        }

        private bool HasAdjacentMergeRecursive(int x, int y)
        {
            if (x >= GridSize) return false;

            if (_grid[x, y] != null)
            {
                // Comprobar vecino derecho
                if (x + 1 < GridSize && _grid[x + 1, y] != null &&
                    FibonacciHelper.AreConsecutiveFibonacci(_grid[x, y].Value, _grid[x + 1, y].Value))
                    return true;

                // Comprobar vecino superior
                if (y + 1 < GridSize && _grid[x, y + 1] != null &&
                    FibonacciHelper.AreConsecutiveFibonacci(_grid[x, y].Value, _grid[x, y + 1].Value))
                    return true;
            }

            int nextY = y + 1;
            int nextX = x;
            if (nextY >= GridSize)
            {
                nextY = 0;
                nextX = x + 1;
            }

            return HasAdjacentMergeRecursive(nextX, nextY);
        }

        // ── Estado para Undo ──

        private void SaveState()
        {
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    _previousGridValues[x, y] = _grid[x, y] != null ? _grid[x, y].Value : 0;

            _hasPreviousState = true;
        }

        public bool CanUndo() => _hasPreviousState;

        public void Undo()
        {
            if (!_hasPreviousState) return;

            ClearBoard();
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    if (_previousGridValues[x, y] > 0)
                    {
                        Vector2Int pos = new Vector2Int(x, y);
                        SpawnTile(pos, _previousGridValues[x, y]);
                    }
                }
            }

            _hasPreviousState = false;
            OnBoardChanged?.Invoke();
        }

        // ── Acceso al grid (para PowerUps) ──

        public Tile GetTileAt(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= GridSize || pos.y < 0 || pos.y >= GridSize)
                return null;
            return _grid[pos.x, pos.y];
        }

        public void SetTileAt(Vector2Int pos, Tile tile)
        {
            if (pos.x < 0 || pos.x >= GridSize || pos.y < 0 || pos.y >= GridSize)
                return;
            _grid[pos.x, pos.y] = tile;
        }

        public void RemoveTileAt(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= GridSize || pos.y < 0 || pos.y >= GridSize)
                return;
            if (_grid[pos.x, pos.y] != null)
            {
                _grid[pos.x, pos.y].DestroyTile();
                _grid[pos.x, pos.y] = null;
            }
        }

        /// <summary>
        /// Devuelve todas las fichas activas en el tablero.
        /// Recolección recursiva.
        /// </summary>
        public List<Tile> GetAllTiles()
        {
            var result = new List<Tile>();
            CollectTilesRecursive(0, 0, result);
            return result;
        }

        private void CollectTilesRecursive(int x, int y, List<Tile> result)
        {
            if (x >= GridSize) return;

            if (_grid[x, y] != null)
                result.Add(_grid[x, y]);

            int nextY = y + 1;
            int nextX = x;
            if (nextY >= GridSize)
            {
                nextY = 0;
                nextX = x + 1;
            }

            CollectTilesRecursive(nextX, nextY, result);
        }

        /// <summary>
        /// Devuelve un snapshot de valores del tablero (para PowerUps/UI).
        /// </summary>
        public long[,] GetGridSnapshot()
        {
            long[,] snapshot = new long[GridSize, GridSize];
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    snapshot[x, y] = _grid[x, y] != null ? _grid[x, y].Value : 0;
            return snapshot;
        }
    }
}
