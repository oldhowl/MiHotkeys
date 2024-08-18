using System.Management;
using Microsoft.Win32;

namespace MiHotkeys.Services.PowerMonitor;

public class PowerMonitorService
{
    public event EventHandler<PowerStatusChangedEventArgs>? PowerStatusChanged;
    private ACLineStatus?                                   _lastState;

    public PowerMonitorService()
    {
        SystemEvents.PowerModeChanged += OnPowerEventArrived;
    }

    private void OnPowerEventArrived(object sender, PowerModeChangedEventArgs e)
    {
        if (e.Mode.HasFlag(PowerModes.StatusChange))
        {
            var status = PowerState.GetPowerLineStatus();
            if (status == ACLineStatus.Unknown || _lastState == status)
                return;
            PowerStatusChanged?.Invoke(this,
                new PowerStatusChangedEventArgs(PowerState.GetPowerLineStatus() == ACLineStatus.Online
                    ? PowerSource.AC
                    : PowerSource.Battery));
            _lastState = status;
        }
    }
}
