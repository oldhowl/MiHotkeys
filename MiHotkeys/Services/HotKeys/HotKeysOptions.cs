namespace MiHotkeys.Services.HotKeys;

public class HotKeysOptions
{
    public long ScreenModeSwitchCombination { get; }
    public long PowerModeSwitchCombination  { get; }

    //Fn buttons
    public long MicSwitchCombination      { get; }
    public long SnippingToolCombination   { get; }
    public long SystemSettingsCombination { get; }

    public HotKeysOptions(
        long[] screenModeSwitchCombination,
        long[] powerModeSwitchCombination,
        long[] micSwitchCombination,
        long[] snippingToolCombination,
        long[] systemSettingsCombination)
    {
        ScreenModeSwitchCombination = screenModeSwitchCombination.Sum();
        PowerModeSwitchCombination  = powerModeSwitchCombination.Sum();
        MicSwitchCombination        = micSwitchCombination.Sum();
        SnippingToolCombination     = snippingToolCombination.Sum();
        SystemSettingsCombination   = systemSettingsCombination.Sum();
    }
}