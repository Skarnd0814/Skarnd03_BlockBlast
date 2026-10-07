using UnityEngine;

// 화면 비율이 달라도 지정한 영역(가로/세로)이 항상 화면 안에 들어오도록 카메라 크기를 자동 조절
[RequireComponent(typeof(Camera))]
public class CameraFitter : MonoBehaviour
{
    [Tooltip("항상 화면에 보여야 하는 최소 가로 폭 (유닛). 판 테두리 + 양옆 여백")]
    [SerializeField] private float minVisibleWidth = 9.6f;
    [Tooltip("항상 화면에 보여야 하는 최소 세로 높이 (유닛). 기존 카메라 Size 8.5 × 2")]
    [SerializeField] private float minVisibleHeight = 17f;
    [Tooltip("화면을 빈틈없이 덮도록 크기를 맞출 배경 (선택)")]
    [SerializeField] private SpriteRenderer background;

    private Camera targetCamera;
    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
        Fit();
    }

    private void Update()
    {
        // 에디터에서 Game 창 비율을 바꾸거나 폰을 회전하는 경우를 대비해 해상도 변화 감지
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            Fit();
        }
    }

    private void Fit()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        float aspect = targetCamera.aspect;
        float sizeForHeight = minVisibleHeight / 2f;
        float sizeForWidth = minVisibleWidth / (2f * aspect);

        targetCamera.orthographicSize = Mathf.Max(sizeForHeight, sizeForWidth);

        FitBackground();
    }

    private void FitBackground()
    {
        if (background == null || background.sprite == null)
        {
            return;
        }

        float viewHeight = targetCamera.orthographicSize * 2f;
        float viewWidth = viewHeight * targetCamera.aspect;
        Vector2 spriteSize = background.sprite.bounds.size;

        float scale = Mathf.Max(viewWidth / spriteSize.x, viewHeight / spriteSize.y);
        background.transform.localScale = new Vector3(scale, scale, 1f);

        Vector3 cameraPosition = transform.position;
        background.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, background.transform.position.z);
    }
}
