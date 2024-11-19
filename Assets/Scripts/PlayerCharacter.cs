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
    public Sprite fullHeart;              // Sprite for full heart
    public Sprite emptyHeart;             // Sprite for empty heart

    [Header("UI Elements")]
    public Slider expSlider;              // Slider to represent EXP bar
    public TMP_Text levelText;            // Text to display the current level

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
            GameObject newHeart = Instantiate(heartContainerPrefab.gameObject, heartContainer);
            newHeart.GetComponent<Image>().sprite = fullHeart; 
        }
    }


    private void UpdateHearts()
    {
        for (int i = 0; i < heartContainer.childCount; i++)
        {
            Image heartImage = heartContainer.GetChild(i).GetComponent<Image>();
            heartImage.sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
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
        expToNextLevel = Mathf.RoundToInt(expToNextLevelBase * Mathf.Pow(1.5f, currentLevel - 1));
        OnLevelUp?.Invoke();
    }

    private void UpdateUI()
    {
        // Update EXP bar
        if (expSlider != null)
        {
            expSlider.maxValue = expToNextLevel;
            expSlider.value = currentExp;
        }

        // Update level text
        if (levelText != null)
        {
            levelText.text = $"{currentLevel}";
        }

        // Update heart UI
        UpdateHearts();
    }
}
