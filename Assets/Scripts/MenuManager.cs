using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPopup;
    [SerializeField] private string gameSceneName = "InGameScene";

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private AudioClip clickSfx;

    [Header("Intro")]
    [SerializeField] private CanvasGroup menuButtons;
    [SerializeField] private float fadeDelay = 0.1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float slideDistance = 60f;

    private void Start()
    {
        settingsPopup.SetActive(false);

        musicSlider.value = SoundManager.Instance.BgmVolume;
        soundSlider.value = SoundManager.Instance.SfxVolume;

        StartCoroutine(FadeInButtons());
    }

    private IEnumerator FadeInButtons()
    {
        RectTransform buttonsRect = (RectTransform)menuButtons.transform;
        Vector2 endPosition = buttonsRect.anchoredPosition;
        Vector2 startPosition = endPosition - new Vector2(0f, slideDistance);

        menuButtons.alpha = 0f;
        menuButtons.interactable = false;
        menuButtons.blocksRaycasts = false;
        buttonsRect.anchoredPosition = startPosition;

        if (fadeDelay > 0f)
        {
            yield return new WaitForSeconds(fadeDelay);
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeDuration);
            float smooth = Mathf.SmoothStep(0f, 1f, progress);

            menuButtons.alpha = smooth;
            buttonsRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, smooth);

            yield return null;
        }

        menuButtons.alpha = 1f;
        buttonsRect.anchoredPosition = endPosition;
        menuButtons.interactable = true;
        menuButtons.blocksRaycasts = true;
    }

    public void OnClickPlay()
    {
        SoundManager.Instance.PlaySFX(clickSfx);
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickSettings()
    {
        SoundManager.Instance.PlaySFX(clickSfx);
        settingsPopup.SetActive(true);
    }

    public void OnClickCloseSettings()
    {
        SoundManager.Instance.PlaySFX(clickSfx);
        settingsPopup.SetActive(false);
        PlayerPrefs.Save();
    }

    public void OnMusicVolumeChanged(float value)
    {
        SoundManager.Instance.SetBgmVolume(value);
    }

    public void OnSoundVolumeChanged(float value)
    {
        SoundManager.Instance.SetSfxVolume(value);
    }
}