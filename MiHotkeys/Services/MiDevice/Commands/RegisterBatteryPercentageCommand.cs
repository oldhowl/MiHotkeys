namespace MiHotkeys.Services.MiDevice.Commands;

public class RegisterBatteryPercentageCommand : IMiCommand
{
    public string  Method => CommandsList.RegisterBatteryPercentage;
    public object? Params { get; }= null;
}