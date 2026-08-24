using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUIController : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Image resultIcon;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Sprite bombIcon;
    [SerializeField] private float showDuration = 0.3f;

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

    private void OnDisable()
    {
        KillContentTweens(resetScale: false);
    }

    public void ShowReward(Sprite icon, int value)
    {
        if (resultIcon != null)
        {
            resultIcon.sprite = icon;
            resultIcon.enabled = icon != null;
        }

        if (resultText != null)
            resultText.text = "+" + value;

        PlayShow();
    }

    public void ShowBomb()
    {
        if (resultIcon != null)
        {
            resultIcon.sprite = bombIcon;
            resultIcon.enabled = bombIcon != null;
        }

        if (resultText != null)
            resultText.text = "BOMB";

        PlayShow();
    }

    public void Hide()
    {
        KillContentTweens(resetScale: true);

        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void PlayShow()
    {
        if (panelRoot == null)
            return;

        panelRoot.SetActive(true);

        Transform root = panelRoot.transform;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            child.DOKill();
            child.localScale = Vector3.zero;
            child.DOScale(Vector3.one, showDuration).SetEase(Ease.OutBack);
        }
    }

    private void KillContentTweens(bool resetScale)
    {
        if (panelRoot == null)
            return;

        Transform root = panelRoot.transform;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            child.DOKill();
            if (resetScale)
                child.localScale = Vector3.one;
        }
    }
}
