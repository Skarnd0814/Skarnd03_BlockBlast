using System.Collections.Generic;
using UnityEngine;

public static class PlacementSolver
{
    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    public static bool CanPlace(bool[,] grid, Vector2Int[] shape, Vector2Int origin)
    {
        int size = grid.GetLength(0);

        foreach (Vector2Int cell in shape)
        {
            int x = origin.x + cell.x;
            int y = origin.y + cell.y;

            if (x < 0 || x >= size || y < 0 || y >= size || grid[x, y])
            {
                return false;
            }
        }

        return true;
    }

    public static bool HasAnyPlacement(bool[,] grid, Vector2Int[] shape)
    {
        int size = grid.GetLength(0);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (CanPlace(grid, shape, new Vector2Int(x, y)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static List<Vector2Int> GetAllPlacements(bool[,] grid, Vector2Int[] shape)
    {
        List<Vector2Int> placements = new List<Vector2Int>();
        int size = grid.GetLength(0);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2Int origin = new Vector2Int(x, y);

                if (CanPlace(grid, shape, origin))
                {
                    placements.Add(origin);
                }
            }
        }

        return placements;
    }

    public static int PlaceAndClear(bool[,] grid, Vector2Int[] shape, Vector2Int origin)
    {
        int size = grid.GetLength(0);

        foreach (Vector2Int cell in shape)
        {
            grid[origin.x + cell.x, origin.y + cell.y] = true;
        }

        bool[] fullRows = new bool[size];
        bool[] fullColumns = new bool[size];
        int lineCount = 0;

        for (int i = 0; i < size; i++)
        {
            fullRows[i] = true;
            fullColumns[i] = true;

            for (int j = 0; j < size; j++)
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

            if (fullRows[i])
            {
                lineCount++;
            }

            if (fullColumns[i])
            {
                lineCount++;
            }
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (fullRows[y] || fullColumns[x])
                {
                    grid[x, y] = false;
                }
            }
        }

        return lineCount;
    }

    // 🆕 이어진 빈칸 덩어리 중 maxSize 이하인 것들 (= 구멍)
    public static List<Vector2Int[]> FindEmptyRegions(bool[,] grid, int maxSize)
    {
        int size = grid.GetLength(0);
        bool[,] visited = new bool[size, size];
        List<Vector2Int[]> regions = new List<Vector2Int[]>();

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (grid[x, y] || visited[x, y])
                {
                    continue;
                }

                List<Vector2Int> cells = new List<Vector2Int>();
                Stack<Vector2Int> stack = new Stack<Vector2Int>();

                stack.Push(new Vector2Int(x, y));
                visited[x, y] = true;

                while (stack.Count > 0)
                {
                    Vector2Int current = stack.Pop();
                    cells.Add(current);

                    foreach (Vector2Int direction in Directions)
                    {
                        Vector2Int next = current + direction;

                        if (next.x < 0 || next.x >= size || next.y < 0 || next.y >= size)
                        {
                            continue;
                        }

                        if (grid[next.x, next.y] || visited[next.x, next.y])
                        {
                            continue;
                        }

                        visited[next.x, next.y] = true;
                        stack.Push(next);
                    }
                }

                if (cells.Count <= maxSize)
                {
                    regions.Add(cells.ToArray());
                }
            }
        }

        return regions;
    }

    // 🆕 구멍과 도형이 정확히 같은 모양인지 (같으면 놓을 위치도 알려줌)
    public static bool TryMatchRegion(Vector2Int[] region, Vector2Int[] shape, out Vector2Int origin)
    {
        origin = Vector2Int.zero;

        if (region.Length != shape.Length)
        {
            return false;
        }

        int minX = int.MaxValue;
        int minY = int.MaxValue;

        foreach (Vector2Int cell in region)
        {
            minX = Mathf.Min(minX, cell.x);
            minY = Mathf.Min(minY, cell.y);
        }

        foreach (Vector2Int cell in shape)
        {
            Vector2Int target = new Vector2Int(minX + cell.x, minY + cell.y);

            if (!Contains(region, target))
            {
                return false;
            }
        }

        origin = new Vector2Int(minX, minY);
        return true;
    }

    private static bool Contains(Vector2Int[] cells, Vector2Int target)
    {
        foreach (Vector2Int cell in cells)
        {
            if (cell == target)
            {
                return true;
            }
        }

        return false;
    }
}