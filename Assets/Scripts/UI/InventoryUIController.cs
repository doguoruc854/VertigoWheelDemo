using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private float cellWidth = 200f;
    [SerializeField] private float cellHeight = 140f;
    [SerializeField] private float iconSize = 96f;
    [SerializeField] private float fontSize = 42f;

    private readonly List<GameObject> _rows = new List<GameObject>();

    private void OnValidate()
    {
        if (contentRoot == null)
        {
            var t = transform.Find("ui_group_inventory_content");
            if (t != null)
                contentRoot = t as RectTransform;
            if (contentRoot == null)
                contentRoot = transform as RectTransform;
        }
    }

    public void Refresh(IReadOnlyList<InventoryEntry> entries)
    {
        if (contentRoot == null)
            return;

        ClearRows();

        if (entries == null || entries.Count == 0)
            return;

        ResolveCellLayout(out float useCellW, out float useCellH, out float useIcon, out float useFont);

        float totalWidth = entries.Count * useCellW;
        float startX = -totalWidth * 0.5f + useCellW * 0.5f;

        for (int i = 0; i < entries.Count; i++)
            CreateCell(entries[i], i, startX + i * useCellW, useCellW, useCellH, useIcon, useFont);
    }

    private void ResolveCellLayout(out float useCellW, out float useCellH, out float useIcon, out float useFont)
    {
        float panelH = contentRoot.rect.height;
        if (panelH < 1f)
        {
            var panel = transform as RectTransform;
            if (panel != null)
                panelH = panel.rect.height;
        }

        if (panelH < 1f)
            panelH = cellHeight;

        useIcon = Mathf.Max(iconSize, panelH * 0.62f);
        useFont = Mathf.Max(fontSize, panelH * 0.32f);
        useCellH = Mathf.Max(cellHeight, panelH * 0.95f);
        useCellW = Mathf.Max(cellWidth, useIcon + 56f);
    }

    private void ClearRows()
    {
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
            Destroy(contentRoot.GetChild(i).gameObject);

        _rows.Clear();
    }

    private void CreateCell(
        InventoryEntry entry,
        int index,
        float x,
        float useCellW,
        float useCellH,
        float useIcon,
        float useFont)
    {
        var cell = new GameObject($"ui_inventory_cell_{index}", typeof(RectTransform));
        cell.transform.SetParent(contentRoot, false);

        var cellRt = cell.GetComponent<RectTransform>();
        cellRt.anchorMin = new Vector2(0.5f, 0.5f);
        cellRt.anchorMax = new Vector2(0.5f, 0.5f);
        cellRt.pivot = new Vector2(0.5f, 0.5f);
        cellRt.sizeDelta = new Vector2(useCellW, useCellH);
        cellRt.anchoredPosition = new Vector2(x, 0f);

        var iconGo = new GameObject("ui_image_inventory_icon_value", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconGo.transform.SetParent(cell.transform, false);
        var iconRt = iconGo.GetComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0.5f, 1f);
        iconRt.anchorMax = new Vector2(0.5f, 1f);
        iconRt.pivot = new Vector2(0.5f, 1f);
        iconRt.sizeDelta = new Vector2(useIcon, useIcon);
        iconRt.anchoredPosition = new Vector2(0f, -4f);

        var iconImg = iconGo.GetComponent<Image>();
        iconImg.preserveAspect = true;
        iconImg.raycastTarget = false;
        iconImg.sprite = entry.Icon;
        iconImg.enabled = entry.Icon != null;

        var textGo = new GameObject("ui_text_inventory_amount_value", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textGo.transform.SetParent(cell.transform, false);
        var textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0f, 0f);
        textRt.anchorMax = new Vector2(1f, 0f);
        textRt.pivot = new Vector2(0.5f, 0f);
        textRt.sizeDelta = new Vector2(0f, useFont + 8f);
        textRt.anchoredPosition = new Vector2(0f, 4f);

        var tmp = textGo.GetComponent<TextMeshProUGUI>();
        tmp.fontSize = useFont;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        tmp.text = entry.IsItemCount ? ("x" + entry.Amount) : entry.Amount.ToString();

        _rows.Add(cell);
    }
}
