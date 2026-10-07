using UnityEngine;

public class Board : MonoBehaviour
{
    public const int Size = 8;

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private SpriteRenderer blockPrefab;                   // 🆕

    private SpriteRenderer[,] placedBlocks = new SpriteRenderer[Size, Size]; // 🆕

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

    // 🆕 월드 위치 → 칸 번호
    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        float offset = (Size - 1) / 2f;
        Vector3 local = worldPosition - transform.position;
        int x = Mathf.RoundToInt(local.x + offset);
        int y = Mathf.RoundToInt(local.y + offset);
        return new Vector2Int(x, y);
    }

    // 🆕 칸 번호가 판 안쪽인지
    public bool IsInside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < Size && cell.y >= 0 && cell.y < Size;
    }

    // 🆕 이 모양을 이 위치에 놓을 수 있는지
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

    // 🆕 판에 블록 고정하기
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
    // 🆕 꽉 찬 줄을 모두 지우고, 지운 줄 수를 돌려줌
    
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

    // 🆕 가로줄(y)이 꽉 찼는지
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

    // 🆕 세로줄(x)이 꽉 찼는지
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

    // 🆕 한 칸의 블록 삭제
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