namespace MiHotkeys.Services.PowerMonitor;

public class PowerStatusChangedEventArgs : EventArgs
{
    public PowerSource PowerSource { get; }

    public PowerStatusChangedEventArgs(PowerSource powerSource)
    {
        PowerSource = powerSource;
    }
}