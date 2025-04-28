using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private Vector2 originalScorePos;
    private Coroutine scoreShakeCoroutine;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    private int currentScore = 0;
    private float currentMultiplier = 1f;
    public float maxMultiplier = 3f;

    [Header("Multiplier Settings")]
    public float multiplierGain = 0.2f;
    public float multiplierDecayRate = 0.3f;
    public float decayDelay = 2f;

    [Header("Shake Settings")]
    public float multiplierShakeMultiplier = 4f; // Only affects multiplier shake

    private float decayTimer;
    private Coroutine multiplierShakeCoroutine;
    private Vector2 originalMultiplierPos;
    private bool isMultiplierDecayPaused = false;

    [Header("High Score")]
    public TextMeshProUGUI runScoreText;
    public TextMeshProUGUI highScoreText;

    private int highScore = 0;


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        originalScorePos = scoreText.rectTransform.anchoredPosition;
        originalMultiplierPos = multiplierText.rectTransform.anchoredPosition;
        UpdateUI();
    }

    private void Update()
    {
        HandleMultiplierDecay();
    }

    private void HandleMultiplierDecay()
    {
        if (Time.timeScale == 0f) return;

        if (decayTimer > 0)
        {
            decayTimer -= Time.unscaledDeltaTime;
        }
        else if (currentMultiplier > 1f)
        {
            currentMultiplier -= multiplierDecayRate * Time.unscaledDeltaTime;
            currentMultiplier = Mathf.Max(1f, currentMultiplier);
            UpdateUI();

            if (currentMultiplier == 1f && multiplierShakeCoroutine != null)
            {
                StopCoroutine(multiplierShakeCoroutine);
                multiplierText.rectTransform.anchoredPosition = originalMultiplierPos;
            }
        }
    }


    public void AddScore(int baseAmount)
    {
        int finalAmount = Mathf.RoundToInt(baseAmount * currentMultiplier);
        currentScore += finalAmount;
        UpdateUI();
        TriggerScoreTextShake();
    }

    public void OnEnemyKilled()
    {
        decayTimer = decayDelay;

        float oldMultiplier = currentMultiplier;
        currentMultiplier += multiplierGain;
        currentMultiplier = Mathf.Min(currentMultiplier, maxMultiplier);

        if (currentMultiplier > oldMultiplier)
        {
            if (multiplierShakeCoroutine != null)
                StopCoroutine(multiplierShakeCoroutine);

            multiplierShakeCoroutine = StartCoroutine(ShakeMultiplierText());
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = $"{currentScore}";
        multiplierText.text = $"<b>x{currentMultiplier:F1}</b>";
    }

    private IEnumerator ShakeText(TextMeshProUGUI text, Vector2 startPosition, float duration, float magnitude)
    {
        RectTransform rect = text.rectTransform;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            rect.anchoredPosition = startPosition + new Vector2(offsetX, offsetY);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        rect.anchoredPosition = startPosition;
    }

    private void TriggerScoreTextShake()
    {
        if (scoreShakeCoroutine != null)
            StopCoroutine(scoreShakeCoroutine);

        scoreShakeCoroutine = StartCoroutine(ShakeText(scoreText, originalScorePos, 0.2f, 8f));
    }

    private IEnumerator ShakeMultiplierText()
    {
        RectTransform rect = multiplierText.rectTransform;

        while (currentMultiplier > 1f)
        {
            float magnitude = (currentMultiplier - 1f) * multiplierShakeMultiplier;
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            rect.anchoredPosition = originalMultiplierPos + new Vector2(offsetX, offsetY);

            yield return null;
        }

        rect.anchoredPosition = originalMultiplierPos;
    }


    private IEnumerator AnimateFinalScores()
    {
        yield return new WaitForSecondsRealtime(4f);

        int displayedScore = 0;
        float countDuration = 1.5f; 
        float elapsed = 0f;

        while (elapsed < countDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / countDuration);
            displayedScore = Mathf.RoundToInt(Mathf.Lerp(0, currentScore, t));
            if (runScoreText != null)
                runScoreText.text = "Score: " + displayedScore.ToString();
            yield return null;
        }

        if (runScoreText != null)
            runScoreText.text = "Score: " + currentScore.ToString();

        yield return new WaitForSecondsRealtime(0.5f); // Small pause after counting
        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore.ToString();
    }


    public void ShowFinalScores()
    {
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
        }

        StartCoroutine(AnimateFinalScores());
    }

    public void PauseMultiplierDecay()
    {
        isMultiplierDecayPaused = true;
    }

    public void ResumeMultiplierDecay()
    {
        isMultiplierDecayPaused = false;
    }
}
    