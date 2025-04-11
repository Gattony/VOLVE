using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using Unity.VisualScripting;

public class UpgradeManager : MonoBehaviour
{
    public enum UpgradeTypes
    {
        None,  
        MovementSpeed,
        Health,
        Damage,
        FireRate,
        EXP,
        Ammo,
        Reload,
    }



    public static UpgradeManager Instance { get; private set; }

    [SerializeField] private List<UpgradeData> avaiableUpgrades;



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
    }



    public static List<UpgradeData> GetRandomUpgrades(int amount, bool uniqueOnly = true)
    {
        List<UpgradeData> result = new();
        List<UpgradeData> upgradePools = new(Instance.avaiableUpgrades.Count);

        //Randomizing upgrades from upgrade data
        foreach (var upgrade in Instance.avaiableUpgrades)
        {
            UpgradeData instance = ScriptableObject.CreateInstance<UpgradeData>();
            instance.CloneFrom(upgrade);
            upgradePools.Add(instance);
        }

        for (int i = 0; i < amount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, upgradePools.Count - 1);
            result.Add(upgradePools[randomIndex]);

            if (uniqueOnly)
            {
                upgradePools.RemoveAt(randomIndex);
            }
        }

        return result;
    }

    public static void ApplyUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeTypes.None:
                break;
            case UpgradeTypes.MovementSpeed:
                UpgradeMovespeed();
                break;
            case UpgradeTypes.Health:
                UpgradeHealth();
                break;
            case UpgradeTypes.Damage:
                UpgradeDamage();
                break;
            case UpgradeTypes.FireRate:
                UpgradeFireRate();
                break;
            case UpgradeTypes.EXP:
                UpgradeEXPDetection();
                break;
            case UpgradeTypes.Ammo:
                UpgradeAmmoCapacity();
                break;
            case UpgradeTypes.Reload:
                UpgradeReloadSpeed();
                break;
        }
    }



    private static void UpgradeHealth()
    {
        PlayerCharacter player = PlayerCharacter.Instance;

        if (player != null)
        {
            int healthIncrease = 1; 
            player.IncreaseMaxHealth(healthIncrease); 
        }
    }

    private static void UpgradeDamage()
    {
        PlayerStats.Instance.damageMultiplier *= 1.35f;
    }

    private static void UpgradeMovespeed()
    {
        PlayerStats.Instance.speedMultiplier *= 1.05f;
    }
    
    private static void UpgradeFireRate()
    {
        PlayerStats.Instance.fireRateMultiplier *= 1.1f;
    }

    private static void UpgradeEXPDetection()
    {
        PlayerStats.Instance.expDetectionMultiplier *= 1.15f;
    }

    private static void UpgradeAmmoCapacity()
    {
        PlayerStats.Instance.ammoCapacityMultiplier *= 1.2f;
    }

    private static void UpgradeReloadSpeed()
    {
        PlayerStats.Instance.reloadSpeedMultiplier *= 1.15f;
    }
}
