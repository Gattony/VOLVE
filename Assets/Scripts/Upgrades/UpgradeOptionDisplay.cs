using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class UpgradeOptionDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI upgradeName;
    [SerializeField] Image icon;
    [SerializeField] Button button;

    private UpgradeData upgradeType;



    public void UpdateDisplay(UpgradeData _data)
    {
        upgradeName.text = _data.upgradeName;
        icon.sprite = _data.icon;

        upgradeType = _data;
    }

    public void OnClick()
    {
        UpgradeManager.ApplyUpgrade(upgradeType);
        UpgradeDisplay.ClosePopup();
        Time.timeScale = 1f;
    }
}
