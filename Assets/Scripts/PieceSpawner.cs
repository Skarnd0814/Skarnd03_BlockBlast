using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    private const int MaxHoleSize = 9;

    [SerializeField] private Board board;
    [SerializeField] private Piece piecePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Color[] colors;
    [SerializeField] private float trayScale = 0.55f;

    [Header("Difficulty")]
    [SerializeField, Range(0f, 1f)] private float guaranteeChance = 1f;
    [SerializeField] private float bigPieceWeight = 1f;
    [SerializeField, Range(0f, 1f)] private float lineClearBias = 0.5f;
    [SerializeField] private float holeFitWeight = 5f;

    [Header("Spawn Effect")]
    [SerializeField] private float spawnDuration = 0.3f;
    [SerializeField] private float spawnStagger = 0.08f;
    [SerializeField] private AudioClip spawnSfx;

    private Piece[] pieces;
    private bool isSpawnAnimating;

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

            piece.transform.localScale = Vector3.zero;

            pieces[i] = piece;
        }

        StartCoroutine(AnimateSpawn());
    }

    // 왼쪽부터 차례로 톡 튀어나오며 트레이 크기까지 커짐 (게임 시작 시에는 판 채우기가 끝난 뒤 등장)
    private IEnumerator AnimateSpawn()
    {
        isSpawnAnimating = true;

        if (!board.IsReady)
        {
            yield return new WaitUntil(() => board.IsReady);
        }

        SoundManager.Instance.PlaySFX(spawnSfx);

        float totalDuration = spawnStagger * (pieces.Length - 1) + spawnDuration;
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;

            for (int i = 0; i < pieces.Length; i++)
            {
                if (pieces[i] == null)
                {
                    continue;
                }

                float progress = Mathf.Clamp01((elapsed - i * spawnStagger) / spawnDuration);
                pieces[i].transform.localScale = Vector3.one * (trayScale * EaseOutBack(progress));
            }

            yield return null;
        }

        foreach (Piece piece in pieces)
        {
            if (piece != null)
            {
                piece.transform.localScale = Vector3.one * trayScale;
            }
        }

        isSpawnAnimating = false;
    }

    private float EaseOutBack(float t)
    {
        const float overshoot = 1.70158f;
        float shifted = t - 1f;
        return 1f + (overshoot + 1f) * shifted * shifted * shifted + overshoot * shifted * shifted;
    }

    private Vector2Int[][] ChooseShapes()
    {
        Vector2Int[][] shapes = new Vector2Int[spawnPoints.Length][];
        List<Vector2Int[]> allShapes = new List<Vector2Int[]>(BlockShapes.All);
        bool[,] grid = board.GetOccupancy();

        if (Random.value >= guaranteeChance)
        {
            List<Vector2Int[]> holes = PlacementSolver.FindEmptyRegions(grid, MaxHoleSize);

            for (int i = 0; i < shapes.Length; i++)
            {
                shapes[i] = PickWeighted(allShapes, holes);
            }

            return shapes;
        }

        for (int i = 0; i < shapes.Length; i++)
        {
            List<Vector2Int[]> holes = PlacementSolver.FindEmptyRegions(grid, MaxHoleSize);
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
                shapes[i] = PickWeighted(allShapes, holes);
                continue;
            }

            Vector2Int[] chosen = PickWeighted(candidates, holes);
            shapes[i] = chosen;

            Vector2Int origin = ChoosePlacement(grid, chosen, holes);
            PlacementSolver.PlaceAndClear(grid, chosen, origin);
        }

        Shuffle(shapes);
        return shapes;
    }

    private Vector2Int ChoosePlacement(bool[,] grid, Vector2Int[] shape, List<Vector2Int[]> holes)
    {
        if (FindHoleOrigin(shape, holes, out Vector2Int holeOrigin))
        {
            return holeOrigin;
        }

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

    private bool FindHoleOrigin(Vector2Int[] shape, List<Vector2Int[]> holes, out Vector2Int origin)
    {
        foreach (Vector2Int[] hole in holes)
        {
            if (PlacementSolver.TryMatchRegion(hole, shape, out origin))
            {
                return true;
            }
        }

        origin = Vector2Int.zero;
        return false;
    }

    private Vector2Int[] PickWeighted(List<Vector2Int[]> candidates, List<Vector2Int[]> holes)
    {
        float totalWeight = 0f;

        foreach (Vector2Int[] shape in candidates)
        {
            totalWeight += GetWeight(shape, holes);
        }

        float pick = Random.Range(0f, totalWeight);

        foreach (Vector2Int[] shape in candidates)
        {
            pick -= GetWeight(shape, holes);

            if (pick <= 0f)
            {
                return shape;
            }
        }

        return candidates[candidates.Count - 1];
    }

    private float GetWeight(Vector2Int[] shape, List<Vector2Int[]> holes)
    {
        float weight = shape.Length >= 5 ? bigPieceWeight : 1f;

        if (FindHoleOrigin(shape, holes, out _))
        {
            weight *= holeFitWeight;
        }

        return weight;
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
        if (isSpawnAnimating)
        {
            return null;
        }

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