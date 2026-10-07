using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DragController dragController;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Board board;
    [SerializeField] private float popupDelay = 0.3f;
    [SerializeField] private string menuSceneName = "MenuScene";

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPopup;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text finalBestText;
    [SerializeField] private GameObject newBestLabel;

    [Header("Pause UI")]
    [SerializeField] private GameObject pausePopup;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;

    [Header("Sound")]
    [SerializeField] private AudioClip gameOverSfx;
    [SerializeField] private AudioClip newBestSfx;
    [SerializeField] private AudioClip clickSfx;

    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }

    private void Start()
    {
        Time.timeScale = 1f;

        gameOverPopup.SetActive(false);
        pausePopup.SetActive(false);

        musicSlider.value = SoundManager.Instance.BgmVolume;
        soundSlider.value = SoundManager.Instance.SfxVolume;
    }

    public void GameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        dragController.enabled = false;

        float collapseDuration = board.PlayCollapse();
        Invoke(nameof(ShowGameOverPopup), collapseDuration + popupDelay);
    }

    private void ShowGameOverPopup()
    {
        bool isNewBest = scoreManager.SaveBestScore();

        finalScoreText.text = scoreManager.Score.ToString();
        finalBestText.text = $"BEST {scoreManager.BestScore}";
        newBestLabel.SetActive(isNewBest);
        gameOverPopup.SetActive(true);

        SoundManager.Instance.PlaySFX(isNewBest ? newBestSfx : gameOverSfx);
    }

    public void OnClickPause()
    {
        if (IsGameOver || IsPaused)
        {
            return;
        }

        SoundManager.Instance.PlaySFX(clickSfx);

        IsPaused = true;
        Time.timeScale = 0f;
        dragController.enabled = false;
        pausePopup.SetActive(true);
    }

    public void OnClickResume()
    {
        SoundManager.Instance.PlaySFX(clickSfx);

        IsPaused = false;
        Time.timeScale = 1f;
        dragController.enabled = true;
        pausePopup.SetActive(false);

        PlayerPrefs.Save();
    }

    public void OnClickRestart()
    {
        SoundManager.Instance.PlaySFX(clickSfx);
        Time.timeScale = 1f;
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickHome()
    {
        SoundManager.Instance.PlaySFX(clickSfx);
        Time.timeScale = 1f;
        PlayerPrefs.Save();
        SceneManager.LoadScene(menuSceneName);
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