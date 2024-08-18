using System.Runtime.InteropServices;

namespace MiHotkeys.Services.PowerMonitor;

[StructLayout(LayoutKind.Sequential)]
public class PowerState
{
    private ACLineStatus ACLineStatus;

    [DllImport("Kernel32", EntryPoint = "GetSystemPowerStatus")]
    private static extern bool GetSystemPowerStatusByRef(PowerState ps);

    public static ACLineStatus GetPowerLineStatus()
    {
        var ps = new PowerState();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && GetSystemPowerStatusByRef(ps))
            return ps.ACLineStatus;

        return ACLineStatus.Unknown;
    }
}