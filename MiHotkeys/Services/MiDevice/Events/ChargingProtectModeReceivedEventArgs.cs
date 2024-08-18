namespace MiHotkeys.Services.MiDevice.Events;

public class ChargingProtectModeReceivedEventArgs : EventArgs
{
    public bool IsEnabled { get; }
    public ChargingProtectModeReceivedEventArgs(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}