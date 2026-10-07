using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DragController dragController;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private float gameOverDelay = 0.8f;
    [SerializeField] private string menuSceneName = "MenuScene";

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPopup;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text finalBestText;
    [SerializeField] private GameObject newBestLabel;

    [Header("Sound")]
    [SerializeField] private AudioClip gameOverSfx;
    [SerializeField] private AudioClip newBestSfx;
    [SerializeField] private AudioClip clickSfx;

    public bool IsGameOver { get; private set; }

    private void Start()
    {
        gameOverPopup.SetActive(false);
    }

    public void GameOver()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        dragController.enabled = false;
        Invoke(nameof(ShowGameOverPopup), gameOverDelay);
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

    public void OnClickRestart()
    {
        SoundManager.Instance.PlaySFX(clickSfx);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickHome()
    {
        SoundManager.Instance.PlaySFX(clickSfx);
        SceneManager.LoadScene(menuSceneName);
    }
}