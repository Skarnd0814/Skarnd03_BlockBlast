using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private SpriteRenderer blockPrefab;

    public Vector2Int[] Cells { get; private set; }
    public Color Color { get; private set; }
    public Vector2 Center { get; private set; }   // 🆕

    private Vector3 trayScale;                    // 🆕

    public void Setup(Vector2Int[] shape, Color color)
    {
        Cells = shape;
        Color = color;
        Center = GetCenter();                     // 🆕 (변수 → 속성으로 저장)

        foreach (Vector2Int cell in Cells)
        {
            SpriteRenderer block = Instantiate(blockPrefab, transform);
            block.transform.localPosition = new Vector3(cell.x - Center.x, cell.y - Center.y, 0f);
            block.color = color;
        }
    }

    // 🆕 집었을 때
    public void BeginDrag()
    {
        trayScale = transform.localScale;
        transform.localScale = Vector3.one;
        SetSortingOrder(20);
    }

    // 🆕 트레이로 돌아갈 때
    public void ReturnToTray()
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = trayScale;
        SetSortingOrder(10);
    }

    // 🆕 모든 블록의 그리는 순서 바꾸기
    private void SetSortingOrder(int order)
    {
        foreach (SpriteRenderer block in GetComponentsInChildren<SpriteRenderer>())
        {
            block.sortingOrder = order;
        }
    }

    private Vector2 GetCenter()
    {
        int maxX = 0;
        int maxY = 0;

        foreach (Vector2Int cell in Cells)
        {
            maxX = Mathf.Max(maxX, cell.x);
            maxY = Mathf.Max(maxY, cell.y);
        }

        return new Vector2(maxX / 2f, maxY / 2f);
    }
}