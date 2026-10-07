using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public const int Size = 8;

    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private SpriteRenderer blockPrefab;

    [Header("Clear Effect")]
    [SerializeField] private float clearDuration = 0.3f;
    [SerializeField] private float clearPunchScale = 1.25f;
    [SerializeField] private float clearStaggerDelay = 0.02f;

    [Header("Intro Effect")]
    [SerializeField] private float introDropDistance = 12f;
    [SerializeField] private float introCellDuration = 0.35f;
    [SerializeField] private float introRowDelay = 0.04f;
    [SerializeField] private float introColumnDelay = 0.015f;
    [SerializeField] private AudioClip introSfx;

    [Header("Collapse Effect")]
    [SerializeField] private float collapseStartDelay = 0.3f;
    [SerializeField] private float collapseSpread = 0.5f;
    [SerializeField] private float collapseFallDuration = 1.2f;
    [SerializeField] private float collapseGravity = 35f;
    [SerializeField] private float collapseJumpPower = 5f;
    [SerializeField] private float collapseSideSpeed = 2f;
    [SerializeField] private float collapseSpin = 360f;
    [SerializeField] private AudioClip collapseSfx;

    [Header("Placement Preview")]
    [SerializeField, Range(0f, 1f)] private float previewAlpha = 0.4f;
    [SerializeField] private int previewSortingOrder = 5;

    [Header("Line Highlight")]
    [SerializeField] private float highlightPulseSpeed = 8f;
    [SerializeField, Range(0f, 1f)] private float highlightPulseStrength = 0.35f;
    [SerializeField] private bool recolorClearedLines = true;

    private SpriteRenderer[,] placedBlocks = new SpriteRenderer[Size, Size];
    private Color[,] placedColors = new Color[Size, Size];
    private Transform[] cellTransforms;
    private readonly List<SpriteRenderer> previewBlocks = new List<SpriteRenderer>();
    private readonly List<Vector2Int> highlightedCells = new List<Vector2Int>();

    private bool isPreviewVisible;
    private Vector2Int[] lastPreviewShape;
    private Vector2Int lastPreviewOrigin;
    private Color highlightColor;
    private Color lastPlacedColor = Color.white;

    public bool IsReady { get; private set; }

    private void Start()
    {
        CreateCells();
    }

    private void Update()
    {
        if (highlightedCells.Count == 0)
        {
            return;
        }

        Color pulseColor = GetPulseColor();

        foreach (Vector2Int cell in highlightedCells)
        {
            SpriteRenderer block = placedBlocks[cell.x, cell.y];

            if (block != null)
            {
                block.color = pulseColor;
            }
        }
    }

    private void CreateCells()
    {
        int count = Size * Size;
        Transform[] cells = new Transform[count];
        Vector3[] targets = new Vector3[count];
        float[] delays = new float[count];
        Vector3 dropOffset = new Vector3(0f, introDropDistance, 0f);
        int index = 0;

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                Vector3 target = GetCellPosition(x, y);
                GameObject cell = Instantiate(cellPrefab, target - dropOffset, Quaternion.identity, transform);
                cell.name = $"Cell_{x}_{y}";

                cells[index] = cell.transform;
                targets[index] = target;
                delays[index] = y * introRowDelay + x * introColumnDelay;
                index++;
            }
        }

        cellTransforms = cells;
        StartCoroutine(PlayIntro(cells, targets, delays));
    }

    private IEnumerator PlayIntro(Transform[] cells, Vector3[] targets, float[] delays)
    {
        IsReady = false;
        SoundManager.Instance.PlaySFX(introSfx);

        Vector3 dropOffset = new Vector3(0f, introDropDistance, 0f);
        float maxDelay = (Size - 1) * (introRowDelay + introColumnDelay);
        float totalDuration = maxDelay + introCellDuration;
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;

            for (int i = 0; i < cells.Length; i++)
            {
                float progress = Mathf.Clamp01((elapsed - delays[i]) / introCellDuration);
                float eased = EaseOutBack(progress);
                cells[i].position = Vector3.LerpUnclamped(targets[i] - dropOffset, targets[i], eased);
            }

            yield return null;
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].position = targets[i];
        }

        IsReady = true;
    }

    private float EaseOutBack(float t)
    {
        const float overshoot = 1.70158f;
        float shifted = t - 1f;
        return 1f + (overshoot + 1f) * shifted * shifted * shifted + overshoot * shifted * shifted;
    }

    // 🆕 게임 오버 시 판 전체가 무너짐. 전체 소요 시간을 돌려줌
    public float PlayCollapse()
    {
        List<Transform> targets = new List<Transform>(cellTransforms);

        foreach (SpriteRenderer block in placedBlocks)
        {
            if (block != null)
            {
                targets.Add(block.transform);
            }
        }

        StartCoroutine(Collapse(targets.ToArray()));
        return collapseStartDelay + collapseSpread + collapseFallDuration;
    }

    private IEnumerator Collapse(Transform[] targets)
    {
        int count = targets.Length;
        float[] delays = new float[count];
        Vector3[] velocities = new Vector3[count];
        float[] spins = new float[count];

        for (int i = 0; i < count; i++)
        {
            delays[i] = Random.Range(0f, collapseSpread);
            velocities[i] = new Vector3(
                Random.Range(-collapseSideSpeed, collapseSideSpeed),
                Random.Range(collapseJumpPower * 0.5f, collapseJumpPower),
                0f);
            spins[i] = Random.Range(-collapseSpin, collapseSpin);
        }

        if (collapseStartDelay > 0f)
        {
            yield return new WaitForSeconds(collapseStartDelay);
        }

        SoundManager.Instance.PlaySFX(collapseSfx);

        float totalDuration = collapseSpread + collapseFallDuration;
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            float deltaTime = Time.deltaTime;
            elapsed += deltaTime;

            for (int i = 0; i < count; i++)
            {
                if (elapsed < delays[i])
                {
                    continue;
                }

                velocities[i].y -= collapseGravity * deltaTime;
                targets[i].position += velocities[i] * deltaTime;
                targets[i].Rotate(0f, 0f, spins[i] * deltaTime);
            }

            yield return null;
        }
    }

    public Vector3 GetCellPosition(int x, int y)
    {
        float offset = (Size - 1) / 2f;
        return transform.position + new Vector3(x - offset, y - offset, 0f);
    }

    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        float offset = (Size - 1) / 2f;
        Vector3 local = worldPosition - transform.position;
        int x = Mathf.RoundToInt(local.x + offset);
        int y = Mathf.RoundToInt(local.y + offset);
        return new Vector2Int(x, y);
    }

    public bool IsInside(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < Size && cell.y >= 0 && cell.y < Size;
    }

    public bool CanPlace(Vector2Int[] shape, Vector2Int origin)
    {
        foreach (Vector2Int cell in shape)
        {
            Vector2Int target = origin + cell;

            if (!IsInside(target) || placedBlocks[target.x, target.y] != null)
            {
                return false;
            }
        }

        return true;
    }

    public void Place(Vector2Int[] shape, Vector2Int origin, Color color)
    {
        foreach (Vector2Int cell in shape)
        {
            Vector2Int target = origin + cell;
            Vector3 position = GetCellPosition(target.x, target.y);

            SpriteRenderer block = Instantiate(blockPrefab, position, Quaternion.identity, transform);
            block.color = color;

            placedBlocks[target.x, target.y] = block;
            placedColors[target.x, target.y] = color;
        }

        lastPlacedColor = color;
    }

    // 드래그 중인 블록이 놓일 칸을 반투명하게 표시하고, 완성될 줄을 하이라이트
    public void ShowPreview(Vector2Int[] shape, Vector2Int origin, Color color)
    {
        if (isPreviewVisible && shape == lastPreviewShape && origin == lastPreviewOrigin)
        {
            return;
        }

        isPreviewVisible = true;
        lastPreviewShape = shape;
        lastPreviewOrigin = origin;

        UpdateLineHighlight(shape, origin, color);

        color.a = previewAlpha;

        for (int i = 0; i < shape.Length; i++)
        {
            if (i >= previewBlocks.Count)
            {
                SpriteRenderer newPreview = Instantiate(blockPrefab, transform);
                newPreview.name = "Preview";
                newPreview.sortingOrder = previewSortingOrder;
                previewBlocks.Add(newPreview);
            }

            Vector2Int target = origin + shape[i];
            SpriteRenderer preview = previewBlocks[i];
            preview.transform.position = GetCellPosition(target.x, target.y);
            preview.color = color;
            preview.gameObject.SetActive(true);
        }

        for (int i = shape.Length; i < previewBlocks.Count; i++)
        {
            previewBlocks[i].gameObject.SetActive(false);
        }
    }

    public void HidePreview()
    {
        isPreviewVisible = false;
        ClearLineHighlight();

        foreach (SpriteRenderer preview in previewBlocks)
        {
            if (preview != null)
            {
                preview.gameObject.SetActive(false);
            }
        }
    }

    // 이 위치에 놓으면 완성될 줄의 블록들을 드래그 중인 블록 색으로 바꿔 깜빡임
    private void UpdateLineHighlight(Vector2Int[] shape, Vector2Int origin, Color color)
    {
        ClearLineHighlight();

        bool[,] grid = GetOccupancy();

        foreach (Vector2Int cell in shape)
        {
            grid[origin.x + cell.x, origin.y + cell.y] = true;
        }

        bool[] fullRows = new bool[Size];
        bool[] fullColumns = new bool[Size];
        bool anyLine = false;

        for (int i = 0; i < Size; i++)
        {
            fullRows[i] = true;
            fullColumns[i] = true;

            for (int j = 0; j < Size; j++)
            {
                if (!grid[j, i])
                {
                    fullRows[i] = false;
                }

                if (!grid[i, j])
                {
                    fullColumns[i] = false;
                }
            }

            anyLine |= fullRows[i] || fullColumns[i];
        }

        if (!anyLine)
        {
            return;
        }

        color.a = 1f;
        highlightColor = color;
        Color pulseColor = GetPulseColor();

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                if ((fullRows[y] || fullColumns[x]) && placedBlocks[x, y] != null)
                {
                    highlightedCells.Add(new Vector2Int(x, y));
                    placedBlocks[x, y].color = pulseColor;
                }
            }
        }
    }

    private void ClearLineHighlight()
    {
        foreach (Vector2Int cell in highlightedCells)
        {
            SpriteRenderer block = placedBlocks[cell.x, cell.y];

            if (block != null)
            {
                block.color = placedColors[cell.x, cell.y];
            }
        }

        highlightedCells.Clear();
    }

    private Color GetPulseColor()
    {
        float pulse = (Mathf.Sin(Time.time * highlightPulseSpeed) + 1f) * 0.5f;
        return Color.Lerp(highlightColor, Color.white, pulse * highlightPulseStrength);
    }

    public int ClearFullLines()
    {
        bool[] fullRows = new bool[Size];
        bool[] fullColumns = new bool[Size];
        int lineCount = 0;

        for (int i = 0; i < Size; i++)
        {
            if (IsRowFull(i))
            {
                fullRows[i] = true;
                lineCount++;
            }

            if (IsColumnFull(i))
            {
                fullColumns[i] = true;
                lineCount++;
            }
        }

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                if (fullRows[y] || fullColumns[x])
                {
                    RemoveBlock(x, y);
                }
            }
        }

        return lineCount;
    }

    public bool[,] GetOccupancy()
    {
        bool[,] grid = new bool[Size, Size];

        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                grid[x, y] = placedBlocks[x, y] != null;
            }
        }

        return grid;
    }

    private bool IsRowFull(int y)
    {
        for (int x = 0; x < Size; x++)
        {
            if (placedBlocks[x, y] == null)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsColumnFull(int x)
    {
        for (int y = 0; y < Size; y++)
        {
            if (placedBlocks[x, y] == null)
            {
                return false;
            }
        }

        return true;
    }

    private void RemoveBlock(int x, int y)
    {
        SpriteRenderer block = placedBlocks[x, y];

        if (block == null)
        {
            return;
        }

        placedBlocks[x, y] = null;

        if (recolorClearedLines)
        {
            block.color = lastPlacedColor;
        }

        float delay = (x + y) * clearStaggerDelay;
        StartCoroutine(AnimateClear(block, delay));
    }

    private IEnumerator AnimateClear(SpriteRenderer block, float delay)
    {
        block.sortingOrder = 15;

        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        Transform blockTransform = block.transform;
        Vector3 startScale = blockTransform.localScale;
        Color startColor = block.color;
        float growPortion = 0.4f;
        float elapsed = 0f;

        while (elapsed < clearDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / clearDuration);

            float scale;
            float alpha;

            if (progress < growPortion)
            {
                float growProgress = progress / growPortion;
                scale = Mathf.Lerp(1f, clearPunchScale, growProgress);
                alpha = 1f;
            }
            else
            {
                float shrinkProgress = (progress - growPortion) / (1f - growPortion);
                scale = Mathf.Lerp(clearPunchScale, 0f, shrinkProgress);
                alpha = 1f - shrinkProgress;
            }

            blockTransform.localScale = startScale * scale;

            Color color = startColor;
            color.a = alpha;
            block.color = color;

            yield return null;
        }

        Destroy(block.gameObject);
    }
}