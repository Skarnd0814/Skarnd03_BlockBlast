using System.Collections.Generic;
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
    [SerializeField, Range(0f, 1f)] private float lineClearBias = 0.5f;

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
        Vector2Int[][] shapes = new Vector2Int[spawnPoints.Length][];
        List<Vector2Int[]> allShapes = new List<Vector2Int[]>(BlockShapes.All);

        if (Random.value >= guaranteeChance)
        {
            for (int i = 0; i < shapes.Length; i++)
            {
                shapes[i] = PickWeighted(allShapes);
            }

            return shapes;
        }

        bool[,] grid = board.GetOccupancy();

        for (int i = 0; i < shapes.Length; i++)
        {
            List<Vector2Int[]> candidates = new List<Vector2Int[]>();

            foreach (Vector2Int[] shape in BlockShapes.All)
            {
                if (PlacementSolver.HasAnyPlacement(grid, shape))
                {
                    candidates.Add(shape);
                }
            }

            if (candidates.Count == 0)
            {
                shapes[i] = PickWeighted(allShapes);
                continue;
            }

            Vector2Int[] chosen = PickWeighted(candidates);
            shapes[i] = chosen;

            Vector2Int origin = ChoosePlacement(grid, chosen);
            PlacementSolver.PlaceAndClear(grid, chosen, origin);
        }

        Shuffle(shapes);
        return shapes;
    }

    private Vector2Int ChoosePlacement(bool[,] grid, Vector2Int[] shape)
    {
        List<Vector2Int> placements = PlacementSolver.GetAllPlacements(grid, shape);

        if (Random.value < lineClearBias)
        {
            List<Vector2Int> clearingPlacements = new List<Vector2Int>();

            foreach (Vector2Int placement in placements)
            {
                bool[,] testGrid = (bool[,])grid.Clone();

                if (PlacementSolver.PlaceAndClear(testGrid, shape, placement) > 0)
                {
                    clearingPlacements.Add(placement);
                }
            }

            if (clearingPlacements.Count > 0)
            {
                return clearingPlacements[Random.Range(0, clearingPlacements.Count)];
            }
        }

        return placements[Random.Range(0, placements.Count)];
    }

    private Vector2Int[] PickWeighted(List<Vector2Int[]> candidates)
    {
        float totalWeight = 0f;

        foreach (Vector2Int[] shape in candidates)
        {
            totalWeight += GetWeight(shape);
        }

        float pick = Random.Range(0f, totalWeight);

        foreach (Vector2Int[] shape in candidates)
        {
            pick -= GetWeight(shape);

            if (pick <= 0f)
            {
                return shape;
            }
        }

        return candidates[candidates.Count - 1];
    }

    private float GetWeight(Vector2Int[] shape)
    {
        return shape.Length >= 5 ? bigPieceWeight : 1f;
    }

    private void Shuffle(Vector2Int[][] shapes)
    {
        for (int i = shapes.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Vector2Int[] temp = shapes[i];
            shapes[i] = shapes[j];
            shapes[j] = temp;
        }
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