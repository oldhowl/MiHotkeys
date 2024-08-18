namespace MiHotkeys.Services.MiDevice.Commands;

public class RepairMiServiceCommand : IMiCommand
{
    public string  Method => CommandsList.RepairMiDeviceService;
    public object? Params { get; }= null;
}