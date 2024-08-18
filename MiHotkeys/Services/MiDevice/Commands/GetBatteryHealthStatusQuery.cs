namespace MiHotkeys.Services.MiDevice.Commands;

public class GetBatteryHealthStatusQuery : IMiCommand
{
    public string  Method => CommandsList.GetBatteryHealthStatus;
    public object? Params { get; }
}