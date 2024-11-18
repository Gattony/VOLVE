using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public enum UpgradeTypes
    {
        None,
        MovementSpeed,
        Health,
        Damage,
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



    public static List<UpgradeData> GetRandomUpgrades(int _amount, bool _uniqueOnly = true)
    {
        List<UpgradeData> upgradePools = Instance.avaiableUpgrades;
        List<UpgradeData> result = new();

        for (int i = 0; i < _amount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, upgradePools.Count - 1);
            result.Add(upgradePools[randomIndex]);

            if (_uniqueOnly)
            {
                upgradePools.RemoveAt(randomIndex);
            }
        }

        return result;
    }

    public static void ApplyUpgrade(UpgradeData _upgrade)
    {
        switch (_upgrade.type)
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
        Debug.Log("Health Upgraded!");
    }

    private static void UpgradeDamage()
    {
        PlayerStats.Instance.damageMultiplier *= 1.5f;
    }

    private static void UpgradeMovespeed()
    {
        PlayerStats.Instance.damageMultiplier *= 1.5f;
    }
}
