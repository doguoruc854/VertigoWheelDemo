using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI zoneText;

    private void OnValidate()
    {
        if (zoneText == null)
        {
            var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].name == "ui_text_zone_value")
                    zoneText = texts[i];
            }
        }
    }

    public void RefreshZone(int zone)
    {
        if (zoneText != null)
            zoneText.text = "Zone " + zone;
    }
}
