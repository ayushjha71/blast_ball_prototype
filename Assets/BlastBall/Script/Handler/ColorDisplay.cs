using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDisplay : MonoBehaviour
{
    [SerializeField] 
    private ColorCounter counterPrefab;
    [SerializeField] 
    private SpriteRenderer spriteRenderer; 

    private int mCurrentColorIndex = 0; 
    private bool isDragging = false; 
    private Vector3 mOffset; 
    private Vector3 mInitialPosition;

    private List<Color> mAvailableColors = new List<Color>();
    private Dictionary<Color, int> mColorCounts = new Dictionary<Color, int>();
    private ColorCounter mCurrentCounter;

    public ColorCounter CounterPrefab => counterPrefab;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mInitialPosition = transform.position;
    }

    public void SetupColorDisplay(List<Color> colors, Dictionary<Color, int> counts)
    {
        mAvailableColors = new List<Color>(colors);
        mColorCounts = new Dictionary<Color, int>();

        foreach (Color color in mAvailableColors)
        {
            if (counts.ContainsKey(color))
            {
                mColorCounts[color] = counts[color];
            }
        }

        mCurrentColorIndex = 0;

        if (mAvailableColors.Count > 0)
        {
            SetColorWithAlpha(mAvailableColors[mCurrentColorIndex]);
            UpdateCounter();
        }
        else
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1, 1, 1, 0);
            }
            if (mCurrentCounter != null)
            {
                Destroy(mCurrentCounter.gameObject);
                mCurrentCounter = null;
            }
        }
    }

    private void UpdateCounter()
    {
        if (mCurrentCounter == null && counterPrefab != null)
        {
            mCurrentCounter = Instantiate(counterPrefab, transform.position + new Vector3(), Quaternion.identity, transform);
        }

        if (mAvailableColors.Count > 0 && mCurrentCounter != null)
        {
            Color currentColor = mAvailableColors[mCurrentColorIndex];
            if (mColorCounts.ContainsKey(currentColor))
            {
                mCurrentCounter.SetColor(currentColor, mColorCounts[currentColor]);
            }
        }
    }

    private void SetColorWithAlpha(Color color)
    {
        spriteRenderer.color = new Color(color.r, color.g, color.b, 1f);
    }

    private void OnMouseDown()
    {
        if (spriteRenderer.color.a > 0) 
        {
            isDragging = true;
            mOffset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        }
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + mOffset;
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z); 
        }
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            Slot targetSlot = GetTargetSlot();

            if (targetSlot != null && !targetSlot.IsFilled() && mAvailableColors.Count > 0)
            {
                Color currentColor = mAvailableColors[mCurrentColorIndex];
                if (mColorCounts.ContainsKey(currentColor))
                {
                    targetSlot.FillSlot(currentColor, mColorCounts[currentColor]);

                    // Update the grid after filling the slot
                    if (GameManager.Instance.GridManager != null)
                    {
                        GameManager.Instance.GridManager.CheckForMatches(currentColor, mColorCounts[currentColor]);
                    }
                }
            }

            transform.position = mInitialPosition;
        }
    }

    private Slot GetTargetSlot()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);
        foreach (var hit in hits)
        {
            Slot slot = hit.collider.GetComponent<Slot>();
            if (slot != null) return slot;
        }
        return null;
    }
}
