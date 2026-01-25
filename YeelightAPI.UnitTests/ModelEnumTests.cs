using YeelightAPI.Models;
using YeelightAPI.Models.Adjust;
using YeelightAPI.Models.Cron;
using YeelightAPI.Models.Music;
using YeelightAPI.Models.Scene;

namespace YeelightAPI.UnitTests;

public class ModelEnumTests
{
    #region PROPERTIES Enum Tests

    [Test]
    public async Task PROPERTIES_NONE_HasZeroValue()
    {
        await Assert.That((int)PROPERTIES.NONE).IsEqualTo(0);
    }

    [Test]
    public async Task PROPERTIES_ALL_HasAllBitsSet()
    {
        await Assert.That((int)PROPERTIES.ALL).IsEqualTo(~0);
    }

    [Test]
    public async Task PROPERTIES_AreFlagsEnum()
    {
        // Check that combining flags works correctly
        var combined = PROPERTIES.power | PROPERTIES.bright | PROPERTIES.ct;

        await Assert.That(combined.HasFlag(PROPERTIES.power)).IsTrue();
        await Assert.That(combined.HasFlag(PROPERTIES.bright)).IsTrue();
        await Assert.That(combined.HasFlag(PROPERTIES.ct)).IsTrue();
        await Assert.That(combined.HasFlag(PROPERTIES.rgb)).IsFalse();
    }

    [Test]
    [Arguments(PROPERTIES.power, 1)]
    [Arguments(PROPERTIES.bright, 2)]
    [Arguments(PROPERTIES.ct, 4)]
    [Arguments(PROPERTIES.rgb, 8)]
    [Arguments(PROPERTIES.hue, 16)]
    [Arguments(PROPERTIES.sat, 32)]
    [Arguments(PROPERTIES.color_mode, 64)]
    [Arguments(PROPERTIES.flowing, 128)]
    [Arguments(PROPERTIES.delayoff, 256)]
    [Arguments(PROPERTIES.flow_params, 512)]
    [Arguments(PROPERTIES.music_on, 1024)]
    [Arguments(PROPERTIES.name, 2048)]
    public async Task PROPERTIES_HaveCorrectBitValues(PROPERTIES property, int expectedValue)
    {
        await Assert.That((int)property).IsEqualTo(expectedValue);
    }

    [Test]
    public async Task PROPERTIES_BackgroundProperties_HaveCorrectValues()
    {
        await Assert.That((int)PROPERTIES.bg_power).IsEqualTo(1 << 12);
        await Assert.That((int)PROPERTIES.bg_flowing).IsEqualTo(1 << 13);
        await Assert.That((int)PROPERTIES.bg_flow_params).IsEqualTo(1 << 14);
        await Assert.That((int)PROPERTIES.bg_ct).IsEqualTo(1 << 15);
        await Assert.That((int)PROPERTIES.bg_lmode).IsEqualTo(1 << 16);
        await Assert.That((int)PROPERTIES.bg_bright).IsEqualTo(1 << 17);
        await Assert.That((int)PROPERTIES.bg_rgb).IsEqualTo(1 << 18);
        await Assert.That((int)PROPERTIES.bg_hue).IsEqualTo(1 << 19);
        await Assert.That((int)PROPERTIES.bg_sat).IsEqualTo(1 << 20);
    }

    [Test]
    public async Task PROPERTIES_CombinedFlags_CanBeParsed()
    {
        var properties = PROPERTIES.power | PROPERTIES.bright | PROPERTIES.rgb;

        await Assert.That(properties.HasFlag(PROPERTIES.power)).IsTrue();
        await Assert.That(properties.HasFlag(PROPERTIES.bright)).IsTrue();
        await Assert.That(properties.HasFlag(PROPERTIES.rgb)).IsTrue();

        // Verify it doesn't include other properties
        await Assert.That(properties.HasFlag(PROPERTIES.ct)).IsFalse();
        await Assert.That(properties.HasFlag(PROPERTIES.hue)).IsFalse();
    }

    #endregion

    #region METHODS Enum Tests

    [Test]
    [Arguments(METHODS.GetProp, 1)]
    [Arguments(METHODS.SetColorTemperature, 2)]
    [Arguments(METHODS.SetRGBColor, 3)]
    [Arguments(METHODS.SetHSVColor, 4)]
    [Arguments(METHODS.SetBrightness, 5)]
    [Arguments(METHODS.SetPower, 6)]
    [Arguments(METHODS.Toggle, 7)]
    [Arguments(METHODS.SetDefault, 8)]
    [Arguments(METHODS.StartColorFlow, 9)]
    [Arguments(METHODS.StopColorFlow, 10)]
    public async Task METHODS_HaveCorrectValues(METHODS method, int expectedValue)
    {
        await Assert.That((int)method).IsEqualTo(expectedValue);
    }

    [Test]
    public async Task METHODS_BackgroundMethods_Exist()
    {
        await Assert.That((int)METHODS.SetBackgroundLightRGBColor).IsEqualTo(18);
        await Assert.That((int)METHODS.SetBackgroundLightHSVColor).IsEqualTo(19);
        await Assert.That((int)METHODS.SetBackgroundColorTemperature).IsEqualTo(20);
        await Assert.That((int)METHODS.StartBackgroundLightColorFlow).IsEqualTo(21);
        await Assert.That((int)METHODS.StopBackgroundLightColorFlow).IsEqualTo(22);
    }

    [Test]
    public async Task METHODS_AdjustMethods_Exist()
    {
        await Assert.That((int)METHODS.AdjustBright).IsEqualTo(30);
        await Assert.That((int)METHODS.AdjustColorTemperature).IsEqualTo(31);
        await Assert.That((int)METHODS.AdjustColor).IsEqualTo(32);
        await Assert.That((int)METHODS.BackgroundAdjustBright).IsEqualTo(33);
        await Assert.That((int)METHODS.BackgroundAdjustColorTemperature).IsEqualTo(34);
        await Assert.That((int)METHODS.BackgroundAdjustColor).IsEqualTo(35);
    }

    #endregion

    #region MODEL Enum Tests

    [Test]
    public async Task MODEL_Unknown_IsDefault()
    {
        await Assert.That((int)MODEL.Unknown).IsEqualTo(0);
    }

    [Test]
    public async Task MODEL_AllModelsAreDefined()
    {
        var models = Enum.GetValues<MODEL>();

        await Assert.That(models).Contains(MODEL.Unknown);
        await Assert.That(models).Contains(MODEL.Mono);
        await Assert.That(models).Contains(MODEL.Color);
        await Assert.That(models).Contains(MODEL.Stripe);
        await Assert.That(models).Contains(MODEL.Ceiling);
        await Assert.That(models).Contains(MODEL.BedsideLamp);
        await Assert.That(models).Contains(MODEL.DeskLamp);
        await Assert.That(models).Contains(MODEL.TunableWhiteBulb);
    }

    [Test]
    public async Task MODEL_Count()
    {
        var models = Enum.GetValues<MODEL>();
        await Assert.That(models.Length).IsEqualTo(8);
    }

    #endregion

    #region PowerOnMode Enum Tests

    [Test]
    public async Task PowerOnMode_AllModesAreDefined()
    {
        var modes = Enum.GetValues<PowerOnMode>();

        await Assert.That(modes).Contains(PowerOnMode.Normal);
        await Assert.That(modes).Contains(PowerOnMode.Ct);
        await Assert.That(modes).Contains(PowerOnMode.RGB);
        await Assert.That(modes).Contains(PowerOnMode.HSV);
        await Assert.That(modes).Contains(PowerOnMode.ColorFlow);
        await Assert.That(modes).Contains(PowerOnMode.Night);
    }

    [Test]
    [Arguments(PowerOnMode.Normal, 0)]
    [Arguments(PowerOnMode.Ct, 1)]
    [Arguments(PowerOnMode.RGB, 2)]
    [Arguments(PowerOnMode.HSV, 3)]
    [Arguments(PowerOnMode.ColorFlow, 4)]
    [Arguments(PowerOnMode.Night, 5)]
    public async Task PowerOnMode_HaveCorrectValues(PowerOnMode mode, int expected)
    {
        await Assert.That((int)mode).IsEqualTo(expected);
    }

    #endregion

    #region SceneClass Enum Tests

    [Test]
    public async Task SceneClass_AllClassesAreDefined()
    {
        var classes = Enum.GetValues<SceneClass>();

        await Assert.That(classes).Contains(SceneClass.color);
        await Assert.That(classes).Contains(SceneClass.hsv);
        await Assert.That(classes).Contains(SceneClass.ct);
        await Assert.That(classes).Contains(SceneClass.cf);
        await Assert.That(classes).Contains(SceneClass.auto_delay_off);
    }

    [Test]
    public async Task SceneClass_ToString_ReturnsCorrectNames()
    {
        await Assert.That(SceneClass.color.ToString()).IsEqualTo("color");
        await Assert.That(SceneClass.hsv.ToString()).IsEqualTo("hsv");
        await Assert.That(SceneClass.ct.ToString()).IsEqualTo("ct");
        await Assert.That(SceneClass.cf.ToString()).IsEqualTo("cf");
        await Assert.That(SceneClass.auto_delay_off.ToString()).IsEqualTo("auto_delay_off");
    }

    #endregion

    #region AdjustAction Enum Tests

    [Test]
    public async Task AdjustAction_AllActionsAreDefined()
    {
        var actions = Enum.GetValues<AdjustAction>();

        await Assert.That(actions).Contains(AdjustAction.increase);
        await Assert.That(actions).Contains(AdjustAction.decrease);
        await Assert.That(actions).Contains(AdjustAction.circle);
    }

    [Test]
    public async Task AdjustAction_ToString_ReturnsLowercase()
    {
        await Assert.That(AdjustAction.increase.ToString()).IsEqualTo("increase");
        await Assert.That(AdjustAction.decrease.ToString()).IsEqualTo("decrease");
        await Assert.That(AdjustAction.circle.ToString()).IsEqualTo("circle");
    }

    #endregion

    #region AdjustProperty Enum Tests

    [Test]
    public async Task AdjustProperty_AllPropertiesAreDefined()
    {
        var properties = Enum.GetValues<AdjustProperty>();

        await Assert.That(properties).Contains(AdjustProperty.bright);
        await Assert.That(properties).Contains(AdjustProperty.ct);
        await Assert.That(properties).Contains(AdjustProperty.color);
    }

    #endregion

    #region CronType Enum Tests

    [Test]
    public async Task CronType_PowerOff_IsDefined()
    {
        var types = Enum.GetValues<CronType>();
        await Assert.That(types).Contains(CronType.PowerOff);
    }

    [Test]
    public async Task CronType_PowerOff_HasZeroValue()
    {
        await Assert.That((int)CronType.PowerOff).IsEqualTo(0);
    }

    #endregion

    #region MusicAction Enum Tests

    [Test]
    public async Task MusicAction_AllActionsAreDefined()
    {
        var actions = Enum.GetValues<MusicAction>();

        await Assert.That(actions).Contains(MusicAction.Off);
        await Assert.That(actions).Contains(MusicAction.On);
    }

    [Test]
    [Arguments(MusicAction.Off, 0)]
    [Arguments(MusicAction.On, 1)]
    public async Task MusicAction_HaveCorrectValues(MusicAction action, int expected)
    {
        await Assert.That((int)action).IsEqualTo(expected);
    }

    #endregion
}
