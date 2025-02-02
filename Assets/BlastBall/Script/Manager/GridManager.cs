using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text levelText;
    [SerializeField] 
    private Vector2 gridOffset = new Vector2(0, 0);
    [SerializeField]
    private int rows = 4;
    [SerializeField]
    private int cols = 4;
    [SerializeField]
    private float spacing = 0.2f;
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private Transform gridParent;
    [SerializeField]
    private Color[] colors;


    private Tile[,] mGrid;
    private HashSet<Color> mUniqueColors = new HashSet<Color>();
    private Dictionary<Color, int> mColorCounts = new Dictionary<Color, int>();
    private ColorDisplay mColorDisplay;

    public TMP_Text LevelText => levelText;


    void Start()
    {
        if (rows <= 0 || cols <= 0)
        {
            Debug.LogError("Invalid grid size. Ensure rows and cols are set correctly in the Inspector.");
            return;
        }
        if (colors == null || colors.Length == 0)
        {
            Debug.LogError("Color array is empty. Assign colors in the Inspector.");
            return;
        }

        mColorDisplay = FindObjectOfType<ColorDisplay>();
        GenerateGrid();
    }

    private Dictionary<Color, int> GetBottomRowColors()
    {
        Dictionary<Color, int> bottomRowColors = new Dictionary<Color, int>();

        // First count colors in bottom row
        for (int x = 0; x < cols; x++)
        {
            if (mGrid[rows - 1, x] != null)
            {
                Color tileColor = mGrid[rows - 1, x].GetColor();

                // If color not in dictionary, count it in the entire grid
                if (!bottomRowColors.ContainsKey(tileColor))
                {
                    int totalCount = CountColorInGrid(tileColor);
                    bottomRowColors[tileColor] = totalCount;

                    // Update colorCounts dictionary
                    mColorCounts[tileColor] = totalCount;
                }
            }
        }

        return bottomRowColors;
    }

    private int CountColorInGrid(Color color)
    {
        int count = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (mGrid[y, x] != null && ColorEquals(mGrid[y, x].GetColor(), color))
                {
                    count++;
                }
            }
        }
        return count;
    }

    private bool ColorEquals(Color a, Color b)
    {
        return Mathf.Approximately(a.r, b.r) &&
               Mathf.Approximately(a.g, b.g) &&
               Mathf.Approximately(a.b, b.b);
    }

    void GenerateGrid()
    {
        mGrid = new Tile[rows, cols];
        mUniqueColors.Clear();
        mColorCounts.Clear();  // Clear the counts at start

        float gridWidth = (cols - 1) + (cols - 1) * spacing;
        float gridHeight = (rows - 1) + (rows - 1) * spacing;
        Vector2 startPos = new Vector2(-gridWidth / 2, gridHeight / 2) + gridOffset;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector3 tilePosition = new Vector3(startPos.x + x + x * spacing, startPos.y - y - y * spacing, 0);
                GameObject tileObj = Instantiate(tilePrefab, tilePosition, Quaternion.identity, gridParent);
                Tile tile = tileObj.GetComponent<Tile>();

                if (tile != null && colors.Length > 0)
                {
                    Color randomColor = colors[Random.Range(0, colors.Length)];
                    tile.SetColor(randomColor);
                    mUniqueColors.Add(randomColor);

                    // Count colors as we generate the grid
                    if (!mColorCounts.ContainsKey(randomColor))
                    {
                        mColorCounts[randomColor] = 0;
                    }
                    mColorCounts[randomColor]++;
                }
                mGrid[y, x] = tile;
            }
        }

        // Setup slots
        FindObjectOfType<SlotSpawner>().SetupSlots(cols);

        // Update color display with bottom row colors
        UpdateColorDisplay();
    }

    private void UpdateColorDisplay()
    {
        if (mColorDisplay != null)
        {
            Dictionary<Color, int> bottomColors = GetBottomRowColors();
            List<Color> availableColors = new List<Color>(bottomColors.Keys);
            mColorDisplay.SetupColorDisplay(availableColors, mColorCounts);
        }
    }

    public void CheckForMatches(Color color, int requiredCount)
    {
        StartCoroutine(ContinuousMatchCheck());
    }

    private IEnumerator ContinuousMatchCheck()
    {
        bool matchFound;
        do
        {
            matchFound = false;
            // Get current bottom row colors
            Dictionary<Color, int> bottomColors = GetBottomRowColors();

            foreach (Slot slot in FindObjectsOfType<Slot>())
            {
                if (!slot.IsFilled()) continue;

                Color slotColor = slot.GetColor().Value;
                // Only process if color exists in bottom row
                if (!bottomColors.ContainsKey(slotColor)) continue;

                int requiredCount = slot.GetRequiredCount();
                int matchCount = 0;
                List<Vector2Int> matchPositions = new List<Vector2Int>();

                // Check bottom row for matches
                for (int x = 0; x < cols; x++)
                {
                    if (mGrid[rows - 1, x] != null && Color.Equals(mGrid[rows - 1, x].GetColor(), slotColor))
                    {
                        matchCount++;
                        matchPositions.Add(new Vector2Int(rows - 1, x));
                    }
                }

                if (matchCount > 0)
                {
                    matchFound = true;
                    int toRemove = Mathf.Min(matchCount, requiredCount);

                    // Remove matched tiles
                    for (int i = 0; i < toRemove; i++)
                    {
                        Vector2Int pos = matchPositions[i];
                        if (mGrid[pos.x, pos.y] != null)
                        {
                            Destroy(mGrid[pos.x, pos.y].gameObject);
                            mGrid[pos.x, pos.y] = null;
                        }
                    }

                    // Update counts
                    if (mColorCounts.ContainsKey(slotColor))
                    {
                        mColorCounts[slotColor] -= toRemove;
                        if (mColorCounts[slotColor] <= 0)
                        {
                            mColorCounts.Remove(slotColor);
                            mUniqueColors.Remove(slotColor);
                            slot.Clear();
                        }
                        else
                        {
                            slot.UpdateCount(mColorCounts[slotColor]);
                        }
                    }
                    for (int x = 0; x < cols; x++)
                    {
                        ShiftColumnDown(x, rows - 1);
                    }

                    yield return new WaitForSeconds(0.2f);
                    break;
                    
                }
            }
            // Update color display after each iteration
            UpdateColorDisplay();

        } while (matchFound);
        Dictionary<Color, int> bottomColorsCount = GetBottomRowColors();
        GameManager.Instance.CheckLevelCompletion(mColorCounts, bottomColorsCount);
    }

    private void ShiftColumnDown(int col, int startY)
    {
        bool shifted;
        do
        {
            shifted = false;
            for (int y = startY; y > 0; y--)
            {
                if (mGrid[y, col] == null && mGrid[y - 1, col] != null)
                {
                    mGrid[y, col] = mGrid[y - 1, col];
                    mGrid[y - 1, col] = null;

                    Vector3 newPosition = new Vector3(
                        mGrid[y, col].transform.position.x,
                        mGrid[y, col].transform.position.y - (1 + spacing),
                        mGrid[y, col].transform.position.z
                    );
                    mGrid[y, col].transform.position = newPosition;
                    shifted = true;
                }
            }
        } while (shifted);

        UpdateColorDisplay();
    }
}