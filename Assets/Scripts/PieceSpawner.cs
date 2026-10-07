using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private Piece piecePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Color[] colors;
    [SerializeField] private float trayScale = 0.55f;

    [Header("Difficulty")]
    [SerializeField, Range(0f, 1f)] private float guaranteeChance = 1f;
    [SerializeField] private float bigPieceWeight = 1f;
    [SerializeField] private int maxAttempts = 30;

    private Piece[] pieces;

    private void Start()
    {
        SpawnPieces();
    }

    public void SpawnPieces()
    {
        Vector2Int[][] shapes = ChooseShapes();
        pieces = new Piece[spawnPoints.Length];

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform point = spawnPoints[i];
            Piece piece = Instantiate(piecePrefab, point.position, Quaternion.identity, point);

            Color color = colors[Random.Range(0, colors.Length)];
            piece.Setup(shapes[i], color);

            piece.transform.localScale = Vector3.one * trayScale;

            pieces[i] = piece;
        }
    }

    private Vector2Int[][] ChooseShapes()
    {
        bool useGuarantee = Random.value < guaranteeChance;
        bool[,] grid = board.GetOccupancy();
        Vector2Int[][] shapes = new Vector2Int[spawnPoints.Length][];

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            for (int i = 0; i < shapes.Length; i++)
            {
                shapes[i] = GetWeightedRandomShape();
            }

            if (!useGuarantee || PlacementSolver.CanPlaceAll(grid, shapes))
            {
                return shapes;
            }
        }

        return shapes;
    }

    private Vector2Int[] GetWeightedRandomShape()
    {
        float totalWeight = 0f;

        foreach (Vector2Int[] shape in BlockShapes.All)
        {
            totalWeight += GetWeight(shape);
        }

        float pick = Random.Range(0f, totalWeight);

        foreach (Vector2Int[] shape in BlockShapes.All)
        {
            pick -= GetWeight(shape);

            if (pick <= 0f)
            {
                return shape;
            }
        }

        return BlockShapes.All[BlockShapes.All.Length - 1];
    }

    private float GetWeight(Vector2Int[] shape)
    {
        return shape.Length >= 5 ? bigPieceWeight : 1f;
    }

    public Piece GetPieceAt(Vector3 worldPosition, float radius)
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i] == null)
            {
                continue;
            }

            float distance = Vector2.Distance(worldPosition, spawnPoints[i].position);

            if (distance <= radius)
            {
                return pieces[i];
            }
        }

        return null;
    }

    public void RemovePiece(Piece piece)
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i] == piece)
            {
                pieces[i] = null;
            }
        }

        Destroy(piece.gameObject);

        if (IsTrayEmpty())
        {
            SpawnPieces();
        }
    }

    public bool HasPlaceablePiece()
    {
        bool[,] grid = board.GetOccupancy();

        foreach (Piece piece in pieces)
        {
            if (piece != null && PlacementSolver.HasAnyPlacement(grid, piece.Cells))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTrayEmpty()
    {
        foreach (Piece piece in pieces)
        {
            if (piece != null)
            {
                return false;
            }
        }

        return true;
    }
}