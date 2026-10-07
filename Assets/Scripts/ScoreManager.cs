using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int pointsPerLine = 10;

    public int Score { get; private set; }

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

    private void UpdateScoreText()
    {
        scoreText.text = Score.ToString();
    }
}