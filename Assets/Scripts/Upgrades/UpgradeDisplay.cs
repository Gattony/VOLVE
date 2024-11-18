using System.Collections.Generic;
using UnityEngine;

public class UpgradeDisplay : MonoBehaviour
{
    public static UpgradeDisplay Instance { get; private set; }

    [SerializeField] GameObject upgradeContainer;
    [SerializeField] List<UpgradeOptionDisplay> upgradeOptions = new();



    private void OnEnable()
    {
        PlayerCharacter.OnLevelUp += OnLevelUp;
    }

    private void OnDisbale()
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



    void OnLevelUp()
    {
        upgradeContainer.SetActive(true);

        // Get random upgrades from UpgradeManager.
        var upgrades = UpgradeManager.GetRandomUpgrades(upgradeOptions.Count);

        // Display Info
        for (int i = 0; i < upgrades.Count; i++)
        {
            upgradeOptions[i].UpdateDisplay(upgrades[i]);
        }
    }

    public static void ClosePopup()
    {
        Instance.upgradeContainer.SetActive(false);
    }
}