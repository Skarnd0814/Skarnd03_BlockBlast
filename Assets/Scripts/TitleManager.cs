using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private TMP_Text touchText;
    [SerializeField] private float blinkSpeed = 1.5f;
    [SerializeField] private string nextSceneName = "MenuScene";
    [SerializeField] private AudioClip gameStartSfx;

    [Header("Transition")]
    [SerializeField] private RectTransform titleLogo;
    [SerializeField] private Image blurBackground;
    [SerializeField] private TMP_Text creatorText;
    [SerializeField] private Vector2 menuLogoPosition = new Vector2(0f, 0f);
    [SerializeField] private Vector2 menuLogoSize = new Vector2(800f, 800f);
    [SerializeField] private float transitionDuration = 0.8f;

    private bool isTransitioning;

    private void Update()
    {
        if (isTransitioning)
        {
            return;
        }

        BlinkTouchText();

        if (Pointer.current != null &&
            Pointer.current.press.wasReleasedThisFrame)
        {
            StartCoroutine(TransitionToMenu());
        }
    }

    private void BlinkTouchText()
    {
        float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
        touchText.alpha = alpha;
    }

    private IEnumerator TransitionToMenu()
    {
        isTransitioning = true;
        SoundManager.Instance.PlaySFX(gameStartSfx);

        Vector2 startPosition = titleLogo.anchoredPosition;
        Vector2 startSize = titleLogo.sizeDelta;
        float startTouchAlpha = touchText.alpha;
        float startCreatorAlpha = creatorText.alpha;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / transitionDuration);
            float smooth = Mathf.SmoothStep(0f, 1f, progress);

            titleLogo.anchoredPosition = Vector2.Lerp(startPosition, menuLogoPosition, smooth);
            titleLogo.sizeDelta = Vector2.Lerp(startSize, menuLogoSize, smooth);

            SetImageAlpha(blurBackground, 1f - smooth);
            touchText.alpha = Mathf.Lerp(startTouchAlpha, 0f, smooth);
            creatorText.alpha = Mathf.Lerp(startCreatorAlpha, 0f, smooth);

            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}