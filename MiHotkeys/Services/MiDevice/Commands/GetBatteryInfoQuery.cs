namespace MiHotkeys.Services.MiDevice.Commands;

public class GetBatteryInfoQuery : IMiCommand
{
    public string  Method => CommandsList.GetBatteryInfo;
    public object? Params { get; } = null;
}