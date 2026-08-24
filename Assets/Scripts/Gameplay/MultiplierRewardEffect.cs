public class MultiplierRewardEffect : IRewardEffect
{
    public void Apply(RewardContext context)
    {
        if (context == null || context.Rewards == null || context.Data == null)
            return;

        context.Rewards.AddReward(context.Data, context.Amount > 0 ? context.Amount : 1);
    }
}
