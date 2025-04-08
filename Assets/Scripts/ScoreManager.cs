using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 5f;

    private int currentScore = 0;
    private Vector3 originalPos;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (scoreText != null)
            originalPos = scoreText.rectTransform.anchoredPosition;

        scoreText.text = $"Score: {currentScore}";
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        scoreText.text = $"Score: {currentScore}";

        if (scoreText != null)
            StartCoroutine(ShakeScoreText());
    }

    private IEnumerator ShakeScoreText()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;

            scoreText.rectTransform.anchoredPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            yield return null;
        }

        scoreText.rectTransform.anchoredPosition = originalPos;
    }

    public int GetScore()
    {
        return currentScore;
    }
}
