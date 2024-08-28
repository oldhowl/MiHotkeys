using System.Text;
using MiHotkeys.Services.BatteryInfo;
using MiHotkeys.Services.DisplayManager;
using MiHotkeys.Services.PowerManager;

namespace MiHotkeys.Common;

public class TextFactory
{
    public const string? ChargingProtectionTrayMenuItemTitle = "Charging protection";
    public const string? PowerLoadMonitorTrayMenuItemTitle   = "Power load monitor";

    #region Notification in left corner

    public static string GetRefreshRateMessage(RefreshRateMode rateMode)
    {
        return rateMode switch
        {
            RefreshRateMode.Hz60  => TextConstants.RefreshRate60Hz,
            RefreshRateMode.Hz120 => TextConstants.RefreshRate120Hz,
            RefreshRateMode.Hz165 => TextConstants.RefreshRate165Hz,
            _                     => throw new ArgumentOutOfRangeException(nameof(rateMode), rateMode, null)
        };
    }

    public static string GetPowerModeMessage(PowerMode powerMode)
    {
        return powerMode switch
        {
            PowerMode.Silence  => TextConstants.PowerModeSilence,
            PowerMode.Balance  => TextConstants.PowerModeBalance,
            PowerMode.MaxPower => TextConstants.PowerModeMaxPower,
            PowerMode.Turbo    => TextConstants.PowerModeTurbo,
            PowerMode.Pending  => TextConstants.PowerModeRequested,
            _                  => throw new ArgumentOutOfRangeException(nameof(powerMode), powerMode, null)
        };
    }

    public static string MicSwitched(bool micEnabled) =>
        micEnabled ? TextConstants.MicOnMessage : TextConstants.MicOffMessage;

    #endregion


    #region ToolTip

    public static string ToolTipMainText(PowerLoad? powerLoad, PowerMode powerMode, RefreshRateMode refreshRate,
                                         bool       micEnabled)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Power Mode: {GetPowerModeState(powerMode)}");
        sb.AppendLine($"🖥 {GetRefreshRateState(refreshRate)}");
        sb.AppendLine($"Mic: {GetMicState(micEnabled)}");

        if (powerLoad != null)
            sb.AppendLine(
                $"Power Load: {GetPowerLoadEmoji(powerLoad?.CalculatePowerLoadIndex())} {(powerLoad?.CalculatePowerLoadIndex().ToString() ?? "unknown")}/10");

        return sb.ToString();
    }

    private static string GetPowerModeState(PowerMode powerMode)
    {
        return powerMode switch
        {
            PowerMode.Silence  => "🟩",
            PowerMode.Balance  => "🟨",
            PowerMode.MaxPower => "🟥",
            PowerMode.Turbo    => "🔥",
            PowerMode.Pending  => "⏳",

            _ => throw new ArgumentOutOfRangeException(nameof(powerMode), powerMode, null)
        };
    }

    private static string GetRefreshRateState(RefreshRateMode refreshRate)
    {
        return refreshRate switch
        {
            RefreshRateMode.Hz60  => TextConstants.RefreshRate60Hz,
            RefreshRateMode.Hz120 => TextConstants.RefreshRate120Hz,
            RefreshRateMode.Hz165 => TextConstants.RefreshRate165Hz,
            _                     => throw new ArgumentOutOfRangeException(nameof(refreshRate), refreshRate, null)
        };
    }

    private static string GetMicState(bool micEnabled)
    {
        return micEnabled ? TextConstants.MicOnMessage : TextConstants.MicOffMessage;
    }

    #endregion

    public static string GetPowerLoadEmoji(int? powerLoadIndex)
    {
        return powerLoadIndex switch
        {
            <= 2  => "🔋🙂",
            <= 4  => "🔋😌",
            <= 6  => "🔋😐",
            <= 8  => "🔋😈",
            <= 10 => "🔋😨",
            _     => "🔋❓"
        };
    }
}