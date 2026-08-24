using NUnit.Framework;
using UnityEngine;

public class RewardManagerTests
{
    private static RewardDataSO Currency(string id, int min, int max)
    {
        var data = ScriptableObject.CreateInstance<RewardDataSO>();
        data.id = id;
        data.displayName = id;
        data.rewardType = RewardType.Currency;
        data.minValue = min;
        data.maxValue = max;
        data.value = min;
        return data;
    }

    private static RewardDataSO Item(string id)
    {
        var data = ScriptableObject.CreateInstance<RewardDataSO>();
        data.id = id;
        data.displayName = id;
        data.rewardType = RewardType.SpecialItem;
        data.minValue = 1;
        data.maxValue = 1;
        data.value = 1;
        return data;
    }

    [Test]
    public void AddReward_IgnoresNull_And_StacksCurrencyById()
    {
        var rm = new RewardManager();
        rm.AddReward(null, 10);
        Assert.AreEqual(0, rm.Entries.Count);

        var gold = Currency("gold", 10, 30);
        rm.AddReward(gold, 20);
        rm.AddReward(gold, 15);
        Assert.AreEqual(1, rm.Entries.Count);
        Assert.AreEqual(35, rm.Entries[0].Amount);
        Assert.AreEqual(35, rm.TotalCurrency);
        Assert.IsFalse(rm.Entries[0].IsItemCount);
    }

    [Test]
    public void AddReward_StacksItemsByCount_AndKeepsSeparateRows()
    {
        var rm = new RewardManager();
        var chest = Item("chest_bronze");
        var gold = Currency("gold", 10, 30);

        rm.AddReward(chest, 1);
        rm.AddReward(gold, 30);
        rm.AddReward(chest, 1);

        Assert.AreEqual(2, rm.Entries.Count);
        Assert.AreEqual("chest_bronze", rm.Entries[0].Id);
        Assert.AreEqual(2, rm.Entries[0].Amount);
        Assert.IsTrue(rm.Entries[0].IsItemCount);
        Assert.AreEqual("gold", rm.Entries[1].Id);
        Assert.AreEqual(30, rm.Entries[1].Amount);
    }

    [Test]
    public void ClearAll_EmptiesInventory()
    {
        var rm = new RewardManager();
        rm.AddReward(Currency("cash", 500, 3000), 1000);
        rm.ClearAll();
        Assert.AreEqual(0, rm.Entries.Count);
        Assert.AreEqual(0, rm.TotalCurrency);
    }

    [Test]
    public void TrySpend_DeductsGold_AndFailsWhenInsufficient()
    {
        var rm = new RewardManager();
        var gold = Currency("gold", 10, 30);
        rm.AddReward(gold, 40);

        Assert.IsFalse(rm.TrySpend("gold", 50));
        Assert.AreEqual(40, rm.GetAmount("gold"));

        Assert.IsTrue(rm.TrySpend("gold", 25));
        Assert.AreEqual(15, rm.GetAmount("gold"));

        Assert.IsTrue(rm.TrySpend("gold", 15));
        Assert.AreEqual(0, rm.GetAmount("gold"));
        Assert.AreEqual(0, rm.Entries.Count);
    }
}
