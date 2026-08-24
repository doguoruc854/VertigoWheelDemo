using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaveSuccessUIController : MonoBehaviour
{
    [SerializeField] private RectTransform panelRoot;
    [SerializeField] private Sprite greyButtonSprite;
    [SerializeField] private float showDuration = 0.3f;
    [SerializeField] private Vector2 buttonSize = new Vector2(480f, 140f);
    [SerializeField] private float rewardCellWidth = 180f;
    [SerializeField] private float rewardIconSize = 96f;
    [SerializeField] private float rewardFontSize = 40f;

    public event Action ContinueClicked;

    private CanvasGroup _canvasGroup;
    private RectTransform _rewardsRoot;
    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _subtitleText;
    private readonly List<GameObject> _rewardCells = new List<GameObject>();
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

    public void Show(IReadOnlyList<InventoryEntry> entries, int zone)
    {
        EnsureBuilt();
        if (panelRoot == null)
            return;

        if (_titleText != null)
            _titleText.text = "YOU CASHED OUT!";

        if (_subtitleText != null)
            _subtitleText.text = "Zone " + zone + " — here is what you kept.";

        RebuildRewards(entries);
        SetVisible(true, instant: false);
    }

    public void Hide()
    {
        SetVisible(false, instant: true);
    }

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

        _canvasGroup.alpha = 1f;
        if (instant)
        {
            panelRoot.localScale = Vector3.one;
            return;
        }

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
        dim.color = new Color(0.03f, 0.06f, 0.04f, 1f);
        dim.raycastTarget = true;

        _canvasGroup = panelRoot.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = panelRoot.gameObject.AddComponent<CanvasGroup>();

        _titleText = CreateTmp(panelRoot, "ui_text_leave_title", 56f, FontStyles.Bold, "YOU CASHED OUT!");
        SetCenterRect(_titleText.rectTransform, new Vector2(0f, 720f), new Vector2(960f, 140f));

        _subtitleText = CreateTmp(panelRoot, "ui_text_leave_subtitle", 36f, FontStyles.Normal,
            "Here is what you kept.");
        _subtitleText.color = new Color(0.9f, 0.95f, 0.9f, 1f);
        SetCenterRect(_subtitleText.rectTransform, new Vector2(0f, 580f), new Vector2(900f, 80f));

        var rewardsGo = new GameObject("ui_group_leave_rewards", typeof(RectTransform));
        rewardsGo.transform.SetParent(panelRoot, false);
        _rewardsRoot = rewardsGo.GetComponent<RectTransform>();
        SetCenterRect(_rewardsRoot, new Vector2(0f, 80f), new Vector2(1000f, 420f));

        var continueButton = CreateButton(panelRoot, "ui_button_leave_continue", greyButtonSprite,
            new Color(0.18f, 0.72f, 0.28f, 1f));
        SetCenterRect(continueButton.GetComponent<RectTransform>(), new Vector2(0f, -700f), buttonSize);
        var continueLabel = CreateTmp(continueButton.transform, "ui_text_leave_continue", 44f, FontStyles.Bold, "CONTINUE");
        SetCenterRect(continueLabel.rectTransform, Vector2.zero, buttonSize);
        continueButton.onClick.AddListener(() => ContinueClicked?.Invoke());
    }

    private void RebuildRewards(IReadOnlyList<InventoryEntry> entries)
    {
        for (int i = 0; i < _rewardCells.Count; i++)
        {
            if (_rewardCells[i] != null)
                Destroy(_rewardCells[i]);
        }
        _rewardCells.Clear();

        if (_rewardsRoot == null)
            return;

        for (int i = _rewardsRoot.childCount - 1; i >= 0; i--)
            Destroy(_rewardsRoot.GetChild(i).gameObject);

        if (entries == null || entries.Count == 0)
        {
            var empty = CreateTmp(_rewardsRoot, "ui_text_leave_empty", 40f, FontStyles.Normal, "No rewards collected.");
            SetCenterRect(empty.rectTransform, Vector2.zero, new Vector2(800f, 80f));
            return;
        }

        int count = entries.Count;
        int columns = Mathf.Min(4, count);
        int rows = Mathf.CeilToInt(count / (float)columns);
        float cellW = rewardCellWidth;
        float cellH = rewardIconSize + rewardFontSize + 24f;
        float totalW = columns * cellW;
        float totalH = rows * cellH;
        float startX = -totalW * 0.5f + cellW * 0.5f;
        float startY = totalH * 0.5f - cellH * 0.5f;

        for (int i = 0; i < count; i++)
        {
            int col = i % columns;
            int row = i / columns;
            float x = startX + col * cellW;
            float y = startY - row * cellH;
            _rewardCells.Add(CreateRewardCell(entries[i], i, new Vector2(x, y), cellW, cellH));
        }
    }

    private GameObject CreateRewardCell(InventoryEntry entry, int index, Vector2 pos, float cellW, float cellH)
    {
        var cell = new GameObject($"ui_leave_reward_cell_{index}", typeof(RectTransform));
        cell.transform.SetParent(_rewardsRoot, false);
        SetCenterRect(cell.GetComponent<RectTransform>(), pos, new Vector2(cellW, cellH));

        var icon = CreateImage(cell.transform, "ui_image_leave_reward_icon_value", entry.Icon);
        SetCenterRect(icon.rectTransform, new Vector2(0f, 24f), new Vector2(rewardIconSize, rewardIconSize));
        icon.type = Image.Type.Simple;
        icon.preserveAspect = true;
        icon.raycastTarget = false;
        icon.enabled = entry.Icon != null;

        string amountText = entry.IsItemCount ? ("x" + entry.Amount) : entry.Amount.ToString();
        var amount = CreateTmp(cell.transform, "ui_text_leave_reward_amount_value", rewardFontSize, FontStyles.Bold, amountText);
        SetCenterRect(amount.rectTransform, new Vector2(0f, -rewardIconSize * 0.5f - 8f), new Vector2(cellW, rewardFontSize + 12f));

        return cell;
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
        if (sprite != null)
        {
            img.type = Image.Type.Sliced;
            img.preserveAspect = false;
        }
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
