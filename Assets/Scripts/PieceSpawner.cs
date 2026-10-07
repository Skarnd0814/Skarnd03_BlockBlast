using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [SerializeField] private Piece piecePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Color[] colors;
    [SerializeField] private float trayScale = 0.55f;

    private Piece[] pieces;                                  // 🆕

    private void Start()
    {
        SpawnPieces();
    }

    public void SpawnPieces()
    {
        pieces = new Piece[spawnPoints.Length];              // 🆕

        for (int i = 0; i < spawnPoints.Length; i++)         // 🆕 foreach → for
        {
            Transform point = spawnPoints[i];
            Piece piece = Instantiate(piecePrefab, point.position, Quaternion.identity, point);

            Vector2Int[] shape = BlockShapes.GetRandom();
            Color color = colors[Random.Range(0, colors.Length)];
            piece.Setup(shape, color);

            piece.transform.localScale = Vector3.one * trayScale;

            pieces[i] = piece;                               // 🆕
        }
    }

    // 🆕 위치 근처의 블록 찾기
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

    // 🆕 판에 놓은 블록 치우기
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

    // 🆕 트레이가 비었는지
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