using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private int pointsPerLine = 10;

    private const string BestScoreKey = "BestScore";

    public int Score { get; private set; }
    public int BestScore { get; private set; }

    private void Awake()
    {
        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
    }

    private void Start()
    {
        UpdateScoreText();
    }

    public void AddPlacePoints(int blockCount)
    {
        Score += blockCount;
        UpdateScoreText();
    }

    public void AddLinePoints(int lineCount)
    {
        Score += pointsPerLine * lineCount * lineCount;
        UpdateScoreText();
    }

    public bool SaveBestScore()
    {
        if (Score <= BestScore)
        {
            return false;
        }

        BestScore = Score;
        PlayerPrefs.SetInt(BestScoreKey, BestScore);
        PlayerPrefs.Save();
        UpdateScoreText();
        return true;
    }

    private void UpdateScoreText()
    {
        scoreText.text = Score.ToString();
        bestScoreText.text = $"BEST {Mathf.Max(Score, BestScore)}";
    }
}