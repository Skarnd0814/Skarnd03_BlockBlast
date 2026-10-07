using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class ComboDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text praiseText;
    [SerializeField] private TMP_Text comboText;

    [Tooltip("한 번에 지운 줄 수별 칭찬 문구 (0번 칸 = 0줄, 1번 칸 = 1줄 ...). 빈 칸이면 표시하지 않음. 줄 수가 더 많으면 마지막 문구 사용")]
    [SerializeField] private string[] praiseWords = { "", "", "GREAT!", "EXCELLENT!", "AMAZING!" };
    [SerializeField] private string comboFormat = "COMBO {0}";

    [Header("Outline")]
    [SerializeField] private Color outlineColor = Color.black;
    [SerializeField, Range(0f, 1f)] private float outlineWidth = 0.25f;

    [Header("Animation")]
    [SerializeField] private float popDuration = 0.25f;
    [SerializeField] private float holdDuration = 0.5f;
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private float floatDistance = 60f;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 basePosition;
    private Coroutine showRoutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = (RectTransform)transform;
        basePosition = rectTransform.anchoredPosition;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        ApplyOutline(praiseText);
        ApplyOutline(comboText);
    }

    // 이 텍스트 전용 머티리얼 복사본에 테두리를 적용 (같은 폰트를 쓰는 다른 텍스트에는 영향 없음)
    private void ApplyOutline(TMP_Text text)
    {
        text.outlineColor = outlineColor;
        text.outlineWidth = outlineWidth;
    }

    public void Show(int lineCount, int combo)
    {
        string praise = GetPraise(lineCount);
        bool hasPraise = !string.IsNullOrEmpty(praise);
        bool hasCombo = combo >= 2;

        if (!hasPraise && !hasCombo)
        {
            return;
        }

        praiseText.text = praise;
        praiseText.gameObject.SetActive(hasPraise);

        comboText.text = string.Format(comboFormat, combo);
        comboText.gameObject.SetActive(hasCombo);

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
        }

        showRoutine = StartCoroutine(Animate());
    }

    private string GetPraise(int lineCount)
    {
        if (praiseWords.Length == 0 || lineCount <= 0)
        {
            return "";
        }

        int index = Mathf.Min(lineCount, praiseWords.Length - 1);
        return praiseWords[index];
    }

    // 톡 튀어나오며 커짐 → 잠깐 유지 → 위로 떠오르며 사라짐
    private IEnumerator Animate()
    {
        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = basePosition;

        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / popDuration);
            rectTransform.localScale = Vector3.one * EaseOutBack(progress);
            yield return null;
        }

        rectTransform.localScale = Vector3.one;

        if (holdDuration > 0f)
        {
            yield return new WaitForSeconds(holdDuration);
        }

        Vector2 endPosition = basePosition + new Vector2(0f, floatDistance);
        elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeDuration);
            canvasGroup.alpha = 1f - progress;
            rectTransform.anchoredPosition = Vector2.Lerp(basePosition, endPosition, progress);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = basePosition;
        showRoutine = null;
    }

    private float EaseOutBack(float t)
    {
        const float overshoot = 1.70158f;
        float shifted = t - 1f;
        return 1f + (overshoot + 1f) * shifted * shifted * shifted + overshoot * shifted * shifted;
    }
}
