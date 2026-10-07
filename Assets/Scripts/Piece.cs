using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private SpriteRenderer blockPrefab;

    public Vector2Int[] Cells { get; private set; }
    public Color Color { get; private set; }

    public void Setup(Vector2Int[] shape, Color color)
    {
        Cells = shape;
        Color = color;

        Vector2 center = GetCenter();

        foreach (Vector2Int cell in Cells)
        {
            SpriteRenderer block = Instantiate(blockPrefab, transform);
            block.transform.localPosition = new Vector3(cell.x - center.x, cell.y - center.y, 0f);
            block.color = color;
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