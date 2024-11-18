using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance;

    public static event System.Action OnLevelUp;

    [Header("Leveling System")]
    public int currentLevel = 1;      // Starting level
    private int currentExp = 0;       // Current experience points
    public int expToNextLevel = 100;  // Experience required for the next level
    private int expToNextLevelBase = 100; // Base EXP for first level-up

    [Header("UI Elements")]
    public Slider expSlider;          // Slider to represent EXP bar
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
        UpdateUI();
    }

    public void AddExp(int amount)
    {
        currentExp += amount;

        // Check for level-up
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }

        // Update EXP slider
        UpdateUI();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExp -= expToNextLevel;

        // Increase EXP requirement for the next level
        expToNextLevel = Mathf.RoundToInt(expToNextLevelBase * Mathf.Pow(1.2f, currentLevel - 1));

        OnLevelUp?.Invoke();

        Time.timeScale = 0f; // Pause the game during level-up

    }

    private void UpdateUI()
    {
        if (expSlider != null)
        {
            expSlider.maxValue = expToNextLevel;
            expSlider.value = currentExp;
        }

        if (levelText != null)
        {
            levelText.text = $"{currentLevel}";
        }
    }
}