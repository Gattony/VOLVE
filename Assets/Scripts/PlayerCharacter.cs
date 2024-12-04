using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance;

    public static event System.Action OnLevelUp;
    public static event System.Action OnPlayerDeath;

    [Header("Leveling System")]
    public int currentLevel = 1;          // Starting level
    private int currentExp = 0;           // Current experience points
    public int expToNextLevel = 100;      // Experience required for the next level
    private int expToNextLevelBase = 100; // Base EXP for first level-up

    [Header("Health System")]
    public int maxHealth = 3;             // Maximum health (number of hearts)
    private int currentHealth;            // Current health
    public Image heartContainerPrefab;    // Prefab for a single heart
    public Transform heartContainer;      // Parent object to hold all hearts
    public Sprite fullHeart;              
    public Sprite emptyHeart;             

    [Header("UI Elements")]
    public Image expBarFill;
    public TMP_Text levelText;            // Text to display the current level

    [Header("Effects")]
    public ParticleSystem expFillEffect; // Particle system for EXP fill
    public RectTransform expBarTransform; // RectTransform for the EXP bar

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        InitializeHearts();
        UpdateUI();
    }

    private void InitializeHearts()
    {
        // Clear existing hearts in the container
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }

        // Instantiate hearts equal to maxHealth
        for (int i = 0; i < maxHealth; i++)
        {
            Instantiate(heartContainerPrefab.gameObject, heartContainer);
        }

        // Update heart sprites to match current health
        UpdateHearts();
    }



    private void UpdateHearts()
    {
        for (int i = 0; i < maxHealth; i++)
        {
            if (i < heartContainer.childCount) // Ensure there's a corresponding heart UI
            {
                Image heartImage = heartContainer.GetChild(i).GetComponent<Image>();
                heartImage.sprite = (i < currentHealth) ? fullHeart : emptyHeart;
            }
        }
    }


    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount; // Increase max health
        currentHealth = maxHealth; // Restore health to full
        InitializeHearts(); // Recreate heart UI
        UpdateUI(); // Update related UI elements
        Debug.Log($"Max health increased to {maxHealth}");
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateHearts();
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke();
        Debug.Log("Player has died!");
        Time.timeScale = 0f; // Pause the game
    }

    public void AddExp(int amount)
    {
        currentExp += amount;

        // Check for level-up
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExp -= expToNextLevel;
        //Increasing the exp limit each time it levels up
        expToNextLevel = Mathf.RoundToInt(expToNextLevelBase * Mathf.Pow(1.35f, currentLevel - 1));
        OnLevelUp?.Invoke();
    }

    private void UpdateUI()
    {

        if (expBarFill != null)
        {
            float normalizedFill = (float)currentExp / expToNextLevel;

            // Animate the bar fill using a coroutine
            StartCoroutine(AnimateBarFill(expBarFill.fillAmount, normalizedFill));

            // Update level text
            if (levelText != null)
            {
                levelText.text = $"Level {currentLevel}";
            }
        }

        // Update heart UI
        UpdateHearts();
    }
    private void UpdateBarFill(float targetFill)
    {
        expBarFill.fillAmount = targetFill;

        // Continuously position the particle system at the edge of the bar
        if (expFillEffect != null)
        {
            // Get the width of the bar container
            float containerWidth = expBarContainer.rect.width;

            // Calculate the edge position in local space
            Vector2 edgeLocalPosition = new Vector2(
                targetFill * containerWidth - (containerWidth * 0.5f), // Adjust for pivot
                0f
            );

            // Convert local to world space for the particle system
            Vector3 edgeWorldPosition = expBarContainer.TransformPoint(edgeLocalPosition);

            expFillEffect.transform.position = edgeWorldPosition;

            // Ensure the particle effect is playing
            if (!expFillEffect.isPlaying)
            {
                expFillEffect.Play();
            }
        }
    }

    private System.Collections.IEnumerator AnimateBarFill(float startFill, float targetFill)
    {
        float duration = 0.5f; // Duration of the animation
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newFill = Mathf.Lerp(startFill, targetFill, elapsed / duration);

            // Update the bar fill and particle system position
            UpdateBarFill(newFill);

            yield return null;
        }

        // Finalize bar fill and particle system position
        UpdateBarFill(targetFill);
    }

    // Assuming expBarContainer is a reference to the parent
    public RectTransform expBarContainer;

    public void SetBarFill(float newFillAmount)
    {
        UpdateBarFill(newFillAmount);
    }

}
