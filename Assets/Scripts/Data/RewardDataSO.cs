using UnityEngine;

[CreateAssetMenu(fileName = "RewardData", menuName = "Vertigo/Reward Data")]
public class RewardDataSO : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public Sprite[] iconVariants;
    public RewardType rewardType;
    public int value;
    public int minValue;
    public int maxValue;
    [Tooltip("By this zone, currency rolls use the full max (min rises toward max). Max never increases.")]
    public int zoneScalePeak = 30;

    public Sprite PickRandomIcon()
    {
        if (iconVariants != null && iconVariants.Length > 0)
        {
            int valid = 0;
            for (int i = 0; i < iconVariants.Length; i++)
            {
                if (iconVariants[i] != null)
                    valid++;
            }

            if (valid > 0)
            {
                int pick = Random.Range(0, valid);
                for (int i = 0; i < iconVariants.Length; i++)
                {
                    if (iconVariants[i] == null)
                        continue;
                    if (pick == 0)
                        return iconVariants[i];
                    pick--;
                }
            }
        }

        return icon;
    }

    public int RollAmount(int zone = 1)
    {
        if (rewardType == RewardType.SpecialItem)
            return 1;

        GetScaledRange(zone, out int min, out int max);

        if (min == 0 && max == 0)
            return value;

        return Random.Range(min, max + 1);
    }

    /// <summary>
    /// Raises the floor toward max as zone increases; max stays fixed at maxValue.
    /// Zone 1 = original min..max; at zoneScalePeak = max..max.
    /// </summary>
    public void GetScaledRange(int zone, out int scaledMin, out int scaledMax)
    {
        int min = minValue;
        int max = maxValue;
        if (max < min)
        {
            int swap = min;
            min = max;
            max = swap;
        }

        scaledMax = max;

        if (min == 0 && max == 0)
        {
            scaledMin = 0;
            return;
        }

        if (rewardType == RewardType.SpecialItem)
        {
            scaledMin = min;
            return;
        }

        int peak = Mathf.Max(2, zoneScalePeak);
        int z = Mathf.Max(1, zone);
        float t = Mathf.Clamp01((z - 1f) / (peak - 1f));
        scaledMin = Mathf.RoundToInt(Mathf.Lerp(min, max, t));
        if (scaledMin > scaledMax)
            scaledMin = scaledMax;
    }
}
