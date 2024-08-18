using MiHotkeys.Services.MiDevice.Events;
using MiHotkeys.Services.MiDevice.Responses;
using Newtonsoft.Json;
using SvrCModuleClrWrapper;

namespace MiHotkeys.Services.MiDevice;

public class MiDeviceEventBus : IDisposable, IMiDeviceService
{
    private static ModuleController? _moduleController;

    public event EventHandler<ChargingProtectModeReceivedEventArgs> OnChargingProtectModeRecieved;
    public event EventHandler<PowerModeReceivedEventArgs>           OnPowerModeRecieved;

    public void Open()
    {
        _moduleController = ModuleController.GetInstance();
        _moduleController.CreateSvrCModule(IntPtr.Zero);
        _moduleController.OnSuccessEvent += OnSuccessEvent;
    }

    private void OnSuccessEvent(string method, string response, long query_id)
    {
        switch (method)
        {
            case CommandsList.GetChargingProtect:
                var obj = JsonConvert.DeserializeObject<MiResponse<ChargingProtectionStatusResponse>>(response);
                OnChargingProtectModeRecieved?.Invoke(this,new ChargingProtectModeReceivedEventArgs(obj?.Data.Mode == 1));
                break;
            case CommandsList.GetWorkLoadMode:
                var workLoadModeResponse = JsonConvert.DeserializeObject<MiResponse<WorkLoadModeResponse>>(response);
                OnPowerModeRecieved?.Invoke(this, new PowerModeReceivedEventArgs(workLoadModeResponse?.Data.Mode ?? 0));
                break;
        }
    }

    public void Execute(IMiCommand command)
    {
        var lowLevelCommand = new MiLowLevelCommand()
        {
            Params = command.Params ?? new object(),
            Method = command.Method,
        };
        _moduleController?.Execute(JsonConvert.SerializeObject(lowLevelCommand), -1);
    }


    public void Dispose()
    {
        Close();
    }


    public void Close() => _moduleController?.DestroySvrCModule();
}