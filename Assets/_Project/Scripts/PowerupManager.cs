using UnityEngine;
using System.Collections.Generic;
using System;

public class PowerupManager : MonoBehaviour
{
    // Evento para notificar cambios en los contadores de powerups
    public event Action OnPowerupCountChanged;
    
    [Header("Límites de powerups")]
    public int maxUndos = 3;
    public int maxHammers = 3;
    public int maxShuffles = 2;
    
    private int undosLeft;
    private int hammersLeft;
    private int shufflesLeft;
    
    // Stack para guardar estados previos del tablero
    private Stack<int[,]> savedStates = new Stack<int[,]>();
    private const int maxSavedStates = 5;
    
    // Referencia al modelo del tablero
    private BoardModel boardModel;
    private BoardView boardView;

    // Flag para saber si el martillo está activo
    private bool hammerActive = false;

    public void Initialize(BoardModel model, BoardView view)
    {
        boardModel = model;
        boardView = view;

        // Inicializar los powerups disponibles
        undosLeft = maxUndos;
        hammersLeft = maxHammers;
        shufflesLeft = maxShuffles;

        // Notificar el cambio inicial
        OnPowerupCountChanged?.Invoke();
    }

    // Guardar el estado actual del tablero antes de un movimiento
    public void SaveState(int[,] gridState)
    {
        // Crear una copia profunda del estado
        int[,] stateCopy = new int[4, 4];
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                stateCopy[row, col] = gridState[row, col];
            }
        }
        
        // Añadir al stack
        savedStates.Push(stateCopy);
        
        // Limitar el tamaño del stack
        if (savedStates.Count > maxSavedStates)
        {
            // Convertir a lista temporal, quitar el más antiguo y volver a stack
            List<int[,]> tempList = new List<int[,]>(savedStates);
            tempList.RemoveAt(tempList.Count - 1);
            savedStates = new Stack<int[,]>(tempList);
        }
    }

    // Usar undo
    public void UseUndo()
    {
        if (!CanUseUndo()) return;

        if (savedStates.Count > 0)
        {
            // Recuperar el estado anterior
            int[,] previousState = savedStates.Pop();

            // Aplicarlo al modelo del tablero
            if (boardModel != null)
            {
                boardModel.RestoreState(previousState);
            }

            // Actualizar la vista
            if (boardView != null)
            {
                boardView.Render(boardModel);
            }

            undosLeft--;
            OnPowerupCountChanged?.Invoke();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayPowerup();
            }
        }
    }

    // Activar el modo martillo
    public void ActivateHammer()
    {
        if (!CanUseHammer()) return;
        
        hammerActive = true;
        Debug.Log("Martillo activado - haz clic en una celda para destruirla");
    }

    // Aplicar el martillo a una celda específica
    public void ApplyHammer(int row, int col)
    {
        if (!hammerActive || boardModel == null) return;

        // Verificar que la celda no esté vacía
        if (boardModel.GetValueAt(row, col) == 0)
        {
            Debug.Log("No puedes usar el martillo en una celda vacía");
            hammerActive = false;
            return;
        }

        // Destruir la celda
        boardModel.SetValueAt(row, col, 0);

        // Actualizar la vista
        if (boardView != null)
        {
            boardView.Render(boardModel);
        }

        hammersLeft--;
        hammerActive = false;
        OnPowerupCountChanged?.Invoke();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPowerup();
        }

        Debug.Log($"Martillo usado en celda ({row}, {col})");
    }

    // Usar shuffle
    public void UseShuffle()
    {
        if (!CanUseShuffle() || boardModel == null) return;

        // Obtener todas las celdas no vacías
        List<int> nonZeroValues = new List<int>();
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                int value = boardModel.GetValueAt(row, col);
                if (value != 0)
                {
                    nonZeroValues.Add(value);
                }
            }
        }

        // Mezclar los valores
        for (int i = nonZeroValues.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            int temp = nonZeroValues[i];
            nonZeroValues[i] = nonZeroValues[randomIndex];
            nonZeroValues[randomIndex] = temp;
        }

        // Redistribuir los valores mezclados
        int valueIndex = 0;
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                if (boardModel.GetValueAt(row, col) != 0)
                {
                    boardModel.SetValueAt(row, col, nonZeroValues[valueIndex]);
                    valueIndex++;
                }
            }
        }

        // Actualizar la vista
        if (boardView != null)
        {
            boardView.Render(boardModel);
        }

        shufflesLeft--;
        OnPowerupCountChanged?.Invoke();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPowerup();
        }

        Debug.Log("Shuffle usado - valores redistribuidos");
    }

    // Getters para los contadores
    public int GetUndosLeft() => undosLeft;
    public int GetHammersLeft() => hammersLeft;
    public int GetShufflesLeft() => shufflesLeft;
    
    // Métodos para verificar si se pueden usar los powerups
    public bool CanUseUndo() => undosLeft > 0 && savedStates.Count > 0;
    public bool CanUseHammer() => hammersLeft > 0;
    public bool CanUseShuffle() => shufflesLeft > 0;
    
    // Getter para saber si el martillo está activo
    public bool IsHammerActive() => hammerActive;
}