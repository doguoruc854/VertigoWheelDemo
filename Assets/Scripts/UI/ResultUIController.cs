using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIController : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Image resultIcon;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Sprite bombIcon;

    private void OnValidate()
    {
        if (panelRoot == null)
            panelRoot = gameObject;

        var images = GetComponentsInChildren<Image>(true);
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i].name == "ui_image_result_icon_value")
                resultIcon = images[i];
        }

        var texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].name == "ui_text_result_value")
                resultText = texts[i];
        }
    }

    private void Awake()
    {
        Hide();
    }

    public void ShowReward(Sprite icon, int value)
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (resultIcon != null)
        {
            resultIcon.sprite = icon;
            resultIcon.enabled = icon != null;
        }

        if (resultText != null)
            resultText.text = "+" + value;
    }

    public void ShowBomb()
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (resultIcon != null)
        {
            resultIcon.sprite = bombIcon;
            resultIcon.enabled = bombIcon != null;
        }

        if (resultText != null)
            resultText.text = "BOMB";
    }

    public void Hide()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }
}