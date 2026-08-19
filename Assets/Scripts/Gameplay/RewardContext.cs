public class RewardContext {
    public RewardManager Rewards { get; }
    public RewardDataSO Data { get;}
    
    public RewardContext(RewardManager rewards, RewardDataSO data) {
        Rewards = rewards;
        Data = data;
    }
}