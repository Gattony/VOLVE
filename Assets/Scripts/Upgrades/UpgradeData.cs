using UnityEngine;

using static UpgradeManager;

[CreateAssetMenu(fileName = "New Upgrade Data", menuName = "Custom Data/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    public UpgradeTypes type;
}
