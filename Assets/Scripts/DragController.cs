using UnityEngine;
using UnityEngine.InputSystem;

public class DragController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Board board;
    [SerializeField] private PieceSpawner spawner;
    [SerializeField] private float pickRadius = 1.6f;
    [SerializeField] private float dragOffsetY = 1.5f;

    [Header("Sound")]
    [SerializeField] private AudioClip pickSfx;
    [SerializeField] private AudioClip placeSfx;
    [SerializeField] private AudioClip failSfx;

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
            spawner.RemovePiece(draggingPiece);
            SoundManager.Instance.PlaySFX(placeSfx);
        }
        else
        {
            draggingPiece.ReturnToTray();
            SoundManager.Instance.PlaySFX(failSfx);
        }

        draggingPiece = null;
    }
}