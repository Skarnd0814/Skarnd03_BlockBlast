using UnityEngine;

public static class PlacementSolver
{
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

    public static bool CanPlaceAll(bool[,] grid, Vector2Int[][] shapes)
    {
        bool[] used = new bool[shapes.Length];
        return CanPlaceRemaining(grid, shapes, used, 0);
    }

    private static bool CanPlaceRemaining(bool[,] grid, Vector2Int[][] shapes, bool[] used, int placedCount)
    {
        if (placedCount == shapes.Length)
        {
            return true;
        }

        int size = grid.GetLength(0);

        for (int i = 0; i < shapes.Length; i++)
        {
            if (used[i])
            {
                continue;
            }

            used[i] = true;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2Int origin = new Vector2Int(x, y);

                    if (!CanPlace(grid, shapes[i], origin))
                    {
                        continue;
                    }

                    bool[,] nextGrid = (bool[,])grid.Clone();
                    PlaceAndClear(nextGrid, shapes[i], origin);

                    if (CanPlaceRemaining(nextGrid, shapes, used, placedCount + 1))
                    {
                        used[i] = false;
                        return true;
                    }
                }
            }

            used[i] = false;
        }

        return false;
    }

    private static void PlaceAndClear(bool[,] grid, Vector2Int[] shape, Vector2Int origin)
    {
        int size = grid.GetLength(0);

        foreach (Vector2Int cell in shape)
        {
            grid[origin.x + cell.x, origin.y + cell.y] = true;
        }

        bool[] fullRows = new bool[size];
        bool[] fullColumns = new bool[size];

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
    }
}