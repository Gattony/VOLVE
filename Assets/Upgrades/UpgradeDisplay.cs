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

        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.PanToPlayer(0.1f);

        // Wait a tiny bit for the pan to finish
        yield return new WaitForSecondsRealtime(0.1f);

        cameraController.StartContinuousShake(0.05f); // smaller intensity now

        // Instantiate the effect prefab
        GameObject effect = Instantiate(levelUpEffectPrefab, effectSpawnPoint.position, Quaternion.identity);
        Animator effectAnimator = effect.GetComponent<Animator>();

        if (effectAnimator != null)
        {
            yield return null;

            AnimatorStateInfo stateInfo = effectAnimator.GetCurrentAnimatorStateInfo(0);
            while (stateInfo.length == 0f || effectAnimator.IsInTransition(0))
            {
                yield return null;
                stateInfo = effectAnimator.GetCurrentAnimatorStateInfo(0);
            }

            float animationLength = stateInfo.length;

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

        upgradeContainer.SetActive(true);

        cameraController.StopContinuousShake();

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