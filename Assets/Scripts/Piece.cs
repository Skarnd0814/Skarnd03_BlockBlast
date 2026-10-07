using System.Collections;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private SpriteRenderer blockPrefab;

    [Header("Feel")]
    [Tooltip("집었을 때 트레이 크기에서 원래 크기로 커지는 시간(초)")]
    [SerializeField] private float pickUpDuration = 0.08f;
    [Tooltip("놓기 실패 시 트레이로 돌아가는 시간(초)")]
    [SerializeField] private float returnDuration = 0.15f;

    public Vector2Int[] Cells { get; private set; }
    public Color Color { get; private set; }
    public Vector2 Center { get; private set; }

    private Vector3 trayScale;
    private bool hasTrayScale;
    private Coroutine motionRoutine;

    public void Setup(Vector2Int[] shape, Color color)
    {
        Cells = shape;
        Color = color;
        Center = GetCenter();

        foreach (Vector2Int cell in Cells)
        {
            SpriteRenderer block = Instantiate(blockPrefab, transform);
            block.transform.localPosition = new Vector3(cell.x - Center.x, cell.y - Center.y, 0f);
            block.color = color;
        }
    }

    // 집었을 때: 짧게 커지면서 앞으로 나옴 (위치는 DragController가 손가락을 따라 매 프레임 지정)
    public void BeginDrag()
    {
        if (!hasTrayScale)
        {
            trayScale = transform.localScale;
            hasTrayScale = true;
        }

        StopMotion();
        SetSortingOrder(20);
        motionRoutine = StartCoroutine(AnimateScale(transform.localScale, Vector3.one, pickUpDuration));
    }

    // 트레이로 돌아갈 때: 현재 위치에서 제자리로 부드럽게 미끄러지며 작아짐
    public void ReturnToTray()
    {
        StopMotion();

        if (!gameObject.activeInHierarchy)
        {
            transform.localPosition = Vector3.zero;
            transform.localScale = trayScale;
            SetSortingOrder(10);
            return;
        }

        motionRoutine = StartCoroutine(AnimateReturn(returnDuration));
    }

    private void StopMotion()
    {
        if (motionRoutine != null)
        {
            StopCoroutine(motionRoutine);
            motionRoutine = null;
        }
    }

    private IEnumerator AnimateScale(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            transform.localScale = Vector3.Lerp(from, to, EaseOutCubic(progress));
            yield return null;
        }

        transform.localScale = to;
        motionRoutine = null;
    }

    private IEnumerator AnimateReturn(float duration)
    {
        Vector3 startPosition = transform.localPosition;
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float smooth = EaseOutCubic(Mathf.Clamp01(elapsed / duration));
            transform.localPosition = Vector3.Lerp(startPosition, Vector3.zero, smooth);
            transform.localScale = Vector3.Lerp(startScale, trayScale, smooth);
            yield return null;
        }

        transform.localPosition = Vector3.zero;
        transform.localScale = trayScale;
        SetSortingOrder(10);
        motionRoutine = null;
    }

    private float EaseOutCubic(float t)
    {
        float inverse = 1f - t;
        return 1f - inverse * inverse * inverse;
    }

    private void SetSortingOrder(int order)
    {
        foreach (SpriteRenderer block in GetComponentsInChildren<SpriteRenderer>())
        {
            block.sortingOrder = order;
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
