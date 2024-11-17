using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance;

    [Header("Leveling System")]
    public int currentLevel = 1;      // Starting level
    private int currentExp = 0;       // Current experience points
    public int expToNextLevel = 100;  // Experience required for the next level
    private int expToNextLevelBase = 100; // Base EXP for first level-up

    [Header("UI Elements")]
    public Slider expSlider;          // Slider to represent EXP bar
    public Text levelText;            // Text to display the current level
    public GameObject levelUpMenu;    // Level-up menu

    [Header("Level-Up Upgrades")]
    public GameObject[] upgradeButtons; // Buttons for upgrades

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
        // Ensure level-up menu is hidden at the start
        levelUpMenu.SetActive(false);
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

        // Show level-up menu
        levelUpMenu.SetActive(true);
        Time.timeScale = 0f; // Pause the game during level-up

        // Enable random upgrade buttons
        EnableRandomUpgrades();
    }

    private void EnableRandomUpgrades()
    {
        // Disable all buttons first
        foreach (var button in upgradeButtons)
        {
            button.SetActive(false);
        }

        // Activate a random selection of buttons
        int upgradesToShow = Mathf.Min(3, upgradeButtons.Length); // Show up to 3 upgrades
        for (int i = 0; i < upgradesToShow; i++)
        {
            int randomIndex = Random.Range(0, upgradeButtons.Length);
            upgradeButtons[randomIndex].SetActive(true);
        }
    }

    public void SelectUpgrade(GameObject upgrade)
    {
        // Apply the upgrade logic here (implement separately for each upgrade)
        Debug.Log($"{upgrade.name} selected!");

        // Close level-up menu
        CloseLevelUpMenu();
    }

    public void CloseLevelUpMenu()
    {
        levelUpMenu.SetActive(false); // Hide the menu
        Time.timeScale = 1f;         // Resume the game
        UpdateUI();
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
            levelText.text = $"Level {currentLevel}";
        }
    }
}
