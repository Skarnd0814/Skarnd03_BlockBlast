using UnityEngine;

public class Board : MonoBehaviour
{
    public const int Size = 8;

    [SerializeField] private GameObject cellPrefab;

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
}
