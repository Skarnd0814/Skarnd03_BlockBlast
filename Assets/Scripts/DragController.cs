using UnityEngine;
using UnityEngine.InputSystem;

public class DragController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Board board;
    [SerializeField] private PieceSpawner spawner;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float pickRadius = 1.6f;
    [SerializeField] private float dragOffsetY = 1.5f;

    [Header("Sound")]
    [SerializeField] private AudioClip pickSfx;
    [SerializeField] private AudioClip placeSfx;
    [SerializeField] private AudioClip failSfx;
    [SerializeField] private AudioClip lineClearSfx;
    [SerializeField] private AudioClip[] multiLineClearSfx;

    private Piece draggingPiece;

    private void Update()
    {
        if (Pointer.current == null)
        {
            return;
        }

        Vector3 pointerWorld = GetPointerWorldPosition();

        if (Pointer.current.press.wasPressedThisFrame)
        {
            TryPickUp(pointerWorld);
        }
        else if (draggingPiece != null && Pointer.current.press.wasReleasedThisFrame)
        {
            Drop();
        }
        else if (draggingPiece != null && Pointer.current.press.isPressed)
        {
            draggingPiece.transform.position = pointerWorld + new Vector3(0f, dragOffsetY, 0f);
        }
    }

    private void OnDisable()
    {
        if (draggingPiece != null)
        {
            draggingPiece.ReturnToTray();
            draggingPiece = null;
        }
    }

    private Vector3 GetPointerWorldPosition()
    {
        Vector2 screenPosition = Pointer.current.position.ReadValue();
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;
        return worldPosition;
    }

    private void TryPickUp(Vector3 pointerWorld)
    {
        Piece piece = spawner.GetPieceAt(pointerWorld, pickRadius);

        if (piece == null)
        {
            return;
        }

        draggingPiece = piece;
        draggingPiece.BeginDrag();
        SoundManager.Instance.PlaySFX(pickSfx);
    }

    private void Drop()
    {
        Vector3 originWorld = draggingPiece.transform.position - (Vector3)draggingPiece.Center;
        Vector2Int origin = board.WorldToCell(originWorld);

        if (board.CanPlace(draggingPiece.Cells, origin))
        {
            board.Place(draggingPiece.Cells, origin, draggingPiece.Color);
            SoundManager.Instance.PlaySFX(placeSfx);

            scoreManager.AddPlacePoints(draggingPiece.Cells.Length);

            int lineCount = board.ClearFullLines();
            if (lineCount > 0)
            {
                scoreManager.AddLinePoints(lineCount);
                PlayLineClearSound(lineCount);
            }

            spawner.RemovePiece(draggingPiece);
            draggingPiece = null;

            if (!spawner.HasPlaceablePiece())
            {
                gameManager.GameOver();
            }
        }
        else
        {
            draggingPiece.ReturnToTray();
            SoundManager.Instance.PlaySFX(failSfx);
            draggingPiece = null;
        }
    }

    private void PlayLineClearSound(int lineCount)
    {
        if (lineCount >= 2 && multiLineClearSfx.Length > 0)
        {
            int index = Random.Range(0, multiLineClearSfx.Length);
            SoundManager.Instance.PlaySFX(multiLineClearSfx[index]);
        }
        else
        {
            SoundManager.Instance.PlaySFX(lineClearSfx);
        }
    }
}