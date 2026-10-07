using UnityEngine;

public static class BlockShapes
{
    public static readonly Vector2Int[][] All =
    {
        // 1칸
        new Vector2Int[] { new(0, 0) },

        // 일자 (가로)
        new Vector2Int[] { new(0, 0), new(1, 0) },
        new Vector2Int[] { new(0, 0), new(1, 0), new(2, 0) },
        new Vector2Int[] { new(0, 0), new(1, 0), new(2, 0), new(3, 0) },
        new Vector2Int[] { new(0, 0), new(1, 0), new(2, 0), new(3, 0), new(4, 0) },

        // 일자 (세로)
        new Vector2Int[] { new(0, 0), new(0, 1) },
        new Vector2Int[] { new(0, 0), new(0, 1), new(0, 2) },
        new Vector2Int[] { new(0, 0), new(0, 1), new(0, 2), new(0, 3) },
        new Vector2Int[] { new(0, 0), new(0, 1), new(0, 2), new(0, 3), new(0, 4) },

        // 네모
        new Vector2Int[] { new(0, 0), new(1, 0), new(0, 1), new(1, 1) },
        new Vector2Int[] { new(0, 0), new(1, 0), new(2, 0),
                           new(0, 1), new(1, 1), new(2, 1),
                           new(0, 2), new(1, 2), new(2, 2) },

        // 작은 L (3칸)
        new Vector2Int[] { new(0, 0), new(1, 0), new(0, 1) },
        new Vector2Int[] { new(0, 0), new(1, 0), new(1, 1) },
        new Vector2Int[] { new(0, 0), new(0, 1), new(1, 1) },
        new Vector2Int[] { new(1, 0), new(0, 1), new(1, 1) },

        // L (4칸)
        new Vector2Int[] { new(0, 0), new(1, 0), new(0, 1), new(0, 2) },
        new Vector2Int[] { new(0, 0), new(1, 0), new(1, 1), new(1, 2) },
        new Vector2Int[] { new(0, 0), new(1, 0), new(2, 0), new(2, 1) },
        new Vector2Int[] { new(0, 0), new(0, 1), new(1, 1), new(2, 1) },

        // T
        new Vector2Int[] { new(0, 0), new(1, 0), new(2, 0), new(1, 1) },
        new Vector2Int[] { new(1, 0), new(0, 1), new(1, 1), new(2, 1) },

        // S / Z
        new Vector2Int[] { new(0, 0), new(1, 0), new(1, 1), new(2, 1) },
        new Vector2Int[] { new(1, 0), new(2, 0), new(0, 1), new(1, 1) },
    };

    public static Vector2Int[] GetRandom()
    {
        int index = Random.Range(0, All.Length);
        return All[index];
    }
}