# Estructura de Scripts - 2548 (Fibonacci 2048)

```
Assets/_Project/Scripts/
├── Core/
│   ├── GameManager.cs      ← Singleton, máquina de estados (Menu/Playing/Won/GameOver/Paused), score, highscore
│   ├── BoardManager.cs     ← Tablero 4x4, spawn, sliding, fusión Fibonacci, detección win/game-over
│   └── Tile.cs             ← Ficha individual: valor, color por nivel Fibonacci, animaciones pop
├── Helpers/
│   └── FibonacciHelper.cs  ← Funciones recursivas con memoización
├── Input/
│   └── InputHandler.cs     ← Teclado (WASD/flechas) + swipe táctil
└── PowerUps/
    ├── PowerUpBase.cs      ← Clase abstracta ScriptableObject (cooldown, usos, interfaz)
    ├── PowerUpManager.cs   ← Gestiona colección de power-ups
    ├── ShufflePowerUp.cs   ← Baraja todas las fichas aleatoriamente
    ├── DestroyTilePowerUp.cs ← Destruye la ficha de menor valor
    ├── PromoteTilePowerUp.cs ← Sube la mayor ficha al siguiente Fibonacci
    └── UndoPowerUp.cs      ← Deshace el último movimiento
```

## Mecánica Fibonacci (la diferencia clave vs 2048)

- En 2048: fichas iguales se fusionan (2+2=4, 4+4=8...)
- En **2548**: fichas **Fibonacci consecutivas** se fusionan → `1+1=2, 1+2=3, 2+3=5, 3+5=8, 5+8=13...` hasta **2584** (el Fibonacci objetivo)
- Nuevas fichas aparecen como **1** (80%) o **2** (20%)

## Funciones Recursivas

Todas en `FibonacciHelper.cs` + usadas extensamente en `BoardManager`:
- `Fibonacci(n)` — cálculo recursivo con memoización
- `IsFibonacci()` / `AreConsecutiveFibonacci()` — validación recursiva
- `MergeTilesRecursive()` — fusión de línea completa
- `FindEmptyCellsRecursive()`, `CheckWinRecursive()`, `HasAdjacentMergeRecursive()` — recorrido recursivo del tablero
- Los PowerUps también usan recursión (shuffle Fisher-Yates recursivo, búsqueda de min/max recursiva)

## Para usar en Unity

1. Crea un **Prefab de Tile** con `SpriteRenderer` + `TextMeshPro` + componente `Tile`
2. En la escena, añade un GameObject con `GameManager`, `BoardManager`, `InputHandler` y `PowerUpManager`
3. Crea los PowerUps como **ScriptableObjects** desde el menú `Create > 2548 > PowerUps`
4. Asigna las referencias en el Inspector y dale Play
