public class CurrencyRewardEffect : IRewardEffect
{
    public void Apply(RewardContext context)
    {
        if (context == null || context.Rewards == null || context.Data == null)
            return;

        context.Rewards.AddReward(context.Data, context.Amount, context.Icon);
    }
}
