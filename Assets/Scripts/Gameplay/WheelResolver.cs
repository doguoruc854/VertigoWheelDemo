using UnityEngine;

public class WheelResolver
{
    public WheelSliceData Resolve(WheelConfigSO config)
    {
        return Resolve(config, Random.value);
    }

    public WheelSliceData Resolve(WheelConfigSO config, float roll01)
    {
        if (config == null || config.slices == null || config.slices.Count == 0)
            return null;

        float total = 0f;
        for (int i = 0; i < config.slices.Count; i++)
        {
            WheelSliceData slice = config.slices[i];
            if (slice != null && slice.weight > 0f)
                total += slice.weight;
        }

        if (total <= 0f)
            return null;

        float clamped = Mathf.Clamp01(roll01);
        float point = clamped * total;
        if (point >= total)
            point = total - 0.0001f;

        float cumulative = 0f;
        for (int i = 0; i < config.slices.Count; i++)
        {
            WheelSliceData slice = config.slices[i];
            if (slice == null || slice.weight <= 0f)
                continue;

            cumulative += slice.weight;
            if (point < cumulative)
                return slice;
        }

        return null;
    }
}