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

    public int RollAmount()
    {
        if (rewardType == RewardType.SpecialItem)
            return 1;

        int min = minValue;
        int max = maxValue;
        if (max < min)
        {
            int swap = min;
            min = max;
            max = swap;
        }

        if (min == 0 && max == 0)
            return value;

        return Random.Range(min, max + 1);
    }
}
