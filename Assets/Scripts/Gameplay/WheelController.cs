using System;
using System.Collections;
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

    private Image[] _slotImages;
    private bool _spinning;

    public bool IsSpinning => _spinning;

    private void Start()
    {
        BuildSlots(config);
    }

    public void BuildSlots(WheelConfigSO wheelConfig)
    {
        config = wheelConfig;
        ClearSlots();

        if (config == null || config.slices == null || config.slices.Count == 0)
            return;

        int count = config.slices.Count;
        _slotImages = new Image[count];
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            var go = new GameObject($"ui_image_slot_{i}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(slotsParent, false);

            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = slotSize;

            float angleDeg = i * step;
            float rad = (angleDeg - 90f) * Mathf.Deg2Rad;
            rt.anchoredPosition = new Vector2(Mathf.Cos(rad) * slotRadius, Mathf.Sin(rad) * slotRadius);

            var img = go.GetComponent<Image>();
            img.preserveAspect = true;
            img.raycastTarget = false;
            ApplySliceVisual(img, config.slices[i]);
            _slotImages[i] = img;
        }
    }

    private void ApplySliceVisual(Image img, WheelSliceData slice)
    {
        if (slice != null && slice.isBomb)
        {
            img.sprite = bombIcon;
            img.enabled = bombIcon != null;
            return;
        }

        if (slice != null && slice.reward != null && slice.reward.icon != null)
        {
            img.sprite = slice.reward.icon;
            img.enabled = true;
            return;
        }

        img.sprite = null;
        img.enabled = false;
    }

    private void ClearSlots()
    {
        if (slotsParent == null)
            return;

        for (int i = slotsParent.childCount - 1; i >= 0; i--)
            Destroy(slotsParent.GetChild(i).gameObject);

        _slotImages = null;
    }

    public void SpinToIndex(int index, Action onComplete)
    {
        if (_spinning || spinRoot == null || _slotImages == null || _slotImages.Length == 0)
        {
            onComplete?.Invoke();
            return;
        }

        index = Mathf.Clamp(index, 0, _slotImages.Length - 1);
        StopAllCoroutines();
        StartCoroutine(SpinRoutine(index, onComplete));
    }

    private IEnumerator SpinRoutine(int index, Action onComplete)
    {
        _spinning = true;

        float step = 360f / _slotImages.Length;
        float startZ = spinRoot.localEulerAngles.z;
        float targetZ = -index * step - fullRotations * 360f;

        while (targetZ > startZ)
            targetZ -= 360f;

        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / spinDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            float z = Mathf.Lerp(startZ, targetZ, eased);
            spinRoot.localEulerAngles = new Vector3(0f, 0f, z);
            yield return null;
        }

        spinRoot.localEulerAngles = new Vector3(0f, 0f, targetZ % 360f);
        _spinning = false;
        onComplete?.Invoke();
    }

    [ContextMenu("Debug Spin Index 0")]
    private void DebugSpin0()
    {
        if (!Application.isPlaying)
            return;
        SpinToIndex(0, () => Debug.Log("Spin done → index 0"));
    }
}