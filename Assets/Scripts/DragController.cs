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
    [Tooltip("손가락과 들고 있는 블록의 '아래쪽 끝' 사이 간격 (유닛). 블록이 클수록 자동으로 더 위에 표시됨")]
    [SerializeField] private float dragOffsetY = 1.5f;
    [Tooltip("손가락 이동량 대비 블록 이동량 배율. 1 = 손가락과 똑같이, 1.2 = 20% 더 많이 움직임 (손가락을 덜 움직여도 됨)")]
    [SerializeField] private float dragSensitivity = 1f;

    [Header("Combo")]
    [SerializeField] private ComboDisplay comboDisplay;
    [Tooltip("줄을 지우지 못한 배치가 이 횟수만큼 연속되면 콤보가 끊김")]
    [SerializeField] private int comboKeepMoves = 3;

    [Header("Sound")]
    [SerializeField] private AudioClip pickSfx;
    [SerializeField] private AudioClip placeSfx;
    [SerializeField] private AudioClip failSfx;
    [SerializeField] private AudioClip lineClearSfx;
    [SerializeField] private AudioClip[] multiLineClearSfx;

    private Piece draggingPiece;
    private Vector3 pickPointerPosition;
    private int combo;
    private int movesWithoutClear;

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
            MoveDraggingPiece(pointerWorld);
            UpdatePreview();
        }
    }

    private void OnDisable()
    {
        if (board != null)
        {
            board.HidePreview();
        }

        if (draggingPiece != null)
        {
            draggingPiece.ReturnToTray();
            draggingPiece = null;
        }
    }

    private Vector2Int GetDropOrigin()
    {
        Vector3 originWorld = draggingPiece.transform.position - (Vector3)draggingPiece.Center;
        return board.WorldToCell(originWorld);
    }

    private void UpdatePreview()
    {
        Vector2Int origin = GetDropOrigin();

        if (board.CanPlace(draggingPiece.Cells, origin))
        {
            board.ShowPreview(draggingPiece.Cells, origin, draggingPiece.Color);
        }
        else
        {
            board.HidePreview();
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
        if (!board.IsReady)
        {
            return;
        }

        Piece piece = spawner.GetPieceAt(pointerWorld, pickRadius);

        if (piece == null)
        {
            return;
        }

        draggingPiece = piece;
        pickPointerPosition = pointerWorld;
        draggingPiece.BeginDrag();
        MoveDraggingPiece(pointerWorld);
        SoundManager.Instance.PlaySFX(pickSfx);
    }

    // 블록의 아래쪽 끝이 손가락보다 dragOffsetY만큼 위에 오도록 배치 (세로로 긴 블록도 가려지지 않음)
    private void MoveDraggingPiece(Vector3 pointerWorld)
    {
        Vector3 movedPointer = pickPointerPosition + (pointerWorld - pickPointerPosition) * dragSensitivity;
        float halfHeight = draggingPiece.Center.y + 0.5f;
        draggingPiece.transform.position = movedPointer + new Vector3(0f, dragOffsetY + halfHeight, 0f);
    }

    private void Drop()
    {
        board.HidePreview();
        Vector2Int origin = GetDropOrigin();

        if (board.CanPlace(draggingPiece.Cells, origin))
        {
            board.Place(draggingPiece.Cells, origin, draggingPiece.Color);
            SoundManager.Instance.PlaySFX(placeSfx);

            scoreManager.AddPlacePoints(draggingPiece.Cells.Length);

            int lineCount = board.ClearFullLines();
            UpdateCombo(lineCount);

            if (lineCount > 0)
            {
                scoreManager.AddLinePoints(lineCount, combo);
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

    // 줄을 지울 때마다 콤보 +1, 지우지 못한 배치가 comboKeepMoves번 연속되면 초기화
    private void UpdateCombo(int lineCount)
    {
        if (lineCount > 0)
        {
            combo++;
            movesWithoutClear = 0;

            if (comboDisplay != null)
            {
                comboDisplay.Show(lineCount, combo);
            }
        }
        else
        {
            movesWithoutClear++;

            if (movesWithoutClear >= comboKeepMoves)
            {
                combo = 0;
            }
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