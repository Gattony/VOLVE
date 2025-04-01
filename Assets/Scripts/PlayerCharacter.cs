using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerCharacter : MonoBehaviour
{
    public static PlayerCharacter Instance;

    public static event System.Action OnLevelUp;
    public static event System.Action OnPlayerDeath;

    [Header("Leveling System")]
    public int currentLevel = 1;          // Starting level
    private int currentExp = 0;           
    public int expToNextLevel = 100;      
    private int expToNextLevelBase = 100; 

    [Header("Health System")]
    public int maxHealth = 3;             
    private int currentHealth;            
    public Image heartContainerPrefab;    
    public Transform heartContainer;      
    public Sprite fullHeart;              
    public Sprite emptyHeart;             

    [Header("UI Elements")]
    public Image expBarFill;
    public TMP_Text levelText;            

    [Header("Effects")]
    public ParticleSystem expFillEffect;
    public RectTransform expBarTransform;

    [Header("IFrames")]
    [SerializeField] public float iFrameDuration;
    [SerializeField] public float numberOfFlashes;
    public SpriteRenderer spriteRend;

    private bool isDead = false;
    private PlayerControl playerControl;

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
        animator = GetComponent<Animator>();
        playerControl = GetComponent<PlayerControl>();
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
        maxHealth += amount; 
        currentHealth = maxHealth; 
        InitializeHearts();
        UpdateUI(); 
        Debug.Log($"Max health increased to {maxHealth}");
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth > 0)
        {
            StartCoroutine(Invunerability());
        }
        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateHearts();
    }

    private Animator animator;

    private void Die()
    {

        if (isDead) return;
        isDead = true;

        if (playerControl != null)
        {
            playerControl.Die();
        }
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        PlayerControl tentacleController = FindObjectOfType<PlayerControl>();
        if (tentacleController != null)
        {
            tentacleController.StopTentacles();
        }

        OnPlayerDeath?.Invoke();
        Debug.Log("Player has died!");

        StartCoroutine(DeathPause());
    }

    private System.Collections.IEnumerator DeathPause()
    {
        yield return new WaitForSeconds(3f); 
        Time.timeScale = 0f; 
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

        UpdateHearts();
    }
    private void UpdateBarFill(float targetFill)
    {
        expBarFill.fillAmount = targetFill;

        if (expFillEffect != null)
        {

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
        float duration = 0.5f;
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


    public RectTransform expBarContainer;

    public void SetBarFill(float newFillAmount)
    {
        UpdateBarFill(newFillAmount);
    }

    private IEnumerator Invunerability()
    {
        Physics2D.IgnoreLayerCollision(3, 7, true);

        for (int i = 0; i < numberOfFlashes; i++)
        {
            spriteRend.color = new Color(1f, 1f, 1f, .7f);
            yield return new WaitForSeconds(0.4f);
            spriteRend.color = Color.white;
            yield return new WaitForSeconds(0.4f);
        }

        Physics2D.IgnoreLayerCollision(3, 7, false);
    }
}
