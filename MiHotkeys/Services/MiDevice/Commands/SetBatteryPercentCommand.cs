namespace MiHotkeys.Services.MiDevice.Commands;

public class GetBatteryPercentQuery : IMiCommand
{
    public string  Method => CommandsList.GetBatteryPercent;
    public object? Params { get; }= null;
}