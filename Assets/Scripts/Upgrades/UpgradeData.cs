using UnityEngine;

using static UpgradeManager;

[CreateAssetMenu(fileName = "New Upgrade Data", menuName = "Custom Data/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    public UpgradeTypes type;

    public UpgradeData(string upgradeName, Sprite icon, UpgradeTypes type)
    {
        this.upgradeName = upgradeName;
        this.icon = icon;
        this.type = type;
    }

    public void CloneFrom(UpgradeData data)
    {
        this.upgradeName = data.upgradeName;
        this.icon = data.icon;
        this.type = data.type;
    }
}
