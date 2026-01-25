using YeelightAPI.Core;
using YeelightAPI.Models;

namespace YeelightAPI.UnitTests;

public class PropertiesExtensionsTests
{
    #region GetRealNames Tests

    [Test]
    public async Task GetRealNames_SingleProperty_ReturnsOneName()
    {
        var properties = PROPERTIES.power;

        var names = properties.GetRealNames();

        await Assert.That(names).HasSingleItem();
        await Assert.That(names[0]).IsEqualTo("power");
    }

    [Test]
    public async Task GetRealNames_MultipleProperties_ReturnsAllNames()
    {
        var properties = PROPERTIES.power | PROPERTIES.bright | PROPERTIES.ct;

        var names = properties.GetRealNames();

        await Assert.That(names.Count).IsEqualTo(3);
        await Assert.That(names.Cast<string>()).Contains("power");
        await Assert.That(names.Cast<string>()).Contains("bright");
        await Assert.That(names.Cast<string>()).Contains("ct");
    }

    [Test]
    public async Task GetRealNames_NONE_ReturnsEmpty()
    {
        var properties = PROPERTIES.NONE;

        var names = properties.GetRealNames();

        await Assert.That(names).IsEmpty();
    }

    [Test]
    public async Task GetRealNames_ALL_DoesNotIncludeALLOrNONE()
    {
        var properties = PROPERTIES.ALL;

        var names = properties.GetRealNames();

        await Assert.That(names.Cast<string>()).DoesNotContain("ALL");
        await Assert.That(names.Cast<string>()).DoesNotContain("NONE");
    }

    [Test]
    public async Task GetRealNames_ALL_IncludesAllRealProperties()
    {
        var properties = PROPERTIES.ALL;

        var names = properties.GetRealNames();

        // Should include all individual properties except NONE and ALL
        await Assert.That(names.Cast<string>()).Contains("power");
        await Assert.That(names.Cast<string>()).Contains("bright");
        await Assert.That(names.Cast<string>()).Contains("ct");
        await Assert.That(names.Cast<string>()).Contains("rgb");
        await Assert.That(names.Cast<string>()).Contains("hue");
        await Assert.That(names.Cast<string>()).Contains("sat");
        await Assert.That(names.Cast<string>()).Contains("color_mode");
        await Assert.That(names.Cast<string>()).Contains("flowing");
        await Assert.That(names.Cast<string>()).Contains("delayoff");
        await Assert.That(names.Cast<string>()).Contains("flow_params");
        await Assert.That(names.Cast<string>()).Contains("music_on");
        await Assert.That(names.Cast<string>()).Contains("name");
    }

    [Test]
    public async Task GetRealNames_BackgroundProperties()
    {
        var properties = PROPERTIES.bg_power | PROPERTIES.bg_bright | PROPERTIES.bg_rgb;

        var names = properties.GetRealNames();

        await Assert.That(names.Count).IsEqualTo(3);
        await Assert.That(names.Cast<string>()).Contains("bg_power");
        await Assert.That(names.Cast<string>()).Contains("bg_bright");
        await Assert.That(names.Cast<string>()).Contains("bg_rgb");
    }

    [Test]
    public async Task GetRealNames_MixedProperties()
    {
        var properties = PROPERTIES.power | PROPERTIES.bg_power | PROPERTIES.name;

        var names = properties.GetRealNames();

        await Assert.That(names.Count).IsEqualTo(3);
        await Assert.That(names.Cast<string>()).Contains("power");
        await Assert.That(names.Cast<string>()).Contains("bg_power");
        await Assert.That(names.Cast<string>()).Contains("name");
    }

    [Test]
    public async Task GetRealNames_ReturnsListOfObjects()
    {
        var properties = PROPERTIES.power;

        var names = properties.GetRealNames();

        await Assert.That(names).IsTypeOf<List<object>>();
    }

    [Test]
    public async Task GetRealNames_OrderMatchesEnumDefinition()
    {
        var properties = PROPERTIES.power | PROPERTIES.bright | PROPERTIES.ct | PROPERTIES.rgb;

        var names = properties.GetRealNames();

        // The order should match the enum definition order
        await Assert.That(names[0]).IsEqualTo("power");
        await Assert.That(names[1]).IsEqualTo("bright");
        await Assert.That(names[2]).IsEqualTo("ct");
        await Assert.That(names[3]).IsEqualTo("rgb");
    }

    #endregion

    #region PROPERTIES Flags Behavior Tests

    [Test]
    public async Task PROPERTIES_CombineWithOR()
    {
        var combined = PROPERTIES.power | PROPERTIES.bright;

        await Assert.That(combined.HasFlag(PROPERTIES.power)).IsTrue();
        await Assert.That(combined.HasFlag(PROPERTIES.bright)).IsTrue();
        await Assert.That(combined.HasFlag(PROPERTIES.ct)).IsFalse();
    }

    [Test]
    public async Task PROPERTIES_CombineMultiple()
    {
        var combined = PROPERTIES.power | PROPERTIES.bright | PROPERTIES.ct | PROPERTIES.rgb | PROPERTIES.hue;

        await Assert.That(combined.GetRealNames().Count).IsEqualTo(5);
    }

    [Test]
    public async Task PROPERTIES_ALL_HasAllFlags()
    {
        var all = PROPERTIES.ALL;

        await Assert.That(all.HasFlag(PROPERTIES.power)).IsTrue();
        await Assert.That(all.HasFlag(PROPERTIES.bright)).IsTrue();
        await Assert.That(all.HasFlag(PROPERTIES.ct)).IsTrue();
        await Assert.That(all.HasFlag(PROPERTIES.rgb)).IsTrue();
        await Assert.That(all.HasFlag(PROPERTIES.bg_power)).IsTrue();
        await Assert.That(all.HasFlag(PROPERTIES.bg_bright)).IsTrue();
        await Assert.That(all.HasFlag(PROPERTIES.active_mode)).IsTrue();
    }

    [Test]
    public async Task PROPERTIES_NONE_HasNoFlags()
    {
        var none = PROPERTIES.NONE;

        await Assert.That(none.HasFlag(PROPERTIES.power)).IsFalse();
        await Assert.That(none.HasFlag(PROPERTIES.bright)).IsFalse();
    }

    #endregion

    #region Property Names Match Protocol

    [Test]
    [Arguments(PROPERTIES.power, "power")]
    [Arguments(PROPERTIES.bright, "bright")]
    [Arguments(PROPERTIES.ct, "ct")]
    [Arguments(PROPERTIES.rgb, "rgb")]
    [Arguments(PROPERTIES.hue, "hue")]
    [Arguments(PROPERTIES.sat, "sat")]
    [Arguments(PROPERTIES.color_mode, "color_mode")]
    [Arguments(PROPERTIES.flowing, "flowing")]
    [Arguments(PROPERTIES.delayoff, "delayoff")]
    [Arguments(PROPERTIES.flow_params, "flow_params")]
    [Arguments(PROPERTIES.music_on, "music_on")]
    [Arguments(PROPERTIES.name, "name")]
    public async Task Property_ToString_MatchesProtocolName(PROPERTIES property, string expectedName)
    {
        await Assert.That(property.ToString()).IsEqualTo(expectedName);
    }

    [Test]
    [Arguments(PROPERTIES.bg_power, "bg_power")]
    [Arguments(PROPERTIES.bg_flowing, "bg_flowing")]
    [Arguments(PROPERTIES.bg_flow_params, "bg_flow_params")]
    [Arguments(PROPERTIES.bg_ct, "bg_ct")]
    [Arguments(PROPERTIES.bg_lmode, "bg_lmode")]
    [Arguments(PROPERTIES.bg_bright, "bg_bright")]
    [Arguments(PROPERTIES.bg_rgb, "bg_rgb")]
    [Arguments(PROPERTIES.bg_hue, "bg_hue")]
    [Arguments(PROPERTIES.bg_sat, "bg_sat")]
    public async Task BackgroundProperty_ToString_MatchesProtocolName(PROPERTIES property, string expectedName)
    {
        await Assert.That(property.ToString()).IsEqualTo(expectedName);
    }

    #endregion
}
