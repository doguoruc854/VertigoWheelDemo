using NUnit.Framework;
using UnityEngine;

public class RewardScaleTests
{
    private static RewardDataSO Currency(int min, int max, int peak = 30)
    {
        var data = ScriptableObject.CreateInstance<RewardDataSO>();
        data.id = "cash";
        data.displayName = "Cash";
        data.rewardType = RewardType.Currency;
        data.minValue = min;
        data.maxValue = max;
        data.value = min;
        data.zoneScalePeak = peak;
        return data;
    }

    [Test]
    public void Zone1_KeepsOriginalMinAndMax()
    {
        var cash = Currency(500, 3000);
        cash.GetScaledRange(1, out int min, out int max);
        Assert.AreEqual(500, min);
        Assert.AreEqual(3000, max);
    }

    [Test]
    public void HigherZone_RaisesMin_ButMaxStaysFixed()
    {
        var gold = Currency(10, 30, peak: 30);
        gold.GetScaledRange(16, out int midMin, out int midMax);
        Assert.Greater(midMin, 10);
        Assert.AreEqual(30, midMax);

        gold.GetScaledRange(30, out int peakMin, out int peakMax);
        Assert.AreEqual(30, peakMin);
        Assert.AreEqual(30, peakMax);
    }

    [Test]
    public void BeyondPeak_DoesNotRaiseMax()
    {
        var cash = Currency(500, 3000, peak: 30);
        cash.GetScaledRange(100, out int min, out int max);
        Assert.AreEqual(3000, min);
        Assert.AreEqual(3000, max);
    }

    [Test]
    public void SpecialItem_IgnoresZoneScale()
    {
        var item = ScriptableObject.CreateInstance<RewardDataSO>();
        item.rewardType = RewardType.SpecialItem;
        item.minValue = 1;
        item.maxValue = 1;
        Assert.AreEqual(1, item.RollAmount(1));
        Assert.AreEqual(1, item.RollAmount(50));
    }
}
