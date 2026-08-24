using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BombReviveUIController : MonoBehaviour
{
    [SerializeField] private RectTransform panelRoot;
    [SerializeField] private Sprite deathIcon;
    [SerializeField] private Sprite goldIcon;
    [SerializeField] private Sprite greyButtonSprite;
    [SerializeField] private int reviveCost = 25;
    [SerializeField] private float showDuration = 0.3f;
    [SerializeField] private Vector2 deathIconSize = new Vector2(520f, 640f);
    [SerializeField] private Vector2 buttonSize = new Vector2(420f, 140f);

    public event Action GiveUpClicked;
    public event Action ReviveClicked;

    private CanvasGroup _canvasGroup;
    private Button _giveUpButton;
    private Button _reviveButton;
    private TextMeshProUGUI _reviveCostText;
    private bool _built;

    private void Awake()
    {
        _built = false;
        EnsureBuilt();
        SetVisible(false, instant: true);
    }

    private void OnDisable()
    {
        if (panelRoot != null)
            panelRoot.DOKill();
    }

    public void Show(int availableGold)
    {
        EnsureBuilt();
        if (panelRoot == null)
            return;

        if (_reviveCostText != null)
            _reviveCostText.text = reviveCost.ToString();

        if (_reviveButton != null)
            _reviveButton.interactable = availableGold >= reviveCost;

        SetVisible(true, instant: false);
    }

    public void Hide()
    {
        SetVisible(false, instant: true);
    }

    public int ReviveCost => reviveCost;

    private void SetVisible(bool visible, bool instant)
    {
        if (panelRoot == null)
            return;

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        panelRoot.DOKill();

        if (_canvasGroup == null)
            _canvasGroup = panelRoot.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = panelRoot.gameObject.AddComponent<CanvasGroup>();

        _canvasGroup.blocksRaycasts = visible;
        _canvasGroup.interactable = visible;

        if (!visible)
        {
            _canvasGroup.alpha = 0f;
            panelRoot.localScale = Vector3.one;
            return;
        }

        if (instant)
        {
            _canvasGroup.alpha = 1f;
            panelRoot.localScale = Vector3.one;
            return;
        }

        _canvasGroup.alpha = 1f;
        panelRoot.localScale = Vector3.one * 0.85f;
        panelRoot.DOScale(Vector3.one, showDuration).SetEase(Ease.OutBack);
    }

    private void EnsureBuilt()
    {
        if (_built)
            return;
        _built = true;

        if (panelRoot == null)
            panelRoot = transform as RectTransform;

        for (int i = panelRoot.childCount - 1; i >= 0; i--)
            Destroy(panelRoot.GetChild(i).gameObject);

        Image dim = panelRoot.GetComponent<Image>();
        if (dim == null)
            dim = panelRoot.gameObject.AddComponent<Image>();
        dim.sprite = null;
        dim.type = Image.Type.Simple;
        dim.preserveAspect = false;
        dim.color = new Color(0.04f, 0.02f, 0.02f, 1f);
        dim.raycastTarget = true;

        _canvasGroup = panelRoot.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = panelRoot.gameObject.AddComponent<CanvasGroup>();

        var title = CreateTmp(panelRoot, "ui_text_bomb_title", 52f, FontStyles.Bold,
            "OH NO, A BOMB EXPLODED RIGHT IN YOUR HANDS!");
        SetCenterRect(title.rectTransform, new Vector2(0f, 720f), new Vector2(960f, 160f));

        var subtitle = CreateTmp(panelRoot, "ui_text_bomb_subtitle", 36f, FontStyles.Normal,
            "Revive yourself to keep your rewards.");
        subtitle.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        SetCenterRect(subtitle.rectTransform, new Vector2(0f, 580f), new Vector2(900f, 80f));

        var death = CreateImage(panelRoot, "ui_image_bomb_death_icon", deathIcon);
        SetCenterRect(death.rectTransform, new Vector2(0f, 40f), deathIconSize);
        death.type = Image.Type.Simple;
        death.preserveAspect = true;
        death.color = Color.white;
        death.raycastTarget = false;
        death.enabled = deathIcon != null;

        var buttons = new GameObject("ui_group_bomb_buttons", typeof(RectTransform));
        buttons.transform.SetParent(panelRoot, false);
        SetCenterRect(buttons.GetComponent<RectTransform>(), new Vector2(0f, -700f), new Vector2(980f, 180f));

        _giveUpButton = CreateButton(buttons.transform, "ui_button_giveup", greyButtonSprite,
            new Color(0.55f, 0.55f, 0.58f, 1f));
        SetCenterRect(_giveUpButton.GetComponent<RectTransform>(), new Vector2(-240f, 0f), buttonSize);
        var giveUpLabel = CreateTmp(_giveUpButton.transform, "ui_text_giveup", 40f, FontStyles.Bold, "GIVE UP");
        SetCenterRect(giveUpLabel.rectTransform, Vector2.zero, buttonSize);
        _giveUpButton.onClick.AddListener(() => GiveUpClicked?.Invoke());

        _reviveButton = CreateButton(buttons.transform, "ui_button_revive", greyButtonSprite,
            new Color(0.18f, 0.72f, 0.28f, 1f));
        SetCenterRect(_reviveButton.GetComponent<RectTransform>(), new Vector2(240f, 0f), buttonSize);

        var reviveInner = new GameObject("ui_group_revive_content", typeof(RectTransform));
        reviveInner.transform.SetParent(_reviveButton.transform, false);
        SetCenterRect(reviveInner.GetComponent<RectTransform>(), Vector2.zero, buttonSize);

        var costRow = new GameObject("ui_group_revive_cost", typeof(RectTransform));
        costRow.transform.SetParent(reviveInner.transform, false);
        SetCenterRect(costRow.GetComponent<RectTransform>(), new Vector2(0f, 28f), new Vector2(280f, 56f));

        var coin = CreateImage(costRow.transform, "ui_image_revive_gold", goldIcon);
        var coinRt = coin.rectTransform;
        coinRt.anchorMin = new Vector2(0.5f, 0.5f);
        coinRt.anchorMax = new Vector2(0.5f, 0.5f);
        coinRt.pivot = new Vector2(1f, 0.5f);
        coinRt.sizeDelta = new Vector2(48f, 48f);
        coinRt.anchoredPosition = new Vector2(-8f, 0f);
        coin.type = Image.Type.Simple;
        coin.preserveAspect = true;
        coin.raycastTarget = false;
        coin.enabled = goldIcon != null;

        _reviveCostText = CreateTmp(costRow.transform, "ui_text_revive_cost", 40f, FontStyles.Bold, reviveCost.ToString());
        var costRt = _reviveCostText.rectTransform;
        costRt.anchorMin = new Vector2(0.5f, 0.5f);
        costRt.anchorMax = new Vector2(0.5f, 0.5f);
        costRt.pivot = new Vector2(0f, 0.5f);
        costRt.sizeDelta = new Vector2(120f, 56f);
        costRt.anchoredPosition = new Vector2(8f, 0f);
        _reviveCostText.alignment = TextAlignmentOptions.MidlineLeft;

        var reviveLabel = CreateTmp(reviveInner.transform, "ui_text_revive", 44f, FontStyles.Bold, "REVIVE");
        SetCenterRect(reviveLabel.rectTransform, new Vector2(0f, -28f), new Vector2(360f, 56f));

        _reviveButton.onClick.AddListener(() => ReviveClicked?.Invoke());
    }

    private static void SetCenterRect(RectTransform rt, Vector2 anchoredPos, Vector2 size)
    {
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        rt.localScale = Vector3.one;
    }

    private static Image CreateImage(Transform parent, string name, Sprite sprite)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        var img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.type = Image.Type.Simple;
        img.preserveAspect = true;
        return img;
    }

    private static Button CreateButton(Transform parent, string name, Sprite sprite, Color tint)
    {
        var img = CreateImage(parent, name, sprite);
        img.color = tint;
        img.raycastTarget = true;
        // 9-slice keeps button corners; does not distort artwork like stretch-fill.
        if (sprite != null)
            img.type = Image.Type.Sliced;
        img.preserveAspect = false;
        return img.gameObject.AddComponent<Button>();
    }

    private static TextMeshProUGUI CreateTmp(Transform parent, string name, float size, FontStyles style, string text)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        tmp.enableWordWrapping = true;
        tmp.overflowMode = TextOverflowModes.Overflow;
        return tmp;
    }
}
