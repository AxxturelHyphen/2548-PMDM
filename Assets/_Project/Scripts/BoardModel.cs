using System.Collections.Generic;

public class BoardModel
{
    private int[,] grid;
    private int score;
    
    // Secuencia de Fibonacci hasta 2584
    private static readonly Dictionary<int, int> fibonacciSequence = new Dictionary<int, int>
    {
        {0, 0},   // Vacío
        {1, 1},   // 1
        {2, 1},   // 1
        {3, 2},   // 2
        {4, 3},   // 3
        {5, 5},   // 5
        {6, 8},   // 8
        {7, 13},  // 13
        {8, 21},  // 21
        {9, 34},  // 34
        {10, 55}, // 55
        {11, 89}, // 89
        {12, 144}, // 144
        {13, 233}, // 233
        {14, 377}, // 377
        {15, 610}, // 610
        {16, 987}, // 987
        {17, 1597}, // 1597
        {18, 2584}  // 2584
    };

    public BoardModel()
    {
        grid = new int[4, 4];
        score = 0;
        
        // Inicializar con dos fichas
        SpawnNewTile();
        SpawnNewTile();
    }

    // Propiedad pública para acceder al grid directamente
    public int[,] Grid => grid;
    
    public int[,] GetGrid()
    {
        return grid;
    }

    // Propiedad pública para acceder al score directamente
    public int Score => score;
    
    public int GetScore()
    {
        return score;
    }

    public void AddScore(int points)
    {
        score += points;
    }

    // Obtener el valor en una posición específica
    public int GetValueAt(int row, int col)
    {
        if (row < 0 || row >= 4 || col < 0 || col >= 4)
            return 0;
        
        return grid[row, col];
    }

    // Establecer el valor en una posición específica
    public void SetValueAt(int row, int col, int value)
    {
        if (row < 0 || row >= 4 || col < 0 || col >= 4)
            return;
        
        grid[row, col] = value;
    }

    // Restaurar un estado completo del tablero (para undo)
    public void RestoreState(int[,] state)
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                grid[row, col] = state[row, col];
            }
        }
    }

    // Reiniciar el tablero completamente
    public void Reset()
    {
        // Limpiar todo el tablero
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                grid[row, col] = 0;
            }
        }
        
        // Reiniciar el score
        score = 0;
        
        // Generar dos fichas iniciales
        SpawnNewTile();
        SpawnNewTile();
    }

    // Verificar si dos valores pueden fusionarse según Fibonacci
    public bool CanMergeFib(int value1, int value2)
    {
        if (value1 == 0 || value2 == 0) return false;

        // En Fibonacci, dos números consecutivos se suman para dar el siguiente
        // Por ejemplo: 1+1=2, 1+2=3, 2+3=5, 3+5=8, etc.

        int sum = value1 + value2;

        // Verificar si la suma existe en la secuencia de Fibonacci
        foreach (var kvp in fibonacciSequence)
        {
            if (kvp.Value == sum)
            {
                return true;
            }
        }

        return false;
    }

    // Generar una nueva ficha
    public void SpawnNewTile()
    {
        // Encontrar todas las celdas vacías
        List<(int row, int col)> emptyCells = new List<(int row, int col)>();
        
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (grid[row, col] == 0)
                {
                    emptyCells.Add((row, col));
                }
            }
        }
        
        // Si no hay celdas vacías, no hacer nada
        if (emptyCells.Count == 0) return;
        
        // Elegir una celda vacía aleatoria
        int randomIndex = UnityEngine.Random.Range(0, emptyCells.Count);
        var (spawnRow, spawnCol) = emptyCells[randomIndex];
        
        // 90% probabilidad de 1, 10% probabilidad de 2
        int value = UnityEngine.Random.value < 0.9f ? 1 : 2;
        grid[spawnRow, spawnCol] = value;
    }

    // Mover el tablero en una dirección
    public bool Move(Direction direction)
    {
        bool moved = false;
        
        // Crear una copia temporal del grid para comparar
        int[,] oldGrid = new int[4, 4];
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                oldGrid[row, col] = grid[row, col];
            }
        }
        
        // Aplicar el movimiento según la dirección
        switch (direction)
        {
            case Direction.Up:
                moved = MoveUp();
                break;
            case Direction.Down:
                moved = MoveDown();
                break;
            case Direction.Left:
                moved = MoveLeft();
                break;
            case Direction.Right:
                moved = MoveRight();
                break;
        }
        
        // Si hubo movimiento, generar nueva ficha
        if (moved)
        {
            SpawnNewTile();
        }
        
        return moved;
    }

    private bool MoveLeft()
    {
        bool moved = false;
        
        for (int row = 0; row < 4; row++)
        {
            // Comprimir: mover todos los valores hacia la izquierda
            int writePos = 0;
            for (int col = 0; col < 4; col++)
            {
                if (grid[row, col] != 0)
                {
                    if (writePos != col)
                    {
                        grid[row, writePos] = grid[row, col];
                        grid[row, col] = 0;
                        moved = true;
                    }
                    writePos++;
                }
            }
            
            // Fusionar: combinar valores adyacentes que puedan fusionarse
            for (int col = 0; col < 3; col++)
            {
                if (grid[row, col] != 0 && CanMergeFib(grid[row, col], grid[row, col + 1]))
                {
                    // Fusionar
                    int newValue = grid[row, col] + grid[row, col + 1];
                    grid[row, col] = newValue;
                    grid[row, col + 1] = 0;
                    score += newValue;
                    moved = true;
                    
                    // Comprimir de nuevo después de fusionar
                    for (int c = col + 1; c < 3; c++)
                    {
                        grid[row, c] = grid[row, c + 1];
                        grid[row, c + 1] = 0;
                    }
                }
            }
        }
        
        return moved;
    }

    private bool MoveRight()
    {
        bool moved = false;
        
        for (int row = 0; row < 4; row++)
        {
            // Comprimir hacia la derecha
            int writePos = 3;
            for (int col = 3; col >= 0; col--)
            {
                if (grid[row, col] != 0)
                {
                    if (writePos != col)
                    {
                        grid[row, writePos] = grid[row, col];
                        grid[row, col] = 0;
                        moved = true;
                    }
                    writePos--;
                }
            }
            
            // Fusionar de derecha a izquierda
            for (int col = 3; col > 0; col--)
            {
                if (grid[row, col] != 0 && CanMergeFib(grid[row, col], grid[row, col - 1]))
                {
                    int newValue = grid[row, col] + grid[row, col - 1];
                    grid[row, col] = newValue;
                    grid[row, col - 1] = 0;
                    score += newValue;
                    moved = true;
                    
                    // Comprimir
                    for (int c = col - 1; c > 0; c--)
                    {
                        grid[row, c] = grid[row, c - 1];
                        grid[row, c - 1] = 0;
                    }
                }
            }
        }
        
        return moved;
    }

    private bool MoveUp()
    {
        bool moved = false;
        
        for (int col = 0; col < 4; col++)
        {
            // Comprimir hacia arriba
            int writePos = 0;
            for (int row = 0; row < 4; row++)
            {
                if (grid[row, col] != 0)
                {
                    if (writePos != row)
                    {
                        grid[writePos, col] = grid[row, col];
                        grid[row, col] = 0;
                        moved = true;
                    }
                    writePos++;
                }
            }
            
            // Fusionar de arriba hacia abajo
            for (int row = 0; row < 3; row++)
            {
                if (grid[row, col] != 0 && CanMergeFib(grid[row, col], grid[row + 1, col]))
                {
                    int newValue = grid[row, col] + grid[row + 1, col];
                    grid[row, col] = newValue;
                    grid[row + 1, col] = 0;
                    score += newValue;
                    moved = true;
                    
                    // Comprimir
                    for (int r = row + 1; r < 3; r++)
                    {
                        grid[r, col] = grid[r + 1, col];
                        grid[r + 1, col] = 0;
                    }
                }
            }
        }
        
        return moved;
    }

    private bool MoveDown()
    {
        bool moved = false;
        
        for (int col = 0; col < 4; col++)
        {
            // Comprimir hacia abajo
            int writePos = 3;
            for (int row = 3; row >= 0; row--)
            {
                if (grid[row, col] != 0)
                {
                    if (writePos != row)
                    {
                        grid[writePos, col] = grid[row, col];
                        grid[row, col] = 0;
                        moved = true;
                    }
                    writePos--;
                }
            }
            
            // Fusionar de abajo hacia arriba
            for (int row = 3; row > 0; row--)
            {
                if (grid[row, col] != 0 && CanMergeFib(grid[row, col], grid[row - 1, col]))
                {
                    int newValue = grid[row, col] + grid[row - 1, col];
                    grid[row, col] = newValue;
                    grid[row - 1, col] = 0;
                    score += newValue;
                    moved = true;
                    
                    // Comprimir
                    for (int r = row - 1; r > 0; r--)
                    {
                        grid[r, col] = grid[r - 1, col];
                        grid[r - 1, col] = 0;
                    }
                }
            }
        }
        
        return moved;
    }

    // Verificar si el juego ha terminado
    public bool IsGameOver()
    {
        // Si hay celdas vacías, el juego continúa
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (grid[row, col] == 0) return false;
            }
        }
        
        // Verificar si hay fusiones posibles
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                int current = grid[row, col];
                
                // Verificar derecha
                if (col < 3 && CanMergeFib(current, grid[row, col + 1]))
                    return false;
                
                // Verificar abajo
                if (row < 3 && CanMergeFib(current, grid[row + 1, col]))
                    return false;
            }
        }
        
        return true;
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}