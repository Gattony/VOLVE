using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UpgradeOptionDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI upgradeName;
    [SerializeField] Image icon;
    [SerializeField] Button button;

    private UpgradeData upgradeType;



    public void UpdateDisplay(UpgradeData data)
    {
        upgradeName.text = data.upgradeName;
        icon.sprite = data.icon;

        upgradeType = data;
    }

    public void OnClick()
    {
        UpgradeManager.ApplyUpgrade(upgradeType);
        UpgradeDisplay.ClosePopup();
    }
}
