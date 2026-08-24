using UnityEngine;

[CreateAssetMenu(fileName = "RewardData", menuName = "Vertigo/Reward Data")]
public class RewardDataSO : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public RewardType rewardType;
    public int value;
    public int minValue;
    public int maxValue;

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
