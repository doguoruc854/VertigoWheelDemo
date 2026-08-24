using UnityEngine;

public class RewardContext
{
    public RewardManager Rewards { get; }
    public RewardDataSO Data { get; }
    public int Amount { get; }
    public Sprite Icon { get; }

    public RewardContext(RewardManager rewards, RewardDataSO data, int amount, Sprite icon = null)
    {
        Rewards = rewards;
        Data = data;
        Amount = amount;
        Icon = icon;
    }
}
