public class SpecialItemRewardEffect : IRewardEffect
{
    public void Apply(RewardContext context)
    {
        if (context == null || context.Rewards == null || context.Data == null)
            return;

        int amount = context.Amount > 0 ? context.Amount : 1;
        context.Rewards.AddReward(context.Data, amount);
    }
}
