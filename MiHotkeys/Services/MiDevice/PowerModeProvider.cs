using MiHotkeys.Services.MiDevice.Commands;
using MiHotkeys.Services.PowerManager;

namespace MiHotkeys.Services.MiDevice;

public class PowerModeProvider : IPowerModeProvider
{
    private MiDeviceEventBus _miDeviceEventBus;

    public PowerModeProvider(MiDeviceEventBus miDeviceEventBus)
    {
        _miDeviceEventBus = miDeviceEventBus;
    }

    public void RequestCurrentPowerMode()
    {
        _miDeviceEventBus.Execute(new GetWorkloadMode());
    }

    public void SetPowerMode(int mode)
    {
        _miDeviceEventBus.Execute(new SetWorkLoadMode(mode));
    }
}