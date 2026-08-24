using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class WheelResolverTests
{
    private static WheelConfigSO Config(params WheelSliceData[] slices)
    {
        var config = ScriptableObject.CreateInstance<WheelConfigSO>();
        config.slices.AddRange(slices);
        return config;
    }

    private static void AssertNoBombSlices(WheelConfigSO config, string label)
    {
        Assert.IsNotNull(config, label + " missing");
        Assert.IsNotNull(config.slices, label + " slices null");
        Assert.Greater(config.slices.Count, 0, label + " empty");

        for (int i = 0; i < config.slices.Count; i++)
        {
            Assert.IsFalse(
                config.slices[i].isBomb,
                label + " slice " + i + " must not be bomb");
        }
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
    public void ProductionSafeAndSuperAssets_ContainNoBombSlices()
    {
        var safe = AssetDatabase.LoadAssetAtPath<WheelConfigSO>(
            "Assets/ScriptableObjects/WheelConfigs/WheelConfig_Safe.asset");
        var super = AssetDatabase.LoadAssetAtPath<WheelConfigSO>(
            "Assets/ScriptableObjects/WheelConfigs/WheelConfig_Super.asset");

        AssertNoBombSlices(safe, "WheelConfig_Safe");
        AssertNoBombSlices(super, "WheelConfig_Super");
    }

    [Test]
    public void ProductionNormalAsset_ContainsAtLeastOneBombSlice()
    {
        var normal = AssetDatabase.LoadAssetAtPath<WheelConfigSO>(
            "Assets/ScriptableObjects/WheelConfigs/WheelConfig_Test_Normal.asset");
        Assert.IsNotNull(normal);
        Assert.IsNotNull(normal.slices);

        bool hasBomb = false;
        for (int i = 0; i < normal.slices.Count; i++)
        {
            if (normal.slices[i].isBomb)
            {
                hasBomb = true;
                break;
            }
        }

        Assert.IsTrue(hasBomb, "Normal config should include at least one bomb slice");
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
