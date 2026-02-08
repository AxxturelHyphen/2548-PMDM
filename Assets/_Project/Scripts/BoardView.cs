using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BoardView : MonoBehaviour
{
    public Transform gridArea;
    public GameObject cellPrefab;
    public float animationDuration = 0.15f;
    public PowerupManager powerupManager;

    private CellView[,] cells = new CellView[4, 4];
    private int[,] previousGrid;

    private void Awake()
    {
        previousGrid = new int[4, 4];
        CreateCells();
    }

    private void CreateCells()
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                GameObject cellObj = Instantiate(cellPrefab, gridArea);
                cells[r, c] = new CellView(cellObj);
                
                CellClickHandler handler = cellObj.AddComponent<CellClickHandler>();
                handler.Initialize(r, c, powerupManager);
            }
        }
    }

    public void Render(BoardModel model)
    {
        StartCoroutine(AnimateRender(model));
    }

    private IEnumerator AnimateRender(BoardModel model)
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                int oldValue = previousGrid[r, c];
                int newValue = model.Grid[r, c];
                
                if (oldValue != newValue)
                {
                    if (newValue == 0)
                    {
                        cells[r, c].AnimateFadeOut(animationDuration);
                    }
                    else if (oldValue == 0)
                    {
                        cells[r, c].SetValue(newValue);
                        cells[r, c].SetColor(GetColorForValue(newValue));
                        cells[r, c].AnimatePop(animationDuration);
                    }
                    else
                    {
                        cells[r, c].SetValue(newValue);
                        cells[r, c].SetColor(GetColorForValue(newValue));
                        cells[r, c].AnimateMerge(animationDuration);
                    }
                }
                else
                {
                    cells[r, c].SetValue(newValue);
                    cells[r, c].SetColor(GetColorForValue(newValue));
                }
                
                previousGrid[r, c] = newValue;
            }
        }
        
        yield return new WaitForSeconds(animationDuration);
    }

    private Color GetColorForValue(int value)
    {
        if (value == 0) return new Color(0.8f, 0.76f, 0.71f);
        if (value == 1) return new Color(0.93f, 0.89f, 0.85f);
        if (value == 2) return new Color(0.93f, 0.88f, 0.78f);
        if (value == 3) return new Color(0.95f, 0.69f, 0.47f);
        if (value == 5) return new Color(0.96f, 0.58f, 0.39f);
        if (value == 8) return new Color(0.96f, 0.49f, 0.37f);
        if (value == 13) return new Color(0.96f, 0.37f, 0.23f);
        if (value == 21) return new Color(0.93f, 0.81f, 0.45f);
        if (value == 34) return new Color(0.93f, 0.80f, 0.38f);
        if (value == 55) return new Color(0.93f, 0.78f, 0.31f);
        if (value == 89) return new Color(0.93f, 0.77f, 0.25f);
        if (value >= 144) return new Color(0.93f, 0.76f, 0.18f);
        return Color.white;
    }
}

// CLASE CELLVIEW DENTRO DEL MISMO ARCHIVO
public class CellView
{
    private GameObject cellObject;
    private Image tileFill;
    private Image tileOverlay;
    private TextMeshProUGUI valueText;

    public CellView(GameObject cellObj)
    {
        cellObject = cellObj;
        tileFill = cellObj.transform.Find("TileFill").GetComponent<Image>();
        tileOverlay = cellObj.transform.Find("TileOverlay")?.GetComponent<Image>();
        valueText = cellObj.transform.Find("ValueText").GetComponent<TextMeshProUGUI>();
    }

    public void SetValue(int value)
    {
        valueText.text = value > 0 ? value.ToString() : "";
    }

    public void SetColor(Color color)
    {
        tileFill.color = color;
    }

    public void AnimatePop(float duration)
    {
        cellObject.transform.localScale = Vector3.zero;
        LeanTween.scale(cellObject, Vector3.one, duration).setEaseOutBack();
    }

    public void AnimateMerge(float duration)
    {
        LeanTween.scale(cellObject, Vector3.one * 1.2f, duration * 0.5f)
            .setEaseOutQuad()
            .setOnComplete(() => {
                LeanTween.scale(cellObject, Vector3.one, duration * 0.5f).setEaseInQuad();
            });
    }

    public void AnimateFadeOut(float duration)
    {
        LeanTween.scale(cellObject, Vector3.zero, duration).setEaseInBack();
    }
}