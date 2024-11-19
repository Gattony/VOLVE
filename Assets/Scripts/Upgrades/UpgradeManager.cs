using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public enum UpgradeTypes
    {
        None,  
        MovementSpeed,
        Health,
        Damage,
        FireRate,
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
        }
    }



    private static void UpgradeHealth()
    {
        PlayerStats.Instance.healthIncrease += 1;
    }

    private static void UpgradeDamage()
    {
        PlayerStats.Instance.damageMultiplier *= 1.5f;
    }

    private static void UpgradeMovespeed()
    {
        PlayerStats.Instance.speedMultiplier *= 1.2f;
    }
}
