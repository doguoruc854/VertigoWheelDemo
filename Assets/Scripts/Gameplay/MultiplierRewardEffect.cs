public class MultiplierRewardEffect : IRewardEffect {
    public void Apply(RewardContext context) {
        if (context == null || context.Rewards == null)
            return;

        context.Rewards.AddReward(context.Data);
    }
}