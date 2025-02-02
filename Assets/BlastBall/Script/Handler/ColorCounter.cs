using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorCounter : MonoBehaviour
{
    [SerializeField] 
    private TextMeshPro countText;

    private Color mCurrentColor;

    private void Awake()
    {
        if (countText == null)
        {
            countText = GetComponentInChildren<TextMeshPro>();
            if (countText == null)
            {
                countText = gameObject.AddComponent<TextMeshPro>();
            }
        }
    }

    public void SetColor(Color color, int count)
    {
        mCurrentColor = color;
        UpdateCount(count);
    }

    public void UpdateCount(int count)
    {
        if (countText != null)
        {
            countText.text = count.ToString();
        }
    }

    public Color GetCurrentColor()
    {
        return mCurrentColor;
    }
}
