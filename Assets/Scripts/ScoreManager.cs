using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

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

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
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

    private void TriggerScoreTextShake()
    {
        StartCoroutine(ShakeText(scoreText, 0.2f, 8f));
    }

    private IEnumerator ShakeMultiplierText()
    {
        RectTransform rect = multiplierText.rectTransform;
        originalMultiplierPos = rect.anchoredPosition;

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

    private IEnumerator ShakeText(TextMeshProUGUI text, float duration, float magnitude)
    {
        RectTransform rect = text.rectTransform;
        Vector2 originalPos = rect.anchoredPosition;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            rect.anchoredPosition = originalPos + new Vector2(offsetX, offsetY);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        rect.anchoredPosition = originalPos;
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
    