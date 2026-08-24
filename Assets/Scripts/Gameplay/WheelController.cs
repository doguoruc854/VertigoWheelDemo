using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WheelController : MonoBehaviour
{
    [SerializeField] private RectTransform spinRoot;
    [SerializeField] private RectTransform slotsParent;
    [SerializeField] private WheelConfigSO config;
    [SerializeField] private Sprite bombIcon;
    [SerializeField] private float slotRadius = 220f;
    [SerializeField] private Vector2 slotSize = new Vector2(96f, 96f);
    [SerializeField] private float spinDuration = 2.5f;
    [SerializeField] private int fullRotations = 4;
    [SerializeField] private Image baseImage;
    [SerializeField] private Sprite bronzeBase;
    [SerializeField] private Sprite silverBase;
    [SerializeField] private Sprite goldenBase;
    [SerializeField] private Image indicatorImage;
    [SerializeField] private Sprite bronzeIndicator;
    [SerializeField] private Sprite silverIndicator;
    [SerializeField] private Sprite goldenIndicator;

    private Image[] _slotImages;
    private Sprite[] _slotIcons;
    private bool _spinning;

    public bool IsSpinning => _spinning;

    public Sprite GetSlotIcon(int index)
    {
        if (_slotIcons == null || index < 0 || index >= _slotIcons.Length)
            return null;
        return _slotIcons[index];
    }

    public void BuildSlots(WheelConfigSO wheelConfig)
    {
        config = wheelConfig;
        ClearSlots();

        if (config == null || config.slices == null || config.slices.Count == 0)
            return;

        int count = config.slices.Count;
        _slotImages = new Image[count];
        _slotIcons = new Sprite[count];
        float step = 360f / count;
        ResolveSlotLayout(out float radius, out Vector2 size);

        for (int i = 0; i < count; i++)
        {
            var go = new GameObject($"ui_image_slot_{i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(slotsParent, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = size;

            float angleDeg = i * step;
            float rad = (angleDeg + 90f) * Mathf.Deg2Rad;
            rt.anchoredPosition = new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);

            var img = go.GetComponent<Image>();
            img.preserveAspect = true;
            img.raycastTarget = false;
            Sprite display = ResolveSliceIcon(config.slices[i]);
            _slotIcons[i] = display;
            img.sprite = display;
            img.enabled = display != null;
            _slotImages[i] = img;
        }
    }

    private void ResolveSlotLayout(out float radius, out Vector2 size)
    {
        float wheelSize = 0f;
        if (spinRoot != null)
        {
            wheelSize = Mathf.Min(spinRoot.rect.width, spinRoot.rect.height);
            if (wheelSize < 1f)
                wheelSize = Mathf.Min(Mathf.Abs(spinRoot.sizeDelta.x), Mathf.Abs(spinRoot.sizeDelta.y));
        }

        if (wheelSize < 1f)
        {
            radius = slotRadius;
            size = slotSize;
            return;
        }

        radius = wheelSize * 0.306f;
        float icon = wheelSize * 0.122f;
        size = new Vector2(icon, icon);
    }

    private Sprite ResolveSliceIcon(WheelSliceData slice)
    {
        if (slice != null && slice.isBomb)
            return bombIcon;

        if (slice != null && slice.reward != null)
            return slice.reward.PickRandomIcon();

        return null;
    }

    private void ClearSlots()
    {
        if (slotsParent == null)
            return;

        for (int i = slotsParent.childCount - 1; i >= 0; i--)
            Destroy(slotsParent.GetChild(i).gameObject);

        _slotImages = null;
        _slotIcons = null;
    }

    public void SpinToIndex(int index, Action onComplete)
    {
        if (_spinning || spinRoot == null || _slotImages == null || _slotImages.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }

        index = Mathf.Clamp(index, 0, _slotImages.Length - 1);
        spinRoot.DOKill();

        _spinning = true;

        float step = 360f / _slotImages.Length;
        float startZ = spinRoot.localEulerAngles.z;
        float targetZ = -index * step - fullRotations * 360f;

        while (targetZ > startZ)
            targetZ -= 360f;

        spinRoot
            .DOLocalRotate(new Vector3(0f, 0f, targetZ), spinDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                spinRoot.localEulerAngles = new Vector3(0f, 0f, Mathf.Repeat(targetZ, 360f));
                _spinning = false;
                onComplete?.Invoke();
            });
    }

    private void OnDisable()
    {
        if (spinRoot != null)
            spinRoot.DOKill();
        _spinning = false;
    }

    [ContextMenu("Debug Spin Index 0")]
    private void DebugSpin0()
    {
        if (!Application.isPlaying)
            return;
        SpinToIndex(0, () => Debug.Log("Spin done → index 0"));
    }

    public void ApplyZoneLook(ZoneType zoneType)
    {
        Sprite baseSprite = bronzeBase;
        Sprite indicatorSprite = bronzeIndicator;

        if (zoneType == ZoneType.Super)
        {
            if (goldenBase != null)
                baseSprite = goldenBase;
            if (goldenIndicator != null)
                indicatorSprite = goldenIndicator;
        }
        else if (zoneType == ZoneType.Safe)
        {
            if (silverBase != null)
                baseSprite = silverBase;
            if (silverIndicator != null)
                indicatorSprite = silverIndicator;
        }

        if (baseImage != null && baseSprite != null)
            baseImage.sprite = baseSprite;

        if (indicatorImage != null && indicatorSprite != null)
            indicatorImage.sprite = indicatorSprite;
    }
}

