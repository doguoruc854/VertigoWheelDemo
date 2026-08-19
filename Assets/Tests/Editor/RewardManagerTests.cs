using NUnit.Framework;
using UnityEngine;

public class RewardManagerTests
{
    private static RewardDataSO Currency(int value)
    {
        var data = ScriptableObject.CreateInstance<RewardDataSO>();
        data.rewardType = RewardType.Currency;
        data.value = value;
        return data;
    }

    [Test]
    public void AddReward_IgnoresNull_And_SumsCurrency()
    {
        var rm = new RewardManager();
        rm.AddReward(null);
        Assert.AreEqual(0, rm.Collected.Count);

        rm.AddReward(Currency(100));
        rm.AddReward(Currency(50));
        Assert.AreEqual(2, rm.Collected.Count);
        Assert.AreEqual(150, rm.TotalCurrency);
    }

    [Test]
    public void ClearAll_EmptiesInventory()
    {
        var rm = new RewardManager();
        rm.AddReward(Currency(10));
        rm.ClearAll();
        Assert.AreEqual(0, rm.Collected.Count);
        Assert.AreEqual(0, rm.TotalCurrency);
    }
}