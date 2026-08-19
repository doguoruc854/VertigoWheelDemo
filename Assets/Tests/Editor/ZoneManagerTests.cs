using NUnit.Framework;

public class ZoneManagerTests
{
    private static ZoneManager AtZone(int zone)
    {
        var zm = new ZoneManager();
        for (int i = 1; i < zone; i++)
            zm.AdvanceZone();
        return zm;
    }

    [Test]
    public void Zones_1_2_3_AreNormal()
    {
        Assert.AreEqual(ZoneType.Normal, AtZone(1).CurrentType);
        Assert.AreEqual(ZoneType.Normal, AtZone(2).CurrentType);
        Assert.AreEqual(ZoneType.Normal, AtZone(3).CurrentType);
    }

    [Test]
    public void Zones_5_10_15_AreSafe()
    {
        Assert.AreEqual(ZoneType.Safe, AtZone(5).CurrentType);
        Assert.AreEqual(ZoneType.Safe, AtZone(10).CurrentType);
        Assert.AreEqual(ZoneType.Safe, AtZone(15).CurrentType);
        Assert.IsFalse(AtZone(5).IsSuperZone);
    }

    [Test]
    public void Zones_30_60_AreSuper_NotSafe()
    {
        Assert.AreEqual(ZoneType.Super, AtZone(30).CurrentType);
        Assert.AreEqual(ZoneType.Super, AtZone(60).CurrentType);
        Assert.IsFalse(AtZone(30).IsSafeZone);
        Assert.IsTrue(AtZone(30).IsSuperZone);
    }

    [Test]
    public void Reset_ReturnsToZone1()
    {
        var zm = AtZone(10);
        zm.Reset();
        Assert.AreEqual(1, zm.CurrentZone);
        Assert.AreEqual(ZoneType.Normal, zm.CurrentType);
    }
}