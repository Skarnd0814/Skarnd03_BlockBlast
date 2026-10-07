using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [SerializeField] private Piece piecePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Color[] colors;
    [SerializeField] private float trayScale = 0.55f;

    private void Start()
    {
        SpawnPieces();
    }

    public void SpawnPieces()
    {
        foreach (Transform point in spawnPoints)
        {
            Piece piece = Instantiate(piecePrefab, point.position, Quaternion.identity, point);

            Vector2Int[] shape = BlockShapes.GetRandom();
            Color color = colors[Random.Range(0, colors.Length)];
            piece.Setup(shape, color);

            piece.transform.localScale = Vector3.one * trayScale;
        }
    }
}