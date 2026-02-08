using UnityEngine;
using UnityEngine.EventSystems;

public class CellClickHandler : MonoBehaviour, IPointerClickHandler
{
    public int row;
    public int column;
    
    private PowerupManager powerupManager;

    public void Initialize(int r, int c, PowerupManager manager)
    {
        row = r;
        column = c;
        powerupManager = manager;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Solo para click directo (alternativa a drag)
        if (powerupManager != null && powerupManager.IsHammerActive())
        {
            powerupManager.ApplyHammer(row, column);
        }
    }

    public void OnHammerUsed()
    {
        if (powerupManager != null)
        {
            powerupManager.ApplyHammer(row, column);
        }
    }
}