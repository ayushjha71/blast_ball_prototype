using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color tileColor;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        if (spriteRenderer == null) return;

        tileColor = color;

        // Ensure alpha is fully visible
        tileColor.a = 1f;

        spriteRenderer.color = tileColor;
    }


    public Color GetColor()
    {
        return tileColor;
    }
}