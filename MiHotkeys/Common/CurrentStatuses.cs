using MiHotkeys.Services.BatteryInfo;
using MiHotkeys.Services.DisplayManager;
using MiHotkeys.Services.PowerManager;

namespace MiHotkeys.Common;

public class CurrentStatuses
{
    public PowerMode       PowerMode       { get; set; }
    public RefreshRateMode RefreshRateMode { get; set; }
    public bool            MicEnabled      { get; set; }
    public PowerLoad?      PowerLoad       { get; set; }

    public static CurrentStatuses SetCurrent(PowerLoad? powerLoad, PowerMode powerMode, RefreshRateMode refreshRateMode,
                                             bool       micEnabled)
    {
        return new CurrentStatuses()
        {
            PowerLoad       = powerLoad,
            PowerMode       = powerMode,
            MicEnabled      = micEnabled,
            RefreshRateMode = refreshRateMode
        };
    }
}