using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private float rowHeight = 140f;
    [SerializeField] private float iconSize = 110f;
    [SerializeField] private float fontSize = 56f;

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

        for (int i = 0; i < entries.Count; i++)
            CreateRow(entries[i], i);
    }

    private void ClearRows()
    {
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
            Destroy(contentRoot.GetChild(i).gameObject);

        _rows.Clear();
    }

    private void CreateRow(InventoryEntry entry, int index)
    {
        var row = new GameObject($"ui_inventory_row_{index}", typeof(RectTransform));
        row.transform.SetParent(contentRoot, false);

        var rowRt = row.GetComponent<RectTransform>();
        rowRt.anchorMin = new Vector2(0f, 1f);
        rowRt.anchorMax = new Vector2(1f, 1f);
        rowRt.pivot = new Vector2(0.5f, 1f);
        rowRt.sizeDelta = new Vector2(0f, rowHeight);
        rowRt.anchoredPosition = new Vector2(0f, -index * rowHeight);

        var iconGo = new GameObject("ui_image_inventory_icon_value", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconGo.transform.SetParent(row.transform, false);
        var iconRt = iconGo.GetComponent<RectTransform>();
        iconRt.anchorMin = new Vector2(0f, 0.5f);
        iconRt.anchorMax = new Vector2(0f, 0.5f);
        iconRt.pivot = new Vector2(0f, 0.5f);
        iconRt.sizeDelta = new Vector2(iconSize, iconSize);
        iconRt.anchoredPosition = new Vector2(12f, 0f);

        var iconImg = iconGo.GetComponent<Image>();
        iconImg.preserveAspect = true;
        iconImg.raycastTarget = false;
        iconImg.sprite = entry.Icon;
        iconImg.enabled = entry.Icon != null;

        var textGo = new GameObject("ui_text_inventory_amount_value", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textGo.transform.SetParent(row.transform, false);
        var textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0f, 0f);
        textRt.anchorMax = new Vector2(1f, 1f);
        textRt.offsetMin = new Vector2(iconSize + 24f, 8f);
        textRt.offsetMax = new Vector2(-12f, -8f);

        var tmp = textGo.GetComponent<TextMeshProUGUI>();
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        tmp.text = entry.IsItemCount ? ("x" + entry.Amount) : entry.Amount.ToString();

        _rows.Add(row);
    }
}
