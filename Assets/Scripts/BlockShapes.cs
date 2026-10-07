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

        // 네모 2x2
        new Vector2Int[] { new(0, 0), new(1, 0), new(0, 1), new(1, 1) },

        // 네모 3x3
        new Vector2Int[] { new(0, 0), new(1, 0), new(2, 0),
                           new(0, 1), new(1, 1), new(2, 1),
                           new(0, 2), new(1, 2), new(2, 2) },

        // 직사각형 2x3 (가로 2, 세로 3)
        new Vector2Int[] { new(0, 0), new(1, 0),
                           new(0, 1), new(1, 1),
                           new(0, 2), new(1, 2) },

        // 직사각형 3x2 (가로 3, 세로 2)
        new Vector2Int[] { new(0, 0), new(1, 0), new(2, 0),
                           new(0, 1), new(1, 1), new(2, 1) },

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

    // All과 같은 순서의 이름 (Inspector의 모양별 가중치 목록에 표시)
    public static readonly string[] Names =
    {
        "1칸",
        "가로 2칸", "가로 3칸", "가로 4칸 (1x4)", "가로 5칸 (1x5)",
        "세로 2칸", "세로 3칸", "세로 4칸 (4x1)", "세로 5칸 (5x1)",
        "네모 2x2",
        "네모 3x3",
        "직사각형 2x3",
        "직사각형 3x2",
        "작은 L ┗", "작은 L ┛", "작은 L ┏", "작은 L ┓",
        "L ┗ (세로)", "L ┛ (세로)", "L ━┛ (가로)", "L ┏━ (가로)",
        "T ┻", "T ┳",
        "S", "Z",
    };

    // All과 같은 순서의 기본 출현 가중치 (클수록 자주 나옴). 큰 블록 위주로 터뜨리는 손맛을 살리는 값
    public static readonly float[] DefaultWeights =
    {
        1f,
        1f, 1f, 3f, 3f,
        1f, 1f, 3f, 3f,
        2f,
        3f,
        3f,
        3f,
        1f, 1f, 1f, 1f,
        1.5f, 1.5f, 1.5f, 1.5f,
        1.5f, 1.5f,
        1.5f, 1.5f,
    };

    public static Vector2Int[] GetRandom()
    {
        int index = Random.Range(0, All.Length);
        return All[index];
    }
}