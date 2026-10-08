using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private int pointsPerBlock = 100;
    [SerializeField] private int pointsPerLine = 1000;
    [Tooltip("콤보 1 증가당 줄 삭제 점수 추가 배율 (0.5 = 콤보 2에서 x1.5, 콤보 3에서 x2.0)")]
    [SerializeField] private float comboBonusRate = 0.5f;

    [Header("Animation")]
    [SerializeField] private float countDuration = 0.4f;
    [SerializeField] private float punchScale = 1.3f;
    [SerializeField] private float punchDuration = 0.25f;

    public const string BestScoreKey = "BestScore";

    public int Score { get; private set; }
    public int BestScore { get; private set; }

    private int displayedScore;
    private Coroutine scoreRoutine;

    private void Awake()
    {
        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
    }

    private void Start()
    {
        RefreshTexts();
    }

    public void AddPlacePoints(int blockCount)
    {
        AddScore(pointsPerBlock * blockCount);
    }

    public void AddLinePoints(int lineCount, int combo = 1)
    {
        int basePoints = pointsPerLine * lineCount * lineCount;
        AddScore(Mathf.RoundToInt(basePoints * GetComboMultiplier(combo)));
    }

    public float GetComboMultiplier(int combo)
    {
        return 1f + Mathf.Max(0, combo - 1) * comboBonusRate;
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
        RefreshTexts();
        return true;
    }

    private void AddScore(int amount)
    {
        Score += amount;

        if (scoreRoutine != null)
        {
            StopCoroutine(scoreRoutine);
        }

        scoreRoutine = StartCoroutine(AnimateScore(displayedScore, Score));
    }

    private IEnumerator AnimateScore(int from, int to)
    {
        float totalDuration = Mathf.Max(countDuration, punchDuration);
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            elapsed += Time.deltaTime;

            float countProgress = Mathf.Clamp01(elapsed / countDuration);
            displayedScore = Mathf.RoundToInt(Mathf.Lerp(from, to, countProgress));

            float punchProgress = Mathf.Clamp01(elapsed / punchDuration);
            float scale = 1f + (punchScale - 1f) * Mathf.Sin(punchProgress * Mathf.PI);
            scoreText.transform.localScale = Vector3.one * scale;

            RefreshTexts();
            yield return null;
        }

        displayedScore = to;
        scoreText.transform.localScale = Vector3.one;
        RefreshTexts();
        scoreRoutine = null;
    }

    private void RefreshTexts()
    {
        scoreText.text = displayedScore.ToString();
        bestScoreText.text = $"BEST {Mathf.Max(displayedScore, BestScore)}";
    }
}