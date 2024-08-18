using MiHotkeys.Services.MiDevice.Events;

namespace MiHotkeys.Services.MiDevice;

public interface IMiDeviceService
{
    void                                                            Execute(IMiCommand command);
    event        EventHandler<ChargingProtectModeReceivedEventArgs> OnChargingProtectModeRecieved;
    public event EventHandler<PowerModeReceivedEventArgs>           OnPowerModeRecieved;

    void Open();
    void Close();
}