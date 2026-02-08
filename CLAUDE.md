# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

2D Fibonacci puzzle game (similar to 2048 but using Fibonacci sequence merging rules) built with Unity 6. The codebase uses **Spanish-language** naming conventions for variables, methods, and comments.

## Tech Stack

- **Unity 6** (6000.2.13f1) with **Universal Render Pipeline (URP)** configured for 2D
- **C# 9.0** (.NET Standard 2.1)
- **TextMeshPro** for UI text rendering
- **Unity Input System** package (modern input handling via `GameInputs.inputactions`)
- **LeanTween** for animations (tile pop, merge, fade effects)
- **Unity Test Framework** (NUnit-based, no tests written yet)

## Build & Run

This is a standard Unity project — open in Unity Editor 6000.2.13f1. No custom build scripts or CI/CD pipelines exist.

## Project Structure

All game code and assets live under `Assets/_Project/`:

- `Scripts/` — C# game scripts
- `Scenes/` — Three scenes: `MainMenu.unity`, `Game.unity`, `GameOver.unity`
- `Art/` — Sprites and backgrounds
- `Music/` — Audio assets
- `Prefabs/` — Reusable GameObjects

## High-Level Architecture

The game follows **Model-View-Controller (MVC)** pattern with additional singleton systems:

### Core Game Loop (MVC)

- **BoardModel** (`BoardModel.cs`) — Pure game logic layer
  - 4x4 grid state management
  - Fibonacci merge rules: consecutive Fibonacci numbers can merge (1+1=2, 1+2=3, 2+3=5, etc.)
  - Move processing (Up/Down/Left/Right) with compression and merging
  - Score tracking and game over detection
  - NO Unity dependencies (pure C# logic)

- **BoardView** (`BoardView.cs`) — Visual rendering layer
  - Creates 4x4 grid of cell GameObjects from prefab
  - Renders board state with color-coded tiles (Fibonacci value → color mapping)
  - LeanTween animations: pop (new tile), merge (combined tiles), fade-out (destroyed tiles)
  - Contains nested `CellView` helper class for individual cell visuals

- **InputController** (`InputController.cs`) — Controller orchestrator
  - Bridges user input → model updates → view rendering
  - Listens to `GameInputs` actions (WASD for movement, R for reset)
  - Locks input during animations (`canMove` flag)
  - Checks game over state after each move

### Powerup System

- **PowerupManager** (`PowerupManager.cs`)
  - Manages three powerup types: Undo (3x), Hammer (3x), Shuffle (2x)
  - Undo: Stack-based state history (max 5 saved states)
  - Hammer: Click/drag to destroy any single tile
  - Shuffle: Randomly redistributes all non-zero tiles
  - Fires `OnPowerupCountChanged` event for UI updates

- **PowerupsUI** (`PowerupsUI.cs`)
  - Button handlers and counter displays for powerups
  - Updates button interactability based on available counts
  - Audio toggle with persistent state via `PlayerPrefs`

- **HammerDraggable** (`HammerDraggable.cs`)
  - Drag-and-drop mechanic for hammer powerup
  - Creates visual drag icon during drag operation
  - Communicates with `CellClickHandler` on drop

- **CellClickHandler** (`CellClickHandler.cs`)
  - Attached to each grid cell for click/drop detection
  - Routes hammer usage to `PowerupManager.ApplyHammer(row, col)`

### Singleton Systems

- **AudioManager** (`AudioManager.cs`)
  - Singleton pattern with `DontDestroyOnLoad`
  - Manages background music and SFX (move, merge, powerup, game over)
  - Dynamic pitch variation on merge sounds based on tile value
  - Persistent mute state via `PlayerPrefs`

- **SceneController** (`SceneController.cs`)
  - Singleton for scene transitions
  - Methods: `LoadMainMenu()`, `LoadGame()`, `LoadGameOver(score)`, `RestartGame()`, `QuitGame()`
  - Passes final score to Game Over screen via `PlayerPrefs`

### Input System

- **GameInputs** (`GameInputs.cs`, auto-generated)
  - Generated from `Assets/_Project/GameInputs.inputactions`
  - DO NOT manually edit this file
  - Defines "Gameplay" action map with:
    - **Move**: WASD → Vector2 (composite binding)
    - **Reset**: R key → Button
  - Regenerate by modifying the `.inputactions` asset in Unity

## Code Conventions

- **Spanish language**: All variable/method names and comments are in Spanish
  - Examples: `tablero`, `celda`, `fusionar`, `valor`, `Inicializar()`, `ActualizarVisual()`
- **Naming**: PascalCase for public methods/properties, camelCase for private fields
- **Unity patterns**: `[Header]` attributes for Inspector organization, null checks before Unity API calls
- **Event pattern**: Use C# `Action` events for cross-component communication (e.g., `PowerupManager.OnPowerupCountChanged`)

## Common Gotchas

1. **Null Reference Errors**: Always null-check Inspector-assigned references before use (buttons, text fields, managers)
2. **Audio Listeners**: Unity scenes must have exactly ONE Audio Listener (typically on Main Camera)
3. **URP Camera Data**: Main Camera requires "Universal Additional Camera Data" component
4. **Input locking**: `InputController` disables input during animations to prevent rapid-fire moves
5. **Fibonacci merge logic**: Only consecutive Fibonacci numbers merge (not just any adjacent tiles)
6. **Auto-generated code**: Never manually edit `GameInputs.cs` — regenerate via Input Actions asset
