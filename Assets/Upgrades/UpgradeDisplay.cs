using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class UpgradeDisplay : MonoBehaviour
{
    public static UpgradeDisplay Instance { get; private set; }

    [SerializeField] private GameObject upgradeContainer;
    [SerializeField] private List<UpgradeOptionDisplay> upgradeOptions = new();
    [SerializeField] private float delay = 0.5f;



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
        yield return new WaitForSeconds(delay);

        upgradeContainer.SetActive(true);

        // Get random upgrades from UpgradeManager.
        var upgrades = UpgradeManager.GetRandomUpgrades(upgradeOptions.Count);

        // Display Info
        for (int i = 0; i < upgrades.Count; i++)
        {
            upgradeOptions[i].UpdateDisplay(upgrades[i]);
        }

        ScoreManager.Instance?.PauseMultiplierDecay();
        Time.timeScale = 0f;
    }



    public static void ClosePopup()
    {
        Instance.upgradeContainer.SetActive(false);

        ScoreManager.Instance?.ResumeMultiplierDecay();
        Time.timeScale = 1f;

    }
}