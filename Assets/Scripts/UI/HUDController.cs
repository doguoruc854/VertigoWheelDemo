using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalRewardText;
    [SerializeField] private TextMeshProUGUI zoneText;

    private void OnValidate()
    {
        if (totalRewardText == null)
        {
            var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].name == "ui_text_totalreward_value")
                    totalRewardText = texts[i];
                if (texts[i].name == "ui_text_zone_value")
                    zoneText = texts[i];
            }
        }
    }

    public void Refresh(int totalCurrency, int zone)
    {
        if (totalRewardText != null)
            totalRewardText.text = totalCurrency.ToString();
        if (zoneText != null)
            zoneText.text = "Zone " + zone;
    }
}