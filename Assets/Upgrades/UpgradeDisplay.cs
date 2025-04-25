using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UpgradeDisplay : MonoBehaviour
{
    public static UpgradeDisplay Instance { get; private set; }

    [SerializeField] private GameObject upgradeContainer;
    [SerializeField] private List<UpgradeOptionDisplay> upgradeOptions = new();
    [SerializeField] private GameObject levelUpEffectPrefab;
    [SerializeField] public Transform effectSpawnPoint;
    [SerializeField] public GameObject GameplayCanvas;


    private void OnEnable()
    {
        PlayerCharacter.OnLevelUp += OnLevelUp;
    }

    private void OnDisable()
    {
        PlayerCharacter.OnLevelUp -= OnLevelUp;
    }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        upgradeContainer.SetActive(false);
    }

    private void OnLevelUp()
    {
        StartCoroutine(LevelUpCoroutine());
    }

    private IEnumerator LevelUpCoroutine()
    {
        GameplayCanvas.SetActive(false);
        Time.timeScale = 0f;

        Camera.main.GetComponent<CameraController>().PanToPlayer(0.1f);

        // Instantiate the effect prefab
        GameObject effect = Instantiate(levelUpEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
        Animator effectAnimator = effect.GetComponent<Animator>();

        if (effectAnimator != null)
        {
            // Wait one frame to let the Animator enter its first state
            yield return null;

            // Wait until the animator starts a valid animation
            AnimatorStateInfo stateInfo = effectAnimator.GetCurrentAnimatorStateInfo(0);
            while (stateInfo.length == 0f || effectAnimator.IsInTransition(0))
            {
                yield return null;
                stateInfo = effectAnimator.GetCurrentAnimatorStateInfo(0);
            }

            float animationLength = stateInfo.length;

            // Wait for the animation using unscaled time
            float timer = 0f;
            while (timer < animationLength)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("Level-up effect prefab is missing an Animator!");
            yield return new WaitForSecondsRealtime(1f);
        }

        Destroy(effect);

        // Show the upgrade UI
        upgradeContainer.SetActive(true);

        var upgrades = UpgradeManager.GetRandomUpgrades(upgradeOptions.Count);

        for (int i = 0; i < upgrades.Count; i++)
        {
            upgradeOptions[i].UpdateDisplay(upgrades[i]);
        }

        ScoreManager.Instance?.PauseMultiplierDecay();
    }

    public static void ClosePopup()
    {
        Instance.GameplayCanvas.SetActive(true);
        Instance.upgradeContainer.SetActive(false);

        ScoreManager.Instance?.ResumeMultiplierDecay();
        Time.timeScale = 1f;

    }
}