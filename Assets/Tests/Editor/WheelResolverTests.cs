using NUnit.Framework;
using UnityEngine;

public class WheelResolverTests
{
    private static WheelConfigSO Config(params WheelSliceData[] slices)
    {
        var config = ScriptableObject.CreateInstance<WheelConfigSO>();
        config.slices.AddRange(slices);
        return config;
    }

    [Test]
    public void NullOrEmpty_ReturnsNull()
    {
        var resolver = new WheelResolver();
        Assert.IsNull(resolver.Resolve(null, 0.5f));
        Assert.IsNull(resolver.Resolve(Config(), 0.5f));
    }

    [Test]
    public void DeterministicRoll_PicksExpectedSlice()
    {
        var a = new WheelSliceData { weight = 1f };
        var bomb = new WheelSliceData { weight = 1f, isBomb = true };
        var c = new WheelSliceData { weight = 2f };
        var config = Config(a, bomb, c);
        var resolver = new WheelResolver();

        Assert.AreSame(a, resolver.Resolve(config, 0f));
        Assert.AreSame(bomb, resolver.Resolve(config, 0.3f));
        Assert.AreSame(c, resolver.Resolve(config, 0.6f));
    }

    [Test]
    public void SafeConfig_NeverReturnsBomb()
    {
        var config = Config(
            new WheelSliceData { weight = 1f, isBomb = false },
            new WheelSliceData { weight = 2f, isBomb = false },
            new WheelSliceData { weight = 1f, isBomb = false }
        );
        var resolver = new WheelResolver();

        for (int i = 0; i < 500; i++)
        {
            WheelSliceData slice = resolver.Resolve(config);
            Assert.IsNotNull(slice);
            Assert.IsFalse(slice.isBomb);
        }
    }

    [Test]
    public void ZeroWeight_IsSkipped()
    {
        var skipped = new WheelSliceData { weight = 0f };
        var picked = new WheelSliceData { weight = 1f };
        var resolver = new WheelResolver();
        Assert.AreSame(picked, resolver.Resolve(Config(skipped, picked), 0f));
    }
}