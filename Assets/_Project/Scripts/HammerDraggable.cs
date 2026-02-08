using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HammerDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PowerupManager powerupManager;
    public Image hammerImage;
    public Canvas canvas;
    
    private GameObject dragIcon;
    private bool isDragging = false;

    private void Start()
    {
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (powerupManager == null)
        {
            Debug.LogError("PowerupManager is null!");
            return;
        }

        if (powerupManager.GetHammersLeft() <= 0)
        {
            Debug.Log("No hammers left");
            return;
        }

        // Activar el modo martillo
        powerupManager.ActivateHammer();
        isDragging = true;

        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);

        Image icon = dragIcon.AddComponent<Image>();
        icon.sprite = hammerImage.sprite;
        icon.raycastTarget = false;
        icon.SetNativeSize();

        RectTransform rt = dragIcon.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);

        UpdateDragIconPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        UpdateDragIconPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        if (dragIcon != null)
        {
            Destroy(dragIcon);
        }

        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            CellClickHandler cell = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<CellClickHandler>();
            if (cell != null)
            {
                cell.OnHammerUsed();
            }
        }

        isDragging = false;
    }

    private void UpdateDragIconPosition(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out pos
            );
            dragIcon.transform.position = canvas.transform.TransformPoint(pos);
        }
    }
}