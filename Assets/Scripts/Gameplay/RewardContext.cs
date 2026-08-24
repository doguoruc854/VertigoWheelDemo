public class RewardContext
{
    public RewardManager Rewards { get; }
    public RewardDataSO Data { get; }
    public int Amount { get; }

    public RewardContext(RewardManager rewards, RewardDataSO data, int amount)
    {
        Rewards = rewards;
        Data = data;
        Amount = amount;
    }
}
