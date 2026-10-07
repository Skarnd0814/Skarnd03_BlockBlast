using UnityEngine;

public class Board : MonoBehaviour
{
    public const int Size = 8;

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private SpriteRenderer blockPrefab;

    private SpriteRenderer[,] placedBlocks = new SpriteRenderer[Size, Size];

    private void Start()
    {
        CreateCells();
    }

    private void CreateCells()
    {
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                Vector3 position = GetCellPosition(x, y);
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                cell.name = $"Cell_{x}_{y}";
            }
        }
    }

    public Vector3 GetCellPosition(int x, int y)
    {
        float offset = (Size - 1) / 2f;
        return transform.position + new Vector3(x - offset, y - offset, 0f);
    }

    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        float offset = (Size - 1) / 2f;
        Vector3 local = worldPosition - transform.position;
        int x = Mathf.RoundToInt(local.x + offset);
        int y = Mathf.RoundToInt(local.y + offset);
        return new Vector2Int(x, y);
    }

    public bool IsInside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < Size && cell.y >= 0 && cell.y < Size;
    }

    public bool CanPlace(Vector2Int[] shape, Vector2Int origin)
    {
        foreach (Vector2Int cell in shape)
        {
            Vector2Int target = origin + cell;

            if (!IsInside(target) || placedBlocks[target.x, target.y] != null)
            {
                return false;
            }
        }

        return true;
    }

    public void Place(Vector2Int[] shape, Vector2Int origin, Color color)
    {
        foreach (Vector2Int cell in shape)
        {
            Vector2Int target = origin + cell;
            Vector3 position = GetCellPosition(target.x, target.y);

            SpriteRenderer block = Instantiate(blockPrefab, position, Quaternion.identity, transform);
            block.color = color;

            placedBlocks[target.x, target.y] = block;
        }
    }

    public int ClearFullLines()
    {
        bool[] fullRows = new bool[Size];
        bool[] fullColumns = new bool[Size];
        int lineCount = 0;

        for (int i = 0; i < Size; i++)
        {
            if (IsRowFull(i))
            {
                fullRows[i] = true;
                lineCount++;
            }

            if (IsColumnFull(i))
            {
                fullColumns[i] = true;
                lineCount++;
            }
        }

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                if (fullRows[y] || fullColumns[x])
                {
                    RemoveBlock(x, y);
                }
            }
        }

        return lineCount;
    }

    // 🆕 현재 판 상태를 bool 복사본으로 (true = 블록 있음)
    public bool[,] GetOccupancy()
    {
        bool[,] grid = new bool[Size, Size];

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                grid[x, y] = placedBlocks[x, y] != null;
            }
        }

        return grid;
    }

    private bool IsRowFull(int y)
    {
        for (int x = 0; x < Size; x++)
        {
            if (placedBlocks[x, y] == null)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsColumnFull(int x)
    {
        for (int y = 0; y < Size; y++)
        {
            if (placedBlocks[x, y] == null)
            {
                return false;
            }
        }

        return true;
    }

    private void RemoveBlock(int x, int y)
    {
        if (placedBlocks[x, y] == null)
        {
            return;
        }

        Destroy(placedBlocks[x, y].gameObject);
        placedBlocks[x, y] = null;
    }
}